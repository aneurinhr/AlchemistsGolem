using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.PolarisBasic
{
    /// <summary>
    /// Utility class contains product info
    /// </summary>
    public static class VersionInfo
    {
        public static string ProVersionLink
        {
            get
            {
                return "https://www.assetstore.unity3d.com/#!/content/123717?aid=1100l3QbW";
            }
        }

        public static string Code
        {
            get
            {
                return "1.1.0";
            }
        }

        public static string ProductName
        {
            get
            {
                return "Polaris Basic - Low Poly Terrain Engine";
            }
        }

        public static string ProductNameAndVersion
        {
            get
            {
                return string.Format("{0} v{1}", ProductName, Code);
            }
        }

        public static string ProductNameShort
        {
            get
            {
                return "Polaris Basic";
            }
        }

        public static string ProductNameAndVersionShort
        {
            get
            {
                return string.Format("{0} v{1}", ProductNameShort, Code);
            }
        }
    }
}
