using UnityEngine;
using UnityEngine.UI;
using HitMe.UI;
namespace HitMe.Visuals {
// Presentation containers share the Battle canvas coordinate frame; never transform logical positions.
public sealed class NorthernVillageArenaView : MonoBehaviour {
 public RectTransform farBackground,midBackground,arenaFloor,arenaBorder,lightingOverlay,gameplayVisualRoot,foregroundProps,ambientEffects;
 public void Bind(Transform battleRoot,ArenaArtDefinition map){
  Adopt(battleRoot,"StandsPlaceholder",midBackground);
  Adopt(battleRoot,"WallShadow",arenaBorder);Adopt(battleRoot,"WallPlaceholder",arenaBorder);Adopt(battleRoot,"WallInnerPlaceholder",arenaBorder);
  Adopt(battleRoot,"ArenaSandPlaceholder",arenaFloor);Adopt(battleRoot,"ActorLayer",gameplayVisualRoot);
  // Border must remain behind floor. No generated replacement artwork is claimed.
  arenaBorder.SetSiblingIndex(2);
  if(map.shadowOverlay!=null){var image=new GameObject("TreeShadowOverlay",typeof(RectTransform),typeof(Image));image.transform.SetParent(lightingOverlay,false);var r=(RectTransform)image.transform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;var ui=image.GetComponent<Image>();ui.sprite=map.shadowOverlay;ui.color=new Color(1,1,1,.08f);ui.raycastTarget=false;ui.preserveAspect=true;image.AddComponent<ShadowMotionController>();}
 }
 static void Adopt(Transform root,string name,Transform destination){var child=root.Find(name);if(child!=null)child.SetParent(destination,false);}
}
}
