using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HitMe.UI
{
    public sealed class FoundationMenu : MonoBehaviour
    {
        public string sceneName;
        void Start()
        {
            WebMobileBridge.Ensure(); Strings.Load(Strings.Language);
            if(sceneName=="MainMenu") { var theme=Resources.Load<UIThemeDefinition>("UI/MainMenuTheme"); if(theme?.entryPrefab!=null)Instantiate(theme.entryPrefab,transform);else gameObject.AddComponent<MainMenuController>(); return; }
            if(sceneName=="CharacterSelect"){gameObject.AddComponent<CharacterSelectionView>();return;}
            if(sceneName=="Lobby") { gameObject.AddComponent<NetworkLobbyView>(); return; }
            if(sceneName=="Result" && OfflineRunContext.Result!=null) { gameObject.AddComponent<OfflineResultView>(); return; }
            OfflineRunContext.LoadDefaults();
            var go = new GameObject("MenuCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)); go.transform.SetParent(transform);
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(390, 844);
            FoundationFonts.Validate(); var font = FoundationFonts.Text;
            var title = new GameObject("Title", typeof(RectTransform), typeof(HitMeText)); title.transform.SetParent(go.transform, false);
            var r = title.GetComponent<RectTransform>(); r.sizeDelta = new Vector2(350, 120); r.anchoredPosition = new Vector2(0, 230);
            var text = title.GetComponent<Text>(); text.font = font; text.fontSize = 28; text.alignment = TextAnchor.MiddleCenter; text.text = "HIT ME\n" + Strings.Get("scene" + sceneName);
            Add(go.transform, font, Strings.Get("playOffline"), 0, "Battle");
            if (sceneName == "MainMenu") { Add(go.transform, font, Strings.Get("sceneLobby"), -64, "Lobby"); Add(go.transform, font, Strings.Get("sceneCharacterSelect"), -128, "CharacterSelect"); }
            else Add(go.transform, font, Strings.Get("sceneMainMenu"), -64, "MainMenu");
            if(sceneName=="MainMenu")
            {
                AddOption(go.transform,font,"BotCount",-192,()=>Strings.Get("botCount")+": "+OfflineRunContext.Settings.BotCount,()=>OfflineRunContext.Settings.BotCount=OfflineRunContext.Settings.BotCount%5+1);
                AddOption(go.transform,font,"BotDifficulty",-256,()=>Strings.Get("difficulty")+": "+Strings.Get(OfflineRunContext.Settings.Difficulty==HitMe.Core.BotDifficulty.Easy?"easy":"normal"),()=>OfflineRunContext.Settings.Difficulty=OfflineRunContext.Settings.Difficulty==HitMe.Core.BotDifficulty.Easy?HitMe.Core.BotDifficulty.Normal:HitMe.Core.BotDifficulty.Easy);
                AddOption(go.transform,font,"ArenaMap",-314,()=>ArenaMaps.DisplayName,()=>ArenaMaps.Selected=(ArenaMaps.Selected+1)%ArenaMaps.Ids.Length);
                AddOption(go.transform,font,"Quality",-370,()=>Strings.Get(WebMobileBridge.LowQuality?"qualityLow":"qualityHigh"),()=>WebMobileBridge.SetQuality(!WebMobileBridge.LowQuality));
                var note=new GameObject("TimeoutPolicyNote",typeof(RectTransform),typeof(HitMeText)); note.transform.SetParent(go.transform,false); var nr=note.GetComponent<RectTransform>(); nr.sizeDelta=new Vector2(340,64); nr.anchoredPosition=new Vector2(0,145); var nt=note.GetComponent<Text>(); nt.font=font; nt.fontSize=13; nt.alignment=TextAnchor.MiddleCenter; nt.text=Strings.Get("timeoutProposal");
            }
        }
        void AddOption(Transform parent,Font font,string name,float y,System.Func<string> label,UnityEngine.Events.UnityAction change)
        {
            Add(parent,font,label(),y,"MainMenu"); var button=parent.GetChild(parent.childCount-1).GetComponent<Button>(); button.name=name; button.onClick.RemoveAllListeners(); button.onClick.AddListener(()=>{change();button.GetComponentInChildren<Text>().text=label();});
        }
        void Add(Transform parent, Font font, string label, float y, string scene)
        {
            var go = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button)); go.transform.SetParent(parent, false);
            var r = go.GetComponent<RectTransform>(); r.sizeDelta = new Vector2(290, 48); r.anchoredPosition = new Vector2(0, y);
            go.GetComponent<Image>().color = new Color(.12f, .55f, .50f); go.GetComponent<Button>().onClick.AddListener(() => { if(scene=="Battle") OfflineRunContext.Play(); else SceneManager.LoadScene(scene); });
            var child = new GameObject("Label", typeof(RectTransform), typeof(HitMeText)); child.transform.SetParent(go.transform, false);
            child.GetComponent<RectTransform>().sizeDelta = r.sizeDelta;
            var text = child.GetComponent<Text>(); text.font = font; text.fontSize = 19; text.alignment = TextAnchor.MiddleCenter; text.text = label; text.raycastTarget = false;
        }
    }
}

