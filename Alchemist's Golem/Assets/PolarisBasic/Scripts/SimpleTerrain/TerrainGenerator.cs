using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thread = System.Threading.Thread;

namespace Pinwheel.PolarisBasic
{
    /// <summary>
    /// Main class to create terrain
    /// </summary>
    [ExecuteInEditMode]
    public partial class TerrainGenerator : MonoBehaviour
    {
        #region Variables & Properties
#if UNITY_EDITOR
        /// <summary>
        /// Store inspector foldout expanded/collapsed state
        /// </summary>
        [HideInInspector]
        public InspectorUtilities inspector;
#endif

        [SerializeField]
        private MeshFilter meshFilterComponent;
        public MeshFilter MeshFilterComponent
        {
            get
            {
                return meshFilterComponent;
            }
            set
            {
                meshFilterComponent = value;
            }
        }

        [SerializeField]
        private MeshRenderer meshRendererComponent;
        public MeshRenderer MeshRendererComponent
        {
            get
            {
                return meshRendererComponent;
            }
            set
            {
                meshRendererComponent = value;
            }
        }

        [SerializeField]
        private MeshCollider meshColliderComponent;
        public MeshCollider MeshColliderComponent
        {
            get
            {
                if (meshColliderComponent == null)
                {
                    meshColliderComponent = GetComponent<MeshCollider>();
                    if (meshColliderComponent == null)
                    {
                        meshColliderComponent = gameObject.AddComponent<MeshCollider>();
                        meshColliderComponent.sharedMesh = MeshFilterComponent.sharedMesh;
                    }
                }
                return meshColliderComponent;
            }
            set
            {
                meshColliderComponent = value;
            }
        }

        /// <summary>
        /// If turn on, the terrain will update when something changed in the inspector/editor
        /// </summary>
        [SerializeField]
        private bool updateImmediately;
        public bool UpdateImmediately
        {
            get
            {
                return updateImmediately;
            }
            set
            {
                updateImmediately = value;
            }
        }

        [SerializeField]
        private bool showFullHierarchy;
        public bool ShowFullHierarchy
        {
            get
            {
                return showFullHierarchy;
            }
            set
            {
                showFullHierarchy = value;
                //foreach (Transform t in transform)
                //{
                //    t.gameObject.hideFlags = showFullHierarchy ? HideFlags.None : HideFlags.HideInHierarchy;
                //}
            }
        }

        /// <summary>
        /// Surface tiling mode
        /// </summary>
        [SerializeField]
        private SurfaceTilingMode tilingMode;
        public SurfaceTilingMode TilingMode
        {
            get
            {
                return tilingMode;
            }
            set
            {
                tilingMode = value;
            }
        }

        /// <summary>
        /// How many tiles on the surface on X-axis
        /// </summary>
        [SerializeField]
        private int surfaceTileCountX;
        public int SurfaceTileCountX
        {
            get
            {
                return surfaceTileCountX;
            }
            set
            {
                surfaceTileCountX = Mathf.Max(1, value);
            }
        }

        /// <summary>
        /// How many tiles on the surface on Z-axis
        /// </summary>
        [SerializeField]
        private int surfaceTileCountZ;
        public int SurfaceTileCountZ
        {
            get
            {
                return surfaceTileCountZ;
            }
            set
            {
                surfaceTileCountZ = Mathf.Max(1, value);
            }
        }

        /// <summary>
        /// Distance between 2 vertices
        /// </summary>
        [SerializeField]
        private float vertexSpacing;
        public float VertexSpacing
        {
            get
            {
                return vertexSpacing;
            }
            set
            {
                vertexSpacing = value;
            }
        }

        /// <summary>
        /// A grayscale map to sample height
        /// </summary>
        [SerializeField]
        private Texture2D heightMap;
        public Texture2D HeightMap
        {
            get
            {
                return heightMap;
            }
            set
            {
                heightMap = value;
            }
        }

        /// <summary>
        /// Height of the base (underground part)
        /// </summary>
        [SerializeField]
        private float baseHeight;
        public float BaseHeight
        {
            get
            {
                return baseHeight;
            }
            set
            {
                baseHeight = Mathf.Max(0, value);
            }
        }

        /// <summary>
        /// Maximum height of the surface vertices
        /// </summary>
        [SerializeField]
        private float surfaceMaxHeight;
        public float SurfaceMaxHeight
        {
            get
            {
                return surfaceMaxHeight;
            }
            set
            {
                surfaceMaxHeight = Mathf.Max(0, value);
            }
        }

        /// <summary>
        /// Adding some randomness to the surface using Perlin noise
        /// </summary>
        [SerializeField]
        private float roughness;
        public float Roughness
        {
            get
            {
                return roughness;
            }
            set
            {
                roughness = value;
            }
        }

        /// <summary>
        /// A number to generate random noise, different seed generate different noise pattern
        /// </summary>
        [SerializeField]
        private float roughnessSeed;
        public float RoughnessSeed
        {
            get
            {
                return roughnessSeed;
            }
            set
            {
                roughnessSeed = value;
            }
        }

        /// <summary>
        /// The thickness of surface layer
        /// </summary>
        [SerializeField]
        private float groundThickness;
        public float GroundThickness
        {
            get
            {
                return groundThickness;
            }
            set
            {
                groundThickness = Mathf.Clamp(value, 0, BaseHeight);
            }
        }

        /// <summary>
        /// Variation rate of ground thickness across the surface
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        private float groundThicknessVariation;
        public float GroundThicnknessVariation
        {
            get
            {
                return groundThicknessVariation;
            }
            set
            {
                groundThicknessVariation = Mathf.Clamp01(value);
            }
        }

        /// <summary>
        /// A number to sample noise, different seed generate different thickness variation
        /// </summary>
        [SerializeField]
        private float groundThicknessVariationSeed;
        public float GroundThicknessVariationSeed
        {
            get
            {
                return groundThicknessVariationSeed;
            }
            set
            {
                groundThicknessVariationSeed = value;
            }
        }

        /// <summary>
        /// Control thickness variation smoothness/roughness
        /// </summary>
        [SerializeField]
        private float groundThicknessNoiseStep;
        public float GroundThicknessNoiseStep
        {
            get
            {
                return groundThicknessNoiseStep;
            }
            set
            {
                groundThicknessNoiseStep = value;
            }
        }

        /// <summary>
        /// The marerial set by user
        /// </summary>
        [SerializeField]
        private Material customMaterial;
        public Material CustomMaterial
        {
            get
            {
                return customMaterial;
            }
            set
            {
                customMaterial = value;
            }
        }

        /// <summary>
        /// Built-in diffuse material
        /// </summary>
        [SerializeField]
        private Material diffuseMaterial;
        public Material DiffuseMaterial
        {
            get
            {
                if (diffuseMaterial == null)
                {
                    diffuseMaterial = new Material(Shader.Find("Pinwheel/LambertVertexColor"));
                }
                return diffuseMaterial;
            }
        }

        /// <summary>
        /// Built-in specular material
        /// </summary>
        [SerializeField]
        private Material specularMaterial;
        public Material SpecularMaterial
        {
            get
            {
                if (specularMaterial == null)
                {
                    specularMaterial = new Material(Shader.Find("Pinwheel/BlinnPhongVertexColor"));
                }
                return specularMaterial;
            }
        }

        /// <summary>
        /// Determine type of material to use
        /// </summary>
        [SerializeField]
        private MaterialTypes materialType;
        public MaterialTypes MaterialType
        {
            get
            {
                return materialType;
            }
            set
            {
                materialType = value;
            }
        }

        /// <summary>
        /// The actual material to send to render
        /// </summary>
        [SerializeField]
        private Material toRenderMaterial;
        public Material ToRenderMaterial
        {
            get
            {

                return
                    materialType == MaterialTypes.Diffuse ? DiffuseMaterial :
                    materialType == MaterialTypes.Specular ? SpecularMaterial :
                    CustomMaterial != null ? CustomMaterial : DiffuseMaterial;
            }
        }

