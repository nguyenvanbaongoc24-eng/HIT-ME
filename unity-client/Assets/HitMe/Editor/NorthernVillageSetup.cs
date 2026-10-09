using System.IO;
using UnityEditor;
using UnityEngine;
using HitMe.UI;
using HitMe.Visuals;
namespace HitMe.Editor {
public static class NorthernVillageSetup {
 [MenuItem("HIT ME/Configure Northern Village Slice")]
 public static void Configure(){if(AssetDatabase.LoadAssetAtPath<ArenaArtDefinition>("Assets/HitMe/Resources/Arenas/LangQueBacBo.asset")!=null){var error=AssetDatabase.MoveAsset("Assets/HitMe/Resources/Arenas/LangQueBacBo.asset","Assets/HitMe/Resources/Arenas/NorthernVillageVisualDefinition.asset");if(!string.IsNullOrEmpty(error))throw new System.Exception(error);}MapKitSetup.ConfigureAll();
  const string folder="Assets/HitMe/Prefabs/Environments";Directory.CreateDirectory(folder);AssetDatabase.Refresh();
  var root=new GameObject("NorthernVillageArena",typeof(RectTransform),typeof(NorthernVillageArenaView));var rect=(RectTransform)root.transform;Stretch(rect);var view=root.GetComponent<NorthernVillageArenaView>();
  view.farBackground=Layer(rect,"FarBackground");view.midBackground=Layer(rect,"MidBackground");view.arenaBorder=Layer(rect,"ArenaBorder");view.arenaFloor=Layer(rect,"ArenaFloor");view.lightingOverlay=Layer(rect,"LightingOverlay");view.gameplayVisualRoot=Layer(rect,"GameplayVisualRoot");view.foregroundProps=Layer(rect,"ForegroundProps");view.ambientEffects=Layer(rect,"AmbientEffects");
  var prefab=PrefabUtility.SaveAsPrefabAsset(root,folder+"/NorthernVillageArena.prefab");Object.DestroyImmediate(root);
  const string motionPath="Assets/HitMe/Resources/Motion/NorthernVillageMotionProfile.asset";var motion=AssetDatabase.LoadAssetAtPath<EnvironmentMotionProfile>(motionPath);if(motion==null){motion=ScriptableObject.CreateInstance<EnvironmentMotionProfile>();AssetDatabase.CreateAsset(motion,motionPath);}motion.pauseDuringAimThrow=true;motion.lowLayers=1;motion.mediumLayers=2;motion.highLayers=8;motion.parallaxPixels=0;EditorUtility.SetDirty(motion);
  var map=ArenaMaps.Load(0);map.shadowOverlay=ProductionArtSetup.Sprite("NorthernVillage/tree-shadow.png");map.environmentPrefab=prefab;map.motionProfile=motion;EditorUtility.SetDirty(map);AssetDatabase.SaveAssets();
 }
 public static void Build(){Configure();HitMeWebBuild.BuildNorthernVillage();}
 static RectTransform Layer(Transform parent,string name){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);Stretch(r);return r;}
 static void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;r.localScale=Vector3.one;}
}
}
