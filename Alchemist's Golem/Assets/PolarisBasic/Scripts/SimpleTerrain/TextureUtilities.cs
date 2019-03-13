using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.PolarisBasic
{
    /// <summary>
    /// Utility class for creating textures
    /// </summary>
    public static class TextureUtilities
    {
        private static Texture2D vignetteTexture;
        public static Texture2D VignetteTexture
        {
            get
            {
                if (vignetteTexture == null)
                {
                    vignetteTexture = new Texture2D(256, 256);
                    Vector2 center = new Vector2((vignetteTexture.width - 1) / 2, (vignetteTexture.height - 1) / 2);
                    float halfDiag = Mathf.Sqrt(vignetteTexture.width * vignetteTexture.width + vignetteTexture.height * vignetteTexture.height) / 2;
                    Color[] colors = new Color[vignetteTexture.width * vignetteTexture.height];
                    for (int x = 0; x < vignetteTexture.width; ++x)
                    {
                        for (int y = 0; y < vignetteTexture.height; ++y)
                        {
                            Vector2 p = new Vector2(x, y);
                            float d = Vector2.Distance(p, center);
                            float multiplier = Mathf.Clamp01(d / halfDiag);
                            Color c = Color.black * multiplier * multiplier * multiplier;
                            colors[y * vignetteTexture.width + x] = c;
                        }
                    }
                    vignetteTexture.SetPixels(colors);
                    vignetteTexture.Apply();
                }

                return vignetteTexture;
            }
        }
    }
}
