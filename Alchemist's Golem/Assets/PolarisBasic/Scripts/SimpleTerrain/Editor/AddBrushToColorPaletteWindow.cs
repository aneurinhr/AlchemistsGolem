using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.PolarisBasic
{
	public class AddBrushToColorPaletteWindow : EditorWindow
	{
        private ColorPainter painter;

        private const string BRUSH_NAME_TEXT_BOX_NAME = "BrushNameTextBox";
        private const string DEFAULT_BRUSH_NAME = "New Brush";
        private string newBrushName;
        private string NewBrushName
        {
            get
            {
                if (string.IsNullOrEmpty(newBrushName))
                    newBrushName = DEFAULT_BRUSH_NAME;
                return newBrushName;
            }
            set
            {
                newBrushName = value;
            }
        }

        public static void Show(ColorPainter painter)
        {
            AddBrushToColorPaletteWindow window = EditorWindow.GetWindow<AddBrushToColorPaletteWindow>();
            window.titleContent = new GUIContent("Add brush");
            window.minSize = window.GetWindowSize();
            window.maxSize = window.GetWindowSize() + Vector2.one;
            window.painter = painter;
            window.ShowPopup();
        }

        private void OnFocus()
        {
            EditorGUI.FocusTextInControl(BRUSH_NAME_TEXT_BOX_NAME);
        }

        public void OnGUI()
		{
            bool willClose = false;
            EditorGUILayout.Space();
            GUI.SetNextControlName(BRUSH_NAME_TEXT_BOX_NAME);
            NewBrushName = EditorGUILayout.TextField("Brush Name", NewBrushName);
            GUI.enabled = !string.IsNullOrEmpty(newBrushName) && painter!=null;
            if (EditorCommon.RightAnchoredButton("OK"))
            {
                painter.AddCurrentSettingsAsNewBrush(NewBrushName);
                GUI.enabled = true;
                willClose = true;
            }
            GUI.enabled = true;

            if (focusedWindow != this || willClose) 
                Close();
		}

        public Vector2 GetWindowSize()
        {
            return new Vector2(300, 50);
        }
    }
}
