using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.PolarisBasic
{
    public partial class GeometryPainter
    {
        public enum BrushMode
        {
            Cylindrical, Spherical
        }

        /// <summary>
        /// Brush settings, group everything into a class to deal with serialization
        /// </summary>
        [System.Serializable]
        public class ToolsSettings
        {
            public BrushMode brushMode;
            public float brushRadius;
            public float strength;

            /// <summary>
            /// Last saved dimensions of the terrain surface
            /// </summary>
            [SerializeField]
            public Index2D surfaceDimensions;
            /// <summary>
            /// Elevation values
            /// </summary>
            [SerializeField]
            public float[] elevations;

            public ToolsSettings()
            {
                brushRadius = 20;
                strength = 2;
                Validate();
            }

            public void Validate()
            {
                brushRadius = Mathf.Max(0, brushRadius);
                strength = Mathf.Max(0, strength);
            }

            /// <summary>
            /// Resize the grid while maintain old elevations data
            /// </summary>
            /// <param name="oldDimensions"></param>
            /// <param name="newDimensions"></param>
            internal void ResizeGrid(Index2D oldDimensions, Index2D newDimensions)
            {
                if (elevations == null || elevations.Length == 0)
                {
                    elevations = new float[newDimensions.X * newDimensions.Z];
                    Utilities.Fill(elevations, 0);
                }
                else
                {
                    float[] tmp = new float[newDimensions.X * newDimensions.Z];
                    Utilities.Fill(tmp, 0);
                    int xLimit = Mathf.Min(oldDimensions.X, newDimensions.X);
                    int zLimit = Mathf.Min(oldDimensions.Z, newDimensions.Z);
                    float value;

                    for (int z = 0; z < zLimit; ++z)
                    {
                        for (int x = 0; x < xLimit; ++x)
                        {
                            value = elevations[Utilities.To1DIndex(x, z, oldDimensions.X)];
                            tmp[Utilities.To1DIndex(x, z, newDimensions.X)] = value;
                        }
                    }

                    elevations = tmp;
                }
            }

            /// <summary>
            /// Update grid dimension
            /// </summary>
            /// <param name="newSurfaceDimensions"></param>
            internal void UpdateDimension(Index2D newSurfaceDimensions)
            {
                if (surfaceDimensions != newSurfaceDimensions ||
                    elevations == null ||
                    elevations.Length != newSurfaceDimensions.X * newSurfaceDimensions.Z)
                {
                    ResizeGrid(surfaceDimensions, newSurfaceDimensions);
                    surfaceDimensions = newSurfaceDimensions;
                }
            }

            /// <summary>
            /// Get elevation value for a vertex
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public float GetElevation(Index2D i)
            {
                if (elevations == null)
                    return 0;
                int index = Utilities.To1DIndex(i.X, i.Z, surfaceDimensions.X);
                if (index >= elevations.Length)
                    return 0;
                else
                    return elevations[index];
            }

            /// <summary>
            /// Add elevation value for a vertex
            /// </summary>
            /// <param name="surfaceIndex"></param>
            /// <param name="value"></param>
            public void AddElevation(Index2D surfaceIndex, float value)
            {
                if (elevations == null ||
                    elevations.Length != surfaceDimensions.X * surfaceDimensions.Z)
                {
                    elevations = new float[surfaceDimensions.X * surfaceDimensions.Z];
                }

                int index = Utilities.To1DIndex(surfaceIndex.X, surfaceIndex.Z, surfaceDimensions.X);

                if (index < elevations.Length)
                {
                    elevations[index] += value;
                }
            }

            /// <summary>
            /// Reset elevations data
            /// </summary>
            public void ResetElevations()
            {
                if (elevations == null || elevations.Length != surfaceDimensions.X * surfaceDimensions.Z)
                {
                    elevations = new float[surfaceDimensions.X * surfaceDimensions.Z];
                }

                Utilities.Fill(elevations, 0);
            }

            public void EnsureDimensionsUpToDate(Index2D dimensions)
            {
                if (surfaceDimensions != dimensions)
                    UpdateDimension(dimensions);
            }
        }
    }
}
