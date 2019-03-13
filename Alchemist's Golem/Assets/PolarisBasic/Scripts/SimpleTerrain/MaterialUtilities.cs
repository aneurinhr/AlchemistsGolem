using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.PolarisBasic
{
    /// <summary>
    /// Utility class for creating material
    /// </summary>
    public static class MaterialUtilities
    {
        private static Material cyanSemiTransparentDiffuse;
        public static Material CyanSemiTransparentDiffuse
        {
            get
            {
                if (cyanSemiTransparentDiffuse == null)
                {
                    cyanSemiTransparentDiffuse = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
                    cyanSemiTransparentDiffuse.SetColor("_Color", ColorLibrary.GetColor(Color.cyan, 0.3f));
                }
                return cyanSemiTransparentDiffuse;
            }
        }

        private static Material intersectionHighlight;
        public static Material IntersectionHighlight
        {
            get
            {
                if (intersectionHighlight == null)
                {
                    intersectionHighlight = new Material(Shader.Find("Pinwheel/Intersection Highlight"));
                    intersectionHighlight.SetFloat("_Threshold", 2);
                    intersectionHighlight.SetFloat("_AnimationSpeed", 10);
                }
                return intersectionHighlight;
            }
        }
    }
}
