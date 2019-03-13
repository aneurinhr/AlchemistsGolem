using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.PolarisBasic
{
    [CustomEditor(typeof(TerrainGenerator))]
    public class TerrainGeneratorInspector : Editor
    {
        private const int TAB_TERRAIN = 0;
        private const int TAB_GEOMETRY_PAINTER = 1;
        private const int TAB_COLOR_PAINTER = 2;
        private const int TAB_ENVIRONMENTAL_PAINTER = 3;

        private const string TERRAIN_ICON_PATH = "Assets/PolarisBasic/Textures/EditorIcons/terrain.png";
        private const string GEOMETRY_PAINTER_ICON_PATH = "Assets/PolarisBasic/Textures/EditorIcons/geometrypainter.png";
        private const string COLOR_PAINTER_ICON_PATH = "Assets/PolarisBasic/Textures/EditorIcons/colorpainter.png";
        private const string ENVIRONMENTAL_PAINTER_ICON_PATH = "Assets/PolarisBasic/Textures/EditorIcons/environmentalpainter.png";

        TerrainGenerator instance;
        private int currentTab = 0;
        private static int libraryObjectPickerControlId;
        private static GameObject toAddPrefab;
        private Rect lastGroupFoldOutRect;
        private bool hasGenerationError;

        private Texture2D goProBanner;
        private Texture2D GoProBanner
        {
            get
            {
                if (goProBanner == null)
                {
                    goProBanner = Resources.Load<Texture2D>("GoProBanner");
                }
                return goProBanner;
            }
        }

        private void OnEnable()
        {
            instance = (TerrainGenerator)target;
            libraryObjectPickerControlId = GUIUtility.GetControlID(FocusType.Passive);
        }

        private void OnDisable()
        {
            if (instance != null)
            {
                instance.EnvironmentalPainter.HideMask();
                instance.EnvironmentalPainter.HidePreview();
                instance.EnvironmentalPainter.HideHandle();
                instance.GeometryPainter.HideHandle();
                instance.ColorPainter.HideHandle();
            }
        }

        private void OnSceneGUI()
        {
            instance.DrawSceneGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawAds();
            currentTab = EditorCommon.Tabs(
                currentTab, EditorGUIUtility.singleLineHeight,
                EditorGUIUtility.IconContent(TERRAIN_ICON_PATH, "!General terrain settings"),
                EditorGUIUtility.IconContent(GEOMETRY_PAINTER_ICON_PATH, "!Geometry painter tool"),
                EditorGUIUtility.IconContent(COLOR_PAINTER_ICON_PATH, "!Color painter tool"),
                EditorGUIUtility.IconContent(ENVIRONMENTAL_PAINTER_ICON_PATH, "!Environmental painter tool"));
            EditorGUILayout.Space();

            if (currentTab == TAB_TERRAIN)
            {
                TerrainTabInspectorDrawer.DrawGUI(instance);
            }
            else if (currentTab == TAB_GEOMETRY_PAINTER)
            {
                GeometryTabInspectorDrawer.DrawGUI(instance);
            }
            else if (currentTab == TAB_COLOR_PAINTER)
            {
                ColorPainterTabInspectorDrawer.DrawGUI(instance);
            }
            else if (currentTab == TAB_ENVIRONMENTAL_PAINTER)
            {
                EnvironmentalTabInspectorDrawer.DrawGUI(instance, ref libraryObjectPickerControlId);
            }

            instance.ShowVertexWarperSceneGUI =
                currentTab == TAB_TERRAIN &&
                instance.inspector.isDeformationGroupExpanded;
            instance.EnableGeometryPainter =
                currentTab == TAB_GEOMETRY_PAINTER;
            instance.EnableColorPainter =
                currentTab == TAB_COLOR_PAINTER;
            instance.EnableEnvironmentalPainter =
                currentTab == TAB_ENVIRONMENTAL_PAINTER;
            instance.EnvironmentalPainter.Settings.mode =
                currentTab == TAB_TERRAIN ?
                EnvironmentalPainter.Mode.Spawning :
                instance.EnvironmentalPainter.Settings.mode;
            HandleGUICommand();

            if (currentTab == TAB_TERRAIN)
            {
                instance.EnvironmentalPainter.HideMask();
                instance.EnvironmentalPainter.HidePreview();
                instance.EnvironmentalPainter.HideHandle();
                instance.GeometryPainter.HideHandle();
                instance.ColorPainter.HideHandle();
            }
        }

        private void HandleGUICommand()
        {
            if (Event.current != null && Event.current.type == EventType.ExecuteCommand)
            {
                if (Event.current.commandName == "ObjectSelectorUpdated" &&
                    EditorGUIUtility.GetObjectPickerControlID() == libraryObjectPickerControlId)
                {
                    toAddPrefab = EditorGUIUtility.GetObjectPickerObject() as GameObject;
                }
                if (Event.current.commandName == "ObjectSelectorClosed" &&
                    EditorGUIUtility.GetObjectPickerControlID() == libraryObjectPickerControlId)
                {
                    if (toAddPrefab != null)
                    {
                        instance.EnvironmentalPainterSettings.AddPrefab(toAddPrefab);
                        instance.EnvironmentalPainter.UpdatePreviewer();
                        toAddPrefab = null;
                    }
                }
            }
            if (Event.current != null && Event.current.type == EventType.MouseMove)
            {
                if (currentTab != TAB_TERRAIN)
                {
                    SceneView.RepaintAll();
                }
            }
        }

        private void DrawPreferencesTab()
        {
            DrawDebuggingGroup();
            DrawHandlesColorGroup();
        }

        private void DrawDebuggingGroup()
        {
            instance.inspector.isDebuggingGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isDebuggingGroupExpanded, "Debugging");

            if (instance.inspector.isDebuggingGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                instance.EnvironmentalPainterSettings.drawCombinationsBounds = EditorGUILayout.Toggle("Draw Combinations Bounds", instance.EnvironmentalPainterSettings.drawCombinationsBounds);

                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private void DrawHandlesColorGroup()
        {
            instance.inspector.isHandlesColorGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isHandlesColorGroupExpanded, "Colors");

            if (instance.inspector.isHandlesColorGroupExpanded)
            {
                EditorGUI.indentLevel += 1;



                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        public override bool RequiresConstantRepaint()
        {
            return
                instance.inspector.isLibraryGroupExpanded ||
                instance.inspector.isOverallShapeGroupExpanded ||
                instance.inspector.isDataGroupExpanded ||
                instance.inspector.isPalleteGroupExpanded;
        }

        private void DrawAds()
        {
            if (CanShowAd())
            {
                if (GoProBanner != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(256), GUILayout.Height(64));
                    if (GUI.Button(r, "", GUIStyle.none))
                    {
                        Application.OpenURL(VersionInfo.ProVersionLink);
                    }
                    EditorGUI.DrawTextureTransparent(r, GoProBanner, ScaleMode.ScaleToFit);
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
                }
                else
                {
                    Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(25));
                    GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                    style.alignment = TextAnchor.MiddleRight;
                    style.normal.textColor = Color.blue;
                    if (GUI.Button(r, "Go Pro, save 35%", style))
                    {
                        Application.OpenURL(VersionInfo.ProVersionLink);
                    }

                    EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Hide for today", EditorStyles.centeredGreyMiniLabel))
                {
                    HideForToday();
                }
                if (GetHideTodayCount() >= 3)
                {
                    EditorGUILayout.LabelField(" | ", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(10));
                    if (GUILayout.Button("Hide forever", EditorStyles.centeredGreyMiniLabel))
                    {
                        HideForever();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }


        }
        private bool CanShowAd()
        {
            return !IsHideForever() && !IsHideForToday();
        }

        private bool IsHideForToday()
        {
            string dateString = System.DateTime.Now.ToShortDateString();
            string key = "POLARIS_AD_HIDE_" + dateString;
            return PlayerPrefs.GetInt(key, 0) == 1;
        }

        private void HideForToday()
        {
            string dateString = System.DateTime.Now.ToShortDateString();
            string key = "POLARIS_AD_HIDE_" + dateString;
            PlayerPrefs.SetInt(key, 1);
            SetHideTodayCount(GetHideTodayCount() + 1);
        }

        private bool IsHideForever()
        {
            return PlayerPrefs.GetInt("POLARIS_AD_HIDE_FOREVER", 0) == 1;
        }

        private void HideForever()
        {
            PlayerPrefs.SetInt("POLARIS_AD_HIDE_FOREVER", 1);
        }

        private int GetHideTodayCount()
        {
            return PlayerPrefs.GetInt("POLARIS_HIDE_TODAY_COUNT", 0);
        }

        private void SetHideTodayCount(int count)
        {
            PlayerPrefs.SetInt("POLARIS_HIDE_TODAY_COUNT", count);
        }
    }
}
