using System;

namespace HitMe.Core
{
    public enum BattlePhase { LayoutPreview, Placement, Reveal, AwaitingRules }
    public sealed class PlacementSession
    {
        public BattlePhase Phase { get; private set; }
        public bool Locked { get; private set; }
        public bool HasPosition { get; private set; }
        public bool HasAim { get; private set; }
        public Point Position { get; private set; }
        public Point Direction { get; private set; }
        public double Deadline { get; private set; }
        readonly FoundationConfig config;
        public PlacementSession(FoundationConfig config) { config.Validate(); this.config = config; Phase = BattlePhase.LayoutPreview; }
        public void Start(double now)
        {
            Phase = BattlePhase.Placement; Locked = HasPosition = HasAim = false;
            Position = new Point(0, 0); Direction = new Point(0, 0); Deadline = now + config.placementSeconds;
        }
        public bool Place(Point p, double now)
        {
            if (Phase != BattlePhase.Placement || Locked || now >= Deadline) return false;
            Position = config.ArenaGeometry.ClampPosition(p,config.playerRadius);
            HasPosition = true; HasAim = false; return true;
        }
        public bool Aim(Point target, double now)
        {
            if (Phase != BattlePhase.Placement || Locked || !HasPosition || now >= Deadline) return false;
            double dx = target.X - Position.X, dy = target.Y - Position.Y, len = Math.Sqrt(dx * dx + dy * dy);
            if (len < 1e-9) return false;
            Direction = new Point(dx / len, dy / len); HasAim = true; return true;
        }
        public bool Ready(double now)
        {
            if (Phase != BattlePhase.Placement || Locked || !HasAim || now >= Deadline) return false;
            Locked = true; return true; // Never shortens the other participants' deadline.
        }
        public void Tick(double now)
        {
            if (Phase == BattlePhase.Placement && now >= Deadline)
            {
                Locked = true;
                Phase = HasPosition && HasAim ? BattlePhase.Reveal : BattlePhase.AwaitingRules;
            }
        }
        public void FinishReveal() { if (Phase == BattlePhase.Reveal) Phase = BattlePhase.AwaitingRules; }
    }
}
