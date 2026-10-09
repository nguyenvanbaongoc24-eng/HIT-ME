using HitMe.Characters;
using UnityEngine;
namespace HitMe.UI {
// Local visual choices for offline preview/Bot mode. Online profile remains server-owned.
public static class CosmeticPreview {
 public static int Character {get=>Mathf.Clamp(PlayerPrefs.GetInt("OfflineCharacterVisual",0),0,2);set=>PlayerPrefs.SetInt("OfflineCharacterVisual",Mathf.Clamp(value,0,2));}
 public static int Weapon {get=>Mathf.Clamp(PlayerPrefs.GetInt("OfflineWeaponVisual",0),0,2);set=>PlayerPrefs.SetInt("OfflineWeaponVisual",Mathf.Clamp(value,0,2));}
 public static HitMe.Characters.CharacterDefinition Definition=>CharacterCatalog.Load()?.ForSlot(Character);
 public static WeaponDefinition WeaponDefinition=>CharacterCatalog.Load()?.ForSlot(Weapon)?.defaultWeapon;
}
}
