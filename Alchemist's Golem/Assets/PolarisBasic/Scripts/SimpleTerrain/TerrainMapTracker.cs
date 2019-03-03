using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.PolarisBasic
{
    /// <summary>
    /// Utility class for tracking height map changed on asset import
    /// </summary>
    public class TerrainMapTracker
    {
        private static List<TerrainGenerator> terrains;
        public static List<TerrainGenerator> Terrains
        {
            get
            {
                if (terrains == null)
                    terrains = new List<TerrainGenerator>();
                return terrains;
            }
        }

        private static List<TerrainGenerator> terrainsToUpdate;
        public static List<TerrainGenerator> TerrainsToUpdate
        {
            get
            {
                if (terrainsToUpdate == null)
                    terrainsToUpdate = new List<TerrainGenerator>();
                return terrainsToUpdate;
            }
        }

        public static void Register(TerrainGenerator t)
        {
            if (!Terrains.Contains(t) && t != null)
                Terrains.Add(t);
        }

        public static void UnRegister(TerrainGenerator t)
        {
            Terrains.Remove(t);
        }

        public static void CheckTerrainMapChanged(Texture2D tex)
        {
            Terrains.RemoveAll(t => t == null);
            for (int i = 0; i < Terrains.Count; ++i)
            {
                if (Terrains[i].HeightMap == null)
                    continue;

                if (IsPointingToSameTexture(Terrains[i].HeightMap, tex))
                {
                    TerrainsToUpdate.Add(Terrains[i]);
                }
            }
        }

        public static void UpdateTerrains()
        {
            for (int i=0;i<TerrainsToUpdate.Count;++i)
            {
                TerrainsToUpdate[i].SendMessage("OnHeightmapUpdated", SendMessageOptions.DontRequireReceiver);
                Debug.Log("Update terrain: " + Terrains[i].name);
            }
            TerrainsToUpdate.Clear();
        }

        private static bool IsPointingToSameTexture(Texture2D terrainHeightmap, Texture2D importedTexture)
        {
#if UNITY_EDITOR
            string srcPath = UnityEditor.AssetDatabase.GetAssetPath(terrainHeightmap);
            string desPath = importedTexture.name;
            return srcPath != null && srcPath.Equals(desPath);
#else
            return terrainHeightmap == importedTexture;
#endif
        }
    }
}
