using UnityEngine;
using UnityEditor;
using HitMe.UI;
namespace HitMe.Editor {
public static class AimVisualSetup {
 [MenuItem("HIT ME/Configure Production Aim")]
 public static void Configure(){const string dir="Assets/HitMe/Resources/UI/Kit";var root=new GameObject("ProductionAimIndicator",typeof(RectTransform),typeof(ProductionAimIndicator));var r=(RectTransform)root.transform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;root.GetComponent<ProductionAimIndicator>().raycastTarget=false;PrefabUtility.SaveAsPrefabAsset(root,dir+"/ProductionAimIndicator.prefab");Object.DestroyImmediate(root);AssetDatabase.SaveAssets();}
 public static void Build(){Configure();HitMeWebBuild.BuildAimVisual();}
}
}
