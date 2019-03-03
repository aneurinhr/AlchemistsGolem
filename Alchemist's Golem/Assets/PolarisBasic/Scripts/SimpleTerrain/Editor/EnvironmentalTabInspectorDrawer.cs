using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.PolarisBasic
{
    public static class EnvironmentalTabInspectorDrawer
    {
        public static void DrawGUI(TerrainGenerator instance, ref int libraryObjectPickerControlId)
        {
            DrawEnvironmentalBrushSettingsGroup(instance);
            DrawLibraryGroup(instance, ref libraryObjectPickerControlId);
            DrawCombinationsGroup(instance);
        }

        private static void DrawEnvironmentalBrushSettingsGroup(TerrainGenerator instance)
        {
            instance.inspector.isEnvironmentalBrushSettingsGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isEnvironmentalBrushSettingsGroupExpanded, "Brush");

            if (instance.inspector.isEnvironmentalBrushSettingsGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                EnvironmentalPainter.ToolsSettings settings = instance.EnvironmentalPainter.Settings;
                settings.mode = (EnvironmentalPainter.Mode)EditorGUILayout.EnumPopup("Mode", settings.mode);
                settings.brushRadius = EditorGUILayout.FloatField("Radius", settings.brushRadius);
                settings.density = EditorGUILayout.IntField("Density", settings.density);
                settings.scaleMin = EditorGUILayout.FloatField("Scale Min", settings.scaleMin);
                settings.scaleMax = EditorGUILayout.FloatField("Scale Max", settings.scaleMax);
                settings.maxRotation = EditorGUILayout.FloatField("Max Rotation", settings.maxRotation);
                settings.combinationPhysicalSize = EditorGUILayout.FloatField("Combinations Physical Size", settings.combinationPhysicalSize);
                settings.keepColliders = EditorGUILayout.Toggle("Keep Colliders For Groups", settings.keepColliders);
                settings.Validate();

                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void DrawLibraryGroup(TerrainGenerator instance, ref int libraryObjectPickerControlId)
        {
            instance.inspector.isLibraryGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isLibraryGroupExpanded, "Library");
            List<GameObject> prefabs = instance.EnvironmentalPainterSettings.GetPrefabs();

            if (instance.inspector.isLibraryGroupExpanded)
            {
                EditorGUILayout.Space();
                if (prefabs != null && prefabs.Count > 0)
                {
                    float width = EditorGUIUtility.currentViewWidth - EditorGUI.indentLevel * EditorGUIUtility.singleLineHeight;
                    Vector2 tileSize = new Vector2(75, 75);
                    int itemPerRow = (int)(width / tileSize.x);
                    itemPerRow = Mathf.Clamp(itemPerRow, 1, prefabs.Count);
                    int numberOfRow = Mathf.CeilToInt(prefabs.Count * 1.0f / itemPerRow);
                    for (int i = 0; i < itemPerRow * numberOfRow; ++i)
                    {
                        if (i % itemPerRow == 0)
                            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

                        DrawPrefabTile(instance, tileSize, i);

                        if ((i + 1) % itemPerRow == 0)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Try adding a prefab!", MessageType.None);
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add", GUILayout.Width(EditorCommon.standardWidth), GUILayout.Height(EditorCommon.standardHeight)))
                {
                    HandleAddPrefabButtonClicked(ref libraryObjectPickerControlId);
                }
                GUI.enabled = prefabs != null && prefabs.Count > 0;
                if (GUILayout.Button("Remove", GUILayout.Width(EditorCommon.standardWidth), GUILayout.Height(EditorCommon.standardHeight)))
                {
                    HandleRemovePrefabButtonClicked(instance);
                }
                GUI.enabled = true;
                if (GUILayout.Button("Reset", GUILayout.Width(EditorCommon.standardWidth), GUILayout.Height(EditorCommon.standardHeight)))
                {
                    HandleResetButtonClicked(instance);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static void DrawPrefabTile(TerrainGenerator instance, Vector2 tileSize, int i)
        {
            List<GameObject> prefabs = instance.EnvironmentalPainterSettings.GetPrefabs();
            int selectedPrefabIndex = instance.EnvironmentalPainterSettings.prefabIndex;
            EditorGUILayout.BeginVertical();
            Rect buttonRect = EditorGUILayout.GetControlRect(GUILayout.Width(tileSize.x), GUILayout.Height(tileSize.y));
            if (i < prefabs.Count)
            {
                if (selectedPrefabIndex == i)
                {
                    RectOffset ro = new RectOffset(3, 3, 3, 3);
                    Rect highlightRect = ro.Add(buttonRect);
                    EditorGUI.DrawRect(highlightRect, EditorCommon.selectedItemColor);
                }

                Vector2 contextButtonSize = new Vector2(15, 10);
                Vector2 contextButtonPosition = new Vector2(
                    buttonRect.x + buttonRect.width - contextButtonSize.x,
                    buttonRect.y);
                Rect contextButtonRect = new Rect(contextButtonPosition, contextButtonSize);
                if (GUI.Button(contextButtonRect, ""))
                {
                    instance.EnvironmentalPainterSettings.prefabIndex = i;
                    instance.EnvironmentalPainter.UpdatePreviewer();
                    DisplayContextMenuForPrefab(contextButtonRect, instance, i);
                }

                if (GUI.Button(buttonRect, "", EditorCommon.EvenFlatButton))
                {
                    instance.EnvironmentalPainterSettings.prefabIndex = i;
                    instance.EnvironmentalPainter.UpdatePreviewer();
                }

                Texture t = AssetPreview.GetAssetPreview(prefabs[i]);
                if (AssetPreview.IsLoadingAssetPreview(prefabs[i].GetInstanceID()))
                {
                    int dotCount = System.DateTime.Now.Millisecond / 300;
                    GUI.Label(buttonRect, EditorCommon.DOT_ANIM[dotCount], EditorCommon.CenteredLabel);
                }
                else if (t == null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(prefabs[i]);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
                    }
                }
                else
                {
                    EditorGUI.DrawPreviewTexture(buttonRect, t);
                }

                EditorGUI.LabelField(contextButtonRect, "••• ", GuiStyleUtilities.RightAlignedWhiteMiniLabel);

                Vector2 countBadgeSize = new Vector3(tileSize.x, 20);
                Rect countBadgeRect = new Rect(buttonRect.max - countBadgeSize, countBadgeSize);
                EditorGUI.LabelField(
                    countBadgeRect,
                    instance.EnvironmentalPainter.GetGroupChildCount(prefabs[i]).ToString() + " ",
                    GuiStyleUtilities.RightAlignedWhiteLabel);
                EditorGUILayout.LabelField(prefabs[i].name, EditorCommon.CenteredLabel, GUILayout.MaxWidth(tileSize.x));
            }
            EditorGUILayout.EndVertical();
        }

        private static void DisplayContextMenuForPrefab(Rect r, TerrainGenerator instance, int i)
        {
            int childCount = instance.EnvironmentalPainter.GetGroupChildCount(i);
            bool active = instance.EnvironmentalPainter.IsGroupActive(i);

            GameObject prefab = instance.EnvironmentalPainterSettings.GetPrefabs()[i];
            bool hasCombination = instance.EnvironmentalPainter.GetCombinationsGroup(prefab).childCount > 0;

            GenericPopup menu = new GenericPopup();
            if (childCount > 0 && active)
            {
                menu.AddItem(new GUIContent("Group"), false, () => { GroupPrefabInstancesContextHandler(instance, i); });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Group"), false);
            }
            if (childCount > 0 && active && hasCombination)
            {
                menu.AddItem(new GUIContent("UnGroup"), false, () => { UnGroupPrefabInstancesContextHandler(instance, i); });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("UnGroup"), false);
            }
            menu.AddSeparator();
            if (childCount > 0 && !active)
            {
                menu.AddItem(new GUIContent("Show All"), false, () => { ShowAllPrefabInstancesContextHandler(instance, i); });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Show All"), false);
            }
            if (childCount > 0 && active)
            {
                menu.AddItem(new GUIContent("Hide All"), false, () => { HideAllPrefabInstancesContextHandler(instance, i); });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Hide All"), false);
            }
            menu.AddSeparator();
            menu.AddItem(new GUIContent("Export Distribution Map"), false, () => { ExportDistributionMapContextHandler(instance, i); });
            menu.AddItem(new GUIContent("Mass Placement"), false, () => { MassPlacementContextHandler(instance, i); });
            menu.AddSeparator();
            if (childCount > 0)
            {
                menu.AddItem(new GUIContent("Clear All"), false, () => { ClearAllPrefabInstancesContextHandler(instance, i); });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Clear All"), false);
            }

            menu.Show(r);
        }

        private static void GroupPrefabInstancesContextHandler(TerrainGenerator instance, int i)
        {
            instance.EnvironmentalPainter.CombinePrefabInstances(i);
        }

        private static void UnGroupPrefabInstancesContextHandler(TerrainGenerator instance, int i)
        {
            instance.EnvironmentalPainter.RemoveAllCombinationAndActivateInstances(i);
        }

        private static void ShowAllPrefabInstancesContextHandler(TerrainGenerator instance, int i)
        {
            instance.EnvironmentalPainter.SetAllPrefabInstancesActive(i, true);
        }

        private static void HideAllPrefabInstancesContextHandler(TerrainGenerator instance, int i)
        {
            instance.EnvironmentalPainter.SetAllPrefabInstancesActive(i, false);
        }

        private static void ExportDistributionMapContextHandler(TerrainGenerator instance, int i)
        {
            DistributionMapExporterGUI.ShowWindow(instance.EnvironmentalPainter, i);
        }

        private static void MassPlacementContextHandler(TerrainGenerator instance, int i)
        {
            MassPlacementGUI.ShowWindow(instance.EnvironmentalPainter, i);
        }

        private static void ClearAllPrefabInstancesContextHandler(TerrainGenerator instance, int i)
        {
            if (EditorUtility.DisplayDialog(
                "Confirm",
                "Clear all instances of this prefab?",
                "Yes",
                "No"))
            {
                instance.EnvironmentalPainter.ClearAllPrefabInstances(i);
            }
        }

        private static void HandleAddPrefabButtonClicked(ref int libraryObjectPickerControlId)
        {
            EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:Prefab", libraryObjectPickerControlId);
        }

        private static void HandleRemovePrefabButtonClicked(TerrainGenerator instance)
        {
            if (EditorUtility.DisplayDialog(
                "Confirm",
                "Remove this prefab from the Library?\n" +
                "This will remove all of its instances too!",
                "Yes", "No"))
            {
                instance.EnvironmentalPainter.ClearAllPrefabInstances(instance.EnvironmentalPainterSettings.prefabIndex);
                instance.EnvironmentalPainterSettings.RemovePrefabAt(instance.EnvironmentalPainterSettings.prefabIndex);
                instance.EnvironmentalPainter.UpdatePreviewer();
            }
        }

        private static void HandleResetButtonClicked(TerrainGenerator instance)
        {
            if (EditorUtility.DisplayDialog(
                "Confirm",
                "Hard reset the brush gallery? This will cause all spawned instances to be removed!",
                "Yes", "No"))
            {
                instance.EnvironmentalPainter.HardReset();
            }
        }

        private static void DrawCombinationsGroup(TerrainGenerator instance)
        {
            instance.inspector.isCombinationsGroupExpanded = EditorCommon.GroupFoldout(instance.inspector.isCombinationsGroupExpanded, "Combinations");
            instance.EnvironmentalPainterSettings.drawCombinationsBounds = instance.inspector.isCombinationsGroupExpanded;
            if (instance.inspector.isCombinationsGroupExpanded)
            {
                EditorGUI.indentLevel += 1;

                List<GameObject> prefabs = instance.EnvironmentalPainterSettings.GetPrefabs();
                List<Transform> combinationGroups = GetAllCombinationGroups(instance);
                bool hasNonEmptyGroup = combinationGroups.Find(t => t.childCount != 0) != null;
                if (!hasNonEmptyGroup)
                {
                    EditorGUILayout.HelpBox("No combination created yet!", MessageType.None);
                }
                else
                {
                    Vector2 tileSize = new Vector2(50, 50);
                    for (int i = 0; i < combinationGroups.Count; ++i)
                    {
                        if (combinationGroups[i].childCount == 0)
                            continue;
                        EditorGUILayout.BeginHorizontal();
                        Rect tileRect = EditorGUILayout.GetControlRect(GUILayout.Width(tileSize.x), GUILayout.Height(tileSize.y));
                        Texture t = AssetPreview.GetAssetPreview(prefabs[i]);
                        if (AssetPreview.IsLoadingAssetPreview(prefabs[i].GetInstanceID()))
                        {
                            int dotCount = System.DateTime.Now.Millisecond / 300;
                            GUI.Label(tileRect, EditorCommon.DOT_ANIM[dotCount], EditorCommon.CenteredLabel);
                        }
                        else if (t == null)
                        {
                            string assetPath = AssetDatabase.GetAssetPath(prefabs[i]);
                            if (!string.IsNullOrEmpty(assetPath))
                            {
                                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
                            }
                        }
                        else
                        {
                            EditorGUI.DrawPreviewTexture(tileRect, t);
                        }

                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField(prefabs[i].name);
                        EditorGUILayout.LabelField("Count: " + combinationGroups[i].childCount, EditorCommon.ItalicLabel);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.GetControlRect(GUILayout.Height(1));
        }

        private static List<Transform> GetAllCombinationGroups(TerrainGenerator instance)
        {
            List<GameObject> prefabs = instance.EnvironmentalPainterSettings.GetPrefabs();
            List<Transform> combinationGroups = new List<Transform>();
            for (int i = 0; i < prefabs.Count; ++i)
            {
                Transform group = instance.EnvironmentalPainter.GetCombinationsGroup(prefabs[i]);
                combinationGroups.Add(group);
            }
            return combinationGroups;
        }
    }
}
