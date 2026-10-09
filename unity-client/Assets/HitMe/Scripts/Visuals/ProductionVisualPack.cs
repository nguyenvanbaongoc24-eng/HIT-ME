using HitMe.Characters;
using UnityEngine;
namespace HitMe.Visuals {
[CreateAssetMenu(menuName="HIT ME/Production Visual Pack")]
public sealed class ProductionVisualPack : ScriptableObject {
 public CharacterDefinition[] roster;
 public WeaponDefinition[] weapons;
 public EnvironmentLayerDefinition[] menuLayers,battleLayers;
 public Sprite centerLotus;
 public static ProductionVisualPack Load()=>Resources.Load<ProductionVisualPack>("Production/VisualPack");
}
}
