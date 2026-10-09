using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using HitMe.UI;
namespace HitMe.Tests {
public sealed class ProductionArtworkTests {
 [Test] public void RealSpritesBindToExistingThemeAndAllSixteenPrefabs(){
  var theme=Resources.Load<UIThemeDefinition>("UI/MainMenuTheme");Assert.IsNotNull(theme);
  Assert.That(AssetDatabase.GetAssetPath(theme.background),Does.Contain("Production/MainMenu/village-clean.png"));
  Assert.AreEqual(4,theme.featureArtwork.Length);foreach(var sprite in theme.featureArtwork)Assert.IsNotNull(sprite);
  foreach(var sprite in new[]{theme.paper,theme.brick,theme.card,theme.ink}){Assert.IsNotNull(sprite);Assert.Greater(sprite.border.x,0);}
  foreach(string name in new[]{"HitMeButtonPrimary","HitMeButtonSecondary","HitMeButtonTertiary","HitMeIconButton","HitMeFeatureCard","HitMeCharacterCard","HitMeWeaponCard","HitMeMapCard","HitMeTopBar","HitMeBottomNavigation","HitMePopup","HitMePlayerHUD","HitMeChatPanel","HitMeCharacterSelection","HitMeWeaponSelection","HitMeMapSelection"})Assert.IsNotNull(Resources.Load<GameObject>("UI/Kit/"+name),name);
  var arena=ArenaMaps.Load(0);Assert.That(AssetDatabase.GetAssetPath(arena.backdrop),Does.Contain("Production/Environments/battle-clean.png"));Assert.That(AssetDatabase.GetAssetPath(arena.sand),Does.EndWith("terracotta-tile.png"));
 }
}
}
