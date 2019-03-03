using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.PolarisBasic
{
    public static class GeometryTabInspectorDrawer
    {
        public static void DrawGUI(TerrainGenerator instance)
        {
            DrawGeometryBrushSettingsGroup(instance);
        }

        private static void DrawGeometryBrushSettingsGroup(TerrainGenerator instance)
        {
            instance.inspector.isGeometryBrushSettingsGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isGeometryBrushSettingsGroupExpanded, "Brush");

            if (instance.inspector.isGeometryBrushSettingsGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                GeometryPainter.ToolsSettings settings = instance.GeometryPainter.Settings;
                EditorGUI.BeginChangeCheck();
                settings.brushMode = (GeometryPainter.BrushMode)EditorGUILayout.EnumPopup("Mode", settings.brushMode);
                if (EditorGUI.EndChangeCheck())
                {
                    instance.GeometryPainter.UpdateHandle();
                }
                settings.brushRadius = EditorGUILayout.FloatField("Radius", settings.brushRadius);
                settings.strength = EditorGUILayout.FloatField("Strength", settings.strength);
                settings.Validate();

                if (EditorCommon.RightAnchoredButton("Erase All"))
                {
                    ConfirmAndResetElevations(instance);
                }

                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void ConfirmAndResetElevations(TerrainGenerator instance)
        {
            if (EditorUtility.DisplayDialog(
                "Confirm",
                "Reset geometry painting data for this terrain?\nThis action cannot be undone!",
                "OK", "Cancel"))
            {
                instance.GeometryPainter.ResetElevations();
            }
        }
    }
}
