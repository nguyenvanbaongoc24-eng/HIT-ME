using HitMe.Editor;
using HitMe.UI;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
namespace HitMe.Tests {
public sealed class UIKitPrefabTests {
 [Test]public void CatalogHasEditableReferencesAndNoSceneBindings(){
  foreach(string name in UIKitSetup.WidgetNames){var asset=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HitMe/Resources/UI/Kit/"+name+".prefab");Assert.IsNotNull(asset,name);var w=asset.GetComponent<HitMeWidget>();Assert.IsNotNull(w);Assert.IsNotNull(w.theme);Assert.IsNotNull(w.background.sprite);Assert.IsNotNull(w.title.font);Assert.IsNotNull(w.action);Assert.AreEqual(0,w.action.onClick.GetPersistentEventCount());Assert.GreaterOrEqual(asset.GetComponent<RectTransform>().rect.height,44);}
  foreach(string name in UIKitSetup.PanelNames){var asset=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HitMe/Resources/UI/Kit/"+name+".prefab");Assert.IsNotNull(asset,name);var p=asset.GetComponent<HitMePanel>();Assert.IsNotNull(p.heading.font);Assert.IsNotNull(p.body.font);Assert.IsNotNull(p.value.font);Assert.Greater(p.items.Length,0);if(name.Contains("Chat"))Assert.IsNotNull(p.input.textComponent);}
  var showcase=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HitMe/Prefabs/UI/Showcase/UIShowcase.prefab");Assert.IsNotNull(showcase);Assert.AreEqual(Vector3.one,showcase.transform.localScale);
 }
 [Test]public void StateAndBindingNeverMoveParentOrEnableLockedAction(){
  var parent=new GameObject("LogicalParent");parent.transform.position=new Vector3(10,20,0);var w=HitMeWidgetFactory.Create("HitMeCharacterCard",parent.transform);var before=parent.transform.position;
  foreach(var state in new[]{WidgetState.Loading,WidgetState.Locked,WidgetState.Disabled,WidgetState.Unavailable,WidgetState.Eliminated}){w.SetState(state);Assert.IsFalse(w.action.interactable,state.ToString());Assert.AreEqual(before,parent.transform.position);}
  w.SetState(WidgetState.Selected);Assert.IsTrue(w.action.interactable);Assert.IsTrue(w.selection.activeSelf);w.Bind("Tên service","Không đổi damage");Strings.Load("en");w.RefreshLocalization();Assert.AreEqual("Tên service",w.title.text);Object.DestroyImmediate(parent);Strings.Load("vi");
 }
}
}
