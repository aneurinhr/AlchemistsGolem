using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using EditorSceneManager = UnityEditor.SceneManagement.EditorSceneManager;
#endif

namespace Pinwheel.PolarisBasic
{
    /// <summary>
    /// Painter tool to spawn object (tree, rock, etc.) onto the terrain
    /// </summary>
    public partial class EnvironmentalPainter
    {
        /// <summary>
        /// The terrain it associates to
        /// </summary>
        private TerrainGenerator terrain;
        public TerrainGenerator Terrain
        {
            get
            {
                return terrain;
            }
        }

        /// <summary>
        /// Brush settings
        /// </summary>
        [SerializeField]
        private ToolsSettings settings;
        public ToolsSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new ToolsSettings();
                }
                return settings;
            }
            set
            {
                settings = value ?? new ToolsSettings();
            }
        }

        /// <summary>
        /// Is the mask modified in the last paint action?
        /// </summary>
        private bool isMaskDirty = true;

        /// <summary>
        /// Internal mesh to visualize the mask
        /// A simplified version of the terrain, remove some triangles
        /// </summary>
        private Mesh maskVisualizer;
        private Mesh MaskVisualizer
        {
            get
            {
                if (maskVisualizer == null)
                {
                    maskVisualizer = new Mesh();
                    maskVisualizer.MarkDynamic();
                }
                return maskVisualizer;
            }
        }

        private MeshFilter maskVisualizerMf;
        public MeshFilter MaskVisualizerMf
        {
            get
            {
                if (maskVisualizerMf == null)
                {
                    string maskName = "_INTERNAL_Mask";
                    Transform oldMask = Terrain.transform.Find(maskName);
                    if (oldMask != null)
                    {
                        Utilities.DestroyGameobject(oldMask.gameObject);
                    }

                    GameObject g = new GameObject(maskName);
                    g.hideFlags = HideFlags.HideAndDontSave;
                    Utilities.ResetTransform(g.transform, Terrain.transform);

                    maskVisualizerMf = g.AddComponent<MeshFilter>();
                    maskVisualizerMf.sharedMesh = MaskVisualizer;

                    MeshRenderer mr = g.AddComponent<MeshRenderer>();
                    mr.sharedMaterial = MaterialUtilities.CyanSemiTransparentDiffuse;
                }
                return maskVisualizerMf;
            }
        }

        /// <summary>
        /// Internal game object to visualize the brush handle
        /// </summary>
        private GameObject sphereHandle;
        /// <summary>
        /// Internal game object to visualize the current selected prefab
        /// </summary>
        private GameObject previewer;

        private const string ENVIRONMENTAL_ELEMENTS_ROOT_NAME = "_INTERNAL_Environment";
        private const string FOLLOW_NORMAL_PREFIX = "n";

        public EnvironmentalPainter(TerrainGenerator terrain)
        {
            if (terrain == null)
                throw new System.ArgumentException("Terrain cannot be null");
            this.terrain = terrain;
            settings = new ToolsSettings();
            SetEnvironmentActive(true);
        }

        /// <summary>
        /// Get the root transform contain all environmental object
        /// </summary>
        /// <returns></returns>
        private Transform GetEnvironmentRoot()
        {
            Transform t = Utilities.GetChildrenWithName(Terrain.transform, ENVIRONMENTAL_ELEMENTS_ROOT_NAME);
            t.hideFlags = HideFlags.HideInHierarchy;
            return t;
        }

        /// <summary>
        /// Toggle environmental objects active state
        /// </summary>
        /// <param name="active"></param>
        public void SetEnvironmentActive(bool active)
        {
            GetEnvironmentRoot().gameObject.SetActive(active);
        }

        /// <summary>
        /// Get the name for the parent transform of a prefab's instances
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private string GetGroupName(GameObject prefab)
        {
            return string.Format("_INTERNAL_{0}_Group", prefab.name);
        }

        /// <summary>
        /// Get the parent transform contain a prefab's instances
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public Transform GetGroup(GameObject prefab)
        {
            string name = GetGroupName(prefab);
            Transform t = Utilities.GetChildrenWithName(GetEnvironmentRoot(), name);
            t.hideFlags = HideFlags.HideInHierarchy;
            return t;
        }

        /// <summary>
        /// Get the number of spawned instances
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public int GetGroupChildCount(GameObject prefab)
        {
            Transform group = GetGroup(prefab);
            return group.childCount;
        }

        /// <summary>
        /// Get the number of spawned instances
        /// </summary>
        /// <param name="prefabIndex"></param>
        /// <returns></returns>
        public int GetGroupChildCount(int prefabIndex)
        {
            GameObject prefab = Settings.GetPrefabs()[prefabIndex];
            return GetGroupChildCount(prefab);
        }

        /// <summary>
        /// Get the name for the transform contains all combinations for a prefab's instances
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private string GetCombinationsGroupName(GameObject prefab)
        {
            return string.Format("_INTERNAL_{0}_CombinationsGroup", prefab.name);
        }

        /// <summary>
        /// Get the name for a object group
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private string GetCombinationName(GameObject prefab)
        {
            return string.Format("_INTERNAL_{0}_Combination", prefab.name);
        }

        /// <summary>
        /// Get the transform contains all combinations for a prefab's instances
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public Transform GetCombinationsGroup(GameObject prefab)
        {
            string name = GetCombinationsGroupName(prefab);
            Transform t = Utilities.GetChildrenWithName(GetEnvironmentRoot(), name);
            t.hideFlags = HideFlags.HideInHierarchy;
            return t;
        }

        /// <summary>
        /// Get all combinations for a prefab's instances
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public Transform[] GetCombinations(GameObject prefab)
        {
            Transform group = GetCombinationsGroup(prefab);
            Transform[] combinations = Utilities.GetChildrenTransforms(group);
            return combinations;
        }

        /// <summary>
        /// Combine all instances of a prefab into one larger group
        /// </summary>
        /// <param name="prefabIndex"></param>
        public void CombinePrefabInstances(int prefabIndex)
        {
            try
            {
                GameObject prefab = Settings.GetPrefabs()[prefabIndex];
                Transform group = GetGroup(prefab);

                //combine nearby instances into groups
                MeshCombiner combiner = new MeshCombiner(
                    Utilities.GetChildrenGameObjects(group),
                    Settings.combinationPhysicalSize,
                    Settings.keepColliders,
                    Terrain.transform.worldToLocalMatrix);
                GameObject[] combination = combiner.Combine();

                //create new combination group or destroy old combinations if needed
                Transform combinationGroup = GetCombinationsGroup(prefab);
                Utilities.ClearChildren(combinationGroup);
                for (int i = 0; i < combination.Length; ++i)
                {
                    //attach the combination as a child
                    combination[i].gameObject.SetActive(true);
                    combination[i].name = GetCombinationName(prefab);
                    Utilities.ResetTransform(combination[i].transform, combinationGroup);
                }

                foreach (Transform child in group)
                {
                    //hide all instances
                    child.gameObject.SetActive(false);
                }
            }
            catch
            {
                Debug.LogWarning("Failed to combine prefab instances!");
            }
        }
        
        public void RemoveAllCombinationAndActivateInstances(int prefabIndex)
        {
            GameObject prefab = Settings.GetPrefabs()[prefabIndex];
            Transform[] combinations = GetCombinations(prefab);
            for (int i = 0; i < combinations.Length; ++i)
            {
                Utilities.DestroyGameobject(combinations[i].gameObject);
            }

            Transform group = GetGroup(prefab);
            Transform[] instances = Utilities.GetChildrenTransforms(group);
            for (int i = 0; i < instances.Length; ++i)
            {
                instances[i].gameObject.SetActive(true);
            }
            Utilities.MarkCurrentSceneDirty();
        }

        /// <summary>
        /// Perform a paint action at a specific location
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="currentEvent"></param>
        private void Paint(RaycastHit hit, Event currentEvent)
        {
            if (Settings.mode == Mode.Spawning)
            {
                PaintDetail(hit, currentEvent);
            }
            else
            {
                PaintMask(hit, currentEvent);
            }
        }

        /// <summary>
        /// Spawn or remove object on the terrain
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="currentEvent"></param>
        private void PaintDetail(RaycastHit hit, Event currentEvent)
        {
#if UNITY_EDITOR
            if (Settings.prefabIndex < 0 || Settings.prefabIndex >= Settings.prefabs.Count)
                return;
            if (Settings.prefabs[Settings.prefabIndex] == null)
                return;
            GameObject prefab = Settings.prefabs[Settings.prefabIndex];
            Transform group = GetGroup(prefab);

            bool willSpawn = GuiEventUtilities.IsLeftMouse && !GuiEventUtilities.IsCtrl;

            if (willSpawn)
            {
                group.gameObject.SetActive(true);
                float f = GuiEventUtilities.IsLeftMouseDown ? 0 : Random.value;
                int scaledUpCount = Mathf.RoundToInt(f * Settings.density);
                int spawnCount = Settings.density - scaledUpCount;
                //scale up spawned objects
                ScaleUpExistingDetail(scaledUpCount, group, hit.point);
                //spawn new objects
                Spawn(spawnCount, prefab, group, hit.point, hit.normal);
                Utilities.MarkCurrentSceneDirty();
            }

            bool willErase = GuiEventUtilities.IsLeftMouse && GuiEventUtilities.IsCtrl;
            if (willErase)
            {
                group.gameObject.SetActive(true);
                EraseDetail(prefab, group, hit.point);
                Utilities.MarkCurrentSceneDirty();
            }
#endif
        }

        /// <summary>
        /// Spawn a number of objects onto the terrain
        /// </summary>
        /// <param name="count"></param>
        /// <param name="prefab"></param>
        /// <param name="group"></param>
        /// <param name="position"></param>
        /// <param name="normal"></param>
        public void Spawn(int count, GameObject prefab, Transform group, Vector3 position, Vector3 normal)
        {
            for (int i = 0; i < count; ++i)
            {
                GameObject g = GameObject.Instantiate(prefab);
                g.hideFlags = HideFlags.HideInHierarchy;
                g.transform.parent = group;
                g.transform.up = GuiEventUtilities.IsShift ? normal : Vector3.up;
                g.transform.Rotate(Vector3.up * Random.value * Settings.maxRotation, Space.Self);
                g.transform.localScale = Settings.scaleMin * Vector3.one;
                g.transform.position = position + Random.insideUnitSphere * Settings.brushRadius;
                //mark the object as "follow normal" by append the FOLLOW_NORMAL_PREFIX into its name
                g.name = string.Format("{0}_{1}", GuiEventUtilities.IsShift ? FOLLOW_NORMAL_PREFIX : string.Empty, prefab.name);
                ProjectPositionOnTerrainOrDestroy(g.transform);
                
            }
        }

        /// <summary>
        /// Make the object "stick" to the terrain, if it cannot, destroy it
        /// </summary>
        /// <param name="t"></param>
        private void ProjectPositionOnTerrainOrDestroy(Transform t)
        {
            Vector3 ceil = t.position + t.up * 1000000f;
            Ray r = new Ray(ceil, -t.up);
            RaycastHit hit;
            if (Terrain.MeshColliderComponent.Raycast(r, out hit, float.MaxValue))
            {
                if (GetMask(hit.triangleIndex) == true)
                {
                    float rotationY = t.rotation.eulerAngles.y;
                    t.position = hit.point;
                    t.up = t.name.StartsWith(FOLLOW_NORMAL_PREFIX) ? hit.normal : Vector3.up;
                    t.Rotate(rotationY * Vector3.up, Space.Self);
                }
                else
                {
                    Utilities.DestroyGameobject(t.gameObject);
                }
            }
            else
            {
                Utilities.DestroyGameobject(t.gameObject);
            }
        }

        /// <summary>
        /// Perform a mask painting
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="currentEvent"></param>
        private void PaintMask(RaycastHit hit, Event currentEvent)
        {
            if (!GuiEventUtilities.IsLeftMouse)
                return;
            float sqrRadius = Settings.brushRadius * Settings.brushRadius;
            MeshData data = Terrain.MeshData;
            for (int i = 0; i < data.triangles.Length - 2; i += 3)
            {
                //determine if the triangle is in range
                Vector3 p0 = data.vertices[data.triangles[i]];
                Vector3 p1 = data.vertices[data.triangles[i + 1]];
                Vector3 p2 = data.vertices[data.triangles[i + 2]];
                Vector3 center = (p0 + p1 + p2) / 3;
                Vector3 worldCenter = Terrain.transform.TransformPoint(center);
                float sqrMag = (worldCenter - hit.point).sqrMagnitude;
                if (sqrMag > sqrRadius)
                {
                    continue;
                }

                //toggle mask state using triangle index i/3
                if (GuiEventUtilities.IsCtrl)
                {
                    SetMask(i / 3, false);
                    Utilities.MarkCurrentSceneDirty();
                }
                else
                {
                    SetMask(i / 3, true);
                    Utilities.MarkCurrentSceneDirty();
                }
            }
        }

        /// <summary>
        /// Destroy spawned object on the terrain
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="group"></param>
        /// <param name="position"></param>
        public void EraseDetail(GameObject prefab, Transform group, Vector3 position)
        {
            float sqrRadius = Settings.brushRadius * Settings.brushRadius;
            foreach (Transform child in group)
            {
                //grouped instances are not active in hierarchy, so don't destroy them
                if (!child.gameObject.activeInHierarchy)
                    continue;
                float sqrMag = (child.position - position).sqrMagnitude;
                if (sqrRadius >= sqrMag)
                {
                    Utilities.DestroyGameobject(child.gameObject);
                }
            }
        }

        /// <summary>
        /// Scale up spawned object on the terrain
        /// </summary>
        /// <param name="count"></param>
        /// <param name="group"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public int ScaleUpExistingDetail(int count, Transform group, Vector3 position)
        {
            float sqrRadius = Settings.brushRadius * Settings.brushRadius;
            foreach (Transform child in group)
            {
                float sqrMag = (child.position - position).sqrMagnitude;
                float strength = Mathf.Clamp01(1 - sqrMag / sqrRadius);
                if (strength > 0)
                {
                    float x = child.localScale.x;
                    float y = child.localScale.y;
                    float z = child.localScale.z;

                    x = Mathf.Clamp(x + strength * Utilities.DELTA_TIME * Settings.brushRadius, Settings.scaleMin, settings.scaleMax);
                    y = Mathf.Clamp(y + strength * Utilities.DELTA_TIME * Settings.brushRadius, Settings.scaleMin, settings.scaleMax);
                    z = Mathf.Clamp(z + strength * Utilities.DELTA_TIME * Settings.brushRadius, Settings.scaleMin, settings.scaleMax);

                    child.localScale = new Vector3(x, y, z);

                    if (count == 0)
                        break;
                }
            }
            return count;
        }

        /// <summary>
        /// Place a large number of object onto the terrain
        /// </summary>
        /// <param name="prefabIndex"></param>
        /// <param name="data">Tranform matrices</param>
        /// <param name="keepOldObjects"></param>
        public void MassPlacing(int prefabIndex, Matrix4x4[] data, bool keepOldObjects)
        {
            if (!keepOldObjects)
                ClearAllPrefabInstances(prefabIndex);
            GameObject prefab = Settings.GetPrefabs()[prefabIndex];
            Transform group = GetGroup(prefab);
            for (int i = 0; i < data.Length; ++i)
            {
                Matrix4x4 m = data[i];
                GameObject g = GameObject.Instantiate(prefab);
                g.hideFlags = HideFlags.HideInHierarchy;
                g.transform.parent = group;
                g.transform.localPosition = m.GetRow(0);
                Vector4 rotation = m.GetRow(1);
                g.transform.localRotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
                g.transform.localScale = m.GetRow(2);

            }
        }

        /// <summary>
        /// Get mask state for a triangle
        /// </summary>
        /// <param name="trisIndex"></param>
        /// <returns></returns>
        private bool GetMask(int trisIndex)
        {
            int trisCount = Terrain.MeshData.triangles.Length / 3;
            if (Settings.mask == null ||
                Settings.mask.Length == 0 ||
                Settings.mask.Length != trisCount)
            {
                Settings.mask = new bool[trisCount];
                Utilities.Fill(Settings.mask, true);
                isMaskDirty = true;
            }
            return Settings.mask[trisIndex];
        }

        /// <summary>
        /// Set mask state for a triangle
        /// </summary>
        /// <param name="trisIndex"></param>
        /// <param name="value"></param>
        private void SetMask(int trisIndex, bool value)
        {
            int trisCount = Terrain.MeshData.triangles.Length / 3;
            if (Settings.mask == null ||
                Settings.mask.Length == 0 ||
                Settings.mask.Length != trisCount)
            {
                Settings.mask = new bool[trisCount];
                Utilities.Fill(Settings.mask, true);
            }
            Settings.mask[trisIndex] = value;
            isMaskDirty = true;
        }

        /// <summary>
        /// Generate a simple mesh data to create the mask visualizer
        /// </summary>
        /// <returns></returns>
        private MeshData CreateMaskVisualizerMeshData()
        {
            MeshData src = Terrain.MeshData;
            MeshData des = new MeshData();
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

            int trisIndex = 0;
            int trisCount = src.triangles.Length / 3;
            for (int i = 0; i < trisCount; ++i)
            {
                if (GetMask(i) == false)
                    continue;
                verts.Add(src.vertices[src.triangles[i * 3]]);
                verts.Add(src.vertices[src.triangles[i * 3 + 1]]);
                verts.Add(src.vertices[src.triangles[i * 3 + 2]]);

                tris.Add(trisIndex++);
                tris.Add(trisIndex++);
                tris.Add(trisIndex++);
            }

            des.vertices = verts.ToArray();
            des.triangles = tris.ToArray();

            return des;
        }

        public void ShowMask()
        {
            MaskVisualizerMf.gameObject.SetActive(true);
        }

        public void HideMask()
        {
            MaskVisualizerMf.gameObject.SetActive(false);
        }

        /// <summary>
        /// Get all spawned instances for a prefab
        /// </summary>
        /// <param name="prefabIndex"></param>
        /// <returns></returns>
        public GameObject[] GetAllPrefabInstances(int prefabIndex)
        {
            GameObject prefab = Settings.GetPrefabs()[prefabIndex];
            Transform group = GetGroup(prefab);
            GameObject[] instances = new GameObject[group.childCount];
            int i = 0;
            foreach (Transform child in group)
            {
                instances[i] = child.gameObject;
                i += 1;
            }
            return instances;
        }

        public void SetAllPrefabInstancesActive(int prefabIndex, bool active, bool alsoSetCombination = true)
        {
            GameObject prefab = Settings.GetPrefabs()[prefabIndex];
            Transform group = GetGroup(prefab);
            group.gameObject.SetActive(active);

            if (alsoSetCombination)
            {
                Transform combinationGroup = GetCombinationsGroup(prefab);
                combinationGroup.gameObject.SetActive(active);
            }

            Utilities.MarkCurrentSceneDirty();
        }

        public bool IsGroupActive(int prefabIndex)
        {
            GameObject prefab = Settings.GetPrefabs()[prefabIndex];
            Transform group = GetGroup(prefab);
            Transform combinationGroup = GetCombinationsGroup(prefab);
            return group.gameObject.activeInHierarchy && combinationGroup.gameObject.activeInHierarchy;
        }

        public void ClearAllPrefabInstances(int prefabIndex, bool alsoClearCombination = true)
        {
            GameObject prefab = Settings.GetPrefabs()[prefabIndex];
            Transform group = GetGroup(prefab);
            Utilities.DestroyGameobject(group.gameObject);
            if (alsoClearCombination)
            {
                Transform combinationGroup = GetCombinationsGroup(prefab);
                Utilities.DestroyGameobject(combinationGroup.gameObject);
            }
            Utilities.MarkCurrentSceneDirty();
        }

        public void DrawSceneGUI()
        {
#if UNITY_EDITOR
            DrawCombinationsBounds();
            DrawMask();
            DrawHandles();
            DrawOverlayInstruction();
#endif
        }

        private void DrawCombinationsBounds()
        {
#if UNITY_EDITOR
            if (!Settings.drawCombinationsBounds)
                return;
            Handles.color = ColorLibrary.GetColor(Color.yellow, 0.5f);
            List<GameObject> prefabs = Settings.GetPrefabs();
            for (int prefabIndex = 0; prefabIndex < prefabs.Count; ++prefabIndex)
            {
                Transform[] combinations = GetCombinations(prefabs[prefabIndex]);
                for (int combIndex = 0; combIndex < combinations.Length; ++combIndex)
                {
                    MeshRenderer[] mr = combinations[combIndex].GetComponentsInChildren<MeshRenderer>();
                    for (int mrIndex = 0; mrIndex < mr.Length; ++mrIndex)
                    {
                        Bounds b = mr[mrIndex].bounds;
                        Handles.DrawWireCube(b.center, b.size);
                    }
                }
            }
#endif
        }

        private void DrawMask()
        {
#if UNITY_EDITOR
            if (Settings.mode == Mode.Masking)
            {
                ShowMask();
                if (isMaskDirty || Terrain.MeshData.vertices.Length != MaskVisualizer.vertexCount)
                {
                    MeshData data = CreateMaskVisualizerMeshData();
                    MaskVisualizer.Clear();
                    MaskVisualizer.vertices = data.vertices;
                    MaskVisualizer.triangles = data.triangles;
                    MaskVisualizerMf.sharedMesh = MaskVisualizer;
                    isMaskDirty = false;
                }

                Handles.BeginGUI();
                GUI.DrawTexture(
                new Rect(0, 0, Screen.width, Screen.height),
                    TextureUtilities.VignetteTexture);
                GUILayout.Label("  Mask editing", GuiStyleUtilities.BigBoldWhiteLabel);
                Handles.EndGUI();
            }
            else
            {
                HideMask();
            }
#endif
        }

        private void DrawHandles()
        {
#if UNITY_EDITOR
            if (Event.current == null)
                return;

            if (Event.current.type != EventType.Repaint &&
                Event.current.type != EventType.MouseDown &&
                Event.current.type != EventType.MouseDrag &&
                Event.current.type != EventType.MouseUp &&
                Event.current.type != EventType.KeyDown)
                return;

            int controlId = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive);
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    Tools.hidden = true;
                    //Set the hot control to this tool, to disable marquee selection tool on mouse dragging
                    GUIUtility.hotControl = controlId;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                if (GUIUtility.hotControl == controlId)
                {
                    Tools.hidden = false;
                    //Return the hot control back to Unity, use the default
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                }
            }
            else if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    Terrain.EnableEnvironmentalPainter = false;
                    Tools.hidden = false;
                }
                if (Event.current.keyCode == KeyCode.Tab)
                {
                    //toggle mask painting mode
                    Settings.mode = (Mode)(1 - (int)Settings.mode);
                }
            }

            //increase/decrease brush size shortcut
            if (GuiEventUtilities.IsPlus && !GuiEventUtilities.IsShift)
            {
                Settings.brushRadius += Utilities.DELTA_TIME * (Settings.brushRadius == 0 ? 1 : Settings.brushRadius);
                Settings.Validate();
            }
            if (GuiEventUtilities.IsMinus && !GuiEventUtilities.IsShift)
            {
                Settings.brushRadius -= Utilities.DELTA_TIME * (Settings.brushRadius == 0 ? 1 : Settings.brushRadius);
                Settings.Validate();
            }

            //increase/decrease object scale shortcut
            if (GuiEventUtilities.IsPlus && GuiEventUtilities.IsShift)
            {
                Settings.scaleMin += Utilities.DELTA_TIME * (Settings.scaleMin == 0 ? 1 : Settings.scaleMin);
                Settings.scaleMax += Utilities.DELTA_TIME * (Settings.scaleMax == 0 ? 1 : Settings.scaleMax);
                Settings.Validate();
            }
            if (GuiEventUtilities.IsMinus && GuiEventUtilities.IsShift)
            {
                Settings.scaleMin -= Utilities.DELTA_TIME * (Settings.scaleMin == 0 ? 1 : Settings.scaleMin);
                Settings.scaleMax -= Utilities.DELTA_TIME * (Settings.scaleMax == 0 ? 1 : Settings.scaleMax);
                Settings.Validate();
            }

            Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (Terrain.MeshColliderComponent.Raycast(r, out hit, float.MaxValue))
            {
                DrawHandlesAtCursor(hit);
                DrawPreviewAtCursor(hit);
                Paint(hit, Event.current);
            }
            else
            {
                HidePreview();
                HideHandle();
            }

            if (Event.current.type == EventType.MouseDown &&
                Event.current.button == 0)
            {
                Event.current.Use();
            }
