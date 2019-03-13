using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.PolarisBasic
{
    public static class TerrainTabInspectorDrawer
    {
        public static void DrawGUI(TerrainGenerator instance)
        {
            bool isChanged = false;
            DrawGeneralGroup(instance, ref isChanged);
            DrawOverallShapeGroup(instance, ref isChanged);
            DrawSurfaceGroup(instance, ref isChanged);
            DrawGroundThicknessGroup(instance, ref isChanged);
            DrawRenderingGroup(instance, ref isChanged);
            DrawDeformationGroup(instance, ref isChanged);
            DrawUtilitiesGroup(instance, ref isChanged);
            DrawDataGroup(instance);
            if (isChanged)
            {
                instance.SendMessage("OnValidate", SendMessageOptions.DontRequireReceiver);
            }
        }

        private static void DrawGeneralGroup(TerrainGenerator instance, ref bool isChanged)
        {
            instance.inspector.isGeneralGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isGeneralGroupExpanded, "General");
            if (instance.inspector.isGeneralGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                EditorGUI.BeginChangeCheck();
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(instance), typeof(MonoScript), false);
                GUI.enabled = true;
                instance.MeshFilterComponent = EditorGUILayout.ObjectField(
                    "Mesh Filter",
                    instance.MeshFilterComponent,
                    typeof(MeshFilter), true) as MeshFilter;
                instance.MeshRendererComponent = EditorGUILayout.ObjectField(
                    "Mesh Renderer",
                    instance.MeshRendererComponent,
                    typeof(MeshRenderer), true) as MeshRenderer;
                instance.MeshColliderComponent = EditorGUILayout.ObjectField(
                    "Collider",
                    instance.MeshColliderComponent,
                    typeof(MeshCollider), true) as MeshCollider;
                instance.UpdateImmediately = EditorGUILayout.Toggle("Update Immediately", instance.UpdateImmediately);
                instance.ShowFullHierarchy = EditorGUILayout.Toggle("Show Full Hierarchy", instance.ShowFullHierarchy);
                isChanged = isChanged || EditorGUI.EndChangeCheck();

                EditorGUI.indentLevel -= 1;
            }
            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void DrawOverallShapeGroup(TerrainGenerator instance, ref bool isChanged)
        {
            instance.inspector.isOverallShapeGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isOverallShapeGroupExpanded, "Overall Shape");
            if (instance.inspector.isOverallShapeGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                EditorGUI.BeginChangeCheck();
                DrawHeightMapField(instance);
                DrawDimensionErrorMessage(instance);
                instance.SurfaceTileCountX = EditorGUILayout.DelayedIntField("Tile X", instance.SurfaceTileCountX);
                instance.SurfaceTileCountZ = EditorGUILayout.DelayedIntField("Tile Z", instance.SurfaceTileCountZ);
                instance.VertexSpacing = EditorGUILayout.FloatField("Vertex Spacing", instance.VertexSpacing);
                instance.BaseHeight = EditorGUILayout.FloatField("Base Height", instance.BaseHeight);
                instance.ShouldGenerateUnderground = EditorGUILayout.Toggle("Underground", instance.ShouldGenerateUnderground);
                GUI.enabled = instance.ShouldGenerateUnderground;
                instance.ShouldEncloseBottomPart = EditorGUILayout.Toggle("Enclose Volume", instance.ShouldEncloseBottomPart);
                GUI.enabled = true;
                isChanged = isChanged || EditorGUI.EndChangeCheck();

                EditorGUI.indentLevel -= 1;
            }
            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void DrawHeightMapField(TerrainGenerator instance)
        {
            bool succeed = instance.HeightMap != null;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            Texture2D t = EditorGUILayout.ObjectField("Height Map", instance.HeightMap, typeof(Texture2D), false) as Texture2D;
            Rect contextButtonRect = EditorGUILayout.GetControlRect(GUILayout.Width(15), GUILayout.Height(64));
            if (GUI.Button(contextButtonRect, "▼", EditorCommon.EvenFlatButton))
            {
                ShowHeightMapContextMenu(instance);
            }

            EditorGUILayout.EndHorizontal();
            bool hasChanged = EditorGUI.EndChangeCheck();
            if (t == null)
            {
                instance.HeightMap = null;
                succeed = false;
            }
            else if (hasChanged && t != null)
            {
                try
                {
                    t.GetPixel(0, 0);
                    instance.HeightMap = t;
                    succeed = true;
                }
                catch
                {
                    instance.HeightMap = null;
                    hasChanged = false;
                }
            }
            if (!succeed)
                EditorGUILayout.HelpBox("Height Map must be Read/Write Enabled", MessageType.None, false);
        }

        private static void DrawDimensionErrorMessage(TerrainGenerator instance)
        {
            if (!string.IsNullOrEmpty(instance.inspector.errorMessage) &&
                instance.inspector.errorMessage.Equals(TerrainGenerator.InspectorUtilities.LARGE_DIMENSION_ERROR_MSG))
            {
                EditorGUILayout.HelpBox(TerrainGenerator.InspectorUtilities.LARGE_DIMENSION_ERROR_MSG, MessageType.None, false);
            }
        }

        private static void ShowHeightMapContextMenu(TerrainGenerator instance)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(
                new GUIContent("Highest"),
                false,
                () => { instance.SetHeightMap(1); });
            menu.AddItem(
                new GUIContent("Middle"),
                false,
                () => { instance.SetHeightMap(0.5f); });
            menu.AddItem(
                new GUIContent("Lowest"),
                false,
                () => { instance.SetHeightMap(0); });
            menu.ShowAsContext();
        }

        private static void DrawSurfaceGroup(TerrainGenerator instance, ref bool isChanged)
        {
            instance.inspector.isSurfaceGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isSurfaceGroupExpanded, "Surface");
            if (instance.inspector.isSurfaceGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                EditorGUI.BeginChangeCheck();
                instance.TilingMode = (TerrainGenerator.SurfaceTilingMode)EditorGUILayout.EnumPopup("Tiling", instance.TilingMode);
                instance.SurfaceMaxHeight = EditorGUILayout.FloatField("Max Height", instance.SurfaceMaxHeight);
                instance.Roughness = EditorGUILayout.FloatField("Roughness", instance.Roughness);
                instance.RoughnessSeed = EditorGUILayout.FloatField("Seed", instance.RoughnessSeed);
                isChanged = isChanged || EditorGUI.EndChangeCheck();

                EditorGUI.indentLevel -= 1;
            }
            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void DrawGroundThicknessGroup(TerrainGenerator instance, ref bool isChanged)
        {
            instance.inspector.isGroundThicknessGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isGroundThicknessGroupExpanded, "Ground Thickness");
            if (instance.inspector.isGroundThicknessGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                EditorGUI.BeginChangeCheck();
                instance.GroundThickness = EditorGUILayout.Slider("Thickness", instance.GroundThickness, 0f, instance.BaseHeight);
                instance.GroundThicnknessVariation = EditorGUILayout.Slider("Variation", instance.GroundThicnknessVariation, 0f, 1f);
                instance.GroundThicknessVariationSeed = EditorGUILayout.FloatField("Seed", instance.GroundThicknessVariationSeed);
                instance.GroundThicknessNoiseStep = EditorGUILayout.FloatField("Noise Step", instance.GroundThicknessNoiseStep);
                isChanged = isChanged || EditorGUI.EndChangeCheck();

                EditorGUI.indentLevel -= 1;
            }
            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void DrawRenderingGroup(TerrainGenerator instance, ref bool isChanged)
        {
            instance.inspector.isRenderingGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isRenderingGroupExpanded, "Rendering");
            if (instance.inspector.isRenderingGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                EditorGUI.BeginChangeCheck();

                instance.MaterialType = (TerrainGenerator.MaterialTypes)EditorGUILayout.EnumPopup("Material Type", instance.MaterialType);
                if (instance.MaterialType == TerrainGenerator.MaterialTypes.Custom)
                {
                    instance.CustomMaterial = EditorGUILayout.ObjectField("Material", instance.CustomMaterial, typeof(Material), false) as Material;
                }

                SerializedObject serializedObject = new SerializedObject(instance);

                SerializedProperty colorByHeightProps = serializedObject.FindProperty("colorByHeight");
                EditorGUILayout.PropertyField(colorByHeightProps, new GUIContent("Color By Height"), false);

                SerializedProperty colorByNormalProps = serializedObject.FindProperty("colorByNormal");
                EditorGUILayout.PropertyField(colorByNormalProps, new GUIContent("Color By Normal"), false);

                SerializedProperty colorBlendProps = serializedObject.FindProperty("colorBlendFraction");
                EditorGUILayout.PropertyField(colorBlendProps, new GUIContent("Color Blend Fraction (A)"), false);

                SerializedProperty undergoundColorProps = serializedObject.FindProperty("undergroundColor");
                EditorGUILayout.PropertyField(undergoundColorProps, new GUIContent("Underground Color"), false);

                serializedObject.ApplyModifiedProperties();

                instance.UseFlatShading = EditorGUILayout.Toggle("Flat Shading", instance.UseFlatShading);
                instance.UseVertexColor = EditorGUILayout.Toggle("Vertex Color", instance.UseVertexColor);
                GUI.enabled = instance.UseFlatShading && instance.UseVertexColor;
                instance.UseSolidFaceColor = EditorGUILayout.Toggle("Solid Face Color", instance.UseSolidFaceColor);
                GUI.enabled = instance.HeightMap != null;
                instance.ShouldReduceSurfaceColorNoise = EditorGUILayout.Toggle("Reduce Surface Noise", instance.ShouldReduceSurfaceColorNoise);
                GUI.enabled = true;
                isChanged = isChanged || EditorGUI.EndChangeCheck();

                EditorGUI.indentLevel -= 1;
            }
            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void DrawDeformationGroup(TerrainGenerator instance, ref bool isChanged)
        {
            instance.inspector.isDeformationGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isDeformationGroupExpanded, "Deformation");

            if (instance.inspector.isDeformationGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                EditorGUI.BeginChangeCheck();
                instance.UseVertexWarping = EditorGUILayout.Toggle("Enable", instance.UseVertexWarping);
                instance.WarpingTemplate = (VertexWarper.Template)EditorGUILayout.EnumPopup("Template", instance.WarpingTemplate);
                isChanged = isChanged || EditorGUI.EndChangeCheck();

                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void DrawUtilitiesGroup(TerrainGenerator instance, ref bool isChanged)
        {
            instance.inspector.isUtilitiesGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isUtilitiesGroupExpanded, "Utilities");

            if (instance.inspector.isUtilitiesGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                EditorGUI.BeginChangeCheck();
                instance.ShouldRecalculateBounds = EditorGUILayout.Toggle("Recalculate Bounds", instance.ShouldRecalculateBounds);
                instance.ShouldRecalculateNormals = EditorGUILayout.Toggle("Recalculate Normals", instance.ShouldRecalculateNormals);
                instance.ShouldRecalculateTangents = EditorGUILayout.Toggle("Recalculate Tangents", instance.ShouldRecalculateTangents);
                instance.ShouldUnwrapUv = EditorGUILayout.Toggle("Unwrap UV", instance.ShouldUnwrapUv);
                isChanged = isChanged || EditorGUI.EndChangeCheck();

                EditorGUI.indentLevel -= 1;
            }
            instance.ShowVertexWarperSceneGUI = instance.inspector.isDeformationGroupExpanded;

            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void DrawDataGroup(TerrainGenerator instance)
        {
            instance.inspector.isDataGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isDataGroupExpanded, "Data");

            if (instance.inspector.isDataGroupExpanded)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.GetControlRect(GUILayout.Height(2));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.GetControlRect(GUILayout.Width(EditorGUIUtility.singleLineHeight));
                Rect buttonGroupRect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorCommon.standardHeight));
                Rect exportButtonRect = new Rect(
                    buttonGroupRect.min.x,
                    buttonGroupRect.min.y,
                    buttonGroupRect.width * 0.5f,
                    buttonGroupRect.height);
                if (GUI.Button(exportButtonRect, "Export...", EditorStyles.miniButtonLeft))
                {
                    ShowExportDataContextMenu(exportButtonRect, instance);
                }
                Rect importButtonRect = new Rect(
                    buttonGroupRect.center.x,
                    buttonGroupRect.min.y,
                    buttonGroupRect.width * 0.5f,
                    buttonGroupRect.height);
                if (GUI.Button(importButtonRect, "Import...", EditorStyles.miniButtonRight))
                {
                    ShowImportDataContextMenu(importButtonRect, instance);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel -= 1;
            }
            instance.ShowVertexWarperSceneGUI = instance.inspector.isDeformationGroupExpanded;

            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void ShowExportDataContextMenu(Rect r, TerrainGenerator instance)
        {
            GenericPopup menu = new GenericPopup();
            menu.AddItem(
                new GUIContent("3D file from Mesh"),
                false,
                () => { MeshSaverGUI.ShowWindow(instance); });
            menu.AddSeparator();
            menu.AddItem(
                new GUIContent("UV layout"),
                false,
                () => { TextureExporterGUI.ShowWindow(instance, TextureExporter.TextureType.UvLayout); });
            menu.AddItem(
                new GUIContent("Vertex color map"),
                false,
                () => { TextureExporterGUI.ShowWindow(instance, TextureExporter.TextureType.VertexColor); });
            menu.AddItem(
                new GUIContent("Height map"),
                false,
                () => { HeightMapExporterGUI.ShowWindow(instance); });
            menu.Show(r);
        }

        private static void ShowImportDataContextMenu(Rect r, TerrainGenerator instance)
        {
            GenericPopup menu = new GenericPopup();
            menu.AddDisabledItem(new GUIContent("Coming soon!"), false);
            menu.Show(r);
        }
    }
}
