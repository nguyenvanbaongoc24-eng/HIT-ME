using UnityEngine;
using UnityEngine.U2D;
namespace HitMe.UI {
[CreateAssetMenu(menuName="HIT ME/UI Theme")]
public sealed class UIThemeDefinition : ScriptableObject {
 public Sprite paper,brick,ink,card,logo,shadow,background;
 public Sprite[] featureArtwork;
 public Sprite staticCharacterPreview;
 public Sprite[] icons; public string[] iconIds;
 public SpriteAtlas atlas;
 public GameObject entryPrefab;
 public Color cream=new Color(.98f,.89f,.71f),muted=new Color(.72f,.75f,.66f),dark=new Color(.19f,.12f,.08f);
 public Sprite Icon(string id){if(iconIds!=null)for(int i=0;i<iconIds.Length;i++)if(iconIds[i]==id&&i<icons.Length)return icons[i];return null;}
}
}