        /// <summary>
        /// Determine color of a vertex based on its height
        /// </summary>
        [SerializeField]
        private Gradient colorByHeight;
        public Gradient ColorByHeight
        {
            get
            {
                if (colorByHeight == null)
                    colorByHeight = new Gradient();
                return colorByHeight;
            }
            set
            {
                colorByHeight = value;
            }
        }

        /// <summary>
        /// Determine color of a vertex based on its normal vector
        /// </summary>
        [SerializeField]
        private Gradient colorByNormal;
        public Gradient ColorByNormal
        {
            get
            {
                if (colorByNormal == null)
                    colorByNormal = new Gradient();
                return colorByNormal;
            }
            set
            {
                colorByNormal = value;
            }
        }

        /// <summary>
        /// A fraction to blend between ColorByHeight & ColorByNormal, take the alpha channel
        /// </summary>
        [SerializeField]
        private Gradient colorBlendFraction;
        public Gradient ColorBlendFraction
        {
            get
            {
                if (colorBlendFraction == null)
                    colorBlendFraction = new Gradient();
                return colorBlendFraction;
            }
            set
            {
                colorBlendFraction = value;
            }
        }

        /// <summary>
        /// Color of the underground part
        /// </summary>
        [SerializeField]
        private Color undergroundColor;
        public Color UndergroundColor
        {
            get
            {
                return undergroundColor;
            }
            set
            {
                undergroundColor = value;
                if (undergroundColor == Color.clear)
                    undergroundColor.a = 1.0f / 255;
            }
        }

        /// <summary>
        /// Determine the shading model
        /// </summary>
        [SerializeField]
        private bool useFlatShading;
        //flat shading is forced disable when using geometry painter in some condition
        private bool forceDisableFlatShading;
        public bool UseFlatShading
        {
            get
            {
                return useFlatShading && !forceDisableFlatShading;
            }
            set
            {
                useFlatShading = value;
            }
        }

        /// <summary>
        /// Should it generate vertex color or not
        /// </summary>
        [SerializeField]
        private bool useVertexColor;
        public bool UseVertexColor
        {
            get
            {
                return useVertexColor;
            }
            set
            {
                useVertexColor = value;
            }
        }

        /// <summary>
        /// If on, color will not be blended across a triangle
        /// </summary>
        [SerializeField]
        private bool useSolidFaceColor;
        public bool UseSolidFaceColor
        {
            get
            {
                return useSolidFaceColor;
            }
            set
            {
                useSolidFaceColor = value;
            }
        }

        /// <summary>
        /// Should it apply a bilinear filter when sample height map?
        /// </summary>
        [SerializeField]
        private bool shouldReduceSurfaceColorNoise;
        public bool ShouldReduceSurfaceColorNoise
        {
            get
            {
                return shouldReduceSurfaceColorNoise;
            }
            set
            {
                shouldReduceSurfaceColorNoise = value;
            }
        }

        /// <summary>
        /// Should recalculate mesh bound after generated?
        /// </summary>
        [SerializeField]
        private bool shouldRecalculateBounds;
        public bool ShouldRecalculateBounds
        {
            get
            {
                return shouldRecalculateBounds;
            }
            set
            {
                shouldRecalculateBounds = value;
            }
        }

        /// <summary>
        /// Should it recalculate normal vectors after generated?
        /// </summary>
        [SerializeField]
        private bool shouldRecalculateNormals;
        public bool ShouldRecalculateNormals
        {
            get
            {
                return shouldRecalculateNormals;
            }
            set
            {
                shouldRecalculateNormals = value;
            }
        }

        /// <summary>
        /// Should it recalculate tangent vectors after generated
        /// </summary>
        [SerializeField]
        private bool shouldRecalculateTangents;
        public bool ShouldRecalculateTangents
        {
            get
            {
                return shouldRecalculateTangents;
            }
            set
            {
                shouldRecalculateTangents = value;
            }
        }

        /// <summary>
        /// Should it unwrap mesh uv?
        /// </summary>
        [SerializeField]
        private bool shouldUnwrapUv;
        public bool ShouldUnwrapUv
        {
            get
            {
                return shouldUnwrapUv;
            }
            set
            {
                shouldUnwrapUv = value;
            }
        }

        /// <summary>
        /// Should it generate the underground part?
        /// </summary>
        [SerializeField]
        private bool shouldGenerateUnderground;
        public bool ShouldGenerateUnderground
        {
            get
            {
                return shouldGenerateUnderground;
            }
            set
            {
                shouldGenerateUnderground = value;
            }
        }

        /// <summary>
        /// Should it enclose the mesh volume
        /// </summary>
        [SerializeField]
        private bool shouldEncloseBottomPart;
        public bool ShouldEncloseBottomPart
        {
            get
            {
                return shouldEncloseBottomPart;
            }
            set
            {
                shouldEncloseBottomPart = value;
            }
        }

        /// <summary>
        /// Enable vertex warper?
        /// </summary>
        [SerializeField]
        private bool useVertexWarping;
        public bool UseVertexWarping
        {
            get
            {
                return useVertexWarping;
            }
            set
            {
                useVertexWarping = value;
            }
        }

        /// <summary>
        /// Draw or hide vertex warper GUI in Scene view
        /// </summary>
        [SerializeField]
        private bool showVertexWarperSceneGUI;
        public bool ShowVertexWarperSceneGUI
        {
            get
            {
                return showVertexWarperSceneGUI;
            }
            set
            {
                showVertexWarperSceneGUI = value;
            }
        }

        /// <summary>
        /// The current warping template
        /// </summary>
        [SerializeField]
        private VertexWarper.Template warpingTemplate;
        public VertexWarper.Template WarpingTemplate
        {
            get
            {
                return warpingTemplate;
            }
            set
            {
                warpingTemplate = value;
            }
        }

        /// <summary>
        /// Size of the entire vertices grid
        /// </summary>
        public Index2D VerticesGridLength
        {
            get
            {
                int x = SurfaceTileCountX + 1 + SurfaceInset * 2;
                int z = SurfaceTileCountZ + 1 + SurfaceInset * 2;
                return new Index2D(x, z);
            }
        }

        /// <summary>
        /// Index of the bottom-left corner vertex on surface
        /// </summary>
        public Index2D SurfaceMinIndex
        {
            get
            {
                int x = SurfaceInset;
                int z = SurfaceInset;
                return new Index2D(x, z);
            }
        }

        /// <summary>
        /// Index of the top-right corner vertex on surface
        /// </summary>
        public Index2D SurfaceMaxIndex
        {
            get
            {
                int x = VerticesGridLength.X - 1 - SurfaceInset;
                int z = VerticesGridLength.Z - 1 - SurfaceInset;
                return new Index2D(x, z);
            }
        }

        /// <summary>
        /// Total tile count on X-axis (including surface tiles and underground tiles)
        /// </summary>
        public int TotalTileCountX
        {
            get
            {
                return SurfaceTileCountX + 2 * SurfaceInset;
            }
        }

        /// <summary>
        /// Total tile count on Z-axis (including surface tiles and underground tiles)
        /// </summary>
        public int TotalTileCountZ
        {
            get
            {
                return SurfaceTileCountZ + 2 * SurfaceInset;
            }
        }

        /// <summary>
        /// Surface padding factor
        /// </summary>
        public int SurfaceInset
        {
            get
            {
                return ShouldGenerateUnderground ? SURFACE_INSET : SURFACE_INSET_NO_UNDERGROUND;
            }
        }

        private VertexWarper vertexWarper;
        private VertexWarper VertexWarper
        {
            get
            {
                if (vertexWarper == null)
                {
                    vertexWarper = new VertexWarper();
                    vertexWarper.Transform = transform;
                    vertexWarper.BezierChanged += OnVertexWarperBezierChanged;
                }
                return vertexWarper;
            }
        }

