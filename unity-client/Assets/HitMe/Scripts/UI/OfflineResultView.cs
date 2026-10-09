using HitMe.Core;
using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI
{
    public sealed class OfflineResultView : MonoBehaviour
    {
        public string DisplayedOutcome { get; private set; }
        void Start()
        {
            var result=OfflineRunContext.Result; FoundationFonts.Validate();
            var go=new GameObject("ResultCanvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster)); go.transform.SetParent(transform,false);
            go.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay; var scaler=go.GetComponent<CanvasScaler>(); scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution=new Vector2(390,844); scaler.matchWidthOrHeight=0;
            var safe=new GameObject("ResultSafeArea",typeof(RectTransform)).GetComponent<RectTransform>(); safe.SetParent(go.transform,false);
            var area=Screen.safeArea; safe.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height); safe.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height); safe.offsetMin=safe.offsetMax=Vector2.zero;
            safe.gameObject.AddComponent<SafeAreaAdapter>().Apply();safe.gameObject.AddComponent<UIMotionController>();
            DisplayedOutcome=Strings.Get(result.Outcome==MatchOutcome.Draw?"draw":result.Winner=="player"?"victory":"defeat");
            Label(safe,"ResultOutcome",DisplayedOutcome,140,30);
            Label(safe,"ResultWinner",result.Outcome==MatchOutcome.Draw?Strings.Get("draw"):Strings.Get("winner")+": "+(result.Winner=="player"?Strings.Get("you"):Strings.Get("bot")+" "+result.Winner.Substring(4)),75,20);
            Label(safe,"ResultRounds",Strings.Get("roundsPlayed")+": "+OfflineRunContext.Rounds,25,18);
            Button(safe,"Replay",Strings.Get("replay"),-60,OfflineRunContext.Play);
            Button(safe,"BackToMenu",Strings.Get("backMenu"),-124,OfflineRunContext.Menu);
        }
        void Label(Transform parent,string name,string text,float y,int size)
        { var go=new GameObject(name,typeof(RectTransform),typeof(HitMeText)); go.transform.SetParent(parent,false); var r=go.GetComponent<RectTransform>(); r.sizeDelta=new Vector2(340,56); r.anchoredPosition=new Vector2(0,y); var t=go.GetComponent<Text>(); t.font=FoundationFonts.Text; t.fontSize=size; t.alignment=TextAnchor.MiddleCenter; t.text=text; t.color=Color.white; t.raycastTarget=false; }
        void Button(Transform parent,string name,string text,float y,UnityEngine.Events.UnityAction action)
        { var widget=HitMeWidgetFactory.Create("HitMeButtonPrimary",parent);widget.name=name;var r=widget.GetComponent<RectTransform>();r.sizeDelta=new Vector2(290,48);r.anchoredPosition=new Vector2(0,y);widget.Bind(text);widget.action.onClick.AddListener(action); }
    }
}
