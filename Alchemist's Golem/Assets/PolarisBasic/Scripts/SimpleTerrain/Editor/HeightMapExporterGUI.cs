using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace Pinwheel.PolarisBasic
{
    public class HeightMapExporterGUI : EditorWindow
    {
        private TerrainGenerator terrain;
        private string fileName;
        private int size;
        private ImageFileType fileType;
        private string path;

        private readonly string[] EDITOR_PREF_PATH_KEYS = new string[2] { "HeightMapExporter", "path" };
        private readonly string[] EDITOR_PREF_SIZE_KEYS = new string[2] { "HeightMapExporter", "size" };
        private readonly string[] EDITOR_PREF_FILE_TYPE_KEYS = new string[2] { "HeightMapExporter", "filetype" };

        public static void ShowWindow(TerrainGenerator terrain)
        {
            HeightMapExporterGUI window = GetWindow<HeightMapExporterGUI>();
            window.terrain = terrain;
            window.titleContent = new GUIContent("Height Map Exporter");
            window.fileName = string.Format("{0}-{1}", terrain.name, "HeightMap");
            window.ShowUtility();
        }

        private void OnEnable()
        {
            path = EditorPrefs.GetString(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_PATH_KEYS), "Assets/");
            size = EditorPrefs.GetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_SIZE_KEYS), 512);
            fileType = (ImageFileType)EditorPrefs.GetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_FILE_TYPE_KEYS), 0);
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_PATH_KEYS), path);
            EditorPrefs.SetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_SIZE_KEYS), size);
            EditorPrefs.SetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_FILE_TYPE_KEYS), (int)fileType);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Terrain", terrain.name);
            fileName = EditorGUILayout.TextField("File name", fileName);
            size = EditorGUILayout.IntPopup("Size", size, TextureExporter.textureSizeName, TextureExporter.textureSize);
            fileType = (ImageFileType)EditorGUILayout.EnumPopup("File type", fileType);
            EditorCommon.BrowseFolder("Path", ref path);

            GUI.enabled = CanSave();
            if (EditorCommon.RightAnchoredButton("Export"))
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
                    Save();
                }
            }
            GUI.enabled = true;
        }

        private bool CanSave()
        {
            return !string.IsNullOrEmpty(fileName) &&
                !string.IsNullOrEmpty(path);
        }

        private void Save()
        {
            TerrainGenerator.HeightMapExporter exporter = new TerrainGenerator.HeightMapExporter(terrain, size);
            Texture2D map = exporter.ExportTextureData();
            TextureExporter.ExportCustomTexture(map, path, fileName, fileType);
            DestroyImmediate(map);
            Close();
        }
    }
}
