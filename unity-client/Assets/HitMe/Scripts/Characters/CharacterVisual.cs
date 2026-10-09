using System;
using HitMe.UI;
using UnityEngine;
using UnityEngine.UI;
using HitMe.Visuals;
namespace HitMe.Characters
{
    // The sole sprite/frame/motion renderer. Never writes to a fighter, foot anchor or hitbox.
    [DisallowMultipleComponent,RequireComponent(typeof(Image))]
    public sealed class CharacterVisual : MonoBehaviour
    {
        [SerializeField] CharacterDefinition definition;
        public CharacterDefinition Definition => definition;
        WeaponDefinition weaponOverride;
        public WeaponDefinition Weapon=>weaponOverride!=null?weaponOverride:definition?.defaultWeapon;
        public void SetCosmeticWeapon(WeaponDefinition weapon){if(weaponOverride==weapon)return;weaponOverride=weapon;UpdateWeapon();}
        public VisualState State { get; private set; }
        public FacingDirection Facing { get; private set; } = FacingDirection.Right;
        public bool HasSprite => image!=null && image.sprite!=null;
        public float DesiredHeight => HasSprite && definition!=null?definition.referenceHeight*definition.visualScale:58;
        public event Action<VisualState> StateChanged;
        Image image,heldWeapon,artwork; CharacterMotionProfile motion; CharacterManifest legacy; string legacyId; float stateStarted;
        bool initialized,presented; Vector2 direction=Vector2.right;
        public void Initialize(CharacterDefinition data,CharacterManifest fallback=null,string id=null)
        {
            definition=data; legacy=fallback; legacyId=id; image=GetComponent<Image>(); image.raycastTarget=false;
            Facing=data!=null?data.authoredFacing:FacingDirection.Right;
            direction=new Vector2((int)Facing,0); initialized=true; presented=false;
            motion=Resources.Load<CharacterMotionProfile>("Motion/Character");
        }
        public void SetFacing(Vector2 aim)
        {
            if(!float.IsFinite(aim.x)||!float.IsFinite(aim.y)||aim.sqrMagnitude<1e-10f)return;
            direction=aim.normalized;
            if(Mathf.Abs(aim.x)>1e-6f)Facing=aim.x<0?FacingDirection.Left:FacingDirection.Right;
        }
        public void Present(VisualState state,Color fallbackTint,float now)
        {
            if(!initialized)Initialize(definition);
            if(!presented || State!=state) { State=state;stateStarted=now;presented=true;StateChanged?.Invoke(state); }
            float elapsed=Mathf.Max(0,now-stateStarted);
            Sprite sprite=definition!=null?definition.Frame(state,elapsed):null;
            if(sprite==null && legacy!=null) sprite=legacy.LoadSprite(legacyId,ToLegacy(state));
            image.sprite=sprite;image.enabled=true;image.preserveAspect=sprite!=null;
            image.color=sprite==null?fallbackTint:state==VisualState.Eliminated?new Color(.5f,.5f,.5f,.7f):Color.white;
            float strength=MotionSettings.Strength;
            float throwAngle=motion!=null?motion.throwAngle:8;
            float angle=(state==VisualState.Eliminated?58:state==VisualState.Throw?elapsed<.12f?throwAngle*elapsed/.12f:-throwAngle*Mathf.Exp(-(elapsed-.12f)*8):state==VisualState.Aim?motion!=null?motion.aimAngle:3:0)*strength;
            // Authored multi-frame clips supply their own acting; don't rotate an entire animated sheet.
            var authored=definition?.Animation(state);
            if(sprite!=null && authored?.frames!=null && authored.frames.Length>1) angle=0;
            transform.localRotation=Quaternion.Euler(0,0,angle);
            float flip=sprite!=null && definition!=null && Facing!=definition.authoredFacing?-1:1;
            transform.localScale=new Vector3(flip,1,1);
            if(state==VisualState.Hit) image.color=Color.Lerp(image.color,new Color(1,.3f,.3f),(.5f+.5f*Mathf.Sin(now*30)));
            if(sprite!=null){
                if(artwork==null){var go=new GameObject("MotionArtwork",typeof(RectTransform),typeof(Image));go.transform.SetParent(transform,false);go.transform.SetAsFirstSibling();artwork=go.GetComponent<Image>();artwork.raycastTarget=false;}
                artwork.gameObject.SetActive(true);artwork.sprite=sprite;artwork.preserveAspect=true;artwork.color=image.color;
                // The logical foot/root stays fixed. Only this artwork child breathes and bobs.
                bool procedural=authored?.frames==null||authored.frames.Length<=1;
                float bob=state==VisualState.Idle?Mathf.Sin(now*3)*(motion!=null?motion.idleBob:.65f):state==VisualState.Victory?Mathf.Abs(Mathf.Sin(now*7))*(motion!=null?motion.victoryBounce:1.6f):0;
                float shake=state==VisualState.Hit?Mathf.Sin(elapsed*55)*Mathf.Exp(-elapsed*7)*(motion!=null?motion.hitShake:1.3f):0;
                var ar=artwork.rectTransform;ar.anchorMin=Vector2.zero;ar.anchorMax=Vector2.one;ar.offsetMin=ar.offsetMax=Vector2.zero;ar.pivot=image.rectTransform.pivot;
                ar.anchoredPosition=new Vector2(shake,bob)*(procedural?strength:0);
                float breath=state==VisualState.Idle?Mathf.Sin(now*3)*(motion!=null?motion.breathing:.006f)*strength:0;
                ar.localScale=Vector3.one*(1+(procedural?breath:0));image.enabled=false;
            }else if(artwork!=null)artwork.gameObject.SetActive(false);
            var label=transform.Find("MissingSpriteLabel");if(label!=null)label.gameObject.SetActive(sprite==null);
            UpdateWeapon();
        }
        public void Fit(float height,float maxWidth=200)
        {
            if(!HasSprite)return;
            var r=image.rectTransform; var pivot=definition!=null?definition.footPivot:image.sprite.pivot/image.sprite.rect.size;
            float aspect=image.sprite.rect.width/image.sprite.rect.height;
            // Reserve rotation envelope so the visible sprite stays out of the HUD.
            float angle=Mathf.Abs(r.localEulerAngles.z>180?r.localEulerAngles.z-360:r.localEulerAngles.z)*Mathf.Deg2Rad;
            float envelope=Mathf.Abs(Mathf.Cos(angle))+aspect*Mathf.Abs(Mathf.Sin(angle));
            float h=Mathf.Min(DesiredHeight,Mathf.Max(2,height-2*MotionSettings.Strength)/Mathf.Max(1,envelope));
            h=Mathf.Min(h,Mathf.Max(2,maxWidth)/(aspect+Mathf.Abs(Mathf.Sin(angle))));
            r.pivot=pivot;r.anchoredPosition=Vector2.zero;r.sizeDelta=new Vector2(h*aspect,h);
            UpdateWeapon();
        }
        void UpdateWeapon()
        {
            var weapon=Weapon;
            bool visible=weapon!=null && weapon.heldSprite!=null && State!=VisualState.Eliminated && State!=VisualState.Throw;
            if(!visible){if(heldWeapon!=null)heldWeapon.gameObject.SetActive(false);return;}
            if(heldWeapon==null) { var go=new GameObject("HeldWeaponSprite",typeof(RectTransform),typeof(Image));go.transform.SetParent(transform,false);heldWeapon=go.GetComponent<Image>();heldWeapon.raycastTarget=false; }
            heldWeapon.gameObject.SetActive(true);heldWeapon.sprite=weapon.heldSprite;heldWeapon.preserveAspect=true;
            var r=heldWeapon.rectTransform;r.anchorMin=r.anchorMax=weapon.handAnchor;r.anchoredPosition=Vector2.zero;
            float size=Mathf.Min(weapon.visualSize,image.rectTransform.rect.width*.4f,image.rectTransform.rect.height*.3f);r.pivot=weapon.gripPivot;
            r.sizeDelta=new Vector2(size,size*weapon.heldSprite.rect.height/weapon.heldSprite.rect.width);
            var local=transform.InverseTransformVector(new Vector3(direction.x,direction.y));
            r.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(local.y,local.x)*Mathf.Rad2Deg);
        }
        public static VisualState FromLegacy(CharacterPose pose) => pose==CharacterPose.Dead?VisualState.Eliminated:pose==CharacterPose.Win?VisualState.Victory:(VisualState)(int)pose;
        static CharacterPose ToLegacy(VisualState state) => state==VisualState.Eliminated?CharacterPose.Dead:state==VisualState.Victory?CharacterPose.Win:(CharacterPose)(int)state;
    }
}
