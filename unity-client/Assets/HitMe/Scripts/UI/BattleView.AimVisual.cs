using HitMe.Core;
using UnityEngine;
namespace HitMe.UI {
public sealed partial class BattleView {
 void ShowProductionAim(Point position,Point direction,bool locked){Point endpoint=Config.ArenaGeometry.ProjectileCollision(position,direction,Config.projectileRadius);var visual=aim.GetComponent<ProductionAimIndicator>();visual.Present(ToCanvas(position)-ArenaOffset,ToCanvas(endpoint)-ArenaOffset,locked);}
 public void SnapAimVisual(){if(aim!=null)aim.GetComponent<ProductionAimIndicator>().Snap();}
}
}
