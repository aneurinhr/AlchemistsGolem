using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.PolarisBasic
{
    /// <summary>
    /// Painter tool to manipulate terrain color
    /// </summary>
    public partial class ColorPainter
    {
        /// <summary>
        /// Fired when user hold the left mouse to paint
        /// </summary>
        /// <param name="modifiedVertexIndices"></param>
        /// <param name="deltas"></param>
        public delegate void PaintingHandler(List<int> modifiedVertexIndices, List<Color> deltas);
        public event PaintingHandler Painting;

        /// <summary>
        /// Fired on paint started
        /// </summary>
        public delegate void PaintStartedHanlder();
        public event PaintStartedHanlder PaintStarted;

        /// <summary>
        /// Fired on paint ended
        /// </summary>
        public delegate void PaintEndedHandler();
        public event PaintEndedHandler PaintEnded;

        /// <summary>
        /// Fired when elevations data is cleared
        /// </summary>
        public delegate void ColorsResettedHandler();
        public event ColorsResettedHandler ColorsResetted;

        /// <summary>
        /// The terrain it assocciates to
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
                return settings;
            }
            set
            {
                settings = value ?? new ToolsSettings();
            }
        }

        /// <summary>
        /// Internal object to visulize brush in Scene view
        /// </summary>
        private GameObject brushHandle;

        public ColorPainter(TerrainGenerator terrain)
        {
            if (terrain == null)
                throw new System.ArgumentException("Terrain cannot be null");
            this.terrain = terrain;
            settings = new ToolsSettings();
        }

        /// <summary>
        /// Perform a paint action at a specific location
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="currentEvent"></param>
        private void Paint(RaycastHit hit, Event currentEvent)
        {
            if (!GuiEventUtilities.IsLeftMouse)
                return;
            Vector3[] vertices = Terrain.MeshData.vertices;
            float sqrRadius = Settings.brushSettings.brushRadius * Settings.brushSettings.brushRadius;
            float sqrMagnitude = 0;
            List<int> modifiedVertexIndices = new List<int>();
            List<Color> deltas = new List<Color>();
            List<float> f = new List<float>(); //how much a vertex be affected by the brush

            //determine the vertices are being modified and how much they are affected (f)
            for (int i = 0; i < vertices.Length; ++i)
            {
                Index2D i2d = Utilities.To2DIndex(i, Terrain.VerticesGridLength.X);
                //only modify surface vertex
                if (!Terrain.IsSurfaceVertex(i2d))
                    continue;
                Vector3 worldPos = Terrain.transform.TransformPoint(vertices[i]);
                sqrMagnitude = Vector3.SqrMagnitude(hit.point - worldPos);
                if (sqrMagnitude >= sqrRadius)
                    continue;

                f.Add(1 - sqrMagnitude / sqrRadius);
                modifiedVertexIndices.Add(i);
            }

            float sign = GuiEventUtilities.IsCtrl ? -1 : 1;
            for (int i = 0; i < modifiedVertexIndices.Count; ++i)
            {
                int index = modifiedVertexIndices[i];
                if (sign > 0)
                    AddColor(index, settings.brushSettings.color, f[i] * settings.brushSettings.strength);
                else
                    EraseColor(index, f[i] * settings.brushSettings.strength);
            }

            //send data to the terrain to modify itself
            if (Painting != null)
                Painting(modifiedVertexIndices, deltas);
        }

        /// <summary>
        /// Get color value for a vertex
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Color GetColor(Index2D i)
        {
            return Settings.GetColor(i);
        }

        /// <summary>
        /// Add elevation value for a vertex
        /// </summary>
        /// <param name="vertexIndex"></param>
        /// <param name="value"></param>
        public void AddColor(int vertexIndex, Color value, float f)
        {
            Settings.EnsureDimensionsUpToDate(new Index2D(Terrain.SurfaceTileCountX + 1, Terrain.SurfaceTileCountZ + 1));
            Index2D i2d = Utilities.To2DIndex(vertexIndex, Terrain.VerticesGridLength.X);
            if (!Terrain.IsSurfaceVertex(i2d))
                return;
            Index2D surfaceIndex = Terrain.GetSurfaceVertexIndex(i2d);
            Settings.AddColor(surfaceIndex, value, f);
        }

        public void EraseColor(int vertexIndex, float f)
        {
            Settings.EnsureDimensionsUpToDate(new Index2D(Terrain.SurfaceTileCountX + 1, Terrain.SurfaceTileCountZ + 1));
            Index2D i2d = Utilities.To2DIndex(vertexIndex, Terrain.VerticesGridLength.X);
            if (!Terrain.IsSurfaceVertex(i2d))
                return;
            Index2D surfaceIndex = Terrain.GetSurfaceVertexIndex(i2d);
            Settings.EraseColor(surfaceIndex, f);
        }

        /// <summary>
        /// Clear colors data
        /// </summary>
        public void ResetColors()
        {
            Settings.ResetColors();
            if (ColorsResetted != null)
                ColorsResetted();
        }

        public void DrawSceneGUI()
        {
#if UNITY_EDITOR
            DrawHandles();
            DrawOverlayInstruction();
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
                    //set hot control to this tool, to disable marquee selection tool when dragging mouse in Scene view
                    GUIUtility.hotControl = controlId;
                    if (PaintStarted != null)
                        PaintStarted();
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                if (GUIUtility.hotControl == controlId)
                {
                    Tools.hidden = false;
                    //return the hot control to Unity, use the default one
                    GUIUtility.hotControl = 0;
                    if (PaintEnded != null)
                        PaintEnded();
                    Event.current.Use();
                }
            }

            //increase/decrease brush radius shortcuts
            if (GuiEventUtilities.IsPlus && !GuiEventUtilities.IsShift)
            {
                Settings.brushSettings.brushRadius += Utilities.DELTA_TIME * (Settings.brushSettings.brushRadius == 0 ? 1 : Settings.brushSettings.brushRadius);
                Settings.Validate();
            }
            if (GuiEventUtilities.IsMinus && !GuiEventUtilities.IsShift)
            {
                Settings.brushSettings.brushRadius -= Utilities.DELTA_TIME * (Settings.brushSettings.brushRadius == 0 ? 1 : Settings.brushSettings.brushRadius);
                Settings.Validate();
            }

            //increase/decrease brush strength shortcuts
            if (GuiEventUtilities.IsPlus && GuiEventUtilities.IsShift)
            {
                Settings.brushSettings.strength += Utilities.DELTA_TIME * (Settings.brushSettings.strength == 0 ? 0.1f : Settings.brushSettings.strength);
                Settings.Validate();
            }
            if (GuiEventUtilities.IsMinus && GuiEventUtilities.IsShift)
            {
                Settings.brushSettings.strength -= Utilities.DELTA_TIME * (Settings.brushSettings.strength == 0 ? 0.1f : Settings.brushSettings.strength);
                Settings.Validate();
            }

            Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (Terrain.MeshColliderComponent.Raycast(r, out hit, float.MaxValue))
            {
                DrawHandlesAtCursor(hit);
                Paint(hit, Event.current);
            }
            else
            {
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
            if (brushHandle == null)
                UpdateHandle();
            Material mat = MaterialUtilities.IntersectionHighlight;
            Color color =
                GuiEventUtilities.IsCtrl ? ColorLibrary.RedSemiTransparent :
                ColorLibrary.CyanSemiTransparent;
            mat.SetColor("_Color", color);
            Color intersectColor =
                GuiEventUtilities.IsCtrl ? ColorLibrary.Red :
                ColorLibrary.Cyan;
            mat.SetColor("_IntersectColor", intersectColor);
            brushHandle.transform.position = hit.point;

            brushHandle.transform.up = hit.normal;
            brushHandle.transform.localScale = Vector3.one * Settings.brushSettings.brushRadius * 2;

            brushHandle.gameObject.SetActive(true);

            //brush handle will not update correctly if AnimatedMaterial option is disable in Scene view
            //in such case, we update the Scene view manually
            foreach (SceneView sv in SceneView.sceneViews)
            {
                //the sceneViewState property is not exposed in 5.6.1 yet
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

        public void UpdateHandle()
        {
#if UNITY_EDITOR
            string name = "_INTERNAL_BrushHandle";
            Transform oldHandle = Terrain.transform.Find(name);
            if (oldHandle != null)
            {
                Utilities.DestroyGameobject(oldHandle.gameObject);
            }

            brushHandle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            brushHandle.name = name;
            Utilities.ResetTransform(brushHandle.transform, Terrain.transform);
            brushHandle.hideFlags = HideFlags.HideAndDontSave;

            //override its material
            MeshRenderer mr = brushHandle.GetComponent<MeshRenderer>();
            mr.sharedMaterial = MaterialUtilities.IntersectionHighlight;

            //override its mesh
            MeshFilter mf = brushHandle.GetComponent<MeshFilter>();
            mf.sharedMesh = Utilities.GetPrimitiveMesh(PrimitiveType.Sphere);

            //remove unneccessary components
            Component[] components = brushHandle.GetComponentsInChildren<Component>(true);
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

        public void HideHandle()
        {
#if UNITY_EDITOR
            if (brushHandle != null)
                brushHandle.gameObject.SetActive(false);
            //brush handle will not update correctly if AnimatedMaterial option is disable in Scene view
            //in such case, we update the Scene view manually
            foreach (SceneView sv in SceneView.sceneViews)
            {
                //the sceneViewState property is not exposed in 5.6.1 yet
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
            Vector2 guiSize = new Vector2(200, 85);
            Vector2 guiPosition = new Vector2(Screen.width - guiSize.x, Screen.height - guiSize.y);
            Rect r = new Rect(guiPosition, guiSize);
            Handles.BeginGUI();
            GUILayout.BeginArea(r);

            string tabText = GuiEventUtilities.IsButtonPressed(KeyCode.Tab) ? "<b>Tab</b>" : "Tab";
            string shiftText = GuiEventUtilities.IsShift ? "<b>Shift</b>" : "Shift";
            string ctrlText = GuiEventUtilities.IsCtrl ? "<b>Ctrl</b>" : "Ctrl";

            string instruction = string.Format(
                "Press {0} to switch brush mode\n" +
                "Hold {1} to smooth terrain\n" +
                "Hold {2} to lower terrain",
                tabText,
                shiftText,
                ctrlText);
            GUILayout.Box(instruction, GuiStyleUtilities.OverlayInstructionStyle);
            GUILayout.EndArea();
            Handles.EndGUI();
#endif
        }

        public void AddCurrentSettingsAsNewBrush(string name)
        {
            BrushSettings s = new BrushSettings();
            BrushSettings.Copy(settings.brushSettings, s);
            s.name = name;
            settings.Palette.Add(s);
        }

        public void RemoveBrushFromPaletteAtIndex(int i)
        {
            if (i >= 0 && i < settings.Palette.Count)
            {
                settings.Palette.RemoveAt(i);
            }
        }

        public void SetBrushAtIndex(int i, BrushSettings brush)
        {
            if (i >= 0 && i < settings.Palette.Count)
            {
                BrushSettings.Copy(brush, settings.Palette[i]);
            }
        }

        public void ApplyBrushFromPaletteAtIndex(int i)
        {
            if (i >= 0 && i < settings.Palette.Count)
            {
                BrushSettings.Copy(settings.Palette[i], settings.brushSettings);
            }
        }
    }
}
