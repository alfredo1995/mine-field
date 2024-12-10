using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OpaGames.Editor.Build
{
    public class TwiceBuildActionEditor : EditorWindow
    {
        [MenuItem("Game Build Menu/Dual Build")]
        public static void ShowWindow()
        {
            GetWindow<TwiceBuildActionEditor>("Dual Build");
        }

        private ReorderableList _sceneListMobile;
        private ReorderableList _sceneListDesktop;
        
        private readonly List<SceneAsset> _sceneAssetsMobile = new();
        private readonly List<SceneAsset> _sceneAssetsDesktop = new();

        private void OnEnable()
        {
            CreateListMobile();
            CreateListDesktop();
        }

        private void CreateListMobile()
        {
            _sceneListMobile = new ReorderableList(
                _sceneAssetsMobile,
                typeof(SceneAsset),
                true, true, true, true
            );

            _sceneListMobile.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Scene List To Mobile");
        
            _sceneListMobile.drawElementCallback = (rect, index, _, _) =>
            {
                EditorGUI.BeginChangeCheck();
                var scene = (SceneAsset) EditorGUI.ObjectField(rect, _sceneAssetsMobile[index], typeof(SceneAsset), false);
                if (EditorGUI.EndChangeCheck())
                {
                    _sceneAssetsMobile[index] = scene;
                }
            };

            _sceneListMobile.onAddCallback = _ =>
            {
                _sceneAssetsMobile.Add(null);
            };
        }

        private void CreateListDesktop()
        {
            _sceneListDesktop = new ReorderableList(
                _sceneAssetsDesktop,
                typeof(SceneAsset),
                true, true, true, true
            )
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Scene List To Desktop"),
                drawElementCallback = (rect, index, _, _) =>
                {
                    EditorGUI.BeginChangeCheck();
                    var scene = (SceneAsset) EditorGUI.ObjectField(rect, _sceneAssetsDesktop[index], typeof(SceneAsset), false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _sceneAssetsDesktop[index] = scene;
                    }
                },
                onAddCallback = _ =>
                {
                    _sceneAssetsDesktop.Add(null);
                }
            };
        }

        private void OnGUI()
        {
            _sceneListMobile.DoLayoutList();
            _sceneListDesktop.DoLayoutList();
            var checkMobileIsNotNull = _sceneAssetsMobile != null && _sceneAssetsMobile.Count > 0 && _sceneAssetsMobile.TrueForAll(scene => scene != null);
            var checkDesktopIsNotNull = _sceneAssetsDesktop != null && _sceneAssetsDesktop.Count > 0 && _sceneAssetsDesktop.TrueForAll(scene => scene != null);
            if (checkMobileIsNotNull && checkDesktopIsNotNull)
            {
                if (GUILayout.Button("Start Dual Build"))
                {
                    foreach (SceneAsset sceneAsset in _sceneAssetsMobile)
                    {
                        var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                        EditorGUILayout.LabelField(sceneAsset.name, scenePath);
                    }
                    var mobilePath = _sceneAssetsMobile.Select(scene => AssetDatabase.GetAssetPath(scene)).ToArray();
                    var desktopPath = _sceneAssetsDesktop.Select(scene => AssetDatabase.GetAssetPath(scene)).ToArray();
                    TwiceBuildAction.LoadScenesPath(mobilePath, desktopPath, callback =>
                    {
                        if (callback)
                            TwiceBuildAction.BuildGame();
                        else
                            Debug.LogError("Scene list is null or contains null elements.");
                    });
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No scenes added to list or list contains null elements.", MessageType.Warning);
                if (GUILayout.Button("Mostrar pasta de cenas"))
                {
                    string[] allScenePaths = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
                    string firstScenePath = AssetDatabase.GUIDToAssetPath(allScenePaths[0]);

                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(firstScenePath);
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }
            }
        }
    }
}