        [SerializeField]
        private bool enableEnvironmentalPainter;
        public bool EnableEnvironmentalPainter
        {
            get
            {
                return enableEnvironmentalPainter;
            }
            set
            {
                enableEnvironmentalPainter = value;
            }
        }

        private EnvironmentalPainter environmentalPainter;
        public EnvironmentalPainter EnvironmentalPainter
        {
            get
            {
                if (environmentalPainter == null)
                {
                    environmentalPainter = new EnvironmentalPainter(this);
                }
                return environmentalPainter;
            }
        }

        [SerializeField]
        private EnvironmentalPainter.ToolsSettings environmentalPainterSettings;
        public EnvironmentalPainter.ToolsSettings EnvironmentalPainterSettings
        {
            get
            {
                if (environmentalPainterSettings == null)
                {
                    environmentalPainterSettings = new EnvironmentalPainter.ToolsSettings();
                }
                return environmentalPainterSettings;
            }
            set
            {
                environmentalPainterSettings = value;
                if (environmentalPainterSettings != null)
                {
                    environmentalPainterSettings.Validate();
                    EnvironmentalPainter.Settings = environmentalPainterSettings;
                }
            }
        }

        [SerializeField]
        private bool enableGeometryPainter;
        public bool EnableGeometryPainter
        {
            get
            {
                return enableGeometryPainter;
            }
            set
            {
                enableGeometryPainter = value;
            }
        }

        [SerializeField]
        private GeometryPainter.ToolsSettings geometryPainterSettings;
        public GeometryPainter.ToolsSettings GeometryPainterSettings
        {
            get
            {
                if (geometryPainterSettings == null)
                {
                    geometryPainterSettings = new GeometryPainter.ToolsSettings();
                }
                return geometryPainterSettings;
            }
            set
            {
                geometryPainterSettings = value;
                if (geometryPainterSettings != null)
                {
                    geometryPainterSettings.Validate();
                    GeometryPainter.Settings = geometryPainterSettings;
                }
            }
        }

        private GeometryPainter geometryPainter;
        public GeometryPainter GeometryPainter
        {
            get
            {
                if (geometryPainter == null)
                {
                    geometryPainter = new GeometryPainter(this);
                }
                return geometryPainter;
            }
        }

        [SerializeField]
        private bool enableColorPainter;
        public bool EnableColorPainter
        {
            get
            {
                return enableColorPainter;
            }
            set
            {
                enableColorPainter = value;
            }
        }

        [SerializeField]
        private ColorPainter.ToolsSettings colorPainterSettings;
        public ColorPainter.ToolsSettings ColorPainterSettings
        {
            get
            {
                if (colorPainterSettings == null)
                {
                    colorPainterSettings = new ColorPainter.ToolsSettings();
                }
                return colorPainterSettings;
            }
            set
            {
                colorPainterSettings = value;
                if (colorPainterSettings != null)
                {
                    colorPainterSettings.Validate();
                    ColorPainter.Settings = colorPainterSettings;
                }
            }
        }

        private ColorPainter colorPainter;
        public ColorPainter ColorPainter
        {
            get
            {
                if (colorPainter == null)
                {
                    colorPainter = new ColorPainter(this);
                }
                return colorPainter;
            }
        }

        /// <summary>
        /// Contains its bezier & dimensions data
        /// </summary>
        [SerializeField, HideInInspector]
        private VertexWarper.SerializeData vertexWarperSerializeData;

        private const int SURFACE_INSET = 3;
        private const int SURFACE_INSET_NO_UNDERGROUND = 0;
        private const float NOISE_STEP_MULTIPLIER = 0.0167f;

        /// <summary>
        /// The mesh after generation process
        /// </summary>
        private Mesh generatedMesh;
        public Mesh GeneratedMesh
        {
            get
            {
                if (generatedMesh == null)
                {
                    generatedMesh = new Mesh();
                    generatedMesh.MarkDynamic();
                }
                return generatedMesh;
            }
            private set
            {
                generatedMesh = value;
            }
        }

        /// <summary>
        /// Contains generated data, use to construct new mesh or other purpose
        /// </summary>
        private MeshData meshData;
        public MeshData MeshData
        {
            get
            {
                return meshData;
            }
            private set
            {
                meshData = value;
            }
        }

        /// <summary>
        /// A vertices splitted version of MeshData
        /// </summary>
        private MeshData flatShadingMeshData;
        public MeshData FlatShadingMeshData
        {
            get
            {
                return flatShadingMeshData;
            }
            private set
            {
                flatShadingMeshData = value;
            }
        }

        private Texture2D oldHeightMap;
        private int heightMapWidth;
        private int heightMapHeight;
        private Color[] heightMapData;

        //is the terrain marked as dirty and need to update?
        //terrain only update at the end of the frame
        private bool needUpdate;
        //what kind of update is it
        private UpdateFlag updateFlag;

        #endregion

        /// <summary>
        /// A callback fired when user modifies the beziers in Scene view
        /// </summary>
        /// <param name="beziers"></param>
        private void OnVertexWarperBezierChanged(Bezier[] beziers)
        {
            if (UpdateImmediately)
            {
                needUpdate = true;
                updateFlag = UpdateFlag.OnWarp;
            }
        }

        /// <summary>
        /// A callback fired when user start to modify the terrain using geometry painter
        /// </summary>
        private void OnGeometryPaintStarted()
        {
            forceDisableFlatShading = CalculateVertexCount() > Constraints.MAX_VERTICES * 0.33f;
            EnvironmentalPainter.SetEnvironmentActive(false);
        }

        /// <summary>
        /// A callback fired when user hold left mouse to paint using geometry painter
        /// </summary>
        /// <param name="modifiedVertexIndices"></param>
        /// <param name="deltas"></param>
        private void OnGeometryPainting(List<int> modifiedVertexIndices, List<float> deltas)
        {
            if (UpdateImmediately)
            {
                needUpdate = true;
                updateFlag = UpdateFlag.OnPaint;

                for (int i = 0; i < modifiedVertexIndices.Count; ++i)
                {
                    int vertexIndex = modifiedVertexIndices[i];
                    Vector3 vertex = MeshData.vertices[vertexIndex];
                    float newY = Mathf.Clamp(vertex.y + deltas[i], BaseHeight, BaseHeight + SurfaceMaxHeight);
                    float clampedDelta = newY - vertex.y;
                    //clamped elevation data and send it back to the painter to cached
                    GeometryPainter.AddElevation(vertexIndex, clampedDelta);
                    vertex.y = newY;
                    MeshData.vertices[vertexIndex] = vertex;
                    GenerateVertexColor(vertexIndex, true);
                }
            }
        }

        /// <summary>
        /// A callback fired when user release the mouse to stop painting using geometry painter
        /// </summary>
        private void OnGeometryPaintEnded()
        {
            EnvironmentalPainter.SetEnvironmentActive(true);
            forceDisableFlatShading = false;
            if (UpdateImmediately)
            {
                needUpdate = true;
                updateFlag = UpdateFlag.OnPaintEnded;
            }
        }

        /// <summary>
        /// A callback fired when clicking on Erase All button
        /// </summary>
        private void OnGeometryElevationsResetted()
        {
            if (UpdateImmediately)
            {
                needUpdate = true;
                updateFlag = UpdateFlag.OnElevationsResetted;
            }
        }

        private void OnColorPaintStarted()
        {
            forceDisableFlatShading = CalculateVertexCount() > Constraints.MAX_VERTICES * 0.33f;
            //forceDisableFlatShading = true;
        }

        private void OnColorPainting(List<int> modifiedVertexIndices, List<Color> data)
        {
            if (UpdateImmediately)
            {
                for (int i=0;i<modifiedVertexIndices.Count;++i)
                {
                    GenerateVertexColor(modifiedVertexIndices[i], true);
                }
                needUpdate = true;
                updateFlag = UpdateFlag.OnPaint;
            }
        }

