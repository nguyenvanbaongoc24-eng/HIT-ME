using UnityEngine.SceneManagement;
namespace HitMe.UI {
public static class MenuSceneIds {public const string Home="MainMenu",Lobby="Lobby",Battle="Battle",Result="Result",Characters="CharacterSelect";}
// A one-shot route carries intent into the existing lobby, never another lobby controller.
public static class MenuNavigationService {
 public static string LobbyPage="lobby";
 public static void Lobby(string page="lobby"){LobbyPage=page;SceneManager.LoadScene(MenuSceneIds.Lobby);}
 public static string ConsumePage(){var page=LobbyPage;LobbyPage="lobby";return page;}
 public static void Practice(){OfflineRunContext.Play();}
}
}
