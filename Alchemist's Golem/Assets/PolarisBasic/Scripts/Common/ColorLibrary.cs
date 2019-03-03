using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.PolarisBasic
{
    /// <summary>
    /// Utility class to get color value
    /// </summary>
    public static class ColorLibrary
    {
        public static Color GetColor(Color baseColor, float alpha)
        {
            return new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }

        public static Color Cyan
        {
            get
            {
                return Color.cyan;
            }
        }

        public static Color CyanSemiTransparent
        {
            get
            {
                Color c = Color.cyan;
                return new Color(c.r, c.g, c.b, 0.7f);
            }
        }

        public static Color Red
        {
            get
            {
                return Color.red;
            }
        }

        public static Color RedSemiTransparent
        {
            get
            {
                Color c = Color.red;
                return new Color(c.r, c.g, c.b, 0.7f);
            }
        }

        public static Color Yellow
        {
            get
            {
                return Color.yellow;
            }
        }

        public static Color YellowSemiTransparent
        {
            get
            {
                Color c = Color.yellow;
                return new Color(c.r, c.g, c.b, 0.7f);
            }
        }

        public static Color UnityLightSkinColor
        {
            get
            {
                return new Color32(194, 194, 194, 255);
            }
        }

        public static Color UnityDarkSkinColor
        {
            get
            {
                return new Color32(45, 45, 45, 255);
            }
        }

        public static Color Blend(Color srcColor, float srcFactor, Color desColor, float desFactor)
        {
            return srcColor * srcFactor + desColor * desFactor;
        }

        public static Color BlendNormal(Color srcColor, Color desColor)
        {
            Color c = Color.Lerp(desColor, srcColor, srcColor.a);
            c.a = 1 - (1 - desColor.a) * (1 - srcColor.a);
            return c;
        }
    }
}
