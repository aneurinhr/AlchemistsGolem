using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.PolarisBasic
{
    public partial class ColorPainter
    {
        [System.Serializable]
        public class BrushSettings
        {
            public string name;
            public float brushRadius;
            public float strength;
            public Color color;

            public static void Copy(BrushSettings src, BrushSettings des)
            {
                des.name = src.name;
                des.brushRadius = src.brushRadius;
                des.strength = src.strength;
                des.color = src.color;
            }

            public override string ToString()
            {
                string s = string.Format(
                    "Name: {0}\n" +
                    "Radius: {1}\n" +
                    "Strength: {2}\n" +
                    "Color: {3}",
                    name, 
                    brushRadius.ToString("0.000"), 
                    strength.ToString("0.000"), 
                    color.ToString());

                return s;
            }
        }

        /// <summary>
        /// Brush settings, group everything into a class to deal with serialization
        /// </summary>
        [System.Serializable]
        public class ToolsSettings
        {
            public BrushSettings brushSettings;

            /// <summary>
            /// Last saved dimensions of the terrain surface
            /// </summary>
            [SerializeField]
            public Index2D surfaceDimensions;
            /// <summary>
            /// Color values
            /// </summary>
            [SerializeField]
            public Color[] colors;

            /// <summary>
            /// Set of brush settings
            /// </summary>
            [SerializeField]
            private List<BrushSettings> palette;
            public List<BrushSettings> Palette
            {
                get
                {
                    if (palette == null)
                        palette = new List<BrushSettings>();
                    return palette;
                }
                private set
                {
                    palette = value;
                }
            }

            public ToolsSettings()
            {
                brushSettings = new BrushSettings();
                brushSettings.brushRadius = 20;
                brushSettings.strength = 1;
                brushSettings.color = Color.white;
                Validate();
            }

            public void Validate()
            {
                brushSettings.brushRadius = Mathf.Max(0, brushSettings.brushRadius);
                brushSettings.strength = Mathf.Clamp01(brushSettings.strength);
            }

            /// <summary>
            /// Resize the grid while maintain old Colors data
            /// </summary>
            /// <param name="oldDimensions"></param>
            /// <param name="newDimensions"></param>
            internal void ResizeGrid(Index2D oldDimensions, Index2D newDimensions)
            {
                if (colors == null || colors.Length == 0)
                {
                    colors = new Color[newDimensions.X * newDimensions.Z];
                    Utilities.Fill(colors, Color.clear);
                }
                else
                {
                    Color[] tmp = new Color[newDimensions.X * newDimensions.Z];
                    Utilities.Fill(tmp, Color.clear);
                    int xLimit = Mathf.Min(oldDimensions.X, newDimensions.X);
                    int zLimit = Mathf.Min(oldDimensions.Z, newDimensions.Z);
                    Color value;

                    for (int z = 0; z < zLimit; ++z)
                    {
                        for (int x = 0; x < xLimit; ++x)
                        {
                            value = colors[Utilities.To1DIndex(x, z, oldDimensions.X)];
                            tmp[Utilities.To1DIndex(x, z, newDimensions.X)] = value;
                        }
                    }

                    colors = tmp;
                }
            }

            /// <summary>
            /// Update grid dimension
            /// </summary>
            /// <param name="newSurfaceDimensions"></param>
            internal void UpdateDimension(Index2D newSurfaceDimensions)
            {
                if (surfaceDimensions != newSurfaceDimensions ||
                    colors == null ||
                    colors.Length != newSurfaceDimensions.X * newSurfaceDimensions.Z)
                {
                    ResizeGrid(surfaceDimensions, newSurfaceDimensions);
                    surfaceDimensions = newSurfaceDimensions;
                }
            }

            /// <summary>
            /// Get Color value for a vertex
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public Color GetColor(Index2D i)
            {
                if (colors == null)
                    return Color.clear;
                int index = Utilities.To1DIndex(i.X, i.Z, surfaceDimensions.X);
                if (index >= colors.Length)
                    return Color.clear;
                else
                    return colors[index];
            }

            /// <summary>
            /// Add Color value for a vertex
            /// </summary>
            /// <param name="surfaceIndex"></param>
            /// <param name="value"></param>
            public void AddColor(Index2D surfaceIndex, Color value, float f)
            {
                if (colors == null ||
                    colors.Length != surfaceDimensions.X * surfaceDimensions.Z)
                {
                    colors = new Color[surfaceDimensions.X * surfaceDimensions.Z];
                }

                int index = Utilities.To1DIndex(surfaceIndex.X, surfaceIndex.Z, surfaceDimensions.X);

                if (index < colors.Length)
                {
                    //colors[index] = ColorLibrary.Blend(colors[index], 1 - Mathf.Clamp01(f), value, Mathf.Clamp01(f));
                    colors[index] = Color.Lerp(colors[index], value, f);
                }
            }

            /// <summary>
            /// Add Color value for a vertex
            /// </summary>
            /// <param name="surfaceIndex"></param>
            /// <param name="value"></param>
            public void EraseColor(Index2D surfaceIndex, float f)
            {
                if (colors == null ||
                    colors.Length != surfaceDimensions.X * surfaceDimensions.Z)
                {
                    colors = new Color[surfaceDimensions.X * surfaceDimensions.Z];
                }

                int index = Utilities.To1DIndex(surfaceIndex.X, surfaceIndex.Z, surfaceDimensions.X);

                if (index < colors.Length)
                {
                    colors[index] = Color.Lerp(colors[index], Color.clear, f);
                }
            }

            /// <summary>
            /// Reset Colors data
            /// </summary>
            public void ResetColors()
            {
                if (colors == null || colors.Length != surfaceDimensions.X * surfaceDimensions.Z)
                {
                    colors = new Color[surfaceDimensions.X * surfaceDimensions.Z];
                }

                Utilities.Fill(colors, Color.clear);
            }

            public void EnsureDimensionsUpToDate(Index2D dimensions)
            {
                if (surfaceDimensions != dimensions)
                    UpdateDimension(dimensions);
            }
        }
    }
}
