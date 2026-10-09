using System.Collections.Generic;
using HitMe.Visuals;
using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI {
public sealed partial class BattleView {
 MapMotionController mapMotion;
 void ConfirmedCrowdResponse(bool hit){if(hit)mapMotion?.ConfirmedCheer();}
 readonly List<RectTransform> projectilePool=new List<RectTransform>(6);ImpactFeedbackController impactFeedback;
 void BuildMotion(){projectilePool.Clear();var map=ArenaMaps.ForBattle(online);NorthernVillageArenaView village=null;if(map?.environmentPrefab!=null){var instance=Instantiate(map.environmentPrefab,root,false);instance.transform.SetSiblingIndex(0);village=instance.GetComponent<NorthernVillageArenaView>();village?.Bind(root,map);}var mapRoot=Rect("MapMotionRoot",village!=null?village.foregroundProps:root,root.rect.size,Vector2.zero);Stretch(mapRoot);if(village==null)mapRoot.SetSiblingIndex(1);mapMotion=mapRoot.gameObject.AddComponent<MapMotionController>();mapMotion.Initialize(map);var impacts=Rect("ConfirmedImpactFeedback",root,root.rect.size,Vector2.zero);impactFeedback=impacts.gameObject.AddComponent<ImpactFeedbackController>();}
 void ReleaseProjectiles(){foreach(var p in projectiles)if(p!=null){if(p.GetComponent<ProjectileVisualController>()==null){Destroy(p.gameObject);continue;}p.gameObject.SetActive(false);projectilePool.Add(p);}projectiles.Clear();}
 RectTransform RentProjectile(Sprite sprite,float size,Vector2 origin){RectTransform r;if(projectilePool.Count>0){int last=projectilePool.Count-1;r=projectilePool[last];projectilePool.RemoveAt(last);}else{r=Box("VisualProjectile",root,Vector2.one*size,origin,Color.white).rectTransform;r.gameObject.AddComponent<ProjectileVisualController>();}r.name=online?"NetworkProjectile":"ProjectilePH";r.sizeDelta=Vector2.one*size;r.anchoredPosition=origin;r.GetComponent<Image>().sprite=sprite;r.GetComponent<Image>().preserveAspect=true;r.gameObject.SetActive(true);var motion=r.GetComponent<ProjectileVisualController>();HitMe.Characters.WeaponDefinition definition=null;var catalog=HitMe.Characters.CharacterCatalog.Load();if(catalog?.characters!=null)foreach(var c in catalog.characters)if(c?.defaultWeapon!=null&&c.defaultWeapon.FlightSprite==sprite){definition=c.defaultWeapon;break;}motion.Configure(definition);motion.ResetFlight();return r;}
}
}
