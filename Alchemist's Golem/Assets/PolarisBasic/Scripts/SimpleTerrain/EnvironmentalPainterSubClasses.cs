using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Pinwheel.PolarisBasic
{
    public partial class EnvironmentalPainter
    {
        /// <summary>
        /// Paint mode
        /// </summary>
        public enum Mode
        {
            Spawning, Masking
        }

        /// <summary>
        /// Brush settings, group everything into a class to deal with serialization
        /// </summary>
        [System.Serializable]
        public class ToolsSettings
        {
            public Mode mode;
            public float brushRadius;
            public int prefabIndex;
            public float scaleMin;
            public float scaleMax;
            public float maxRotation;
            public int density;
            public float combinationPhysicalSize;
            public List<GameObject> prefabs;
            public bool[] mask;
            public bool keepColliders;
            public bool drawCombinationsBounds;

            public ToolsSettings()
            {
                brushRadius = 10;
                prefabIndex = 0;
                scaleMin = 1f;
                scaleMax = 2f;
                maxRotation = 360;
                density = 1;
                combinationPhysicalSize = 50;
                drawCombinationsBounds = true;
                prefabs = new List<GameObject>();
                Validate();
            }

            public void Validate()
            {
                brushRadius = Mathf.Max(0, brushRadius);
                prefabIndex = Mathf.Max(0, prefabIndex);
                scaleMax = Mathf.Max(scaleMin, scaleMax);
                density = Mathf.Clamp(density, 1, 100);
                combinationPhysicalSize = Mathf.Max(0, combinationPhysicalSize);
                if (prefabs == null)
                    prefabs = new List<GameObject>();
            }

            /// <summary>
            /// Add a prefab into the collection, update currently selected prefab
            /// </summary>
            /// <param name="g"></param>
            public void AddPrefab(GameObject g)
            {
                if (prefabs == null)
                    prefabs = new List<GameObject>();
                if (!prefabs.Contains(g))
                    prefabs.Add(g);
                prefabIndex = prefabs.FindIndex((p) => p == g);
            }

            /// <summary>
            /// Remove a prefab from the collection, update currently selected prefab
            /// </summary>
            /// <param name="i"></param>
            public void RemovePrefabAt(int i)
            {
                if (prefabs == null)
                    return;
                if (i >= 0 && i < prefabs.Count)
                {
                    prefabs.RemoveAt(i);
                    prefabIndex = Mathf.Clamp(prefabIndex, 0, prefabs.Count - 1);
                }
            }

            /// <summary>
            /// Get the prefab collection, not null
            /// </summary>
            /// <returns></returns>
            public List<GameObject> GetPrefabs()
            {
                if (prefabs == null)
                    prefabs = new List<GameObject>();
                prefabs.RemoveAll((p) => p == null);
                prefabIndex = Mathf.Clamp(prefabIndex, 0, prefabs.Count - 1);
                return prefabs;
            }
        }

        /// <summary>
        /// Utility class to combine prefab instances
        /// </summary>
        public class MeshCombiner
        {
            //objects to combine
            private GameObject[] src;
            //size of the combination in world space
            private float physicalSize;
            //keep collider or not?
            private bool keepColliders;
            //the largest value in groupIndices array
            private int maxGroupIndex;
            //indicate which group an instace belongs to
            private int[] groupIndices;
            //maximum number of instances can be grouped into one, depends on some constraint, like MAX_VETICES in a mesh
            private int maxInstancePerGroup;
            //a matrix to transform the combination between coordinate systems
            private Matrix4x4 matrix;
            //a grid of mesh filters contain in the prefab, to handle complex prefab structure (modular trees)
            private MeshFilter[][] meshFilterGrid;

            public MeshCombiner(GameObject[] src, float groupPhysicalSize, bool keepColliders, Matrix4x4 matrix)
            {
                if (src == null || src.Length == 0)
                {
                    throw new System.ArgumentException("Invalid source contains no game object!");
                }
                this.src = src;
                this.physicalSize = groupPhysicalSize;
                this.keepColliders = keepColliders;
                this.matrix = matrix;
            }

            /// <summary>
            /// Combine prefab instances into one larger group
            /// </summary>
            /// <returns></returns>
            public GameObject[] Combine()
            {
                List<GameObject> combinationCollection = new List<GameObject>();
                if (src.Length == 0)
                    return combinationCollection.ToArray();

                Analyze();

                //Create a clone game object from the prefab, then perform the combine meshes action on each level of its hierarchy
                for (int group = 0; group < maxGroupIndex; ++group)
                {
                    GameObject combination = GameObject.Instantiate(src[0]);
                    MeshFilter[] combinationMf = combination.GetComponentsInChildren<MeshFilter>(true);
                    for (int mfIndex = 0; mfIndex < combinationMf.Length; ++mfIndex)
                    {
                        List<CombineInstance> combineInstances = new List<CombineInstance>();
                        for (int srcIndex = 0; srcIndex < src.Length; ++srcIndex)
                        {
                            if (groupIndices[srcIndex] != group)
                                continue;
                            CombineInstance ci = new CombineInstance();
                            ci.mesh = meshFilterGrid[srcIndex][mfIndex].sharedMesh;
                            ci.transform = matrix * meshFilterGrid[srcIndex][mfIndex].transform.localToWorldMatrix;
                            combineInstances.Add(ci);
                        }
                        Mesh m = new Mesh();
                        m.CombineMeshes(combineInstances.ToArray(), true, true, true);
                        combinationMf[mfIndex].mesh = m;
                        combinationMf[mfIndex].transform.localPosition = Vector3.zero;
                        combinationMf[mfIndex].transform.localRotation = Quaternion.identity;
                        combinationMf[mfIndex].transform.localScale = Vector3.one;
                    }
                    //remove the old collider, they are not at the correct position now
                    RemoveColliders(combination);

                    if (keepColliders)
                    {
                        //create new colliders which have correct position in world space
                        CloneColliders(combination, group);
                    }
                    combinationCollection.Add(combination);
                }

                return combinationCollection.ToArray();
            }

            private void Analyze()
            {
                GetGroupConstraints();
                GetGroupIndices();
                GetMeshFilterGrid();
            }

            /// <summary>
            /// Calculate group constranints like maxInstancePerGroup
            /// </summary>
            private void GetGroupConstraints()
            {
                MeshFilter[] mf = src[0].GetComponentsInChildren<MeshFilter>();
                int highestVertexCount = 0;
                for (int i = 0; i < mf.Length; ++i)
                {
                    highestVertexCount = mf[i].sharedMesh.vertexCount > highestVertexCount ? mf[i].sharedMesh.vertexCount : highestVertexCount;
                }

                this.maxInstancePerGroup = Constraints.MAX_VERTICES / highestVertexCount;
            }

            /// <summary>
            /// Assign each instance into a group based on their distance to others
            /// </summary>
            private void GetGroupIndices()
            {
                groupIndices = new int[src.Length];
                Utilities.Fill(groupIndices, -1);
                int currentGroupIndex = 0;
                int groupInstancesCount = 0;
                for (int i = 0; i < src.Length; ++i)
                {
                    if (groupIndices[i] >= 0)
                        continue;
                    groupIndices[i] = currentGroupIndex;
                    groupInstancesCount += 1;
                    for (int j = 0; j < src.Length; ++j)
                    {
                        bool isUngrouped = groupIndices[j] < 0;
                        bool isInRange =
                            Vector3.SqrMagnitude(src[j].transform.position - src[i].transform.position) <
                            physicalSize * physicalSize;
                        bool isInstanceSlotAvailable = groupInstancesCount < maxInstancePerGroup;
                        if (isUngrouped && isInRange && isInstanceSlotAvailable)
                        {
                            groupIndices[j] = currentGroupIndex;
                            groupInstancesCount += 1;
                        }
                    }

                    currentGroupIndex += 1;
                    groupInstancesCount = 0;
                }
                maxGroupIndex = currentGroupIndex;
            }

            private void GetMeshFilterGrid()
            {
                meshFilterGrid = new MeshFilter[src.Length][];
                for (int i = 0; i < meshFilterGrid.Length; ++i)
                {
                    meshFilterGrid[i] = src[i].GetComponentsInChildren<MeshFilter>(true);
                }
            }

            private void RemoveColliders(GameObject g)
            {
                Collider[] colliders = g.GetComponentsInChildren<Collider>();
                for (int i = 0; i < colliders.Length; ++i)
                {
                    Utilities.DestroyObject(colliders[i]);
                }
            }

            /// <summary>
            /// Create additional game objects with collider, which maintains colliders position in world space
            /// </summary>
            /// <param name="g"></param>
            /// <param name="group"></param>
            private void CloneColliders(GameObject g, int group)
            {
                for (int srcIndex = 0; srcIndex < src.Length; ++srcIndex)
                {
                    if (groupIndices[srcIndex] != group)
                        continue;
                    Collider[] colliders = src[srcIndex].GetComponentsInChildren<Collider>();
                    for (int colIndex = 0; colIndex < colliders.Length; ++colIndex)
                    {
                        GameObject colObject = GameObject.Instantiate(colliders[colIndex].gameObject);
                        colObject.gameObject.SetActive(true);
                        colObject.transform.parent = g.transform;
                        colObject.name = "Collider";

                        //only keep Transform and Collider
                        Component[] components = colObject.GetComponentsInChildren<Component>(true);
                        for (int compIndex = 0; compIndex < components.Length; ++compIndex)
                        {
                            bool dontDestroy =
                                components[compIndex] is Transform ||
                                components[compIndex] is Collider;
                            if (!dontDestroy)
                                Utilities.DestroyObject(components[compIndex]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Utility class to create a grayscale map for trees distribution
        /// </summary>
        public class DistributionMapExporter
        {
            private GameObject[] trees;
            private TerrainGenerator terrain;
            private int textureSize;
            private int spread;

            private float[][] density;
            private float maxDensity;

            public DistributionMapExporter(GameObject[] trees, TerrainGenerator terrain, int textureSize, int spread)
            {
                this.trees = trees;
                this.terrain = terrain;
                this.textureSize = textureSize;
                this.spread = spread;
            }

            /// <summary>
            /// Create the distribution map
            /// </summary>
            /// <returns></returns>
            public Texture2D ExportTextureData()
            {
                Texture2D tex = new Texture2D(textureSize, textureSize);
                density = Utilities.CreateJaggedArray<float>(textureSize, textureSize);
                for (int i = 0; i < trees.Length; ++i)
                {
                    Vector2 uv = GetUv(trees[i]);
                    IncreaseDensity(density, uv);
                }

                Color[] colors = ColorsFromDensity();
                tex.SetPixels(colors);
                tex.Apply();

                return tex;
            }

            /// <summary>
            /// Get a tree uv coordinate on the map
            /// </summary>
            /// <param name="tree"></param>
            /// <returns></returns>
            private Vector2 GetUv(GameObject tree)
            {
                Ray r = new Ray(tree.transform.position + tree.transform.up, -tree.transform.up);
                RaycastHit hit;
                if (terrain.MeshColliderComponent.Raycast(r, out hit, 100))
                {
                    return hit.textureCoord;
                }
                else
                {
                    return -Vector2.one;
                }
            }

            /// <summary>
            /// Increase density value at a uv coordinate of the map, also effect nearby pixel, depends on spread value.
            /// </summary>
            /// <param name="density"></param>
            /// <param name="uv"></param>
            private void IncreaseDensity(float[][] density, Vector2 uv)
            {
                int pixelSpread = Mathf.Max(0, spread * textureSize / 100);
                int halfSpread = pixelSpread / 2;
                int centerX = Mathf.RoundToInt(uv.x * (textureSize - 1));
                int centerY = Mathf.RoundToInt(uv.y * (textureSize - 1));
                int minX = Mathf.Max(0, centerX - halfSpread);
                int maxX = Mathf.Min(textureSize - 1, centerX + halfSpread);
                int minY = Mathf.Max(0, centerY - halfSpread);
                int maxY = Mathf.Min(textureSize - 1, centerY + halfSpread);

                Vector2 center = new Vector2(centerX, centerY);
                Vector2 p = new Vector2();
                float f = 0;
                float sqrHalfSpread = halfSpread * halfSpread;
                for (int x = minX; x <= maxX; ++x)
                {
                    for (int y = minY; y <= maxY; ++y)
                    {
                        p.x = x;
                        p.y = y;
                        f = Vector3.SqrMagnitude(p - center) / sqrHalfSpread;
                        density[y][x] += Mathf.Clamp01((1 - Mathf.Sqrt(f)));
                        if (density[y][x] > maxDensity)
                            maxDensity = density[y][x];
                    }
                }
            }

            /// <summary>
            /// Get the colors based on density values
            /// </summary>
            /// <returns></returns>
            private Color[] ColorsFromDensity()
            {
                Color[] colors = new Color[textureSize * textureSize];

                Color c;
                for (int y = 0; y < textureSize; ++y)
                {
                    for (int x = 0; x < textureSize; ++x)
                    {
                        c = Color.white * (density[y][x] / maxDensity);
                        colors[textureSize * y + x] = c;
                    }
                }
                return colors;
            }
        }

        /// <summary>
        /// Utility class for generating mass placement transform matrices
        /// </summary>
        public class MassPlacementProcessor
        {
            TerrainGenerator terrain;
            MeshData data;
            Texture2D map;
            int density;
            float distributionThreshold;
            float scaleMin;
            float scaleMax;
            float maxRotation;
            bool followNormal;

            List<Matrix4x4> matrices;
            int trisCount;

            private GameObject tmpGameObject;
            private Transform transform;

            private const string TEMP_MAP_NAME = "_INTERNAL_TempDistributionMap";

            public MassPlacementProcessor(TerrainGenerator terrain, Texture2D map, int density, float distributionThreshold, float scaleMin, float scaleMax, float maxRotation, bool followNormal)
            {
                this.terrain = terrain;
                this.data = terrain.MeshData;
                this.map = map;
                this.density = density;
                this.distributionThreshold = distributionThreshold;
                this.scaleMin = scaleMin;
                this.scaleMax = scaleMax;
                this.maxRotation = maxRotation;
                this.followNormal = followNormal;

            }

            /// <summary>
            /// Sample distribution map and create placement matrices
            /// </summary>
            /// <returns></returns>
            public Matrix4x4[] CalculatePlacementData()
            {
                tmpGameObject = new GameObject();
                tmpGameObject.hideFlags = HideFlags.HideAndDontSave;
                tmpGameObject.transform.parent = terrain.transform;
                transform = tmpGameObject.transform;

                //create a default texture if there is no distribution map
                if (map == null)
                {
                    map = new Texture2D(1, 1);
                    map.SetPixel(0, 0, Color.white);
                    map.Apply();
                    map.name = TEMP_MAP_NAME;
                }

                matrices = new List<Matrix4x4>();
                trisCount = data.triangles.Length / 3;
                for (int i = 0; i < trisCount; ++i)
                {
                    SampleTriangle(i);
                }

                Utilities.DestroyGameobject(tmpGameObject);
                //if the map is temporary, destroy it
                if (map != null && map.name.Equals(TEMP_MAP_NAME))
                    Utilities.DestroyObject(map);
                return matrices.ToArray();
            }

            /// <summary>
            /// Sample a triangle to create placement data
            /// </summary>
            /// <param name="trisIndex"></param>
            private void SampleTriangle(int trisIndex)
            {
                Vector3 p0 = data.vertices[data.triangles[trisIndex * 3]];
                Vector3 p1 = data.vertices[data.triangles[trisIndex * 3 + 1]];
                Vector3 p2 = data.vertices[data.triangles[trisIndex * 3 + 2]];

                //the triangle has no area
                if (p0 == p1 || p1 == p2 || p2 == p0)
                    return;

                Vector3 center = (p0 + p1 + p2) / 3;
                float radius = (p0 - center).magnitude;
                Vector3 normal = Vector3.Cross(p0 - center, p1 - center).normalized;
                Vector3 binormal = (p0 - center).normalized;

                if (normal == Vector3.zero || binormal == Vector3.zero)
                    return;

                float angle;
                float d;
                Vector3 dir = new Vector3();
                Vector3 point = new Vector3();
                Vector3 bary = new Vector3();
                Vector3 uv = new Vector2();
                float gray;

                List<Vector3> spawnPositions = new List<Vector3>();
                List<Vector3> barycentrics = new List<Vector3>();
                List<Vector2> uvCoords = new List<Vector2>();
                List<float> grayscales = new List<float>();

                //sample points inside the triangle
                for (int i = 0; i < density; ++i)
                {
                    angle = Random.value * 360;
                    d = Random.value * radius;
                    dir = Utilities.RotateVectorAroundNormal(binormal * d, normal, angle);
                    point = center + dir;
                    bary = new Vector3();
                    Utilities.CalculateBarycentricCoord(point, p0, p1, p2, ref bary);
                    //test if point is inside triangle
                    if (float.IsNaN(bary.x) || float.IsNaN(bary.y) || float.IsNaN(bary.z))
                        continue;
                    if (bary.x < 0 || bary.y < 0 || bary.z < 0)
                        continue;

                    //interpolate vertex data
                    uv =
                        data.uvCoords[data.triangles[trisIndex * 3]] * bary.x +
                        data.uvCoords[data.triangles[trisIndex * 3 + 1]] * bary.y +
                        data.uvCoords[data.triangles[trisIndex * 3 + 2]] * bary.z;
                    gray = map.GetPixelBilinear(uv.x, uv.y).grayscale;
                    gray = Mathf.Lerp(0, distributionThreshold, gray);

                    //use randomness to determine to spawn or not
                    if (Random.value > gray)
                        continue;

                    spawnPositions.Add(point);
                    barycentrics.Add(bary);
                    uvCoords.Add(uv);
                    grayscales.Add(gray);
                }

                //actually create placement data on successfully sampled points
                for (int i = 0; i < spawnPositions.Count; ++i)
                {
                    transform.localPosition = spawnPositions[i];
                    transform.localScale = Vector3.one * Mathf.Lerp(scaleMin, scaleMax, grayscales[i]);
                    if (followNormal)
                        transform.up = terrain.transform.TransformPoint(normal);
                    transform.Rotate(Vector3.up * Random.value * maxRotation, Space.Self);
                    Matrix4x4 matrix = new Matrix4x4();
                    matrix.SetRow(0, transform.localPosition);
                    Vector4 r = new Vector4(
                        transform.localRotation.x,
                        transform.localRotation.y,
                        transform.localRotation.z,
                        transform.localRotation.w);
                    matrix.SetRow(1, r);
                    matrix.SetRow(2, transform.localScale);
                    matrices.Add(matrix);
                }
            }
        }
    }
}
