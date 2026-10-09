using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI {
/// <summary>Aspect-fill artwork only; no input or gameplay coordinates.</summary>
[RequireComponent(typeof(Image))]
public sealed class ImageCover : MonoBehaviour {
 RectTransform rect; Image artwork;
 void Awake(){rect=(RectTransform)transform;artwork=GetComponent<Image>();artwork.raycastTarget=false;}
 void LateUpdate(){var parent=rect.parent as RectTransform;if(parent==null||artwork.sprite==null)return;var size=artwork.sprite.rect.size;float scale=Mathf.Max(parent.rect.width/size.x,parent.rect.height/size.y);rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);rect.anchoredPosition=Vector2.zero;rect.sizeDelta=size*scale;}
}
}
