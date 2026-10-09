using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace HitMe.Editor {
public static class HitMeWebBuild {
 [MenuItem("HIT ME/Build Sprint 3B Web")]
 public static void Build()=>BuildTo("SPRINT3B_WEB_BUILD.json");
 [MenuItem("HIT ME/Build Sprint 4 Web")]
 public static void BuildSprint4()=>BuildTo("SPRINT4_WEB_BUILD.json");
 [MenuItem("HIT ME/Build Sprint 5 Web")]
 public static void BuildSprint5(){MainMenuAssetSetup.Configure();BuildTo("SPRINT5_WEB_BUILD.json");}
 public static void BuildMasterPhase0()=>BuildTo("PHASE0_WEB_BUILD.json");
 public static void BuildMasterPhase1(){MainMenuAssetSetup.Configure();BuildTo("PHASE1_WEB_BUILD.json");}
 public static void BuildPhase2A(){UIKitSetup.Configure();BuildTo("PHASE2A_WEB_BUILD.json");}
 public static void BuildSprint6A(){MotionSetup.Configure();UIKitSetup.Configure();BuildTo("SPRINT6A_WEB_BUILD.json");}
 public static void BuildUIKit(){UIKitSetup.Configure();BuildTo("UIKIT_WEB_BUILD.json");}
  public static void BuildMasterProduction()=>BuildTo("MASTER_PRODUCTION_WEB_BUILD.json");
  public static void BuildMaps()=>BuildTo("MAP_WEB_BUILD.json");
  public static void BuildNorthernVillage()=>BuildTo("NORTHERN_VILLAGE_WEB_BUILD.json");
  public static void BuildAimVisual()=>BuildTo("AIM_WEB_BUILD.json");
 public static void BuildArtwork()=>BuildTo("ARTWORK_WEB_BUILD.json");
 static void BuildTo(string reportFile){
  if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL,BuildTarget.WebGL))throw new InvalidOperationException("Web Build Support missing.");
  string[] scenes=FoundationSetup.Scenes.Select(s=>"Assets/HitMe/Scenes/"+s+".unity").ToArray();
  foreach(string scene in scenes)if(!File.Exists(scene))throw new FileNotFoundException("Required scene missing",scene);
  TextMeshProSetup.Configure();ArenaArtSetup.Configure();AssetDatabase.SaveAssets();
  PlayerSettings.WebGL.template="PROJECT:HitMePortrait";PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Gzip;PlayerSettings.WebGL.decompressionFallback=true;
  PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;
  var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=scenes,locationPathName="Builds/Web",target=BuildTarget.WebGL,options=BuildOptions.None});
  string summary="{\"result\":\""+report.summary.result+"\",\"bytes\":"+report.summary.totalSize+",\"errors\":"+report.summary.totalErrors+",\"warnings\":"+report.summary.totalWarnings+",\"seconds\":"+report.summary.totalTime.TotalSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture)+"}";
  File.WriteAllText(Path.GetFullPath("../docs/"+reportFile),summary);Debug.Log("HITME_WEB_BUILD: "+summary);
  if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Web build failed.");
 }
}}

