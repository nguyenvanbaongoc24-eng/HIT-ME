using UnityEngine;
using HitMe.UI;
namespace HitMe.Visuals {
// Reuses isolated layer renderer and global quality/reduced-motion policy. Never moves gameplay roots.
public sealed class MapMotionController:MonoBehaviour {
 EnvironmentLayerView[] layers;EnvironmentMotionProfile profile;AmbientVFXController ambient;bool focused;ShadowMotionController shadow;
 public int LayerCount=>layers?.Length??0;
 public void Initialize(ArenaArtDefinition map){profile=map?.motionProfile;shadow=GetComponentInParent<NorthernVillageArenaView>()?.GetComponentInChildren<ShadowMotionController>();var back=EnvironmentLayerView.Create(transform,map?.backgroundLayers);var front=EnvironmentLayerView.Create(transform,map?.foregroundLayers);back.name="MapBackgroundLayers";back.gameObject.AddComponent<ParallaxController>().Configure(profile!=null?profile.parallaxPixels:0);front.name="MapForegroundLayers";layers=GetComponentsInChildren<EnvironmentLayerView>();var fx=EnvironmentLayerView.Create(transform,null);fx.name="MapAmbientSpritePool";ambient=fx.gameObject.AddComponent<AmbientVFXController>();ambient.Initialize(map?.ambientEffectProfile);}
 public void SetFocused(bool value){focused=value;ambient?.SetFocused(value);if(shadow!=null)shadow.Focused=value;}
 public void ConfirmedCheer(){if(layers!=null)foreach(var layer in layers)layer.Celebrate();}
 void LateUpdate(){if(layers==null)return;int limit=profile==null?8:MotionSettings.Quality==MotionQuality.Low?profile.lowLayers:MotionSettings.Quality==MotionQuality.Medium?profile.mediumLayers:profile.highLayers;for(int i=0;i<layers.Length;i++){bool visible=i<limit;layers[i].gameObject.SetActive(visible);layers[i].enabled=visible&&!MotionSettings.Reduced&&!(focused&&profile!=null&&profile.pauseDuringAimThrow);}}
 void OnDisable(){if(layers!=null)foreach(var layer in layers)if(layer!=null)layer.enabled=false;}
 void OnApplicationPause(bool pause){enabled=!pause;}
}
}
