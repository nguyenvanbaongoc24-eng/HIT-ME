using System;
using System.Collections.Generic;
using System.Linq;
namespace HitMe.Core
{
    public enum MatchOutcome { Ongoing, Winner, Draw }
    public sealed class LockedAction
    {
        public readonly string Id; public readonly Point Position, Direction; public readonly bool CanThrow;
        public LockedAction(string id, Point position, Point direction, bool canThrow = true)
        { Id = id; Position = position; Direction = direction; CanThrow = canThrow; }
    }
    public sealed class FighterSnapshot
    {
        public readonly string Id; public readonly int Hp;
        public FighterSnapshot(string id, int hp) { Id = id; Hp = hp; }
    }
    public sealed class ThrowResult
    {
        public readonly string Thrower, Target; public readonly int Damage;
        public readonly Point Origin, End; public bool Hit => Target != null;
        public ThrowResult(string thrower, string target, Point origin, Point end)
        { Thrower = thrower; Target = target; Damage = target == null ? 0 : 1; Origin = origin; End = end; }
    }
    public sealed class HealthResult
    {
        public readonly string Id; public readonly int HPBefore, HPAfter;
        public bool Eliminated => HPBefore > 0 && HPAfter == 0;
        public HealthResult(string id, int before, int after) { Id = id; HPBefore = before; HPAfter = after; }
    }
    public sealed class RoundResolution
    {
        public readonly IReadOnlyList<ThrowResult> Throws; public readonly IReadOnlyList<HealthResult> Health;
        public readonly MatchOutcome Outcome; public readonly string Winner;
        public RoundResolution(List<ThrowResult> throws, List<HealthResult> health)
        {
            Throws = throws.AsReadOnly(); Health = health.AsReadOnly();
            var alive = health.Where(h => h.HPAfter > 0).ToArray();
            Outcome = alive.Length == 0 ? MatchOutcome.Draw : alive.Length == 1 ? MatchOutcome.Winner : MatchOutcome.Ongoing;
            Winner = alive.Length == 1 ? alive[0].Id : null;
        }
    }
    public static class CombatRules
    {
        // Exact projected-distance ties use ordinal ID, independent of collection/render order.
        public static RoundResolution Resolve(IEnumerable<FighterSnapshot> fighters, IEnumerable<LockedAction> actions, FoundationConfig c)
        {
            c.Validate(); var people = fighters.OrderBy(p => p.Id, StringComparer.Ordinal).ToArray();
            if (people.Select(p => p.Id).Distinct().Count() != people.Length || people.Any(p => string.IsNullOrEmpty(p.Id) || p.Hp < 0 || p.Hp > c.maxHp)) throw new ArgumentException("Invalid fighters.");
            var alive = people.Where(p => p.Hp > 0).ToArray();
            var input = actions.ToArray();
            if (input.Select(a => a.Id).Distinct().Count() != input.Length || input.Length != alive.Length || input.Any(a => !alive.Any(p => p.Id == a.Id))) throw new ArgumentException("One locked action per survivor required.");
            foreach (var a in input)
                if (!c.ArenaGeometry.PlayerPlacementValidation(a.Position,c.playerRadius) || (a.CanThrow && (!Geometry.Finite(a.Direction.X) || !Geometry.Finite(a.Direction.Y) || !Geometry.Finite(Math.Sqrt(a.Direction.X*a.Direction.X+a.Direction.Y*a.Direction.Y)) || Math.Sqrt(a.Direction.X*a.Direction.X+a.Direction.Y*a.Direction.Y) < 1e-9))) throw new ArgumentException("Invalid action.");
            var hits = new List<ThrowResult>(); var damage = people.ToDictionary(p => p.Id, p => 0);
            foreach (var a in input.OrderBy(a => a.Id, StringComparer.Ordinal))
            {
                if (!a.CanThrow) continue;
                double len = Math.Sqrt(a.Direction.X*a.Direction.X+a.Direction.Y*a.Direction.Y);
                Point d = new Point(a.Direction.X/len, a.Direction.Y/len);
                Point boundary=c.ArenaGeometry.RayBoundaryIntersection(a.Position,d,c.arenaShape=="roundedRectangle"?c.projectileRadius:0);
                double wall=(boundary.X-a.Position.X)*d.X+(boundary.Y-a.Position.Y)*d.Y;
                LockedAction target = null; double nearest = double.PositiveInfinity;
                foreach (var b in input.OrderBy(b => b.Id, StringComparer.Ordinal))
                {
                    if (b.Id == a.Id || !Geometry.IntersectsTarget(a.Position,d,b.Position,c.playerRadius+c.projectileRadius,wall)) continue;
                    double t = (b.Position.X-a.Position.X)*d.X+(b.Position.Y-a.Position.Y)*d.Y;
                    if (t < nearest) { nearest = t; target = b; }
                }
                Point end = target == null ? boundary : new Point(a.Position.X+d.X*nearest,a.Position.Y+d.Y*nearest);
                hits.Add(new ThrowResult(a.Id,target?.Id,a.Position,end)); if (target != null) damage[target.Id]++;
            }
            return new RoundResolution(hits,people.Select(p => new HealthResult(p.Id,p.Hp,Math.Max(0,p.Hp-damage[p.Id]))).ToList());
        }
    }
}
