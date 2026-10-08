using System;
using System.Collections.Generic;
using System.Linq;
namespace HitMe.Core
{
    public enum BotDifficulty { Easy, Normal, Hard }
    public sealed class SeededRandom
    {
        uint state; public SeededRandom(uint seed) { state=seed==0?0x9e3779b9u:seed; }
        public double Next() { state^=state<<13; state^=state>>17; state^=state<<5; return state/4294967296.0; }
    }
    // This capability contains public prior rounds only: no match reference/current opponent inputs.
    public sealed class BotContext
    {
        public readonly string Self; public readonly int Hp,Round; public readonly double Deadline;
        public readonly IReadOnlyList<string> Alive; public readonly IReadOnlyList<PublicRound> History;
        public BotContext(string self,int hp,int round,double deadline,IEnumerable<string> alive,IEnumerable<PublicRound> history)
        { Self=self; Hp=hp; Round=round; Deadline=deadline; Alive=Array.AsReadOnly(alive.ToArray()); History=Array.AsReadOnly(history.ToArray()); }
    }
    public sealed class BotDecision
    {
        public readonly Point Position,Direction; public readonly double LockAt;
        public BotDecision(Point p,Point d,double at) { Position=p; Direction=d; LockAt=at; }
    }
    public sealed class BotController
    {
        readonly SeededRandom rng; readonly BotDifficulty difficulty; int decidedRound=-1,submittedRound=-1;
        BotDecision pending;
        public BotController(uint seed,BotDifficulty difficulty) { if(difficulty==BotDifficulty.Hard) throw new NotSupportedException("Hard is a scaffold, not enabled."); rng=new SeededRandom(seed); this.difficulty=difficulty; }
        public static Point RandomPosition(SeededRandom rng,FoundationConfig c)
        {
            if(c.arenaShape=="roundedRectangle")
            {
                var bounds=c.ArenaGeometry.Bounds;
                for(int i=0;i<64;i++){var p=new Point((rng.Next()*2-1)*(bounds.MaxX-c.playerRadius),(rng.Next()*2-1)*(bounds.MaxY-c.playerRadius));if(c.ArenaGeometry.PlayerPlacementValidation(p,c.playerRadius))return p;}
                return new Point();
            }
            double angle=rng.Next()*Math.PI*2,radius=Math.Sqrt(rng.Next()); return new Point(Math.Cos(angle)*radius*(c.arenaA-c.playerRadius),Math.Sin(angle)*radius*(c.arenaB-c.playerRadius)); }
        public BotDecision Decide(BotContext context,FoundationConfig c,double roundStarted)
        {
            if(context.Hp<=0) return null; if(decidedRound==context.Round)return pending;
            var position=RandomPosition(rng,c); double angle=rng.Next()*Math.PI*2; var direction=new Point(Math.Cos(angle),Math.Sin(angle));
            if(difficulty==BotDifficulty.Normal && context.History.Count>0)
            {
                var targets=context.History[context.History.Count-1].Actions.Where(a=>a.Id!=context.Self && context.Alive.Contains(a.Id)).OrderBy(a=>a.Id,StringComparer.Ordinal).ToArray();
                if(targets.Length>0) { var t=targets[(int)(rng.Next()*targets.Length)].Position; double dx=t.X-position.X,dy=t.Y-position.Y,length=Math.Sqrt(dx*dx+dy*dy); if(length>1e-9)direction=new Point(dx/length,dy/length); }
            }
            pending=new BotDecision(position,direction,roundStarted+c.placementSeconds*(.35+.5*rng.Next())); decidedRound=context.Round; return pending;
        }
        // Submission is injected; the bot never gets authority/state access or current opponent actions.
        public bool Submit(BotContext context,double now,Func<string,Point,double,bool> place,Func<string,Point,double,bool> aim,Func<string,double,bool> lockAction)
        {
            if(context.Hp<=0 || pending==null || decidedRound!=context.Round || submittedRound==context.Round || now<pending.LockAt || now>=context.Deadline) return false;
            if(!place(context.Self,pending.Position,now) || !aim(context.Self,pending.Direction,now) || !lockAction(context.Self,now))return false;
            submittedRound=context.Round; return true;
        }
    }
}
