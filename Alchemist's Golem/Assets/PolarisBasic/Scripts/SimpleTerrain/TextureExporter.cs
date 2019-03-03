using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Pinwheel.PolarisBasic
{
    public class TextureExporter
    {
        public enum Source
        {
            FromMesh, FromMeshFilter
        }

        public enum TextureType
        {
            UvLayout, VertexColor
        }

        public static int[] textureSize = new int[] { 32, 64, 128, 256, 512, 1024, 2048, 4086, 8192 };
        public static string[] textureSizeName = new string[] { "32", "64", "128", "256", "512", "1024", "2048", "4086", "8192" };

        public static void Export(MeshFilter target, string filePath, string fileName, int size, ImageFileType fileType, TextureType textureType)
        {
            if (target == null || target.sharedMesh == null)
                return;
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileName))
                return;
            if (size <= 0)
                return;

            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.name = fileName;
            Color[] clearColors = new Color[size * size];
            Utilities.Fill(clearColors, new Color(1, 1, 1, 0));
            tex.SetPixels(clearColors);

            int[] tris = target.sharedMesh.triangles;
            Vector2[] uv = target.sharedMesh.uv;
            if (textureType == TextureType.UvLayout)
            {
                DrawUv(tex, tris, uv);
            }
            else if (textureType == TextureType.VertexColor)
            {
                Color[] vertexColors = target.sharedMesh.colors;
                DrawVertexColor(tex, tris, uv, vertexColors);
            }
            SaveTexture(tex, filePath, fileName, fileType);
            Object.DestroyImmediate(tex);
        }

        public static void Export(Mesh target, string filePath, string fileName, int size, ImageFileType fileType, TextureType textureType)
        {
            if (target == null)
                return;
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileName))
                return;
            if (size <= 0)
                return;

            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.name = fileName;
            Color[] clearColors = new Color[size * size];
            Utilities.Fill(clearColors, new Color(1, 1, 1, 0));
            tex.SetPixels(clearColors);

            int[] tris = target.triangles;
            Vector2[] uv = target.uv;
            if (textureType == TextureType.UvLayout)
            {
                DrawUv(tex, tris, uv);
            }
            else if (textureType == TextureType.VertexColor)
            {
                Color[] vertexColors = target.colors;
                DrawVertexColor(tex, tris, uv, vertexColors);
            }

            SaveTexture(tex, filePath, fileName, fileType);
            Object.DestroyImmediate(tex);
        }

        private static void DrawUv(Texture2D tex, int[] tris, Vector2[] uv)
        {
            Color c = Color.black;
            Vector2 uvStart;
            Vector2 uvEnd;
            Color[] textureData = tex.GetPixels();
            for (int i = 0; i < tris.Length - 2; i += 3)
            {
                uvStart = uv[tris[i]];
                uvEnd = uv[tris[i + 1]];
                DrawLineOnTexture(tex, uvStart, uvEnd, textureData, c);

                uvStart = uv[tris[i + 1]];
                uvEnd = uv[tris[i + 2]];
                DrawLineOnTexture(tex, uvStart, uvEnd, textureData, c);

                uvStart = uv[tris[i]];
                uvEnd = uv[tris[i + 2]];
                DrawLineOnTexture(tex, uvStart, uvEnd, textureData, c);
            }
            tex.SetPixels(textureData);
            tex.Apply();
        }

        private static void DrawVertexColor(Texture2D tex, int[] tris, Vector2[] uv, Color[] colors)
        {
            Color[] textureData = tex.GetPixels();
            Vector2[] trisUv = new Vector2[3];
            Color[] trisColors = new Color[3];
            for (int i = 0; i < tris.Length - 2; i += 3)
            {
                trisUv[0] = uv[tris[i]];
                trisColors[0] = colors[tris[i]];

                trisUv[1] = uv[tris[i + 1]];
                trisColors[1] = colors[tris[i + 1]];

                trisUv[2] = uv[tris[i + 2]];
                trisColors[2] = colors[tris[i + 2]];

                DrawTriangleOnTexture(tex, trisUv, trisColors, textureData);
            }
            tex.SetPixels(textureData);
            tex.Apply();
        }

        public static void DrawLineOnTexture(Texture2D tex, Vector2 uvStart, Vector2 uvEnd, Color[] textureData, Color c)
        {
            Vector2 startPoint = GetPixelCoord(tex, uvStart);
            Vector2 endPoint = GetPixelCoord(tex, uvEnd);
            Vector2 p = startPoint;
            while (p != endPoint)
            {
                int index = Mathf.RoundToInt(p.y) * tex.width + Mathf.RoundToInt(p.x);
                textureData[index] = c;
                p = Vector2.MoveTowards(p, endPoint, 1);
            }
        }

        public static void DrawTriangleOnTexture(Texture2D tex, Vector2[] trisUv, Color[] trisColor, Color[] textureData)
        {
            Utilities.ExpandTrisUvCoord(tex, trisUv);

            Vector2 texelSize = tex.texelSize;
            Vector2 min = new Vector2(
                Mathf.Min(trisUv[0].x, trisUv[1].x, trisUv[2].x),
                Mathf.Min(trisUv[0].y, trisUv[1].y, trisUv[2].y));
            Vector2 max = new Vector2(
                Mathf.Max(trisUv[0].x, trisUv[1].x, trisUv[2].x),
                Mathf.Max(trisUv[0].y, trisUv[1].y, trisUv[2].y));

            Vector3 barycentric = Vector3.zero;
            Vector2 uv;
            Color c;

            for (float x = min.x; x <= max.x; x += texelSize.x)
            {
                for (float y = min.y; y <= max.y; y += texelSize.y)
                {
                    uv = new Vector2(x, y);
                    Utilities.CalculateBarycentricCoord(uv, trisUv[0], trisUv[1], trisUv[2], ref barycentric);

                    if (barycentric.x < 0 || barycentric.y < 0 || barycentric.z < 0)
                        continue;

                    c = trisColor[0] * barycentric.x + trisColor[1] * barycentric.y + trisColor[2] * barycentric.z;
                    Vector2 p = GetPixelCoord(tex, uv);
                    int index = Mathf.RoundToInt(p.y) * tex.width + Mathf.RoundToInt(p.x);
                    textureData[index] = c;
                }
            }
        }

        public static Vector2 GetPixelCoord(Texture2D tex, Vector2 uv)
        {
            return new Vector2(
                Mathf.RoundToInt(uv.x * (tex.width - 1)),
                Mathf.RoundToInt(uv.y * (tex.height - 1)));
        }

        public static void SaveTexture(Texture2D tex, string filePath, string fileName, ImageFileType fileType)
        {
            byte[] data;
            if (fileType == ImageFileType.Png)
                data = tex.EncodeToPNG();
            else if (fileType == ImageFileType.Jpg)
                data = tex.EncodeToJPG();
            else
                data = null;
            string fullName = string.Format("{0}{1}", fileName, GetExtension(fileType));
            string fullPath = Path.Combine(filePath, fullName);
            System.IO.File.WriteAllBytes(fullPath, data);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public static string GetExtension(ImageFileType fileType)
        {
            if (fileType == ImageFileType.Png)
                return ".png";
            else if (fileType == ImageFileType.Jpg)
                return ".jpg";
            else
                return string.Empty;
        }

        public static void ExportCustomTexture(Texture2D tex, string filePath, string fileName, ImageFileType fileType)
        {
            SaveTexture(tex, filePath, fileName, fileType);
        }
    }
}