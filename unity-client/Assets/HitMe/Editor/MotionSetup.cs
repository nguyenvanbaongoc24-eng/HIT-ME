using UnityEditor;
using UnityEngine;
using HitMe.Visuals;
namespace HitMe.Editor {
public static class MotionSetup {
 [MenuItem("HIT ME/Configure Motion Profiles")]
 public static void Configure(){const string folder="Assets/HitMe/Resources/Motion";System.IO.Directory.CreateDirectory(folder);
  var character=AssetDatabase.LoadAssetAtPath<CharacterMotionProfile>(folder+"/Character.asset");if(character==null){character=ScriptableObject.CreateInstance<CharacterMotionProfile>();AssetDatabase.CreateAsset(character,folder+"/Character.asset");}
  for(int i=0;i<3;i++){string path=folder+"/"+(MotionQuality)i+".asset";var p=AssetDatabase.LoadAssetAtPath<MotionQualityProfile>(path);if(p==null){p=ScriptableObject.CreateInstance<MotionQualityProfile>();AssetDatabase.CreateAsset(p,path);}p.strength=i==0?.4f:i==1?.7f:1;p.ambientCount=i==0?3:i==1?6:10;p.trailCount=i==0?2:i==1?4:6;EditorUtility.SetDirty(p);}AssetDatabase.SaveAssets();AssetDatabase.Refresh();
 }
}
}