        private void OnColorPaintEnded()
        {
            forceDisableFlatShading = false;
            if (UpdateImmediately)
            {
                needUpdate = true;
                updateFlag = UpdateFlag.OnPaintEnded;
            }
        }

        private void OnColorsResetted()
        {
            if (UpdateImmediately)
            {
                needUpdate = true;
                updateFlag = UpdateFlag.OnColorsResetted;
            }
        }

        /// <summary>
        /// Init default values
        /// </summary>
        private void Reset()
        {
            MeshFilterComponent = GetComponent<MeshFilter>();
            MeshRendererComponent = GetComponent<MeshRenderer>();
            MeshColliderComponent = GetComponent<MeshCollider>();
            UpdateImmediately = true;
            ShowVertexWarperSceneGUI = true;
            TilingMode = SurfaceTilingMode.Quad;
            SurfaceTileCountX = 30;
            SurfaceTileCountZ = 30;
            VertexSpacing = 10;
            BaseHeight = 10;
            SurfaceMaxHeight = 100;
            GroundThickness = 10f;
            GroundThicnknessVariation = 0;
            Roughness = 0;
            RoughnessSeed = 0;
            UndergroundColor = Color.black;
            ColorByHeight = Utilities.CreateFullWhiteGradient();
            ColorByNormal = Utilities.CreateFullWhiteGradient();
            ColorBlendFraction = Utilities.CreateFullTransparentGradient();
            UseFlatShading = true;
            UseVertexColor = true;
            UseSolidFaceColor = false;
            ShouldReduceSurfaceColorNoise = true;
            ShouldRecalculateBounds = true;
            ShouldRecalculateNormals = true;
            ShouldRecalculateTangents = true;
            ShouldUnwrapUv = true;
            ShouldEncloseBottomPart = false;
            ShouldGenerateUnderground = true;
            vertexWarper = null;
            Generate();
        }

        /// <summary>
        /// Validate parameters and mark the terrain dirty
        /// </summary>
        public void OnValidate()
        {
            ShowFullHierarchy = showFullHierarchy;
            SurfaceTileCountX = surfaceTileCountX;
            SurfaceTileCountZ = surfaceTileCountZ;
            HeightMap = heightMap;
            BaseHeight = baseHeight;
            SurfaceMaxHeight = surfaceMaxHeight;
            GroundThickness = groundThickness;
            GroundThicnknessVariation = groundThicknessVariation;
            UndergroundColor = undergroundColor;
            UseVertexWarping = useVertexWarping;
            if (UseVertexWarping)
            {
                VertexWarper.DeSerialize(vertexWarperSerializeData);
                UpdateVertexWarperDimension();
                VertexWarper.ApplyTemplate(WarpingTemplate);
            }

            //these 3 line keep brush setting serialization work
            EnvironmentalPainterSettings = environmentalPainterSettings;
            GeometryPainterSettings = geometryPainterSettings;
            ColorPainterSettings = colorPainterSettings;

            if (UpdateImmediately)
            {
                needUpdate = true;
                updateFlag = UpdateFlag.OnValidate;
            }
        }

        private void OnEnable()
        {
            //register height map changed on import callback
            TerrainMapTracker.Register(this);

            //register vertex warper callbacks
            if (VertexWarper != null)
            {
                VertexWarper.BezierChanged += OnVertexWarperBezierChanged;
                VertexWarper.DeSerialize(vertexWarperSerializeData);
                VertexWarper.ApplyTemplate(WarpingTemplate);
                if (UpdateImmediately)
                {
                    needUpdate = true;
                    updateFlag = UpdateFlag.OnWarp;
                }
            }

            //register geometry painter callbacks
            if (GeometryPainter != null)
            {
                GeometryPainter.PaintStarted += OnGeometryPaintStarted;
                GeometryPainter.Painting += OnGeometryPainting;
                GeometryPainter.PaintEnded += OnGeometryPaintEnded;
                GeometryPainter.ElevationsResetted += OnGeometryElevationsResetted;
            }

            //register color painter callbacks
            if (ColorPainter != null)
            {
                ColorPainter.PaintStarted += OnColorPaintStarted;
                ColorPainter.Painting += OnColorPainting;
                ColorPainter.PaintEnded += OnColorPaintEnded;
                ColorPainter.ColorsResetted += OnColorsResetted;
            }

#if UNITY_EDITOR
            //keep the UpdateImmediately works
            UnityEditor.EditorApplication.update += LateUpdate;
#endif
        }

        private void OnDisable()
        {
            //unregister height map changed on import callback
            TerrainMapTracker.UnRegister(this);

            //unregister vertex warper callbacks
            if (VertexWarper != null)
            {
                VertexWarper.BezierChanged -= OnVertexWarperBezierChanged;
                if (vertexWarperSerializeData == null)
                    vertexWarperSerializeData = new VertexWarper.SerializeData();
                VertexWarper.Serialize(vertexWarperSerializeData);
            }

            //unregister geometry painter callbacks
            if (GeometryPainter != null)
            {
                GeometryPainter.PaintStarted -= OnGeometryPaintStarted;
                GeometryPainter.Painting -= OnGeometryPainting;
                GeometryPainter.PaintEnded -= OnGeometryPaintEnded;
                GeometryPainter.ElevationsResetted -= OnGeometryElevationsResetted;
            }
            //unregister color painter callbacks
            if (ColorPainter != null)
            {
                ColorPainter.PaintStarted -= OnColorPaintStarted;
                ColorPainter.Painting -= OnColorPainting;
                ColorPainter.PaintEnded -= OnColorPaintEnded;
                ColorPainter.ColorsResetted -= OnColorsResetted;
            }

#if UNITY_EDITOR
            //keep the UpdateImmediately works
            UnityEditor.EditorApplication.update -= LateUpdate;
#endif
        }

        private void LateUpdate()
        {
            //update the terrain at the end of the frame
            if (needUpdate)
            {
                if (updateFlag == UpdateFlag.OnValidate)
                {
                    GeometryPainter.Settings.UpdateDimension(
                        new Index2D(SurfaceTileCountX + 1, SurfaceTileCountZ + 1));
                    ColorPainter.Settings.UpdateDimension(
                        new Index2D(SurfaceTileCountX + 1, SurfaceTileCountZ + 1));
                }

                bool isModifying = updateFlag == UpdateFlag.OnPaint;
                Generate(isModifying);
                if (updateFlag == UpdateFlag.OnHeightmapModified ||
                    updateFlag == UpdateFlag.OnValidate ||
                    updateFlag == UpdateFlag.OnWarp ||
                    updateFlag == UpdateFlag.OnElevationsResetted ||
                    updateFlag == UpdateFlag.OnPaintEnded)
                {
                    EnvironmentalPainter.UpdateEnvironmentOnGeometryChanged();
                }
                needUpdate = false;
                updateFlag = UpdateFlag.OnValidate;
            }
        }

        /// <summary>
        /// A callback fired when height map is changed after asset import (modified outside Unity)
        /// </summary>
        private void OnHeightmapUpdated()
        {
            PreprocessHeightmap(true);
            if (UpdateImmediately)
            {
                needUpdate = true;
                updateFlag = UpdateFlag.OnHeightmapModified;
            }
        }

        /// <summary>
        /// Generate a new terrain
        /// </summary>
        /// <param name="isModifying"></param>
        private void Generate(bool isModifying = false)
        {
            //if isModifying, ignore the generate mesh data step, just upload the current data to graphic API
            bool initSuccessful = isModifying ? true : Init();
            if (initSuccessful)
            {
                if (!isModifying)
                {
                    GenerateVerticesInfo();
                    GenerateTriangles();
                }
                ConstructNewMesh();
            }
        }

