using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace Pinwheel.PolarisBasic
{
    public class MassPlacementGUI : EditorWindow
    {
        private TerrainGenerator terrain;
        private EnvironmentalPainter painter;
        private int prefabIndex;
        private GameObject prefab;
        private Texture2D map;
        private int density;
        private float densityMultiplier;
        private float scaleMin;
        private float scaleMax;
        private float maxRotation;
        private bool followNormals;
        private bool keepOldObjects;

        bool validTexture;

        private readonly string[] EDITOR_PREF_DISTRIBUTION_THRESHOLD_KEYS = new string[2] { "MassPlacement", "threshold" };
        private readonly string[] EDITOR_PREF_FOLLOW_NORMALS_KEYS = new string[2] { "MassPlacement", "follownormals" };
        private readonly string[] EDITOR_PREF_KEEP_OLD_OBJECTS_KEYS = new string[2] { "MassPlacement", "keepoldobjects" };

        public static void ShowWindow(EnvironmentalPainter painter, int prefabIndex)
        {
            MassPlacementGUI window = GetWindow<MassPlacementGUI>();
            window.terrain = painter.Terrain;
            window.painter = painter;
            window.prefabIndex = prefabIndex;
            window.prefab = painter.Settings.GetPrefabs()[prefabIndex];
            window.density = painter.Settings.density;
            window.scaleMin = painter.Settings.scaleMin;
            window.scaleMax = painter.Settings.scaleMax;
            window.maxRotation = painter.Settings.maxRotation;

            window.titleContent = new GUIContent("Mass Placement");
            window.ShowUtility();
        }

        private void OnEnable()
        {
            densityMultiplier = EditorPrefs.GetFloat(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_DISTRIBUTION_THRESHOLD_KEYS), 0.5f);
            followNormals = EditorPrefs.GetBool(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_FOLLOW_NORMALS_KEYS), false);
            keepOldObjects = EditorPrefs.GetBool(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_KEEP_OLD_OBJECTS_KEYS), false);
        }

        private void OnDisable()
        {
            EditorPrefs.SetFloat(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_DISTRIBUTION_THRESHOLD_KEYS), densityMultiplier);
            EditorPrefs.SetBool(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_FOLLOW_NORMALS_KEYS), followNormals);
            EditorPrefs.SetBool(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_KEEP_OLD_OBJECTS_KEYS), keepOldObjects);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Prefab", prefab.name);
            EditorGUI.BeginChangeCheck();
            Texture2D t = EditorGUILayout.ObjectField("Distribution Map", map, typeof(Texture2D), false) as Texture2D;
            bool hasChanged = EditorGUI.EndChangeCheck();
            if (hasChanged && t != null)
            {
                try
                {
                    t.GetPixel(0, 0);
                    validTexture = true;
                    map = t;
                }
                catch
                {
                    validTexture = false;
                    map = null;
                }
            }
            if (!validTexture)
            {
                EditorGUILayout.HelpBox("Please assign a Read/Write Enabled texture!", MessageType.None, false);
            }

            density = EditorGUILayout.IntSlider("Density", density, 1, 100);
            densityMultiplier = EditorGUILayout.Slider("Density Multiplier", densityMultiplier, 0f, 1f);
            scaleMin = EditorGUILayout.FloatField("Scale Min", scaleMin);
            scaleMax = EditorGUILayout.FloatField("Scale Max", scaleMax);
            maxRotation = EditorGUILayout.FloatField("Max Rotation", maxRotation);
            followNormals = EditorGUILayout.Toggle("Follow Normals", followNormals);
            keepOldObjects = EditorGUILayout.Toggle("Keep Old Objects", keepOldObjects);

            if (EditorCommon.RightAnchoredButton("OK"))
            {
                if (painter == null)
                    painter = terrain.EnvironmentalPainter;
                EnvironmentalPainter.MassPlacementProcessor massPlacementProcessor = new EnvironmentalPainter.MassPlacementProcessor(
                    painter.Terrain, map, density, densityMultiplier, scaleMin, scaleMax, maxRotation, followNormals);
                Matrix4x4[] matrices = massPlacementProcessor.CalculatePlacementData();
                painter.MassPlacing(prefabIndex, matrices, keepOldObjects);
            }
            GUI.enabled = true;
        }
    }
}
