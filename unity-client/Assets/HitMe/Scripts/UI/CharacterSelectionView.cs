using HitMe.Characters;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace HitMe.UI {
public sealed class CharacterSelectionView : MonoBehaviour {
 void Start(){Strings.Load(Strings.Language);var canvas=new GameObject("CharacterSelectionCanvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));canvas.transform.SetParent(transform,false);canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;var scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(390,844);var safe=new GameObject("SafeArea",typeof(RectTransform),typeof(SafeAreaAdapter));safe.transform.SetParent(canvas.transform,false);safe.GetComponent<SafeAreaAdapter>().Apply();var go=Instantiate(Resources.Load<GameObject>("UI/Kit/HitMeCharacterSelection"),safe.transform,false);Bind(go.GetComponent<HitMePanel>());var back=HitMeWidgetFactory.Create("HitMeButtonTertiary",safe.transform);back.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-280);back.titleKey="backMenu";back.RefreshLocalization();back.activated.AddListener(()=>SceneManager.LoadScene(MenuSceneIds.Home));}
 public static void Bind(HitMePanel panel,System.Action changed=null){var catalog=CharacterCatalog.Load();panel.headingKey="menuCharacters";for(int i=0;i<3;i++){int index=i;var d=catalog.characters[i];var card=panel.items[i];card.Bind(d.displayName,Strings.Get("offlineVisualChoice"),d.Frame(VisualState.Idle,0));card.activated.AddListener(()=>{CosmeticPreview.Character=index;panel.Select(index);changed?.Invoke();});}panel.Select(CosmeticPreview.Character);}
}
}