        /// <summary>
        /// Initialize the generation process
        /// </summary>
        /// <returns></returns>
        private bool Init()
        {
            if (CalculateVertexCount() > Constraints.MAX_VERTICES)
            {
                if (MeshData != null)
                    MeshData.Clear();
                GeneratedMesh.Clear();
#if UNITY_EDITOR
                if (inspector != null)
                    inspector.errorMessage = InspectorUtilities.LARGE_DIMENSION_ERROR_MSG;
#endif
                return false;
            }
            else
            {
#if UNITY_EDITOR
                if (inspector != null)
                    inspector.errorMessage = string.Empty;
#endif
            }

            MeshData = new MeshData();
            PreprocessHeightmap();
            UpdateVertexWarperDimension();
            return true;
        }

        /// <summary>
        /// Cached height map data
        /// Since we use multithreading, we cannot sample the height map on worker thread
        /// </summary>
        /// <param name="forcedRead"></param>
        private void PreprocessHeightmap(bool forcedRead = false)
        {
            if (HeightMap != null && HeightMap == oldHeightMap && !forcedRead)
                return;
            oldHeightMap = HeightMap;
            if (HeightMap != null)
            {
                heightMapWidth = HeightMap.width;
                heightMapHeight = HeightMap.height;
                heightMapData = HeightMap.GetPixels();
            }
            else
            {
                heightMapWidth = 0;
                heightMapHeight = 0;
                heightMapData = null;
            }
        }

        private void UpdateVertexWarperDimension()
        {
            if (UseVertexWarping)
            {
                VertexWarper.Y = 0;
                VertexWarper.MinX = 0;
                VertexWarper.MaxX = SurfaceTileCountX * VertexSpacing;
                VertexWarper.MinZ = 0;
                VertexWarper.MaxZ = SurfaceTileCountZ * VertexSpacing;
            }
        }

        /// <summary>
        /// Upload mesh data to graphic API & additional actions
        /// </summary>
        private void ConstructNewMesh()
        {
            if (UseVertexColor)
            {
                if (ShouldRecalculateNormals)
                    MeshData.CalculateNormals();
                GenerateSurfaceFaceColors();
            }

            if (UseFlatShading)
            {
                CreateFlatShading();
                if (UseSolidFaceColor)
                    SolidifyFaceColors();
            }

            MeshData data = UseFlatShading ? FlatShadingMeshData : MeshData;

            GeneratedMesh.Clear();
            GeneratedMesh.vertices = data.vertices;
            GeneratedMesh.triangles = data.triangles;

            if (UseVertexColor)
                GeneratedMesh.colors = data.vertexColors;
            if (ShouldRecalculateBounds)
                GeneratedMesh.RecalculateBounds();
            if (ShouldRecalculateTangents)
                GeneratedMesh.RecalculateTangents();
            if (ShouldRecalculateNormals)
                GeneratedMesh.normals = data.normals;
            if (ShouldUnwrapUv)
                GeneratedMesh.uv = data.uvCoords;

            GeneratedMesh.name = "Terrain";

            if (MeshFilterComponent != null)
                MeshFilterComponent.mesh = GeneratedMesh;
            if (MeshRendererComponent != null)
                MeshRendererComponent.sharedMaterial = ToRenderMaterial;
            if (MeshColliderComponent != null)
                MeshColliderComponent.sharedMesh = GeneratedMesh;
        }

        /// <summary>
        /// Generate vertices position, color and uv coordinate
        /// </summary>
        private void GenerateVerticesInfo()
        {
            MeshData.InitVertexCount(VerticesGridLength.Z * VerticesGridLength.X);

            Index2D i00 = new Index2D(0, 0);
            Index2D i01 = new Index2D(VerticesGridLength.X / 2, VerticesGridLength.Z / 2);
            Index2D i10 = new Index2D(i01.X + 1, 0);
            Index2D i11 = new Index2D(VerticesGridLength.X - 1, i01.Z + 1);
            Index2D i20 = new Index2D(0, i01.Z + 1);
            Index2D i21 = new Index2D(i01.X, VerticesGridLength.Z - 1);
            Index2D i30 = new Index2D(i01.X + 1, i01.Z + 1);
            Index2D i31 = new Index2D(VerticesGridLength.X - 1, VerticesGridLength.Z - 1);

            bool group0Completed = false;
            bool group1Completed = false;
            bool group2Completed = false;
            bool group3Completed = false;

            //dispatch worker threads
            if (SystemInfo.processorCount >= 4)
            {
                new Thread(() => GenerateVerticesInfoPartially(i00, i01, out group0Completed)).Start();
                new Thread(() => GenerateVerticesInfoPartially(i10, i11, out group1Completed)).Start();
                new Thread(() => GenerateVerticesInfoPartially(i20, i21, out group2Completed)).Start();
                GenerateVerticesInfoPartially(i30, i31, out group3Completed);
            }
            else
            {
                new Thread(() =>
                {
                    GenerateVerticesInfoPartially(i00, i01, out group0Completed);
                    GenerateVerticesInfoPartially(i10, i11, out group1Completed);
                }).Start();

                GenerateVerticesInfoPartially(i20, i21, out group2Completed);
                GenerateVerticesInfoPartially(i30, i31, out group3Completed);
            }

            //synchronize worker threads
            while (!group0Completed || !group1Completed || !group2Completed || !group3Completed)
            {
                continue;
            }
        }

        /// <summary>
        /// Worker thread execution to generate vertices info
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="completedFlag"></param>
        private void GenerateVerticesInfoPartially(
            Index2D startIndex,
            Index2D endIndex,
            out bool completedFlag)
        {
            //Unity doesn't catch exception on worker thread
            //We do this to detect if there is any error and log it
            try
            {
                //generate surface vertex position
                for (int z = startIndex.Z; z <= endIndex.Z; ++z)
                {
                    for (int x = startIndex.X; x <= endIndex.X; ++x)
                    {
                        Index2D i = new Index2D(x, z);
                        if (IsSurfaceVertex(i))
                            GenerateVertexPositionAtIndex(i);
                    }
                }

                //generate underground vertex position
                //underground vertex position depends on surface vertex position, so it should run after
                for (int z = startIndex.Z; z <= endIndex.Z; ++z)
                {
                    for (int x = startIndex.X; x <= endIndex.X; ++x)
                    {
                        Index2D i = new Index2D(x, z);
                        if (!IsSurfaceVertex(i))
                            GenerateVertexPositionAtIndex(i);
                    }
                }

                //generate vertex color and uv coordinate
                for (int z = startIndex.Z; z <= endIndex.Z; ++z)
                {
                    for (int x = startIndex.X; x <= endIndex.X; ++x)
                    {
                        Index2D i = new Index2D(x, z);
                        if (UseVertexColor)
                            GenerateUndergroundVertexColorAtIndex(i);
                        if (ShouldUnwrapUv)
                            GenerateVertexUvAtIndex(i);
                    }
                }

            }
            catch (System.Exception e)
            {
                string msg = string.Format("PLEASE REPORT THIS ERROR TO THE DEVELOPER: {0}", e.ToString());
                Debug.Log(msg);
            }
            completedFlag = true;
        }

        private void GenerateVertexPositionAtIndex(Index2D i)
        {
            float vertexX = 0;
            float vertexY = 0;
            float vertexZ = 0;

            Vector3 translation = GetVertexTranslationAtIndex(i);

            vertexX = i.X * VertexSpacing + translation.x;
            vertexY = GetVertexHeight(i);
            vertexZ = i.Z * VertexSpacing + translation.z;

            if (UseVertexWarping)
            {
                VertexWarper.Warp(ref vertexX, ref vertexY, ref vertexZ);
            }

            int index = To1DIndex(i.X, i.Z);
            MeshData.vertices[index] = new Vector3(vertexX, vertexY, vertexZ);
        }

