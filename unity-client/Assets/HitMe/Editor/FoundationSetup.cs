using System.IO;
using HitMe.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HitMe.Editor
{
    public static class FoundationSetup
    {
        public static readonly string[] Scenes = { "Boot", "MainMenu", "Lobby", "CharacterSelect", "Battle", "Result" };
        [MenuItem("HIT ME/Configure Foundation")]
        public static void Configure()
        {
            Directory.CreateDirectory("Assets/HitMe/Settings"); Directory.CreateDirectory("Assets/HitMe/Scenes");
            PlayerSettings.companyName = "HitMe"; PlayerSettings.productName = "HIT ME";
            PlayerSettings.defaultScreenWidth = 390; PlayerSettings.defaultScreenHeight = 844;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false; PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.decompressionFallback = false;
            PlayerSettings.WebGL.template = "PROJECT:HitMePortrait";
            PlayerSettings.runInBackground = false;
            SerializedObject settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            var input = settings.FindProperty("activeInputHandler"); if (input != null) { input.intValue = 1; settings.ApplyModifiedPropertiesWithoutUndo(); }
            const string rendererPath = "Assets/HitMe/Settings/Renderer2D.asset";
            var renderer = AssetDatabase.LoadAssetAtPath<Renderer2DData>(rendererPath);
            if (renderer == null) { renderer = ScriptableObject.CreateInstance<Renderer2DData>(); AssetDatabase.CreateAsset(renderer, rendererPath); }
            const string pipelinePath = "Assets/HitMe/Settings/HitMe2DURP.asset";
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelinePath);
            if (pipeline == null) { pipeline = UniversalRenderPipelineAsset.Create(renderer); AssetDatabase.CreateAsset(pipeline, pipelinePath); }
            pipeline.msaaSampleCount = 1; pipeline.supportsHDR = false;
            GraphicsSettings.defaultRenderPipeline = pipeline;
            for (int i = 0; i < QualitySettings.names.Length; i++) { QualitySettings.SetQualityLevel(i); QualitySettings.renderPipeline = pipeline; }
            var buildScenes = new EditorBuildSettingsScene[Scenes.Length];
            for (int i = 0; i < Scenes.Length; i++)
            {
                string path = "Assets/HitMe/Scenes/" + Scenes[i] + ".unity";
                if (!File.Exists(path))
                {
                    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    new GameObject("HitMeScene", typeof(SceneEntry));
                    EditorSceneManager.SaveScene(scene, path);
                }
                buildScenes[i] = new EditorBuildSettingsScene(path, true);
            }
            EditorBuildSettings.scenes = buildScenes; AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene("Assets/HitMe/Scenes/Battle.unity");
            Debug.Log("HITME_FOUNDATION_CONFIGURED: Unity 2D URP, 6 scenes, portrait, Web template.");
        }
        [MenuItem("HIT ME/Open Battle")]
        public static void OpenBattle() { EditorSceneManager.OpenScene("Assets/HitMe/Scenes/Battle.unity"); }
        [MenuItem("HIT ME/Build Web")]
        public static void BuildWeb()
        {
            Configure();
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL)) throw new System.InvalidOperationException("Web Build Support missing.");
            string[] paths = new string[Scenes.Length]; for (int i = 0; i < paths.Length; i++) paths[i] = "Assets/HitMe/Scenes/" + Scenes[i] + ".unity";
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = paths, locationPathName = "Builds/Web", target = BuildTarget.WebGL, options = BuildOptions.None });
            Debug.Log("HITME_WEB_BUILD: " + report.summary.result + ", bytes=" + report.summary.totalSize);
            if (report.summary.result != BuildResult.Succeeded) throw new System.Exception("Web build failed. See build report.");
        }
    }
}
