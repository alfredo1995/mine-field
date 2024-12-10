using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace OpaGames.Editor.Build
{
    public static class TwiceBuildAction
    {
        private const string DualBuildPath = "WebGLBuilds";
        private static string[] _mobileScenes;
        private static string[] _desktopScenes;
        
        public static void BuildGame()
        {
            if(Application.HasProLicense())
                PlayerSettings.SplashScreen.showUnityLogo = false;
            
            BuildForMobile();
            BuildForDesktop();

            ConsolidateMobileDataToDesktop();
            CleanOldData();
        }

        public static void LoadScenesPath(string[] desktopPath, string[] mobilePath, Action<bool> callback)
        {
            _desktopScenes = desktopPath;
            _mobileScenes = mobilePath;
            callback?.Invoke(_desktopScenes != null && _mobileScenes != null);
        }
        private static void BuildForDesktop()
        {
            string desktopPath = Path.Combine(DualBuildPath, "WebGL_Build");
            EnsureDirectoryExists(desktopPath);
            
            EditorUserBuildSettings.webGLBuildSubtarget = WebGLTextureSubtarget.DXT;
            BuildPipeline.BuildPlayer(_desktopScenes, desktopPath, BuildTarget.WebGL, BuildOptions.CleanBuildCache);
        }

        private static void BuildForMobile()
        {
            string mobilePath = Path.Combine(DualBuildPath, "WebGL_Mobile");
            EnsureDirectoryExists(mobilePath);

            EditorUserBuildSettings.webGLBuildSubtarget = WebGLTextureSubtarget.ASTC;
            BuildPipeline.BuildPlayer(_mobileScenes, mobilePath, BuildTarget.WebGL, BuildOptions.CleanBuildCache);
        }

        private static void ConsolidateMobileDataToDesktop()
        {
            string[] possibleFileExtensions = { ".data", ".data.unityweb" };

            foreach (string extension in possibleFileExtensions)
            {
                string sourceDataFile = Path.Combine(DualBuildPath, "WebGL_Mobile", "Build", "WebGL_Mobile" + extension);
                string destDataFile = Path.Combine(DualBuildPath, "WebGL_Build", "Build", "WebGL_Mobile" + extension);

                if (TryCopyFile(sourceDataFile, destDataFile))
                {
                    return;
                }
            }

            Debug.LogWarning("Mobile data file not found. Skipping consolidation.");
        }
        
        private static bool TryCopyFile(string source, string destination)
        {
            if (File.Exists(source))
            {
                FileUtil.CopyFileOrDirectory(source, destination);
                return true;
            }

            return false;
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
        }

        private static void CleanOldData()
        {
            string mobilePath = Path.Combine(DualBuildPath, "WebGL_Mobile");
            
            if (Directory.Exists(mobilePath))
            {
                Directory.Delete(mobilePath, true);
            }
        }
    }
}
