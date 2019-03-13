using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.PolarisBasic
{
    public class TerrainMapPostprocessor : AssetPostprocessor
    {
        private void OnPostprocessTexture(Texture2D tex)
        {
            tex.name = this.assetPath;
            TerrainMapTracker.CheckTerrainMapChanged(tex);
        }

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            TerrainMapTracker.UpdateTerrains();
        }
    }
}
