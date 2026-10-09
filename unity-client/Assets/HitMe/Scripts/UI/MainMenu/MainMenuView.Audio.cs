using HitMe.Audio;
using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI
{
    public sealed partial class MainMenuView
    {
        public void AudioSettings(){var panel=Modal("audioSettings",480,MenuPanel.Settings);
            AudioSlider(panel,"Master",L("audioMaster"),90,HitMeAudio.Master,v=>HitMeAudio.Master=v);
            AudioSlider(panel,"SFX",L("audioSfx"),185,HitMeAudio.Sfx,v=>HitMeAudio.Sfx=v);
            AudioSlider(panel,"Music",L("audioMusic"),280,HitMeAudio.Music,v=>HitMeAudio.Music=v);
            Row(panel,"Mute",L("audioMute")+": "+L(HitMeAudio.Muted?"enabled":"disabled"),385,()=>{HitMeAudio.Muted=!HitMeAudio.Muted;AudioSettings();});
        }
        void AudioSlider(Transform parent,string id,string caption,float y,float initial,UnityEngine.Events.UnityAction<float> changed){
            T(parent,id+"Label",caption,0,y,310,26,17,theme.cream);
            var track=Panel(parent,id+"Slider",theme.card,Color.white,0,y+40,290,24);track.GetComponent<Image>().raycastTarget=true;
            var slider=track.gameObject.AddComponent<Slider>();var handle=Panel(track,"Handle",theme.paper,theme.cream,0,0,28,28);handle.pivot=new Vector2(.5f,.5f);
            slider.handleRect=handle;slider.targetGraphic=handle.GetComponent<Image>();slider.minValue=0;slider.maxValue=1;slider.value=initial;slider.onValueChanged.AddListener(changed);
        }
    }
}
