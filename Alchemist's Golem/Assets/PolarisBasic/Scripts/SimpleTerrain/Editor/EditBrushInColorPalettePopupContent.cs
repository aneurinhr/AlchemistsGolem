using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.PolarisBasic
{
	public class EditBrushInColorPaletteWindow : EditorWindow
	{
        private ColorPainter painter;
        private int index;

        private ColorPainter.BrushSettings brush;

        public static void Show(ColorPainter painter, int index)
        {
            EditBrushInColorPaletteWindow window = GetWindow<EditBrushInColorPaletteWindow>();
            window.titleContent = new GUIContent("Edit brush");
            window.minSize = window.GetWindowSize();
            window.maxSize = window.GetWindowSize() + Vector2.one;
            window.painter = painter;
            window.index = index;
            window.brush = new ColorPainter.BrushSettings();
            ColorPainter.BrushSettings.Copy(painter.Settings.Palette[index], window.brush);
            window.ShowPopup();
        }

		public void OnGUI()
		{
            bool willClose = false;
            EditorGUILayout.Space();

            brush.name = EditorGUILayout.TextField("Name", brush.name);
            brush.brushRadius = EditorGUILayout.FloatField("Radius", brush.brushRadius);
            brush.strength = EditorGUILayout.Slider("Strength", brush.strength, 0f, 1f);
            brush.color = EditorGUILayout.ColorField("Color", brush.color);

            GUI.enabled = !string.IsNullOrEmpty(brush.name) && painter!=null;
            if (EditorCommon.RightAnchoredButton("OK"))
            {
                painter.SetBrushAtIndex(index, brush);
                GUI.enabled = true;
                willClose = true;
            }
            GUI.enabled = true;
            if (willClose)
                Close();
		}

        public Vector2 GetWindowSize()
        {
            return new Vector2(300, 100);
        }
    }
}
