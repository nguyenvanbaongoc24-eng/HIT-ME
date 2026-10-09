using HitMe.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace HitMe.UI
{
    public static class OfflineRunContext
    {
        public static bool Requested;
        public static MatchSettings Settings = new MatchSettings();
        public static RoundResolution Result; public static int Rounds;
        public static NetMatchStats[] Stats;
        static bool loaded;
        public static void LoadDefaults()
        {
            if(loaded)return; loaded=true;
            var resource=Resources.Load<TextAsset>("offline-match-config");
            if(resource!=null) { var data=JsonUtility.FromJson<OfflineOptions>(resource.text); Settings=new MatchSettings { BotCount=data.botCount,Seed=data.seed,Difficulty=(BotDifficulty)data.difficulty,Timeout=data.proposedTimeout?TimeoutPolicy.ProposedStayAndSkip:TimeoutPolicy.Unconfirmed,ThrowSeconds=data.throwSeconds,RoundResultSeconds=data.roundResultSeconds }; }
        }
        [System.Serializable] sealed class OfflineOptions { public int botCount=2,difficulty=1; public uint seed=2026; public bool proposedTimeout; public double throwSeconds=.7,roundResultSeconds=.6; }
        public static void Play() { Result=null; Rounds=0; Requested=true; SceneManager.LoadScene("Battle"); }
        public static void Menu() { Requested=false; Result=null; SceneManager.LoadScene("MainMenu"); }
    }
}