#endif
        }

        private void DrawHandlesAtCursor(RaycastHit hit)
        {
#if UNITY_EDITOR
            if (sphereHandle == null)
                UpdateHandle();
            Material mat = MaterialUtilities.IntersectionHighlight;
            Color color = GuiEventUtilities.IsCtrl ? ColorLibrary.RedSemiTransparent : ColorLibrary.CyanSemiTransparent;
            mat.SetColor("_Color", color);
            Color intersectColor = GuiEventUtilities.IsCtrl ? ColorLibrary.Red : ColorLibrary.Cyan;
            mat.SetColor("_IntersectColor", intersectColor);
            sphereHandle.transform.position = hit.point;
            sphereHandle.transform.localScale = 2 * Settings.brushRadius * Vector3.one;
            sphereHandle.transform.up = hit.normal;
            sphereHandle.gameObject.SetActive(true);

            if (GuiEventUtilities.IsShift && !GuiEventUtilities.IsCtrl && Settings.mode == Mode.Spawning)
            {
                Handles.color = ColorLibrary.Cyan;
                Quaternion rotation = Quaternion.LookRotation(hit.normal);
                Handles.ArrowHandleCap(0, hit.point, rotation, Settings.brushRadius, Event.current.type);
            }

            //brush handle will not update correctly if AnimatedMaterial option is disabled in Scene view
            //in such case, we update the Scene view manually
            foreach (SceneView sv in SceneView.sceneViews)
            {
                //the sceneViewState property is not exposed yet in 5.6.1
                FieldInfo field = sv.GetType().GetField("m_SceneViewState", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    SceneView.SceneViewState state = field.GetValue(sv) as SceneView.SceneViewState;
                    if (!state.showMaterialUpdate)
                    {
                        sv.Repaint();
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Update previewer game object for currently selected prefab
        /// </summary>
        public void UpdatePreviewer()
        {
#if UNITY_EDITOR
            string name = "_INTERNAL_Previewer";
            Transform oldPreviewer = Terrain.transform.Find(name);
            if (oldPreviewer != null)
            {
                Utilities.DestroyGameobject(oldPreviewer.gameObject);
            }

            if (settings.prefabIndex < 0 || settings.prefabIndex >= settings.GetPrefabs().Count)
                return;

            previewer = GameObject.Instantiate(settings.GetPrefabs()[settings.prefabIndex]);
            previewer.name = name;
            Utilities.ResetTransform(previewer.transform, Terrain.transform);
            previewer.hideFlags = HideFlags.HideAndDontSave;

            //override it material
            MeshRenderer[] mrs = previewer.GetComponentsInChildren<MeshRenderer>(true);
            for (int i = 0; i < mrs.Length; ++i)
            {
                mrs[i].sharedMaterial = MaterialUtilities.CyanSemiTransparentDiffuse;
            }

            //remove unneccessary components
            Component[] components = previewer.GetComponentsInChildren<Component>(true);
            for (int i = 0; i < components.Length; ++i)
            {

                bool dontDestroy =
                   components[i] is Transform ||
                   components[i] is MeshFilter ||
                   components[i] is MeshRenderer;
                if (!dontDestroy)
                    Utilities.DestroyObject(components[i]);
            }
#endif
        }

        public void UpdateHandle()
        {
#if UNITY_EDITOR
            string name = "_INTERNAL_SphereHandle";
            Transform oldHandle = Terrain.transform.Find(name);
            if (oldHandle != null)
            {
                Utilities.DestroyGameobject(oldHandle.gameObject);
            }

            sphereHandle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereHandle.name = name;
            Utilities.ResetTransform(sphereHandle.transform, Terrain.transform);
            sphereHandle.hideFlags = HideFlags.HideAndDontSave;

            //override its material
            MeshRenderer mr = sphereHandle.GetComponent<MeshRenderer>();
            mr.sharedMaterial = MaterialUtilities.IntersectionHighlight;

            //remove unneccessary components
            Component[] components = sphereHandle.GetComponentsInChildren<Component>(true);
            for (int i = 0; i < components.Length; ++i)
            {

                bool dontDestroy =
                   components[i] is Transform ||
                   components[i] is MeshFilter ||
                   components[i] is MeshRenderer;
                if (!dontDestroy)
                    Utilities.DestroyObject(components[i]);
            }
#endif
        }

        private void DrawPreviewAtCursor(RaycastHit hit)
        {
            if (previewer == null)
            {
                if (Settings.prefabIndex >= 0 && Settings.prefabIndex < Settings.GetPrefabs().Count)
                {
                    UpdatePreviewer();
                }
                else
                {
                    return;
                }
            }
            previewer.gameObject.SetActive(
                !GuiEventUtilities.IsCtrl &&
                GetMask(hit.triangleIndex) &&
                Settings.mode != Mode.Masking);
            previewer.transform.position = hit.point;
            previewer.transform.localScale = Settings.scaleMax * Vector3.one;
            if (GuiEventUtilities.IsShift)
            {
                previewer.transform.up = hit.normal;
            }
            else
            {
                previewer.transform.up = Vector3.up;
            }
        }

        public void HidePreview()
        {
#if UNITY_EDITOR
            if (previewer != null)
                previewer.gameObject.SetActive(false);
            foreach (SceneView sv in SceneView.sceneViews)
            {
                sv.Repaint();
            }
#endif
        }

        public void HideHandle()
        {
#if UNITY_EDITOR
            if (sphereHandle != null)
                sphereHandle.gameObject.SetActive(false);
            //brush handle will not update correctly if AnimatedMaterial option is disabled in Scene view
            //in such case, we update the Scene view manually
            foreach (SceneView sv in SceneView.sceneViews)
            {
                //the sceneViewState property is not exposed yet in 5.6.1
                FieldInfo field = sv.GetType().GetField("m_SceneViewState", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    SceneView.SceneViewState state = field.GetValue(sv) as SceneView.SceneViewState;
                    if (!state.showMaterialUpdate)
                    {
                        sv.Repaint();
                    }
                }
            }
#endif
        }

        public void DrawOverlayInstruction()
        {
#if UNITY_EDITOR
            Vector2 guiSize = new Vector2(200, 87);
            Vector2 guiPosition = new Vector2(Screen.width - guiSize.x, Screen.height - guiSize.y);
            Rect r = new Rect(guiPosition, guiSize);
            Handles.BeginGUI();
            GUILayout.BeginArea(r);

            string tabText = GuiEventUtilities.IsButtonPressed(KeyCode.Tab) ? "<b>Tab</b>" : "Tab";
            string modeText = Settings.mode == Mode.Spawning ? "Mask" : "Spawn";
            string shiftText = GuiEventUtilities.IsShift ? "<b>Shift</b>" : "Shift";
            string ctrlText = GuiEventUtilities.IsCtrl ? "<b>Ctrl</b>" : "Ctrl";

            string instruction = string.Format(
                "Press {0} to enter {1} mode\n" +
                "Hold {2} to follow normals\n" +
                "Hold {3} to erase\n",
                tabText, modeText,
                shiftText,
                ctrlText);
            GUILayout.Box(instruction, GuiStyleUtilities.OverlayInstructionStyle);
            GUILayout.EndArea();
            Handles.EndGUI();
#endif
        }

        /// <summary>
        /// Update environmental objects position when geometry changed
        /// All grouped instances will be un-grouped
        /// </summary>
        public void UpdateEnvironmentOnGeometryChanged()
        {
            bool isCombinationsBroken = false;
            for (int i = 0; i < Settings.GetPrefabs().Count; ++i)
            {
                isCombinationsBroken = GetCombinationsGroup(Settings.GetPrefabs()[i]).childCount > 0;
                UpdateInstancesOnGeometryChanged(i);
                if (isCombinationsBroken)
                {
                    CombinePrefabInstances(i);
                }
            }
        }

        private void UpdateInstancesOnGeometryChanged(int prefabIndex)
        {
            GameObject prefab = Settings.GetPrefabs()[prefabIndex];
            Transform group = GetGroup(prefab);
            Transform[] child = Utilities.GetChildrenTransforms(group);
            for (int i = 0; i < child.Length; ++i)
            {
                ProjectPositionOnTerrainOrDestroy(child[i]);
            }

            RemoveAllCombinationAndActivateInstances(prefabIndex);
        }

        public void HardReset()
        {
            Transform root = GetEnvironmentRoot();
            Utilities.DestroyGameobject(root.gameObject);
            Settings.GetPrefabs().Clear();
        }
    }
}
