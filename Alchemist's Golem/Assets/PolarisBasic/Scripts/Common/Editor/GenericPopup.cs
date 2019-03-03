using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Threading;

namespace Pinwheel.PolarisBasic
{
    public class GenericPopup : PopupWindowContent
    {
        public class PopupItem
        {
            public GUIContent content;
            public bool isOn;
            public bool isDisabled;
            public bool isSeparator;
            public Action action;

            public PopupItem(GUIContent content, bool isOn, Action action)
            {
                this.content = content;
                this.isOn = isOn;
                this.action = action;
            }
        }

        private const string CHECK_MARK_ICON_PATH = "Assets/Polaris/Textures/EditorIcons/check.png";
        private const int VERTICAL_PADDING = 3;

        private List<PopupItem> items;
        private List<PopupItem> Items
        {
            get
            {
                if (items == null)
                    items = new List<PopupItem>();
                return items;
            }
        }

        public void AddItem(GUIContent content, bool isOn, Action action)
        {
            Items.Add(new PopupItem(content, isOn, action));
        }

        public void AddDisabledItem(GUIContent content, bool isOn)
        {
            PopupItem i = new PopupItem(content, isOn, null);
            i.isDisabled = true;
            Items.Add(i);
        }

        public void AddSeparator()
        {
            PopupItem i = new PopupItem(null, false, null);
            i.isSeparator = true;
            Items.Add(i);
        }

        public void Show(Rect activatorRect)
        {
            PopupWindow.Show(activatorRect, this);
        }

        public override void OnOpen()
        {
            base.OnOpen();
            editorWindow.wantsMouseMove = true;
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUI.DrawRect(rect, Color.white * 0.9f);
            EditorGUILayout.GetControlRect(GUILayout.Height(VERTICAL_PADDING));
            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i].isSeparator)
                {
                    EditorCommon.Separator();
                    continue;
                }
                GUI.enabled = !Items[i].isDisabled;
                EditorGUILayout.BeginHorizontal();
                Rect itemRect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorCommon.standardHeight));
                if (itemRect.Contains(Event.current.mousePosition))
                {
                    EditorGUI.DrawRect(itemRect, EditorCommon.selectedItemColor);
                }

                Rect checkmarkRect = new Rect(itemRect.min.x, itemRect.min.y, itemRect.height, itemRect.height);
                if (Items[i].isOn)
                {
                    GUIContent icon = EditorGUIUtility.IconContent(CHECK_MARK_ICON_PATH);
                    EditorGUI.LabelField(checkmarkRect, icon);
                }

                Rect buttonRect = new Rect(itemRect.min.x + itemRect.height, itemRect.min.y, itemRect.width - itemRect.height, itemRect.height);
                if (GUI.Button(buttonRect, Items[i].content, GUIStyle.none))
                {
                    InvokeAction(Items[i].action);
                    editorWindow.Close();
                }
                EditorGUILayout.EndHorizontal();
                GUI.enabled = true;
            }

            EditorGUILayout.GetControlRect(GUILayout.Height(VERTICAL_PADDING));

            if (Event.current.type == EventType.MouseMove)
                editorWindow.Repaint();
        }

        public override Vector2 GetWindowSize()
        {
            int maxLength = 0;
            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i].content != null &&
                    Items[i].content.text != null &&
                    Items[i].content.text.Length > maxLength)
                {
                    maxLength = Items[i].content.text.Length;
                }
            }

            return new Vector2(
                Mathf.Max(150, maxLength * 8),
                Items.Count * (EditorCommon.standardHeight + EditorGUIUtility.standardVerticalSpacing) + 2 * (VERTICAL_PADDING + EditorGUIUtility.standardVerticalSpacing));
        }

        private void InvokeAction(Action action)
        {
            if (action == null)
                return;
            action.Invoke();
        }
    }
}
