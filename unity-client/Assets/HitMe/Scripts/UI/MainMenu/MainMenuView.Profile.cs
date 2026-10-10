using HitMe.Characters;
using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI {
public sealed partial class MainMenuView {

 bool profileWasConnected;
 public void PlayerProfile(){
  profileWasConnected=controller.Connected;
  var panel=Modal("playerProfile",570);var profile=controller.Profile;var catalog=CharacterCatalog.Load();
  int selected=CosmeticPreview.Character;
  if(profile!=null)for(int i=0;i<catalog.characters.Length;i++)if(catalog.characters[i].characterId==profile.avatar)selected=i;
  var portrait=R(panel,"ProfilePortrait",0,64,88,88).gameObject.AddComponent<Image>();portrait.sprite=catalog.ForSlot(selected).portrait;portrait.preserveAspect=true;portrait.raycastTarget=false;
  T(panel,"LoginStatus",L(controller.Session?.ConnectionStatus??"connectionConnecting"),0,158,310,24,13,theme.muted);
  var nickname=Input(panel,"DisplayName",profile?.name??PlayerPrefs.GetString("PlayerDisplayName",""),193);nickname.characterLimit=24;nickname.interactable=controller.Connected;
  T(panel,"CharacterHeading",L("menuCharacters"),0,250,310,24,15,theme.cream);
  var buttons=new Button[3];
  for(int i=0;i<3;i++){int index=i;var definition=catalog.ForSlot(i);var button=B(panel,"ProfileCharacter"+i,theme.card,Color.white,-108+i*108,286,102,99,()=>{selected=index;portrait.sprite=definition.portrait;for(int n=0;n<3;n++)buttons[n].GetComponent<Image>().color=n==selected?new Color(1,.85f,.5f):Color.white;});buttons[i]=button;
   var icon=R(button.transform,"CharacterPortrait",0,8,58,58).gameObject.AddComponent<Image>();icon.sprite=definition.portrait;icon.preserveAspect=true;icon.raycastTarget=false;T(button.transform,"CharacterName",definition.displayName,0,67,94,25,11,theme.cream);button.GetComponent<Image>().color=i==selected?new Color(1,.85f,.5f):Color.white;
  }
  T(panel,"VerifiedBalance",profile!=null?L("menuCurrencyLabel")+": "+profile.coins:"",0,404,310,28,14,theme.cream);
  var save=Row(panel,"SavePlayerProfile",L("saveProfile"),449,()=>{if(!controller.Connected)return;controller.Session.SaveProfile(nickname.text,catalog.ForSlot(selected).characterId);CosmeticPreview.Character=selected;CloseModal();});save.interactable=controller.Connected;
  Row(panel,"ProfileRetry",L(controller.Connected?"back":"connectionRetry"),511,()=>{if(controller.Connected)CloseModal();else{controller.RetryConnection();PlayerProfile();}});
 }
 public void Searching(){var panel=Modal("matchSearching",360);var spinner=Icon(panel,"SearchSpinner","settings",0,76,42);spinner.gameObject.AddComponent<MenuSpinner>();T(panel,"SearchStatus","",0,135,310,103,16,theme.cream);
  // TMP text is updated directly below; no invented player counts.
  Row(panel,"CancelSearch",L("menuCancel"),263,controller.Cancel);UpdateSearch();
 }
 public void UpdateSearch(){if(modal==null)return;var label=modal.Find("Panel/SearchStatus")?.GetComponent<TMPro.TextMeshProUGUI>();if(label==null)return;var room=controller.Connected?controller.Session.Room:null;label.text=L(controller.Connected?"matchWaitingPlayers":controller.Session.ConnectionStatus)+"\n"+Mathf.FloorToInt(controller.SearchElapsed)+"s"+(room!=null?"\n"+L("matchedPlayers")+": "+room.players.Length+" / "+(room.minimumPlayers>0?room.minimumPlayers:2)+"–"+(room.capacity>0?room.capacity:6):"");}
}
}
