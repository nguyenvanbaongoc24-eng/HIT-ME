using System.Collections.Generic;
using HitMe.Visuals;
using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI {
public sealed partial class BattleView {
 readonly List<RectTransform> projectilePool=new List<RectTransform>(6);ImpactFeedbackController impactFeedback;
 void BuildMotion(){projectilePool.Clear();var environment=Rect("AmbientFallback",root,root.rect.size,Vector2.zero);environment.gameObject.AddComponent<EnvironmentMotionController>();environment.SetSiblingIndex(1);var impacts=Rect("ConfirmedImpactFeedback",root,root.rect.size,Vector2.zero);impactFeedback=impacts.gameObject.AddComponent<ImpactFeedbackController>();}
 void ReleaseProjectiles(){foreach(var p in projectiles)if(p!=null){if(p.GetComponent<ProjectileVisualController>()==null){Destroy(p.gameObject);continue;}p.gameObject.SetActive(false);projectilePool.Add(p);}projectiles.Clear();}
 RectTransform RentProjectile(Sprite sprite,float size,Vector2 origin){RectTransform r;if(projectilePool.Count>0){int last=projectilePool.Count-1;r=projectilePool[last];projectilePool.RemoveAt(last);}else{r=Box("VisualProjectile",root,Vector2.one*size,origin,Color.white).rectTransform;r.gameObject.AddComponent<ProjectileVisualController>();}r.name=online?"NetworkProjectile":"ProjectilePH";r.sizeDelta=Vector2.one*size;r.anchoredPosition=origin;r.GetComponent<Image>().sprite=sprite;r.GetComponent<Image>().preserveAspect=true;r.gameObject.SetActive(true);r.GetComponent<ProjectileVisualController>().ResetFlight();return r;}
}
}