        private Vector3 GetVertexTranslationAtIndex(Index2D i)
        {
            float translationX = 0;
            float translationY = 0;
            float translationZ = 0;

            translationX = -SurfaceMinIndex.X * VertexSpacing;
            if (i.X < SurfaceMinIndex.X)
            {
                translationX += (SurfaceMinIndex.X - i.X) * VertexSpacing;
            }
            else if (i.X > SurfaceMaxIndex.X)
            {
                translationX += (SurfaceMaxIndex.X - i.X) * VertexSpacing;
            }

            translationZ = -SurfaceMinIndex.Z * VertexSpacing;
            if (i.Z < SurfaceMinIndex.Z)
            {
                translationZ += (SurfaceMinIndex.Z - i.Z) * VertexSpacing;
            }
            else if (i.Z > SurfaceMaxIndex.Z)
            {
                translationZ += (SurfaceMaxIndex.Z - i.Z) * VertexSpacing;
            }

            if (WillShiftVertexInHexagonTiling(i))
            {
                translationX += VertexSpacing * 0.5f;
            }

            return new Vector3(translationX, translationY, translationZ);
        }

        private float GetVertexHeight(Index2D i)
        {
            if (!IsSurfaceVertex(i))
                return GetUndergroundVertexHeight(i);
            else
                return GetSurfaceVertexHeight(i);
        }

        private float GetUndergroundVertexHeight(Index2D i)
        {
            //these vertices is at the border of the vertices array, so they don't move
            if (!Utilities.IsInRangeExclusive(i.X, 0, VerticesGridLength.X - 1) ||
                !Utilities.IsInRangeExclusive(i.Z, 0, VerticesGridLength.Z - 1))
            {
                return 0;
            }
            //these vertices is used to created the thickness layer
            else
            {
                Index2D nearestSurfaceVertexIndex = new Index2D(
                    Mathf.Clamp(i.X, SurfaceMinIndex.X, SurfaceMaxIndex.X),
                    Mathf.Clamp(i.Z, SurfaceMinIndex.Z, SurfaceMaxIndex.Z));
                Vector3 nearestSurfaceVertexPosition = MeshData.vertices[To1DIndex(nearestSurfaceVertexIndex.X, nearestSurfaceVertexIndex.Z)];
                float thicknessVariation = 0;
                if (GroundThicnknessVariation > 0)
                {
                    float noiseValue = GetGroundThicknessNoiseValueAtIndex(nearestSurfaceVertexIndex);
                    thicknessVariation = noiseValue * GroundThickness * GroundThicnknessVariation;
                }

                float height = nearestSurfaceVertexPosition.y - GroundThickness + thicknessVariation;
                return height;
            }
        }

        private float GetSurfaceVertexHeight(Index2D i)
        {
            float heightMapMultiplier = 0.5f;
            if (heightMapData != null)
            {
                float xOffset = 0;
                if (WillShiftVertexInHexagonTiling(i))
                {
                    xOffset = 0.5f;
                }
                Vector2 uv = new Vector2(
                    Utilities.GetFraction(i.X + xOffset, SurfaceMinIndex.X, SurfaceMaxIndex.X),
                    Utilities.GetFraction(i.Z, SurfaceMinIndex.Z, SurfaceMaxIndex.Z));

                Vector2 pixelCoord = new Vector2(
                    uv.x * (heightMapWidth - 1),
                    uv.y * (heightMapHeight - 1));

                heightMapMultiplier = GetHeightMapAlpha(pixelCoord);
            }
            float noiseMultiplier = GetSurfaceNoiseValueAtIndex(i);
            float paintedElevation = GeometryPainter.GetElevation(GetSurfaceVertexIndex(i));

            return BaseHeight + Mathf.Clamp(SurfaceMaxHeight * heightMapMultiplier * noiseMultiplier + paintedElevation, 0, SurfaceMaxHeight);
        }

        /// <summary>
        /// Base on this article: https://en.wikipedia.org/wiki/Bilinear_interpolation#Unit_square
        /// </summary>
        /// <param name="pixelCoord"></param>
        /// <returns></returns>
        private float GetHeightMapAlpha(Vector2 pixelCoord)
        {
            if (heightMapData == null)
                return 1;

            float alpha = 1;
            if (ShouldReduceSurfaceColorNoise)
            {
                //apply a bilinear filter
                int xFloor = Mathf.FloorToInt(pixelCoord.x);
                int xCeil = Mathf.CeilToInt(pixelCoord.x);
                int yFloor = Mathf.FloorToInt(pixelCoord.y);
                int yCeil = Mathf.CeilToInt(pixelCoord.y);

                float f00 = heightMapData[Utilities.To1DIndex(xFloor, yFloor, heightMapWidth)].a;
                float f01 = heightMapData[Utilities.To1DIndex(xFloor, yCeil, heightMapWidth)].a;
                float f10 = heightMapData[Utilities.To1DIndex(xCeil, yFloor, heightMapWidth)].a;
                float f11 = heightMapData[Utilities.To1DIndex(xCeil, yCeil, heightMapWidth)].a;

                Vector2 unitCoord = new Vector2(
                    pixelCoord.x - xFloor,
                    pixelCoord.y - yFloor);

                alpha =
                    f00 * (1 - unitCoord.x) * (1 - unitCoord.y) +
                    f01 * (1 - unitCoord.x) * unitCoord.y +
                    f10 * unitCoord.x * (1 - unitCoord.y) +
                    f11 * unitCoord.x * unitCoord.y;
            }
            else
            {
                //use raw pixel data
                int dataIndex = Utilities.To1DIndex((int)pixelCoord.x, (int)pixelCoord.y, heightMapWidth);
                alpha = heightMapData[dataIndex].a;
            }
            return alpha;
        }

        private float GetSurfaceNoiseValueAtIndex(Index2D i)
        {
            if (Roughness == 0)
                return 1;

            Vector2 noiseOrigin = RoughnessSeed * Vector2.one;
            float xIndexOffset = WillShiftVertexInHexagonTiling(i) ? 0.5f : 0;
            Vector2 translationDirection = new Vector2(
                i.X + xIndexOffset - SurfaceMinIndex.X,
                i.Z - SurfaceMinIndex.Z);
            Vector2 noiseCoord = noiseOrigin + translationDirection * Roughness * NOISE_STEP_MULTIPLIER;

            float noiseValue = Mathf.PerlinNoise(noiseCoord.x, noiseCoord.y);
            return noiseValue;
        }

        private float GetGroundThicknessNoiseValueAtIndex(Index2D i)
        {
            Vector2 noiseOrigin = GroundThicknessVariationSeed * Vector2.one;
            Vector2 noiseCoord = noiseOrigin + new Vector2(i.X, i.Z) * GroundThicknessNoiseStep;
            float noiseValue = Mathf.PerlinNoise(noiseCoord.x, noiseCoord.y);
            return noiseValue;
        }

        private void GenerateUndergroundVertexColorAtIndex(Index2D i)
        {
            Color c = Color.white;
            bool isUnderground =
                !Utilities.IsInRange(i.X, SurfaceMinIndex.X - 1, SurfaceMaxIndex.X + 1) ||
                !Utilities.IsInRange(i.Z, SurfaceMinIndex.Z - 1, SurfaceMaxIndex.Z + 1);
            if (isUnderground)
            {
                c = UndergroundColor;
            }
            else
            {
                c = Color.clear; //color code to mark surface vertex
            }

            int cIndex = To1DIndex(i.X, i.Z);
            MeshData.vertexColors[cIndex] = c;
        }

        private void GenerateSurfaceFaceColors()
        {
            int trisCount = MeshData.triangles.Length / 3;
            int i0 = 0;
            int i1 = (trisCount - 1) / 2;
            int i2 = trisCount - 1;

            bool completedFlag0 = false;
            bool completedFlag1 = false;

            //dispatch worker threads
            new Thread(() => { GenerateSufaceFaceColorsPartially(i0, i1, out completedFlag0); }).Start();
            GenerateSufaceFaceColorsPartially(i1 + 1, i2, out completedFlag1);

            //synchronize worker threads
            while (!completedFlag0 || !completedFlag1)
            {
                continue;
            }
        }

