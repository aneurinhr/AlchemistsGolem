using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace Pinwheel.PolarisBasic
{
    public class DistributionMapExporterGUI : EditorWindow
    {
        private EnvironmentalPainter painter;
        private string fileName;
        private int size;
        private int spread;
        private ImageFileType fileType;
        private string path;

        private GameObject prefab;

        private readonly string[] EDITOR_PREF_PATH_KEYS = new string[2] { "DistributionMapExporter", "path" };
        private readonly string[] EDITOR_PREF_SPREAD_KEYS = new string[2] { "DistributionMapExporter", "spread" };
        private readonly string[] EDITOR_PREF_SIZE_KEYS = new string[2] { "DistributionMapExporter", "size" };
        private readonly string[] EDITOR_PREF_FILE_TYPE_KEYS = new string[2] { "DistributionMapExporter", "filetype" };

        public static void ShowWindow(EnvironmentalPainter painter, int prefabIndex)
        {
            DistributionMapExporterGUI window = GetWindow<DistributionMapExporterGUI>();
            window.painter = painter;
            window.prefab = painter.Settings.GetPrefabs()[prefabIndex];
            window.titleContent = new GUIContent("Distribution Map Exporter");
            window.fileName = string.Format("{0}-{1}-{2}", window.painter.Terrain.name, window.prefab.name, "DistributionMap");
            window.ShowUtility();
        }

        private void OnEnable()
        {
            path = EditorPrefs.GetString(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_PATH_KEYS), "Assets/");
            spread = EditorPrefs.GetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_SPREAD_KEYS), 30);
            size = EditorPrefs.GetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_SIZE_KEYS), 512);
            fileType = (ImageFileType)EditorPrefs.GetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_FILE_TYPE_KEYS), 0);
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_PATH_KEYS), path);
            EditorPrefs.SetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_SPREAD_KEYS), spread);
            EditorPrefs.SetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_SIZE_KEYS), size);
            EditorPrefs.SetInt(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_FILE_TYPE_KEYS), (int)fileType);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Prefab", prefab.name);
            fileName = EditorGUILayout.TextField("File name", fileName);
            spread = EditorGUILayout.IntSlider("Spread", spread, 1, 100);
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
            Transform group = painter.GetGroup(prefab);
            GameObject[] trees = Utilities.GetChildrenGameObjects(group);
            EnvironmentalPainter.DistributionMapExporter exporter = new EnvironmentalPainter.DistributionMapExporter(
                trees, painter.Terrain, size, spread);
            Texture2D map = exporter.ExportTextureData();
            TextureExporter.ExportCustomTexture(map, path, fileName, fileType);
            DestroyImmediate(map);
            Close();
        }
    }
}
