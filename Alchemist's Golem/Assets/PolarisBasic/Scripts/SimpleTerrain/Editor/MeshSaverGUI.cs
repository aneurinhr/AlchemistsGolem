using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pinwheel.PolarisBasic
{
    public class MeshSaverGUI : EditorWindow
    {
        private MeshFilter target;
        private string meshName;
        private string path;
        private MeshSaver.FileType fileType;

        private readonly string[] EDITOR_PREF_KEYS = new string[2] { "meshsaver", "path" };

        public static void ShowWindow()
        {
            MeshSaverGUI window = GetWindow<MeshSaverGUI>();
            window.titleContent = new GUIContent("Mesh Saver");
            window.Show();
        }

        public static void ShowWindow(TerrainGenerator terrain)
        {
            MeshSaverGUI window = GetWindow<MeshSaverGUI>();
            window.titleContent = new GUIContent("Mesh Saver");
            window.target = terrain.MeshFilterComponent;
            window.meshName = terrain.name;
            window.Show();
        }

        private void OnEnable()
        {
            path = EditorPrefs.GetString(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_KEYS), "Assets/");
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_KEYS), path);
        }

        private void OnGUI()
        {
            target = EditorGUILayout.ObjectField("Target", target, typeof(MeshFilter), true) as MeshFilter;
            meshName = EditorGUILayout.TextField("Mesh name", meshName);
            fileType = (MeshSaver.FileType)EditorGUILayout.EnumPopup("File type", fileType);
            EditorCommon.BrowseFolder("Path", ref path);
            GUI.enabled =
                target != null &&
                target.sharedMesh != null &&
                !string.IsNullOrEmpty(meshName) &&
                !string.IsNullOrEmpty(path);
            if (EditorCommon.RightAnchoredButton("Save"))
            {
                Material mat = null;
                MeshRenderer mr = target.GetComponent<MeshRenderer>();
                if (mr != null)
                    mat = mr.sharedMaterial;
                MeshSaver.Save(target.sharedMesh, mat, path, meshName, fileType);
            }
            GUI.enabled = true;
        }
    }
}