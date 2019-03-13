using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.PolarisBasic
{
    public static class EditorMenus
    {
#if !POLARIS
        [MenuItem("Polaris/Mesh Saver")]
        public static void ShowMeshSaver()
        {
            MeshSaverGUI.ShowWindow();
        }

        [MenuItem("Polaris/Texture Exporter")]
        public static void ShowTextureExporter()
        {
            TextureExporterGUI.ShowWindow();
        }

        [MenuItem("Polaris/User Guide", priority = 100000)]
        public static void ShowUserGuide()
        {
            Application.OpenURL("https://docs.pinwheel.studio/polaris/");
        }

        [MenuItem("Polaris/Forum Thread", priority = 100001)]
        public static void ShowForumThread()
        {
            Application.OpenURL("https://forum.unity.com/threads/released-polaris-hybrid-procedural-low-poly-terrain-engine.541792/#post-3572618");
        }

        [MenuItem("Polaris/Homepage", priority = 100002)]
        public static void ShowHomepage()
        {
            Application.OpenURL("http://pinwheel.studio/");
        }

        [MenuItem("Polaris/Like Us", priority = 100003)]
        public static void ShowFacebookPage()
        {
            Application.OpenURL("https://www.facebook.com/polaris.terrain");
        }

        [MenuItem("Polaris/Community", priority = 100004)]
        public static void ShowFacebookGroup()
        {
            Application.OpenURL("https://www.facebook.com/groups/335360883883168/");
        }

        [MenuItem("Polaris/Support", priority = 1000005)]
        public static void SendSupportRequest()
        {
            string url = "mailto:support@pinwheel.studio" +
                "?subject=[Polaris Basic]%20SHORT%20QUESTION" +
                "&body=YOUR%20QUESTION%20IN%20DETAILED";
            Application.OpenURL(url);
        }

        [MenuItem("Polaris/Version Info", priority = 1000009)]
        public static void ShowVersionInfo()
        {
            UnityEditor.EditorUtility.DisplayDialog(
                "Version Info",
                VersionInfo.ProductNameAndVersion,
                "OK");
        }

        [MenuItem("Polaris/Rate and Review", priority = 1000010)]
        public static void ShowAssetStorePage()
        {
            Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/118854");
        }
#endif
    }
}
