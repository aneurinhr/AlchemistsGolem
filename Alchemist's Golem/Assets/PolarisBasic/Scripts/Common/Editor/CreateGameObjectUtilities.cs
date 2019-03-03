using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.PolarisBasic
{
    public static class CreateGameObjectUtilities
    {
#if !POLARIS
        [MenuItem("GameObject/Polaris/Simple Low-Poly Terrain", false, 10)]
        public static void CreateDefaultSimpleTerrain(MenuCommand menuCmd)
        {
            GameObject g = new GameObject("Terrain");
            GameObjectUtility.SetParentAndAlign(g, menuCmd.context as GameObject);
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.transform.localScale = Vector3.one;

            MeshFilter mf = g.AddComponent<MeshFilter>();
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            MeshCollider mc = g.AddComponent<MeshCollider>();
            TerrainGenerator tg = g.AddComponent<TerrainGenerator>();

            tg.MeshFilterComponent = mf;
            tg.MeshRendererComponent = mr;
            tg.MeshColliderComponent = mc;

            Undo.RegisterCreatedObjectUndo(g, "Undo creating " + g.name);
        }
#endif
    }
}
