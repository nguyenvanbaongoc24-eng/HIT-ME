using NUnit.Framework;
using HitMe.Characters;
using HitMe.Visuals;
using UnityEngine;
using UnityEngine.UI;
namespace HitMe.Tests {
public sealed class MasterProductionTests {
 [Test] public void SevenStaticActorsAndNinePropsHaveRealSpritesWithoutExpandingGameplayCatalog(){
  var pack=ProductionVisualPack.Load();Assert.IsNotNull(pack);Assert.AreEqual(7,pack.roster.Length);Assert.AreEqual(9,pack.weapons.Length);Assert.AreEqual(3,CharacterCatalog.Load().characters.Length);
  foreach(var d in pack.roster){Assert.IsNotNull(d.portrait);Assert.IsNotNull(d.Frame(VisualState.Idle,0));Assert.AreEqual(1,d.Animation(VisualState.Idle).frames.Length);Assert.IsNull(d.Animation(VisualState.Throw));Assert.AreEqual(74,d.referenceHeight);Assert.That(d.footPivot.x,Is.InRange(0,1));}
  foreach(var d in pack.weapons){Assert.IsNotNull(d.icon);Assert.IsNotNull(d.FlightSprite);Assert.That(d.visualSize,Is.InRange(1,14));Assert.That(d.weaponId,Does.StartWith("Visual_"));}
  Assert.IsNotNull(pack.centerLotus);foreach(var layer in pack.menuLayers){Assert.IsNotNull(layer.sprite);Assert.That(layer.anchor.x,Is.InRange(0,1));}
  Assert.AreSame(UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/HitMe/Art/Weapons/DepToOng/held.png"),pack.weapons[0].icon);
  Assert.AreSame(UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/HitMe/Art/Weapons/Chao/held.png"),pack.weapons[5].icon);
  Assert.AreEqual(LayerMotion.Static,pack.menuLayers[3].motion);
  foreach(var d in pack.weapons){Assert.That(d.trailWidth,Is.GreaterThan(0));Assert.That(d.trailColor.a,Is.InRange(0,1));}
 }
 [Test] public void EveryNewActorStateLeavesItsLogicalFootParentFixed(){
  var pack=ProductionVisualPack.Load();foreach(var d in pack.roster){var foot=new GameObject("LogicalFoot",typeof(RectTransform));var parent=(RectTransform)foot.transform;parent.anchoredPosition=new Vector2(101,-57);var body=new GameObject("VisualBody",typeof(RectTransform),typeof(Image),typeof(CharacterVisual));body.transform.SetParent(parent,false);var visual=body.GetComponent<CharacterVisual>();visual.Initialize(d);
   foreach(VisualState state in System.Enum.GetValues(typeof(VisualState))){visual.SetFacing(Vector2.left);visual.Present(state,Color.white,.25f);visual.Fit(100,100);Assert.AreEqual(new Vector2(101,-57),parent.anchoredPosition);Assert.AreEqual(FacingDirection.Left,visual.Facing);Assert.IsTrue(visual.HasSprite);Assert.AreEqual(d.footPivot,((RectTransform)body.transform).pivot);}
   Object.DestroyImmediate(foot);
  }
 }
}
}
