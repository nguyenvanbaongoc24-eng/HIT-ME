using NUnit.Framework;
using HitMe.UI;
using HitMe.Visuals;
using UnityEngine;
namespace HitMe.Tests {
public sealed class MapKitTests {
 [Test] public void PriorityMapsUseOneExistingDefinitionAndReportMissingArtwork(){var courtyard=ArenaMaps.Load(0);Assert.AreEqual("LangQueBacBo",courtyard.mapId);Assert.IsTrue(courtyard.ArtworkReady);Assert.IsNotNull(courtyard.floorSprite);Assert.IsNotNull(courtyard.previewSprite);foreach(int index in new[]{2,6}){var map=ArenaMaps.Load(index);Assert.IsNotNull(map);Assert.IsFalse(map.ArtworkReady);Assert.AreEqual(MapUnlockState.ArtworkMissing,map.unlockState);Assert.IsNull(map.previewSprite);Assert.IsNull(map.backdrop);Assert.IsTrue(map.CanUseOffline);Assert.AreEqual(0,map.backgroundLayers.Length);}}
 [Test] public void OnlineVisualsIgnoreLocalUnconfirmedMapSelection(){int saved=ArenaMaps.Selected;try{ArenaMaps.Selected=6;Assert.AreEqual("PhoCoHoiAn",ArenaMaps.ForBattle(false).mapId);Assert.AreEqual("LangQueBacBo",ArenaMaps.ForBattle(true).mapId);}finally{ArenaMaps.Selected=saved;}}
 [Test] public void AmbientPoolDoesNotInventParticlesWithoutSprites(){var go=new GameObject("AmbientTest",typeof(RectTransform),typeof(AmbientVFXController));go.GetComponent<AmbientVFXController>().Initialize(null);Assert.AreEqual(0,go.GetComponent<AmbientVFXController>().Capacity);Object.DestroyImmediate(go);}
}
}
