using UnityEditor;
using UnityEngine;
using HitMe.UI;
using HitMe.Visuals;
namespace HitMe.Editor {
public static class MapKitSetup {
 [MenuItem("HIT ME/Configure Vietnam Map Kit")] public static void ConfigureAll(){MasterProductionSetup.Configure();Configure();}
 public static void Build(){ConfigureAll();HitMeWebBuild.BuildMaps();}
 public static void Configure(){var pack=ProductionVisualPack.Load();if(pack==null)return;var profile=AssetDatabase.LoadAssetAtPath<EnvironmentMotionProfile>("Assets/HitMe/Resources/Motion/MapEnvironment.asset");if(profile==null){profile=ScriptableObject.CreateInstance<EnvironmentMotionProfile>();AssetDatabase.CreateAsset(profile,"Assets/HitMe/Resources/Motion/MapEnvironment.asset");}
 string[] ids={"LangQueBacBo","PhoCoHoiAn","VinhHaLong"},vi={"Sân Đình Bắc Bộ","Phố Cổ Hội An","Vịnh Hạ Long"},en={"Northern Village Courtyard","Hoi An Ancient Town","Ha Long Bay"};
 for(int i=0;i<3;i++){string path="Assets/HitMe/Resources/Arenas/"+ArenaMaps.ResourceId(ids[i])+".asset";var map=AssetDatabase.LoadAssetAtPath<ArenaArtDefinition>(path);if(map==null){map=ScriptableObject.CreateInstance<ArenaArtDefinition>();AssetDatabase.CreateAsset(map,path);}map.mapId=ids[i];map.displayNameVi=vi[i];map.displayNameEn=en[i];if(map.sand==null)map.sand=ProductionArtSetup.Sprite("Environments/stone-tile.png");if(map.centerDecoration==null)map.centerDecoration=pack.centerLotus;if(map.motionProfile==null)map.motionProfile=profile;map.unlockState=map.ArtworkReady?MapUnlockState.Available:MapUnlockState.ArtworkMissing;if(map.previewSprite==null)map.previewSprite=map.backdrop;if(map.backgroundLayers==null||map.backgroundLayers.Length==0)map.backgroundLayers=i==0?pack.battleLayers:new EnvironmentLayerDefinition[0];if(map.foregroundLayers==null)map.foregroundLayers=new EnvironmentLayerDefinition[0];EditorUtility.SetDirty(map);}AssetDatabase.SaveAssets();}
}
}
