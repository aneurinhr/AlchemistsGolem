using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.PolarisBasic
{
    public static class ColorPainterTabInspectorDrawer
    {
        public static void DrawGUI(TerrainGenerator instance)
        {
            DrawBrushSettingsGroup(instance);
            DrawPalleteGroup(instance);
        }

        private static void DrawBrushSettingsGroup(TerrainGenerator instance)
        {
            instance.inspector.isColorBrushSettingsGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isColorBrushSettingsGroupExpanded, "Brush");
            if (instance.inspector.isColorBrushSettingsGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                ColorPainter.ToolsSettings settings = instance.ColorPainterSettings;
                settings.brushSettings.brushRadius = EditorGUILayout.FloatField("Radius", settings.brushSettings.brushRadius);
                settings.brushSettings.strength = EditorGUILayout.Slider("Strength", settings.brushSettings.strength, 0f, 1f);
                settings.brushSettings.color = EditorGUILayout.ColorField("Color", settings.brushSettings.color);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (EditorCommon.Button("Add to Palette"))
                {
                    AddBrushToColorPaletteWindow.Show(instance.ColorPainter);
                }
                if (EditorCommon.Button("Erase All"))
                {
                    ConfirmAndEraseAll(instance);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel -= 1;
            }
            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void ConfirmAndEraseAll(TerrainGenerator instance)
        {
            if (EditorUtility.DisplayDialog(
                "Confirm",
                "Erase all color painting data for this terrain?\nThis action cannot be undone!",
                "OK", "Cancel"))
            {
                instance.ColorPainter.ResetColors();
            }
        }

        private static void DrawPalleteGroup(TerrainGenerator instance)
        {
            List<ColorPainter.BrushSettings> brushes = instance.ColorPainter.Settings.Palette;
            instance.inspector.isPalleteGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isPalleteGroupExpanded, "Pallete");
            if (instance.inspector.isColorBrushSettingsGroupExpanded)
            {
                EditorGUILayout.Space();
                if (brushes != null && brushes.Count > 0)
                {
                    float width = EditorGUIUtility.currentViewWidth - EditorGUI.indentLevel * EditorGUIUtility.singleLineHeight;
                    Vector2 tileSize = new Vector2(75, 75);
                    int itemPerRow = (int)(width / tileSize.x);
                    itemPerRow = Mathf.Clamp(itemPerRow, 1, brushes.Count);
                    int numberOfRow = Mathf.CeilToInt(brushes.Count * 1.0f / itemPerRow);
                    for (int i = 0; i < itemPerRow * numberOfRow; ++i)
                    {
                        if (i % itemPerRow == 0)
                            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

                        DrawBrushTile(instance, tileSize, i);

                        if ((i + 1) % itemPerRow == 0)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Try adding a brush!", MessageType.None);
                }
            }
            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void DrawBrushTile(TerrainGenerator instance, Vector2 tileSize, int i)
        {
            List<ColorPainter.BrushSettings> brushes = instance.ColorPainter.Settings.Palette;
            EditorGUILayout.BeginVertical();
            Rect buttonRect = EditorGUILayout.GetControlRect(GUILayout.Width(tileSize.x), GUILayout.Height(tileSize.y));
            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
            if (i < brushes.Count)
            {
                Vector2 contextButtonSize = new Vector2(15, 10);
                Vector2 contextButtonPosition = new Vector2(
                    buttonRect.x + buttonRect.width - contextButtonSize.x,
                    buttonRect.y);
                Rect contextButtonRect = new Rect(contextButtonPosition, contextButtonSize);
                if (GUI.Button(contextButtonRect, ""))
                {
                    DisplayContextMenuForBrush(contextButtonRect, instance, i);
                }

                Color baseColor = ColorLibrary.GetColor(brushes[i].color, 1);
                float alpha = brushes[i].color.a;
                EditorGUI.DrawRect(buttonRect, baseColor);

                Rect alphaBgRect = new Rect(buttonRect.x, buttonRect.max.y - 3, buttonRect.width, 3);
                Rect alphaRect = new Rect(buttonRect.x, buttonRect.max.y - 3, buttonRect.width * alpha, 3);
                EditorGUI.DrawRect(alphaBgRect, Color.black);
                EditorGUI.DrawRect(alphaRect, Color.white);

                GUIContent content = new GUIContent(string.Empty, brushes[i].ToString());
                if (GUI.Button(buttonRect, content, GUIStyle.none))
                {
                    instance.ColorPainter.ApplyBrushFromPaletteAtIndex(i);
                }

                GUIStyle contextLabelStyle = baseColor.grayscale >= 0.5f ? GuiStyleUtilities.RightAlignedGrayMiniLabel : GuiStyleUtilities.RightAlignedWhiteMiniLabel;
                EditorGUI.LabelField(contextButtonRect, "••• ", contextLabelStyle);

                EditorGUILayout.LabelField(brushes[i].name, EditorCommon.CenteredLabel, GUILayout.MaxWidth(tileSize.x));
            }
            EditorGUILayout.EndVertical();
        }

        private static void DisplayContextMenuForBrush(Rect r, TerrainGenerator instance, int i)
        {

            GenericPopup popup = new GenericPopup();
            popup.AddItem(
                new GUIContent("Edit"),
                false,
                () =>
                {
                    DisplayEditBrushPopup(instance, i);
                });
            popup.AddItem(
                new GUIContent("Remove"),
                false,
                () =>
                {
                    ConfirmAndRemoveBrushFromPaletteAt(instance, i);
                });

            popup.Show(r);
        }

        private static void ConfirmAndRemoveBrushFromPaletteAt(TerrainGenerator instance, int i)
        {
            if (EditorUtility.DisplayDialog(
                "Confirm",
                "Remove this brush from Palette?",
                "OK", "Cancel"))
            {
                instance.ColorPainter.RemoveBrushFromPaletteAtIndex(i);
            }
        }

        private static void DisplayEditBrushPopup(TerrainGenerator instance, int i)
        {
            EditBrushInColorPaletteWindow.Show(instance.ColorPainter, i);
        }
    }
}
