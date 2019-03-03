using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.PolarisBasic
{
    public partial class TerrainGenerator
    {
        public enum SurfaceTilingMode
        {
            Quad, Hexagon
        }

#if UNITY_EDITOR
        /// <summary>
        /// Utility class to store inspector information
        /// </summary>
        [System.Serializable]
        public class InspectorUtilities
        {
            public const string LARGE_DIMENSION_ERROR_MSG = "Terrain dimensions is too large, try reducing Width/Length or disable Flat Shading";

            public string errorMessage;

            public bool isGeneralGroupExpanded = true;
            public bool isOverallShapeGroupExpanded = false;
            public bool isSurfaceGroupExpanded = false;
            public bool isGroundThicknessGroupExpanded = false;
            public bool isRenderingGroupExpanded = false;
            public bool isDeformationGroupExpanded = false;
            public bool isUtilitiesGroupExpanded = false;
            public bool isDataGroupExpanded = false;

            public bool isGeometryBrushSettingsGroupExpanded = true;

            public bool isColorBrushSettingsGroupExpanded = true;
            public bool isPalleteGroupExpanded = false;

            public bool isEnvironmentalBrushSettingsGroupExpanded = true;
            public bool isLibraryGroupExpanded = true;
            public bool isCombinationsGroupExpanded = false;

            public bool isDebuggingGroupExpanded = false;
            public bool isHandlesColorGroupExpanded = false;
        }
#endif

        /// <summary>
        /// Indicate type of update geometry request
        /// </summary>
        public enum UpdateFlag
        {
            OnValidate, OnHeightmapModified, OnWarp, OnPaint, OnElevationsResetted, OnPaintEnded, OnColorsResetted
        }

        public enum MaterialTypes
        {
            Diffuse, Specular, Custom
        }

        public class HeightMapExporter
        {
            private TerrainGenerator terrain;
            private int textureSize;

            public HeightMapExporter(TerrainGenerator terrain, int textureSize)
            {
                this.terrain = terrain;
                this.textureSize = textureSize;
            }

            public Texture2D ExportTextureData()
            {
                Texture2D tex = new Texture2D(textureSize, textureSize);
                Color[] textureData = new Color[textureSize * textureSize];
                MeshData data = terrain.MeshData;
                int trisCount = data.triangles.Length / 3;
                for (int i = 0; i < trisCount; ++i)
                {
                    int i0 = data.triangles[i * 3 + 0];
                    int i1 = data.triangles[i * 3 + 1];
                    int i2 = data.triangles[i * 3 + 2];

                    Index2D i2d0 = Utilities.To2DIndex(i0, terrain.VerticesGridLength.X);
                    Index2D i2d1 = Utilities.To2DIndex(i1, terrain.VerticesGridLength.X);
                    Index2D i2d2 = Utilities.To2DIndex(i2, terrain.VerticesGridLength.X);

                    if (!terrain.IsSurfaceVertex(i2d0) || !terrain.IsSurfaceVertex(i2d1) || !terrain.IsSurfaceVertex(i2d2))
                        continue;

                    Vector2 uv0 = terrain.GetVertexUVOnHeightMap(i2d0);
                    Vector2 uv1 = terrain.GetVertexUVOnHeightMap(i2d1);
                    Vector2 uv2 = terrain.GetVertexUVOnHeightMap(i2d2);

                    float minHeight = terrain.BaseHeight;
                    float maxHeight = terrain.BaseHeight + terrain.SurfaceMaxHeight;
                    Color c0 = Color.white * Mathf.InverseLerp(minHeight, maxHeight, data.vertices[i0].y);
                    Color c1 = Color.white * Mathf.InverseLerp(minHeight, maxHeight, data.vertices[i1].y);
                    Color c2 = Color.white * Mathf.InverseLerp(minHeight, maxHeight, data.vertices[i2].y);

                    TextureExporter.DrawTriangleOnTexture(
                        tex,
                        new Vector2[] { uv0, uv1, uv2 },
                        new Color[] { c0, c1, c2 },
                        textureData);
                }
                tex.SetPixels(textureData);
                tex.Apply();
                return tex;
            }
        }
    }
}
