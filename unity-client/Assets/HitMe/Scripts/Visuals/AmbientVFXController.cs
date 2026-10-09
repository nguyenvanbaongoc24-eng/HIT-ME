using UnityEngine;
using UnityEngine.UI;
namespace HitMe.Visuals {
// Fixed sprite pool; no particles are fabricated if artwork is absent.
public sealed class AmbientVFXController:MonoBehaviour {
 MapAmbientEffectProfile profile;Image[] pool=new Image[0];float[] ages;float spawn;bool focused;
 public int Capacity=>pool.Length;
 public void Initialize(MapAmbientEffectProfile data){profile=data;if(data==null||data.sprites==null||data.sprites.Length==0)return;int capacity=Mathf.Clamp(data.capacity,0,32);pool=new Image[capacity];ages=new float[capacity];for(int i=0;i<capacity;i++){var go=new GameObject("AmbientSprite"+i,typeof(RectTransform),typeof(Image));go.transform.SetParent(transform,false);pool[i]=go.GetComponent<Image>();pool[i].raycastTarget=false;pool[i].preserveAspect=true;pool[i].rectTransform.sizeDelta=data.spriteSize;go.SetActive(false);}}
 public void SetFocused(bool value){focused=value;}
 void Update(){if(profile==null||pool.Length==0)return;int count=Mathf.Min(pool.Length,MotionSettings.AmbientCount);for(int i=count;i<pool.Length;i++)pool[i].gameObject.SetActive(false);if(focused||MotionSettings.Reduced)return;float dt=Time.unscaledDeltaTime;spawn+=dt*profile.particlesPerSecond;var bounds=((RectTransform)transform).rect;for(int i=0;i<count;i++){var image=pool[i];if(!image.gameObject.activeSelf){if(spawn<1)continue;spawn-=1;image.sprite=profile.sprites[Random.Range(0,profile.sprites.Length)];if(image.sprite==null)continue;ages[i]=0;image.rectTransform.anchoredPosition=new Vector2(Random.Range(bounds.xMin,bounds.xMax),Random.Range(bounds.yMin,bounds.yMax));image.gameObject.SetActive(true);}ages[i]+=dt;if(ages[i]>=profile.lifetime){image.gameObject.SetActive(false);continue;}image.rectTransform.anchoredPosition+=profile.drift*dt*MotionSettings.Strength;image.color=new Color(1,1,1,.35f*Mathf.Sin(Mathf.PI*ages[i]/profile.lifetime));}}
 void OnDisable(){foreach(var image in pool)if(image!=null)image.gameObject.SetActive(false);spawn=0;}
}
}
