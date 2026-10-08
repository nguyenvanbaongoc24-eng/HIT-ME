using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI {
public sealed class HitMeText : Text {
 TextMeshProUGUI rendered; string lastText; Font lastFont; int lastSize; Color lastColor; TextAnchor lastAlignment; FontStyle lastStyle;
 protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();}
 protected override void OnEnable(){base.OnEnable();Ensure();Sync();}
 void Ensure(){if(rendered!=null)return;var go=new GameObject("TMP",typeof(RectTransform));go.transform.SetParent(transform,false);var r=go.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;rendered=go.AddComponent<TextMeshProUGUI>();rendered.raycastTarget=false;rendered.enableAutoSizing=true;rendered.font=Resources.Load<TMP_FontAsset>("Fonts/NunitoSDF");rendered.text="";}
 void LateUpdate(){Ensure();Sync();}
 void Sync(){
  if(lastFont!=font){lastFont=font;rendered.font=Resources.Load<TMP_FontAsset>(font==FoundationFonts.Symbols?"Fonts/SymbolsSDF":"Fonts/NunitoSDF");}
  if(lastText!=text){lastText=text;rendered.text=text;}
  if(lastSize!=fontSize){lastSize=fontSize;rendered.fontSize=fontSize;rendered.fontSizeMax=fontSize;rendered.fontSizeMin=Mathf.Max(5,fontSize*.8f);}
  if(lastColor!=color){lastColor=color;rendered.color=color;}
  if(lastAlignment!=alignment){lastAlignment=alignment;rendered.alignment=TextAlignmentOptions.Center;}
  if(lastStyle!=fontStyle){lastStyle=fontStyle;rendered.fontStyle=fontStyle==FontStyle.Bold?FontStyles.Bold:FontStyles.Normal;}
 }
}}
