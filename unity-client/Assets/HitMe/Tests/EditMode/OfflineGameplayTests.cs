using System;
using System.Linq;
using HitMe.Core;
using NUnit.Framework;
namespace HitMe.Tests
{
    public sealed class OfflineGameplayTests
    {
        readonly FoundationConfig c=new FoundationConfig();
        OfflineMatch Match(int bots=2,TimeoutPolicy timeout=TimeoutPolicy.ProposedStayAndSkip) => new OfflineMatch(c,new MatchSettings { BotCount=bots,Timeout=timeout },0);
        static void Action(OfflineMatch m,string id,Point p,Point d,double at=.1) { Assert.IsTrue(m.Place(id,p,at)); Assert.IsTrue(m.Aim(id,d,at)); Assert.IsTrue(m.Lock(id,at)); }
        [TestCase(1,2)] [TestCase(5,6)] public void SupportedPlayerCounts(int bots,int count) { Assert.AreEqual(count,Match(bots).Ids.Count); Assert.IsTrue(Match(bots).ViewFor("player").All(p=>p.Hp==3)); }
        [TestCase(0)] [TestCase(6)] [TestCase(-1)] public void RejectInvalidCounts(int bots) { Assert.Throws<ArgumentException>(()=>Match(bots)); }
        [Test] public void ReadyLocksOnlySelfAndHidesSecrets()
        { var m=Match(); Action(m,"player",new Point(0,0),new Point(1,0)); Assert.AreEqual(MatchPhase.Placement,m.Phase); Assert.IsFalse(m.Place("player",new Point(1,1),.2)); Assert.IsFalse(m.Aim("player",new Point(0,1),.2)); Assert.IsFalse(m.Lock("player",.2)); Assert.IsNull(m.ViewFor("bot-1")[0].Action); Assert.AreEqual(0,m.History.Count); Assert.IsNull(m.Resolution); }
        [Test] public void AllLockedRevealsEarlyAndEventsOnce()
        { var m=Match(1); Action(m,"player",new Point(-200,0),new Point(1,0)); Action(m,"bot-1",new Point(200,0),new Point(-1,0),.3); Assert.AreEqual(MatchPhase.Reveal,m.Phase); Assert.AreEqual(.3,m.PhaseStarted); Assert.IsTrue(m.ViewFor("player").All(f=>f.Action!=null)); Assert.AreEqual(1,m.DrainEvents().Count(e=>e.Phase==MatchPhase.Reveal)); m.Tick(.4); Assert.IsEmpty(m.DrainEvents()); }
        [Test] public void DeadlineKeepsValidAndUsesOldPositionForIncomplete()
        { var m=Match(1); m.Place("player",new Point(100,200),.1); m.Aim("player",new Point(1,0),.1); m.Tick(4.999); Assert.AreEqual(MatchPhase.Placement,m.Phase); m.Tick(5); Assert.AreEqual(MatchPhase.Reveal,m.Phase); var v=m.ViewFor("player"); Assert.AreEqual(100,v[0].Action.Position.X); Assert.IsTrue(v[0].Action.CanThrow); Assert.IsFalse(v[1].Action.CanThrow); Assert.IsTrue(v.All(f=>f.Stage==InputStage.Locked)); Assert.IsFalse(m.Lock("player",5)); }
        [Test] public void UnconfirmedFallbackCannotSilentlyBecomeOfficial()
        { var m=Match(1,TimeoutPolicy.Unconfirmed); m.Tick(5); Assert.IsTrue(m.NeedsTimeoutConfirmation); Assert.AreEqual(MatchPhase.Placement,m.Phase); Assert.IsNull(m.Resolution); }
        [Test] public void InvalidActionsAndDeadlineEditsRejected()
        { var m=Match(); Assert.IsFalse(m.Place("player",new Point(1000,1750),.1)); Assert.IsFalse(m.Place("player",new Point(double.NaN,0),.1)); Assert.IsFalse(m.Aim("player",new Point(1,0),.1)); m.Place("player",new Point(0,0),.1); Assert.IsFalse(m.Aim("player",new Point(0,0),.1)); Assert.IsFalse(m.Place("player",new Point(),5)); Assert.IsFalse(m.Lock("unknown",.1)); }
        [TestCase(1,0,200,0)] [TestCase(0,1,0,200)] [TestCase(1,1,200,200)] [TestCase(-1,0,-200,0)]
        public void RayHitsFirstOnly(double dx,double dy,double tx,double ty)
        {
            var result=CombatRules.Resolve(new[]{new FighterSnapshot("a",3),new FighterSnapshot("b",3),new FighterSnapshot("c",3)},new[]{new LockedAction("a",new Point(),new Point(dx,dy)),new LockedAction("b",new Point(tx,ty),new Point(),false),new LockedAction("c",new Point(tx*2,ty*2),new Point(),false)},c);
            Assert.AreEqual("b",result.Throws.Single().Target); Assert.AreEqual(2,result.Health.Single(h=>h.Id=="b").HPAfter); Assert.AreEqual(3,result.Health.Single(h=>h.Id=="c").HPAfter);
        }
        [Test] public void BehindTargetMissAndNearWallHit()
        {
            var people=new[]{new FighterSnapshot("a",3),new FighterSnapshot("b",3)};
            Assert.IsFalse(CombatRules.Resolve(people,new[]{new LockedAction("a",new Point(),new Point(1,0)),new LockedAction("b",new Point(-200,0),new Point(),false)},c).Throws[0].Hit);
            Assert.IsTrue(CombatRules.Resolve(people,new[]{new LockedAction("a",new Point(800,0),new Point(1,0)),new LockedAction("b",new Point(910,0),new Point(),false)},c).Throws[0].Hit);
        }
        [Test] public void ProjectileRadiusIsIncluded()
        { var f=new[]{new FighterSnapshot("a",3),new FighterSnapshot("b",3)}; Assert.IsTrue(CombatRules.Resolve(f,new[]{new LockedAction("a",new Point(),new Point(1,0)),new LockedAction("b",new Point(200,120),new Point(),false)},c).Throws[0].Hit); Assert.IsFalse(CombatRules.Resolve(f,new[]{new LockedAction("a",new Point(),new Point(1,0)),new LockedAction("b",new Point(200,120.1),new Point(),false)},c).Throws[0].Hit); }
        [Test] public void ExactDistanceTieUsesOrdinalIdRegardlessOfOrder()
        { var f=new[]{new FighterSnapshot("a",3),new FighterSnapshot("b",3),new FighterSnapshot("c",3)}; var a=new[]{new LockedAction("a",new Point(),new Point(1,0)),new LockedAction("c",new Point(200,30),new Point(),false),new LockedAction("b",new Point(200,-30),new Point(),false)}; Assert.AreEqual("b",CombatRules.Resolve(f,a,c).Throws.Single().Target); Assert.AreEqual("b",CombatRules.Resolve(f.Reverse(),a.Reverse(),c).Throws.Single().Target); }
        [Test] public void SimultaneousLethalShotsDrawAndReplayResets()
        { var m=Match(1); double now=.1; for(int r=0;r<3;r++){Action(m,"player",new Point(-200,0),new Point(1,0),now);Action(m,"bot-1",new Point(200,0),new Point(-1,0),now); Assert.IsTrue(m.Resolution.Throws.All(t=>t.Hit)); m.Tick(now+1.61); now+=1.7;} Assert.AreEqual(MatchPhase.MatchResult,m.Phase); Assert.AreEqual(MatchOutcome.Draw,m.Resolution.Outcome); Assert.IsTrue(m.ViewFor("player").All(p=>p.Hp==0)); Assert.IsFalse(m.Place("player",new Point(),now)); var replay=Match(1); Assert.AreEqual(1,replay.Round); Assert.IsTrue(replay.ViewFor("player").All(p=>p.Hp==3 && p.Action==null)); Assert.IsEmpty(replay.History); }
        [Test] public void LastSurvivorWinsAndDeadCannotAct()
        { var m=Match(1); double now=.1; for(int r=0;r<3;r++){Action(m,"player",new Point(-200,0),new Point(1,0),now);Action(m,"bot-1",new Point(200,0),new Point(1,0),now);m.Tick(now+1.61);now+=1.7;}Assert.AreEqual("player",m.Resolution.Winner);Assert.AreEqual(MatchOutcome.Winner,m.Resolution.Outcome);Assert.IsFalse(m.Lock("bot-1",now)); }
        [Test] public void DeadExcludedFromNextRoundAndRevealRequirement()
        { var m=Match(); double now=.1; for(int r=0;r<3;r++){Action(m,"player",new Point(-200,0),new Point(1,0),now);Action(m,"bot-1",new Point(200,0),new Point(1,0),now);Action(m,"bot-2",new Point(0,600),new Point(0,1),now);m.Tick(now+1.61);now+=1.7;}Assert.AreEqual(4,m.Round);Assert.IsFalse(m.Place("bot-1",new Point(),now)); Action(m,"player",new Point(-200,0),new Point(1,0),now);Action(m,"bot-2",new Point(200,0),new Point(-1,0),now);Assert.AreEqual(MatchPhase.Reveal,m.Phase); }
        [Test] public void DamageAggregatesAndClampsZero()
        { var f=new[]{new FighterSnapshot("a",3),new FighterSnapshot("b",3),new FighterSnapshot("c",1)};var result=CombatRules.Resolve(f,new[]{new LockedAction("a",new Point(-200,0),new Point(1,0)),new LockedAction("b",new Point(200,0),new Point(-1,0)),new LockedAction("c",new Point(),new Point(),false)},c); Assert.AreEqual(0,result.Health.Single(h=>h.Id=="c").HPAfter);Assert.IsTrue(result.Health.Single(h=>h.Id=="c").Eliminated); }
        [Test] public void FrameRateCannotChangeResultOrEventOrder()
        { var a=Match(1);var b=Match(1);foreach(var m in new[]{a,b}){Action(m,"player",new Point(-200,0),new Point(1,0));Action(m,"bot-1",new Point(200,0),new Point(-1,0));}for(double t=.1;t<1.72;t+=.01)a.Tick(t);a.Tick(1.72);b.Tick(1.72);Assert.AreEqual(a.Phase,b.Phase);Assert.AreEqual(a.Round,b.Round);Assert.AreEqual(a.ViewFor("player")[0].Hp,b.ViewFor("player")[0].Hp);CollectionAssert.AreEqual(a.DrainEvents().Select(e=>e.Phase),b.DrainEvents().Select(e=>e.Phase)); }
        [TestCase(BotDifficulty.Easy)] [TestCase(BotDifficulty.Normal)] public void SeededBotsValidPrivateAndRepeatable(BotDifficulty difficulty)
        { var a=new BotController(17,difficulty);var b=new BotController(17,difficulty);for(int round=1;round<=300;round++){var ctx=new BotContext("bot-1",3,round,5,new[]{"player","bot-1"},new PublicRound[0]);var x=a.Decide(ctx,c,0);var y=b.Decide(ctx,c,0);Assert.IsTrue(Geometry.ValidPlacement(x.Position,1000,1750,90));Assert.AreEqual(x.Position.X,y.Position.X);Assert.AreEqual(x.Direction.Y,y.Direction.Y);Assert.That(Math.Sqrt(x.Direction.X*x.Direction.X+x.Direction.Y*x.Direction.Y),Is.EqualTo(1).Within(1e-9));}Assert.IsFalse(typeof(BotContext).GetFields().Any(f=>f.FieldType==typeof(OfflineMatch))); }
        [Test] public void BotUsesOnlyPublicHistoryAndSubmitsOnce()
        { var m=Match(1);var bot=new BotController(17,BotDifficulty.Normal);var ctx=m.BotContextFor("bot-1");var d=bot.Decide(ctx,c,0);Action(m,"player",new Point(800,0),new Point(1,0));Assert.IsEmpty(ctx.History);Assert.IsTrue(bot.Submit(ctx,d.LockAt,m.Place,m.Aim,m.Lock));Assert.IsFalse(bot.Submit(ctx,d.LockAt,m.Place,m.Aim,m.Lock));Assert.IsNull(bot.Decide(new BotContext("bot-1",0,2,5,new[]{"player"},m.History),c,0)); }
        [Test] public void NormalAimsAtPreviousPublicPosition()
        { var previous=new Point(123,456);var bot=new BotController(17,BotDifficulty.Normal);var ctx=new BotContext("bot-1",3,2,5,new[]{"player","bot-1"},new[]{new PublicRound(1,new[]{new LockedAction("player",previous,new Point(1,0))})});var d=bot.Decide(ctx,c,0);double dx=previous.X-d.Position.X,dy=previous.Y-d.Position.Y;Assert.That(Math.Abs(dx*d.Direction.Y-dy*d.Direction.X),Is.LessThan(1e-9)); }
        [Test] public void SeededOfflineBotsCanFinishWithoutForcedWinner()
        {
            var m=Match(); var bots=new[]{new BotController(2026+7919,BotDifficulty.Normal),new BotController(2026+15838,BotDifficulty.Normal)};
            for(int round=0;round<1000 && m.Phase!=MatchPhase.MatchResult;round++)
            {
                double start=m.PhaseStarted;
                foreach(int i in new[]{0,1}) { var ctx=m.BotContextFor("bot-"+(i+1));var d=bots[i].Decide(ctx,c,start);if(d!=null)bots[i].Submit(ctx,d.LockAt,m.Place,m.Aim,m.Lock); }
                m.Tick(start+5); m.Tick(start+6.61);
            }
            Assert.AreEqual(MatchPhase.MatchResult,m.Phase);Assert.AreNotEqual(MatchOutcome.Ongoing,m.Resolution.Outcome);
        }
    }
}
