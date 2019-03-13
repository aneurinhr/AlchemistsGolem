using UnityEngine;
using System.Collections;
using System;

namespace Pinwheel.PolarisBasic
{
    /// <summary>
    /// Wrapper class contains mesh data
    /// </summary>
    public class MeshData
    {
        public Vector3[] vertices;
        public Color[] vertexColors;
        public Vector2[] uvCoords;
        public int[] triangles;
        public Vector3[] normals;

        public void InitVertexCount(int count)
        {
            vertices = new Vector3[count];
            vertexColors = new Color[count];
            uvCoords = new Vector2[count];
            triangles = new int[count];
            normals = new Vector3[count];
        }

        public void Clear()
        {
            vertices = null;
            vertexColors = null;
            uvCoords = null;
            triangles = null;
            normals = null;
        }

        public void CalculateNormals()
        {
            if (!HasBasicData())
                throw new Exception("Invalid vertices or triangles data");
            if (!HasValidIndices())
                throw new Exception("Invalid triangles indices");

            normals = new Vector3[vertices.Length];
            int trisCount = triangles.Length / 3;
            Vector3[] trisNormals = new Vector3[trisCount];
            int[] divider = new int[vertices.Length];

            for (int i = 0; i < trisCount; ++i)
            {
                int i0 = triangles[i * 3 + 0];
                int i1 = triangles[i * 3 + 1];
                int i2 = triangles[i * 3 + 2];

                divider[i0] += 1;
                divider[i1] += 1;
                divider[i2] += 1;

                Vector3 v0 = vertices[i0];
                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];

                Vector3 n = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                trisNormals[i] = n;
            }

            for (int i = 0; i < trisCount; ++i)
            {
                int i0 = triangles[i * 3 + 0];
                int i1 = triangles[i * 3 + 1];
                int i2 = triangles[i * 3 + 2];

                normals[i0] += trisNormals[i] / divider[i0];
                normals[i1] += trisNormals[i] / divider[i1];
                normals[i2] += trisNormals[i] / divider[i2];
            }

            for (int i = 0; i < normals.Length; ++i)
            {
                normals[i] = normals[i].normalized;
            }
        }

        public bool HasBasicData()
        {
            return vertices != null && triangles != null;
        }

        public bool HasValidIndices()
        {
            if (!HasBasicData())
                return false;
            if (triangles.Length % 3 != 0)
                return false;
            bool valid = true;
            for (int i = 0; i < triangles.Length; ++i)
            {
                if (triangles[i] >= vertices.Length)
                {
                    valid = false;
                    break;
                }
            }
            return valid;
        }
    }
}