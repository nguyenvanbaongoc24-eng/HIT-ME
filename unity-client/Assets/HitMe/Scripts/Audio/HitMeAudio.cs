using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
namespace HitMe.Audio
{
    // Cosmetic only. Clips in this release are the explicitly labelled temporary SFX pack.
    public sealed class HitMeAudio : MonoBehaviour
    {
        static HitMeAudio instance;
        readonly Dictionary<string,AudioClip> clips=new Dictionary<string,AudioClip>();
        readonly HashSet<string> played=new HashSet<string>();
        readonly Queue<string> order=new Queue<string>();
        readonly AudioSource[] voices=new AudioSource[8];
        AudioSource music;int cursor;bool unlocked;
        public static float Master {get=>PlayerPrefs.GetFloat("AudioMaster",.7f);set{PlayerPrefs.SetFloat("AudioMaster",Mathf.Clamp01(value));Apply();}}
        public static float Sfx {get=>PlayerPrefs.GetFloat("AudioSfx",.8f);set{PlayerPrefs.SetFloat("AudioSfx",Mathf.Clamp01(value));Apply();}}
        public static float Music {get=>PlayerPrefs.GetFloat("AudioMusic",.3f);set{PlayerPrefs.SetFloat("AudioMusic",Mathf.Clamp01(value));Apply();}}
        public static bool Muted {get=>PlayerPrefs.GetInt("AudioMute",0)==1;set{PlayerPrefs.SetInt("AudioMute",value?1:0);Apply();}}
        static void Apply(){PlayerPrefs.Save();if(instance!=null)instance.ApplyVolumes();}
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void Boot()=>Ensure();
        public static HitMeAudio Ensure(){if(instance==null)instance=new GameObject("HitMeAudio").AddComponent<HitMeAudio>();return instance;}
        void Awake(){if(instance!=null&&instance!=this){Destroy(gameObject);return;}instance=this;DontDestroyOnLoad(gameObject);
            foreach(var clip in Resources.LoadAll<AudioClip>("Audio"))clips[clip.name]=clip;
            for(int i=0;i<voices.Length;i++)voices[i]=Source();music=Source();music.loop=true;
            SceneManager.sceneLoaded+=SceneLoaded;ApplyVolumes();}
        AudioSource Source(){var source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;return source;}
        void ApplyVolumes(){float master=Muted?0:Master;foreach(var voice in voices)if(voice!=null)voice.volume=master*Sfx;if(music!=null)music.volume=master*Music;}
        void SceneLoaded(Scene scene,LoadSceneMode mode){music.Stop();if(unlocked)StartMusic(scene.name);}
        void StartMusic(string scene){if(clips.TryGetValue(scene=="Battle"?"bgm_match_loop":"bgm_menu_loop",out var clip)){music.clip=clip;music.Play();}}
        void Update(){if(!unlocked&&(Mouse.current?.leftButton.wasPressedThisFrame==true||Touchscreen.current?.primaryTouch.press.wasPressedThisFrame==true)){unlocked=true;StartMusic(SceneManager.GetActiveScene().name);}}
        public bool Remember(string eventId){if(string.IsNullOrEmpty(eventId))return true;if(!played.Add(eventId))return false;order.Enqueue(eventId);if(order.Count>256)played.Remove(order.Dequeue());return true;}
        public static void Play(string clip,string eventId=null){var audio=Ensure();if(!audio.Remember(eventId)||!audio.unlocked||Muted||!audio.clips.TryGetValue(clip,out var asset))return;
            var voice=audio.voices[audio.cursor++%audio.voices.Length];voice.Stop();voice.clip=asset;voice.Play();}
        void OnDestroy(){SceneManager.sceneLoaded-=SceneLoaded;if(instance==this)instance=null;}
    }
}
