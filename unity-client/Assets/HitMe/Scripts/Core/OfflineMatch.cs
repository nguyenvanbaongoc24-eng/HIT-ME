using System;
using System.Collections.Generic;
using System.Linq;
namespace HitMe.Core
{
    public enum MatchPhase { MatchStart, RoundStart, Placement, Reveal, Throw, Resolve, RoundResult, MatchResult }
    public enum InputStage { Placement, Aim, Locked, Eliminated }
    public enum TimeoutPolicy { Unconfirmed, ProposedStayAndSkip }
    public sealed class MatchSettings
    {
        public int BotCount = 2; public uint Seed = 2026; public BotDifficulty Difficulty = BotDifficulty.Normal;
        public double ThrowSeconds = .7, RoundResultSeconds = .6;
        public TimeoutPolicy Timeout = TimeoutPolicy.Unconfirmed;
        public void Validate() { if (BotCount < 1 || BotCount > 5 || !Geometry.Finite(ThrowSeconds) || ThrowSeconds <= 0 || !Geometry.Finite(RoundResultSeconds) || RoundResultSeconds <= 0 || Difficulty == BotDifficulty.Hard) throw new ArgumentException("Unsupported match settings."); }
    }
    public sealed class GameEvent
    {
        public readonly MatchPhase Phase; public readonly int Round; public readonly double At;
        public GameEvent(MatchPhase phase, int round, double at) { Phase = phase; Round = round; At = at; }
    }
    public sealed class PublicFighter
    {
        public readonly string Id; public readonly int Hp; public readonly InputStage Stage;
        public readonly LockedAction Action;
        public PublicFighter(string id,int hp,InputStage stage,LockedAction action) { Id=id; Hp=hp; Stage=stage; Action=action; }
    }
    public sealed class PublicRound
    {
        public readonly int Round; public readonly IReadOnlyList<LockedAction> Actions;
        public PublicRound(int round,IEnumerable<LockedAction> actions) { Round=round; Actions=Array.AsReadOnly(actions.ToArray()); }
    }
    public sealed class OfflineMatch
    {
        sealed class Fighter
        {
            public string Id; public int Hp; public Point Previous, Position, Direction; public bool HasPosition, HasAim, Locked, CanThrow;
        }
        readonly FoundationConfig c; readonly MatchSettings settings; readonly List<Fighter> fighters = new List<Fighter>();
        readonly List<GameEvent> events = new List<GameEvent>(); readonly List<PublicRound> history = new List<PublicRound>();
        public MatchPhase Phase { get; private set; } public int Round { get; private set; }
        public double Deadline { get; private set; } public double PhaseStarted { get; private set; }
        public RoundResolution Resolution { get; private set; } public bool NeedsTimeoutConfirmation { get; private set; }
        public IReadOnlyList<PublicRound> History => history.AsReadOnly();
        public IReadOnlyList<string> Ids => Array.AsReadOnly(fighters.Select(f => f.Id).ToArray());
        public OfflineMatch(FoundationConfig config,MatchSettings options,double now)
        {
            config.Validate(); options.Validate(); if (!Geometry.Finite(now)) throw new ArgumentException("Invalid clock.");
            c=config; settings=new MatchSettings { BotCount=options.BotCount,Seed=options.Seed,Difficulty=options.Difficulty,Timeout=options.Timeout,ThrowSeconds=options.ThrowSeconds,RoundResultSeconds=options.RoundResultSeconds };
            var rng = new SeededRandom(options.Seed);
            for (int i=0;i<=options.BotCount;i++) fighters.Add(new Fighter { Id=i==0?"player":"bot-"+i,Hp=c.maxHp,Previous=BotController.RandomPosition(rng,c) });
            SetPhase(MatchPhase.MatchStart,now); StartRound(now);
        }
        void SetPhase(MatchPhase phase,double now) { Phase=phase; PhaseStarted=now; events.Add(new GameEvent(phase,Round,now)); }
        void StartRound(double now)
        {
            Round++; Resolution=null; SetPhase(MatchPhase.RoundStart,now);
            foreach(var f in fighters) { f.HasPosition=f.HasAim=f.Locked=f.CanThrow=false; f.Position=f.Previous; f.Direction=new Point(); }
            Deadline=now+c.placementSeconds; SetPhase(MatchPhase.Placement,now);
        }
        Fighter Editable(string id,double now) => Phase==MatchPhase.Placement && Geometry.Finite(now) && now>=PhaseStarted && now<Deadline ? fighters.FirstOrDefault(f=>f.Id==id && f.Hp>0 && !f.Locked) : null;
        public bool Place(string id,Point position,double now)
        {
            var f=Editable(id,now); if(f==null || !Geometry.ValidPlacement(position,c.arenaA,c.arenaB,c.playerRadius)) return false;
            f.Position=position; f.HasPosition=true; f.HasAim=false; return true;
        }
        public bool Aim(string id,Point direction,double now)
        {
            var f=Editable(id,now); double length=Math.Sqrt(direction.X*direction.X+direction.Y*direction.Y);
            if(f==null || !f.HasPosition || !Geometry.Finite(length) || length<1e-9) return false;
            f.Direction=new Point(direction.X/length,direction.Y/length); f.HasAim=true; return true;
        }
        public bool Lock(string id,double now)
        {
            var f=Editable(id,now); if(f==null || !f.HasPosition || !f.HasAim) return false;
            f.Locked=f.CanThrow=true; if(fighters.All(p=>p.Hp==0 || p.Locked)) Reveal(now); return true;
        }
        public IReadOnlyList<PublicFighter> ViewFor(string viewer)
        {
            bool revealed=Phase!=MatchPhase.Placement && Phase!=MatchPhase.RoundStart && Phase!=MatchPhase.MatchStart;
            return Array.AsReadOnly(fighters.Select(f=>new PublicFighter(f.Id,f.Hp,f.Hp==0?InputStage.Eliminated:f.Locked?InputStage.Locked:f.HasPosition?InputStage.Aim:InputStage.Placement,
                f.Hp>0 && f.HasPosition && (revealed || f.Id==viewer) ? new LockedAction(f.Id,f.Position,f.Direction,f.CanThrow || f.HasAim):null)).ToArray());
        }
        public BotContext BotContextFor(string id)
        {
            var f=fighters.FirstOrDefault(p=>p.Id==id); if(f==null) throw new ArgumentException("Unknown fighter.");
            return new BotContext(id,f.Hp,Round,Deadline,fighters.Where(p=>p.Hp>0).Select(p=>p.Id),history);
        }
        void Reveal(double now)
        {
            var actions=fighters.Where(f=>f.Hp>0).Select(f=>new LockedAction(f.Id,f.Position,f.Direction,f.CanThrow)).ToArray();
            Resolution=CombatRules.Resolve(fighters.Select(f=>new FighterSnapshot(f.Id,f.Hp)),actions,c);
            history.Add(new PublicRound(Round,actions)); SetPhase(MatchPhase.Reveal,now);
        }
        public void Tick(double now)
        {
            if(!Geometry.Finite(now) || now<PhaseStarted) return;
            for(int step=0;step<32;step++)
            {
                if(Phase==MatchPhase.Placement && now>=Deadline)
                {
                    var incomplete=fighters.Where(f=>f.Hp>0 && !f.Locked && (!f.HasPosition || !f.HasAim)).ToArray();
                    if(incomplete.Length>0 && settings.Timeout==TimeoutPolicy.Unconfirmed) { NeedsTimeoutConfirmation=true; return; }
                    foreach(var f in fighters.Where(f=>f.Hp>0 && !f.Locked))
                    {
                        f.CanThrow=f.HasPosition && f.HasAim;
                        if(!f.CanThrow) { f.Position=f.Previous; f.Direction=new Point(); }
                        f.HasPosition=f.Locked=true;
                    }
                    Reveal(Deadline); continue;
                }
                double end;
                if(Phase==MatchPhase.Reveal) { end=PhaseStarted+c.revealSeconds; if(now<end)return; SetPhase(MatchPhase.Throw,end); continue; }
                if(Phase==MatchPhase.Throw)
                {
                    end=PhaseStarted+settings.ThrowSeconds; if(now<end)return; SetPhase(MatchPhase.Resolve,end);
                    foreach(var h in Resolution.Health) { var f=fighters.First(p=>p.Id==h.Id); f.Hp=h.HPAfter; f.Previous=f.Position; }
                    SetPhase(MatchPhase.RoundResult,end); continue;
                }
                if(Phase==MatchPhase.RoundResult)
                {
                    end=PhaseStarted+settings.RoundResultSeconds; if(now<end)return;
                    if(Resolution.Outcome==MatchOutcome.Ongoing) StartRound(end); else SetPhase(MatchPhase.MatchResult,end);
                    continue;
                }
                return;
            }
        }
        public IReadOnlyList<GameEvent> DrainEvents() { var result=Array.AsReadOnly(events.ToArray()); events.Clear(); return result; }
    }
}
