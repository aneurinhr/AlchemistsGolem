using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Pinwheel.PolarisBasic
{
    public class TextureExporterGUI : EditorWindow
    {
        private TextureExporter.Source source;
        private MeshFilter targetMeshFilter;
        private Mesh targetMesh;
        private string fileName;
        private string path;
        private int size;
        private ImageFileType fileType;
        private TextureExporter.TextureType textureType;

        private readonly string[] EDITOR_PREF_SRC_KEYS = new string[2] { "TextureExporter", "source" };
        private readonly string[] EDITOR_PREF_PATH_KEYS = new string[2] { "TextureExporter", "path" };
        private readonly string[] EDITOR_PREF_SIZE_KEYS = new string[2] { "TextureExporter", "size" };
        private readonly string[] EDITOR_PREF_FILE_TYPE_KEYS = new string[2] { "TextureExporter", "filetype" };
        private readonly string[] EDITOR_PREF_TEXTURE_TYPE_KEYS = new string[2] { "TextureExporter", "texturetype" };

        public static void ShowWindow()
        {
            TextureExporterGUI window = GetWindow<TextureExporterGUI>();
            window.titleContent = new GUIContent("Texture Exporter");
            window.Show();
        }

        public static void ShowWindow(TerrainGenerator terrain, TextureExporter.TextureType type)
        {
            TextureExporterGUI window = GetWindow<TextureExporterGUI>();
            window.titleContent = new GUIContent("Texture Exporter");
            window.source = TextureExporter.Source.FromMeshFilter;
            window.targetMeshFilter = terrain.MeshFilterComponent;
            window.fileName = string.Format("{0}-{1}", terrain.name, type.ToString());
            window.textureType = type;
            window.Show();
        }

        private void OnEnable()
        {
            source = (TextureExporter.Source)EditorPrefs.GetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_SRC_KEYS), 0);
            path = EditorPrefs.GetString(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_PATH_KEYS), "Assets/");
            size = EditorPrefs.GetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_SIZE_KEYS), 512);
            fileType = (ImageFileType)EditorPrefs.GetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_FILE_TYPE_KEYS), 0);
            textureType = (TextureExporter.TextureType)EditorPrefs.GetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_TEXTURE_TYPE_KEYS), 0);
        }

        private void OnDisable()
        {
            EditorPrefs.SetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_SRC_KEYS), (int)source);
            EditorPrefs.SetString(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_PATH_KEYS), path);
            EditorPrefs.SetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_SIZE_KEYS), size);
            EditorPrefs.SetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_FILE_TYPE_KEYS), (int)fileType);
            EditorPrefs.SetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_TEXTURE_TYPE_KEYS), (int)textureType);
        }

        private void OnGUI()
        {
            source = (TextureExporter.Source)EditorGUILayout.EnumPopup("Source", source);
            if (source == TextureExporter.Source.FromMeshFilter)
                targetMeshFilter = EditorGUILayout.ObjectField("Target", targetMeshFilter, typeof(MeshFilter), true) as MeshFilter;
            else
                targetMesh = EditorGUILayout.ObjectField("Target", targetMesh, typeof(Mesh), true) as Mesh;

            fileName = EditorGUILayout.TextField("File name", fileName);
            size = EditorGUILayout.IntPopup("Size", size, TextureExporter.textureSizeName, TextureExporter.textureSize);
            fileType = (ImageFileType)EditorGUILayout.EnumPopup("File type", fileType);
            textureType = (TextureExporter.TextureType)EditorGUILayout.EnumPopup("Texture type", textureType);
            EditorCommon.BrowseFolder("Path", ref path);

            GUI.enabled = CanSave();
            if (EditorCommon.RightAnchoredButton("Save"))
            {
                bool willSave = false;
                string fullName = string.Format("{0}{1}", fileName, TextureExporter.GetExtension(fileType));
                string fullPath = Path.Combine(path, fullName);
                if (System.IO.File.Exists(fullPath))
                {
                    if (EditorUtility.DisplayDialog(
                        "File existed",
                        "The file at specified path is already existed.\nWould you like to overwrite it?",
                        "Yes",
                        "No"))
                    {
                        willSave = true;
                    }
                }
                else
                {
                    willSave = true;
                }

                if (willSave)
                {
                    if (source == TextureExporter.Source.FromMeshFilter)
                        TextureExporter.Export(targetMeshFilter, path, fileName, size, fileType, textureType);
                    else
                        TextureExporter.Export(targetMesh, path, fileName, size, fileType, textureType);
                }
            }
            GUI.enabled = true;
        }

        private bool CanSave()
        {
            bool validMeshFilter =
                source == TextureExporter.Source.FromMeshFilter &&
                targetMeshFilter != null &&
                targetMeshFilter.sharedMesh != null;
            bool validMesh =
                source == TextureExporter.Source.FromMesh &&
                targetMesh != null;
            bool validPath =
                !string.IsNullOrEmpty(fileName) &&
                !string.IsNullOrEmpty(path);
            return (validMeshFilter || validMesh) && validPath;
        }
    }
}