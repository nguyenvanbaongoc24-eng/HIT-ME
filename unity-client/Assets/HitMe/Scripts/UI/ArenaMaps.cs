using UnityEngine;
namespace HitMe.UI {
public static class ArenaMaps {
 public static readonly string[] Ids={"LangQueBacBo","ChoVietNam","VinhHaLong","CoDoHue","ChoNoiCaiRang","TayBac"};
 public static readonly string[] Names={"Làng quê Bắc Bộ","Chợ Việt Nam","Vịnh Hạ Long","Cố đô Huế","Chợ nổi Cái Răng","Tây Bắc"};
 public static int Selected {get=>Mathf.Clamp(PlayerPrefs.GetInt("ArenaMap",0),0,Ids.Length-1);set{PlayerPrefs.SetInt("ArenaMap",Mathf.Clamp(value,0,Ids.Length-1));PlayerPrefs.Save();}}
 public static ArenaArtDefinition LoadSelected()=>Resources.Load<ArenaArtDefinition>("Arenas/"+Ids[Selected]);
 public static string DisplayName=>Strings.Get("map"+Ids[Selected])+(Selected==0?"":" · "+Strings.Get("missingArenaArt"));
}
}
