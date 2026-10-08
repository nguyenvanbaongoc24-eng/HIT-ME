using System;
using HitMe.Core;
using NUnit.Framework;

namespace HitMe.Tests
{
    public sealed class FoundationTests
    {
        readonly FoundationConfig config = new FoundationConfig();
        [TestCase(1, 0, 1000)] [TestCase(0, 1, 1750)] [TestCase(-1, 0, 1000)] [TestCase(0, -1, 1750)] [TestCase(5, 0, 1000)]
        public void AxisRays(double x, double y, double expected)
        { Assert.That(Geometry.RayWallDistance(new Point(0, 0), new Point(x, y), 1000, 1750), Is.EqualTo(expected).Within(1e-7)); }
        [TestCase(1, 1)] [TestCase(-1, 1)] [TestCase(1, -1)] [TestCase(-1, -1)]
        public void DiagonalEndsOnWall(double x, double y)
        { Point p = Geometry.RayWallPoint(new Point(110, -80), new Point(x, y), 1000, 1750); Assert.That(p.X * p.X / 1e6 + p.Y * p.Y / (1750 * 1750), Is.EqualTo(1).Within(1e-9)); }
        [Test] public void BoundaryOutwardIsZeroInwardIsDiameter()
        { Assert.That(Geometry.RayWallDistance(new Point(1000, 0), new Point(1, 0), 1000, 1750), Is.EqualTo(0).Within(1e-9)); Assert.That(Geometry.RayWallDistance(new Point(1000, 0), new Point(-1, 0), 1000, 1750), Is.EqualTo(2000).Within(1e-9)); }
        [Test] public void NearWallAndTangent()
        { Assert.That(Geometry.RayWallDistance(new Point(999, 0), new Point(1, 0), 1000, 1750), Is.EqualTo(1).Within(1e-6)); Assert.That(Geometry.RayWallDistance(new Point(0, 1750), new Point(1, 0), 1000, 1750), Is.EqualTo(0).Within(1e-6)); }
        [Test] public void InvalidRayInputRejected()
        { Assert.Throws<ArgumentException>(() => Geometry.RayWallDistance(new Point(0, 0), new Point(0, 0), 1000, 1750)); Assert.Throws<ArgumentException>(() => Geometry.RayWallDistance(new Point(1100, 0), new Point(1, 0), 1000, 1750)); Assert.Throws<ArgumentException>(() => Geometry.RayWallDistance(new Point(double.NaN, 0), new Point(1, 0), 1000, 1750)); }
        [Test] public void PlacementUsesFeetRadius()
        { Assert.IsTrue(Geometry.ValidPlacement(new Point(910, 0), 1000, 1750, 90)); Assert.IsFalse(Geometry.ValidPlacement(new Point(911, 0), 1000, 1750, 90)); Assert.IsTrue(Geometry.ValidPlacement(new Point(0, 1660), 1000, 1750, 90)); }
        [Test] public void ClampPreservesDirection()
        { Point p = Geometry.Clamp(new Point(1300, 1800), 1000, 1750, 90); Assert.IsTrue(Geometry.ValidPlacement(p, 1000, 1750, 90)); Assert.That(p.X / p.Y, Is.EqualTo(1300.0 / 1800).Within(1e-9)); }
        [Test] public void BehindTargetNotHitAndAlignedTargetsRemainCandidates()
        {
            Assert.IsFalse(Geometry.IntersectsTarget(new Point(0, 0), new Point(1, 0), new Point(-100, 0), 120, 1000));
            Assert.IsTrue(Geometry.IntersectsTarget(new Point(0, 0), new Point(1, 0), new Point(100, 0), 120, 1000));
            Assert.IsTrue(Geometry.IntersectsTarget(new Point(0, 0), new Point(1, 0), new Point(300, 0), 120, 1000)); // Pierce policy deliberately not inferred.
            Assert.IsTrue(Geometry.IntersectsTarget(new Point(0, 0), new Point(1, 0), new Point(100, 120), 120, 1000));
            Assert.IsFalse(Geometry.IntersectsTarget(new Point(0, 0), new Point(1, 0), new Point(100, 121), 120, 1000));
        }
        [TestCase(360, 800)] [TestCase(390, 844)] [TestCase(412, 915)] [TestCase(430, 932)]
        public void ResponsiveCoverageAndRoundTrip(int w, int h)
        {
            var view = new ArenaViewport(w, h, config);
            Assert.That(view.OuterWidth / w, Is.GreaterThanOrEqualTo(.92)); Assert.That(view.OuterHeight / h, Is.GreaterThanOrEqualTo(.70));
            Point original = new Point(213, -790), restored = view.ScreenToWorld(view.WorldToScreen(original));
            Assert.That(restored.X, Is.EqualTo(original.X).Within(1e-7)); Assert.That(restored.Y, Is.EqualTo(original.Y).Within(1e-7));
        }
        [Test] public void ReadyDoesNotRevealEarly()
        {
            var session = new PlacementSession(config); session.Start(10); session.Place(new Point(0, 0), 11); session.Aim(new Point(1, 0), 11);
            Assert.IsTrue(session.Ready(12)); Assert.IsFalse(session.Place(new Point(100, 100), 12)); session.Tick(14.99);
            Assert.That(session.Phase, Is.EqualTo(BattlePhase.Placement)); session.Tick(15); Assert.That(session.Phase, Is.EqualTo(BattlePhase.Reveal));
        }
        [Test] public void NoInputOrAimDoesNotInventThrow()
        {
            var s = new PlacementSession(config); s.Start(0); s.Tick(5); Assert.That(s.Phase, Is.EqualTo(BattlePhase.AwaitingRules));
            s.Start(10); s.Place(new Point(0, 0), 11); s.Tick(15); Assert.That(s.Phase, Is.EqualTo(BattlePhase.AwaitingRules));
        }
        [Test] public void DeadlineRejectsLateInput() { var s = new PlacementSession(config); s.Start(0); Assert.IsFalse(s.Place(new Point(0, 0), 5)); }
    }
}
