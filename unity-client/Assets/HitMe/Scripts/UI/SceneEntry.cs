using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace HitMe.UI
{
    public sealed class SceneEntry : MonoBehaviour
    {
        void Start()
        {
            // Web uses the portrait canvas; browser orientation locking is not universally available.
#if !UNITY_WEBGL || UNITY_EDITOR
            Screen.orientation = ScreenOrientation.Portrait;
#endif
            WebMobileBridge.Ensure(); // Preserve the selected 60/30 FPS target across scenes.
            if (FindAnyObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            if (Camera.main == null)
            {
                var c = new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();
                c.tag = "MainCamera"; c.orthographic = true; c.backgroundColor = new Color(.42f, .26f, .22f);
            }
            if (FindAnyObjectByType<AudioListener>() == null) Camera.main.gameObject.AddComponent<AudioListener>();
            string scene = SceneManager.GetActiveScene().name;
            if (scene == "Boot") { SceneManager.LoadScene("MainMenu"); return; }
            if (scene == "Battle") { var view=gameObject.AddComponent<BattleView>(); view.Initialize(); OfflineRunContext.LoadDefaults(); if(NetworkSession.Instance!=null && NetworkSession.Instance.OnlineBattle) view.StartOnline(); else if(OfflineRunContext.Requested) { OfflineRunContext.Requested=false; view.StartOffline(OfflineRunContext.Settings); } else view.AddOfflineLauncher(); }
            else gameObject.AddComponent<FoundationMenu>().sceneName = scene;
        }
    }
}
