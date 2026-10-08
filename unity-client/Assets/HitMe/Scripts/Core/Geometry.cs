using System;

namespace HitMe.Core
{
    // Feet positions, y-up. No dependency on UnityEngine, physics, or screen resolution.
    public static class Geometry
    {
        public static bool Finite(double n) { return !double.IsNaN(n) && !double.IsInfinity(n); }
        static void Axes(double a, double b)
        {
            if (!Finite(a) || !Finite(b) || a <= 0 || b <= 0) throw new ArgumentException("Positive finite ellipse axes required.");
        }
        static void Check(Point p) { if (!Finite(p.X) || !Finite(p.Y)) throw new ArgumentException("Finite point required."); }
        public static bool Contains(Point p, double a, double b)
        {
            Axes(a, b);
            return Finite(p.X) && Finite(p.Y) && p.X * p.X / (a * a) + p.Y * p.Y / (b * b) <= 1 + 1e-10;
        }
        // Prompt 1C's accepted approximation: shrink both axes by the circular feet radius.
        public static bool ValidPlacement(Point p, double a, double b, double radius)
        {
            Axes(a, b);
            if (!Finite(radius) || radius < 0 || radius >= Math.Min(a, b)) throw new ArgumentException("Invalid radius.");
            return Contains(p, a - radius, b - radius);
        }
        public static Point Clamp(Point p, double a, double b, double radius)
        {
            Check(p);
            ValidPlacement(new Point(0, 0), a, b, radius);
            double x = a - radius, y = b - radius;
            double q = p.X * p.X / (x * x) + p.Y * p.Y / (y * y);
            if (q <= 1) return p;
            double s = 1 / Math.Sqrt(q);
            return new Point(p.X * s, p.Y * s);
        }
        // Normalizes direction: the returned value is distance in logical units.
        public static double RayWallDistance(Point origin, Point direction, double a, double b)
        {
            Axes(a, b); Check(origin); Check(direction);
            if (!Contains(origin, a, b)) throw new ArgumentException("Ray origin outside arena.");
            double length = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            if (length <= 1e-12) throw new ArgumentException("Zero direction.");
            double dx = direction.X / length, dy = direction.Y / length;
            double aa = dx * dx / (a * a) + dy * dy / (b * b);
            double bb = 2 * (origin.X * dx / (a * a) + origin.Y * dy / (b * b));
            double cc = origin.X * origin.X / (a * a) + origin.Y * origin.Y / (b * b) - 1;
            return Math.Max(0, (-bb + Math.Sqrt(Math.Max(0, bb * bb - 4 * aa * cc))) / (2 * aa));
        }
        public static Point RayWallPoint(Point origin, Point direction, double a, double b)
        {
            double t = RayWallDistance(origin, direction, a, b);
            double length = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            return new Point(origin.X + direction.X / length * t, origin.Y + direction.Y / length * t);
        }
        // Confirmed line-hit criterion only; no choice of pierce count or damage resolution.
        public static bool IntersectsTarget(Point origin, Point direction, Point target, double reach, double wallDistance)
        {
            Check(origin); Check(direction); Check(target);
            if (!Finite(reach) || reach < 0 || !Finite(wallDistance) || wallDistance < 0) throw new ArgumentException("Invalid ray bounds.");
            double length = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            if (length <= 1e-12) throw new ArgumentException("Zero direction.");
            double dx = direction.X / length, dy = direction.Y / length;
            double ox = target.X - origin.X, oy = target.Y - origin.Y;
            double t = ox * dx + oy * dy;
            return t > 1e-9 && t <= wallDistance + 1e-9 && Math.Abs(ox * dy - oy * dx) <= reach + 1e-9;
        }
    }
}