        /// <summary>
        /// Worker thread execution for generating vertex colors
        /// </summary>
        /// <param name="startTrisIndex"></param>
        /// <param name="endTrisIndex"></param>
        /// <param name="completedFlag"></param>
        private void GenerateSufaceFaceColorsPartially(int startTrisIndex, int endTrisIndex, out bool completedFlag)
        {
            try
            {
                int iMin = startTrisIndex * 3;
                int iMax = endTrisIndex * 3 + 2;

                for (int i = iMin; i <= iMax; ++i)
                {
                    GenerateVertexColor(MeshData.triangles[i]);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("PLEASE REPORT THIS ERROR TO THE DEVELOPER: " + e.ToString());
            }
            completedFlag = true;
        }

        private void GenerateVertexColor(int vIndex, bool overwrite = false)
        {
            if (MeshData.vertexColors[vIndex] != Color.clear && !overwrite)
                return;
            float height = MeshData.vertices[vIndex].y;
            Vector3 normal = MeshData.normals != null ? MeshData.normals[vIndex] : Vector3.up;
            Color c = GetGeometryBlendedVertexColor(height, normal);
            MeshData.vertexColors[vIndex] = c;

            Index2D i2d = Utilities.To2DIndex(vIndex, VerticesGridLength.X);
            if (!IsSurfaceVertex(i2d)) //this function also generate color for the underground layer, so we need to double check this
                return;
            Color paintedColor = ColorPainter.GetColor(GetSurfaceVertexIndex(i2d));
            Color baseColor = ColorLibrary.GetColor(c, 1);
            MeshData.vertexColors[vIndex] = Color.Lerp(baseColor, paintedColor, paintedColor.a);
        }

        private void SolidifyFaceColors()
        {
            int trisCount = FlatShadingMeshData.triangles.Length / 3;
            for (int i = 0; i < trisCount; ++i)
            {
                Color c0 = FlatShadingMeshData.vertexColors[FlatShadingMeshData.triangles[i * 3 + 0]];
                Color c1 = FlatShadingMeshData.vertexColors[FlatShadingMeshData.triangles[i * 3 + 1]];
                Color c2 = FlatShadingMeshData.vertexColors[FlatShadingMeshData.triangles[i * 3 + 2]];
                Color c = (c0 + c1 + c2) / 3;
                FlatShadingMeshData.vertexColors[FlatShadingMeshData.triangles[i * 3 + 0]] = c;
                FlatShadingMeshData.vertexColors[FlatShadingMeshData.triangles[i * 3 + 1]] = c;
                FlatShadingMeshData.vertexColors[FlatShadingMeshData.triangles[i * 3 + 2]] = c;
            }
        }

        /// <summary>
        /// Blend between color by height and color by normals
        /// </summary>
        /// <param name="height"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        private Color GetGeometryBlendedVertexColor(float height, Vector3 normal)
        {
            Color c;
            float heightFraction = Utilities.GetFraction(height - BaseHeight, 0, SurfaceMaxHeight);
            Color cbh = ColorByHeight.Evaluate(heightFraction);

            float blendFraction = ColorBlendFraction.Evaluate(heightFraction).a;
            if (blendFraction > 0)
            {
                float normalDotUp = Vector3.Dot(normal, Vector3.up);
                Color cbn = ColorByNormal.Evaluate(normalDotUp);
                c = Vector4.Lerp(cbh, cbn, blendFraction);
            }
            else
            {
                c = cbh;
            }
            return c;
        }
        
        private void GenerateVertexUvAtIndex(Index2D i)
        {
            float xOffset = 0;
            if (WillShiftVertexInHexagonTiling(i))
            {
                xOffset = 0.5f;
            }
            Vector2 uv = new Vector2(
                Utilities.GetFraction(i.X + xOffset, 0, VerticesGridLength.X - 1),
                Utilities.GetFraction(i.Z, 0, VerticesGridLength.Z - 1));
            MeshData.uvCoords[To1DIndex(i.X, i.Z)] = uv;
        }
        
        /// <summary>
        /// Split vertices data to create flat shading, vertices count will be tripled
        /// </summary>
        private void CreateFlatShading()
        {
            FlatShadingMeshData = new MeshData();
            FlatShadingMeshData.InitVertexCount(MeshData.triangles.Length);
            for (int i = 0; i < MeshData.triangles.Length; ++i)
            {
                FlatShadingMeshData.vertices[i] = MeshData.vertices[MeshData.triangles[i]];
                FlatShadingMeshData.vertexColors[i] = MeshData.vertexColors[MeshData.triangles[i]];
                FlatShadingMeshData.uvCoords[i] = MeshData.uvCoords[MeshData.triangles[i]];
                FlatShadingMeshData.triangles[i] = i;
            }
            if (ShouldRecalculateNormals)
                FlatShadingMeshData.CalculateNormals();
        }

        public bool IsSurfaceVertex(Index2D i)
        {
            return Utilities.IsInRange(i.X, SurfaceMinIndex.X, SurfaceMaxIndex.X) &&
                Utilities.IsInRange(i.Z, SurfaceMinIndex.Z, SurfaceMaxIndex.Z);
        }

        public bool WillShiftVertexInHexagonTiling(Index2D i)
        {
            bool validTilingMode = TilingMode == SurfaceTilingMode.Hexagon;
            bool validIndex =
                Utilities.IsInRangeExclusive(i.X, SurfaceMinIndex.X, SurfaceMaxIndex.X) &&
                Utilities.IsInRange(i.Z, SurfaceMinIndex.Z, SurfaceMaxIndex.Z);
            bool validZIndex = ShouldGenerateUnderground ? i.Z % 2 == 0 : i.Z % 2 != 0;

            return validTilingMode && validIndex && validZIndex;
        }

        /// <summary>
        /// Calculate number of element needed for triangle array
        /// </summary>
        /// <returns></returns>
        private int GetTrisArrayLength()
        {
            int surfaceAndUndergroundTrisCount = TotalTileCountX * TotalTileCountZ * 2;
            int bottomPartTrisCount =
                (ShouldGenerateUnderground && ShouldEncloseBottomPart) ?
                TotalTileCountZ * 2 + (TotalTileCountX - 1) * 2 : 0;
            int trisArrayLength = (surfaceAndUndergroundTrisCount + bottomPartTrisCount) * 3;
            return trisArrayLength;
        }

        private void GenerateTriangles()
        {
            int trisArrayLength = GetTrisArrayLength();
            MeshData.triangles = new int[trisArrayLength];
            int currentTrisIndex = 0;

            WrapSurfaceAndUnderground(ref currentTrisIndex);

            if (ShouldGenerateUnderground && ShouldEncloseBottomPart)
                EncloseBottomPart(ref currentTrisIndex);
        }

        private void WrapSurfaceAndUnderground(ref int currentTrisIndex)
        {
            int z0 = 0;
            int z1 = (VerticesGridLength.Z - 1) / 2;
            int z2 = VerticesGridLength.Z - 1;

            int i0 = currentTrisIndex;
            int i1 = (VerticesGridLength.X - 1) * z1 * 2 * 3;

            bool completedFlag0 = false;
            bool completedFlag1 = false;

            //dispatch worker threads
            new Thread(() => WrapSurfaceAndUndergroundPartially(z0, z1, ref i0, out completedFlag0)).Start();
            WrapSurfaceAndUndergroundPartially(z1, z2, ref i1, out completedFlag1);

            //synchronize worker threads
            while (!completedFlag0 || !completedFlag1)
            {
                continue;
            }

            currentTrisIndex = i1;
        }

        /// <summary>
        /// Worker thread execution for generate triangle
        /// </summary>
        /// <param name="startZ"></param>
        /// <param name="endZ"></param>
        /// <param name="currentTrisIndex"></param>
        /// <param name="completedFlag"></param>
        private void WrapSurfaceAndUndergroundPartially(int startZ, int endZ, ref int currentTrisIndex, out bool completedFlag)
        {
            for (int z = startZ; z < endZ; ++z)
            {
                for (int x = 0; x < VerticesGridLength.X - 1; ++x)
                {
                    int[] tris = GetTris(x, z);
                    AddTris(ref currentTrisIndex, tris);
                }
            }
            completedFlag = true;
        }

        /// <summary>
        /// Generate some additional triangle to enclose mesh volume
        /// </summary>
        /// <param name="currentTrisIndex"></param>
        private void EncloseBottomPart(ref int currentTrisIndex)
        {
            int x = 0;
            int z = 0;
            int minX = 0;
            int maxX = VerticesGridLength.X - 1;
            int minZ = 0;
            int maxZ = VerticesGridLength.Z - 1;
            int[] t = new int[6];
            for (z = 0; z < VerticesGridLength.Z - 1; ++z)
            {
                t[0] = To1DIndex(maxX, z + 1);
                t[1] = To1DIndex(x, z + 1);
                t[2] = To1DIndex(x, z);

                t[3] = To1DIndex(maxX, z);
                t[4] = To1DIndex(maxX, z + 1);
                t[5] = To1DIndex(x, z);

                AddTris(ref currentTrisIndex, t);
            }

            z = 0;
            for (x = 1; x < VerticesGridLength.X - 1; ++x)
            {
                t[0] = To1DIndex(minX, minZ);
                t[1] = To1DIndex(x, minZ);
                t[2] = To1DIndex(x + 1, minZ);

                t[3] = To1DIndex(maxX - minX, maxZ);
                t[4] = To1DIndex(maxX - x, maxZ);
                t[5] = To1DIndex(maxX - x - 1, maxZ);

                AddTris(ref currentTrisIndex, t);
            }
        }

        private int To1DIndex(int x, int z)
        {
            return Utilities.To1DIndex(x, z, VerticesGridLength.X);
        }

        /// <summary>
        /// Get triangle indices for a tile
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private int[] GetTris(int x, int z)
        {
            if (TilingMode == SurfaceTilingMode.Quad)
                return GetTrisForQuadTiling(x, z);
            else
                return GetTrisForHexagonTiling(x, z);
        }

        private int[] GetTrisForQuadTiling(int x, int z)
        {
            int[] t = new int[6];
            if ((x + z) % 2 == 0)
            {
                t[0] = To1DIndex(x, z);
                t[1] = To1DIndex(x, z + 1);
                t[2] = To1DIndex(x + 1, z + 1);

                t[3] = To1DIndex(x, z);
                t[4] = To1DIndex(x + 1, z + 1);
                t[5] = To1DIndex(x + 1, z);
            }
            else
            {
                t[0] = To1DIndex(x, z);
                t[1] = To1DIndex(x, z + 1);
                t[2] = To1DIndex(x + 1, z);

                t[3] = To1DIndex(x + 1, z + 1);
                t[4] = To1DIndex(x + 1, z);
                t[5] = To1DIndex(x, z + 1);
            }

            return t;
        }

        private int[] GetTrisForHexagonTiling(int x, int z)
        {
            int[] t = new int[6];
            if ((z % 2 == 0 && ShouldGenerateUnderground) ||
                (z % 2 != 0 && !ShouldGenerateUnderground))
            {
                t[0] = To1DIndex(x, z);
                t[1] = To1DIndex(x, z + 1);
                t[2] = To1DIndex(x + 1, z + 1);

                t[3] = To1DIndex(x, z);
                t[4] = To1DIndex(x + 1, z + 1);
                t[5] = To1DIndex(x + 1, z);
            }
            else
            {
                t[0] = To1DIndex(x, z);
                t[1] = To1DIndex(x, z + 1);
                t[2] = To1DIndex(x + 1, z);

                t[3] = To1DIndex(x + 1, z + 1);
                t[4] = To1DIndex(x + 1, z);
                t[5] = To1DIndex(x, z + 1);
            }

            return t;
        }

        private void AddTris(ref int currentTrisIndex, int[] tris)
        {
            if (tris.Length % 3 != 0)
                throw new System.ArgumentException("Invalid indices array");
            for (int i = 0; i < tris.Length; ++i)
            {
                MeshData.triangles[currentTrisIndex] = tris[i];
                currentTrisIndex += 1;
            }
        }

        public void DrawSceneGUI()
        {
            //draw vertex warper scene gui
            if (VertexWarper != null && UseVertexWarping && ShowVertexWarperSceneGUI)
            {
                VertexWarper.DrawSceneGUI();
                if (vertexWarperSerializeData == null)
                    vertexWarperSerializeData = new VertexWarper.SerializeData();
                VertexWarper.Serialize(vertexWarperSerializeData);
                HandlesWarpingTemplateChange();
            }

            //draw geometry painter scene gui
            if (GeometryPainter != null && EnableGeometryPainter)
            {
                GeometryPainter.DrawSceneGUI();
            }

            //draw color painter scene gui
            if (ColorPainter != null && EnableColorPainter)
            {
                ColorPainter.DrawSceneGUI();
            }

            //draw environmental painter scene gui
            if (EnvironmentalPainter != null && EnableEnvironmentalPainter)
            {
                EnvironmentalPainter.DrawSceneGUI();
            }
            else
            {
                EnvironmentalPainter.HideMask();
            }
        }

        /// <summary>
        /// Set the warping template to appropriate value when user modifies the beziers to change value in inspector
        /// </summary>
        private void HandlesWarpingTemplateChange()
        {
            if (WarpingTemplate == VertexWarper.Template.Custom)
                return;
            Vector3[] currentControlPointInfo = vertexWarperSerializeData.controlPointsInfo;
            Vector3[] templateControlPointInfo = VertexWarper.TemplateMaker.GetTemplate(
                WarpingTemplate,
                vertexWarperSerializeData.y,
                vertexWarperSerializeData.minX,
                vertexWarperSerializeData.maxX,
                vertexWarperSerializeData.minZ,
                vertexWarperSerializeData.maxZ);
            if (!Utilities.ContainIdenticalElements(currentControlPointInfo, templateControlPointInfo))
            {
                WarpingTemplate = VertexWarper.Template.Custom;
            }
        }

        /// <summary>
        /// Calculate number of vertex needed to generate the mesh
        /// </summary>
        /// <returns></returns>
        private int CalculateVertexCount()
        {
            int vertexCount = 0;
            if (!UseFlatShading)
            {
                vertexCount = VerticesGridLength.X * VerticesGridLength.Z;
            }
            else
            {
                vertexCount = GetTrisArrayLength();
            }
            return vertexCount;
        }

        /// <summary>
        /// Convert a grid vertex index to surface vertex index
        /// </summary>
        /// <param name="gridVertexIndex"></param>
        /// <returns></returns>
        public Index2D GetSurfaceVertexIndex(Index2D gridVertexIndex)
        {
            if (!IsSurfaceVertex(gridVertexIndex))
                throw new System.ArgumentException("Invalid gridVertexIndex");
            return gridVertexIndex - SurfaceMinIndex;
        }

        public void SetHeightMap(float grayscale)
        {
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, Color.white * Mathf.Clamp01(grayscale));
            t.wrapMode = TextureWrapMode.Clamp;
            t.Apply();
            HeightMap = t;
            needUpdate = true;
            updateFlag = UpdateFlag.OnHeightmapModified;
        }

        public Vector2 GetVertexUVOnHeightMap(Index2D i)
        {
            if (!IsSurfaceVertex(i))
                return -Vector2.one;
            return new Vector2(
                Mathf.InverseLerp(SurfaceMinIndex.X, SurfaceMaxIndex.X, i.X),
                Mathf.InverseLerp(SurfaceMinIndex.Z, SurfaceMaxIndex.Z, i.Z));
        }
    }
}