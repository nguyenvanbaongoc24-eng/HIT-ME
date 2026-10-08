using System;

namespace HitMe.Core
{
    public sealed class ArenaViewport
    {
        public readonly double Scale, CenterX, CenterY, OuterWidth, OuterHeight;
        public ArenaViewport(double width, double height, FoundationConfig config, double topInset=0, double bottomInset=0)
        {
            config.Validate();
            if (!Geometry.Finite(width) || !Geometry.Finite(height) || width <= 0 || height <= 0) throw new ArgumentException("Invalid viewport.");
            var bounds=config.ArenaGeometry.Bounds;
            double outerA = bounds.MaxX + config.wallLogic, outerB = bounds.MaxY + config.wallLogic;
            Scale = Math.Min(width * 0.96 / (2 * outerA), height * (1 - 2 * config.hudFraction) / (2 * outerB));
            if(config.arenaShape=="roundedRectangle") Scale=Math.Min(Scale,Math.Max(100,height-topInset-bottomInset-260)/(2*outerB));
            CenterX = width / 2; CenterY = height / 2+(config.arenaShape=="roundedRectangle"?(topInset-bottomInset)/2:0);
            OuterWidth = 2 * outerA * Scale; OuterHeight = 2 * outerB * Scale;
        }
        // Screen convention is top-left origin; Unity adapter flips y once.
        public Point WorldToScreen(Point world) { return new Point(CenterX + world.X * Scale, CenterY - world.Y * Scale); }
        public Point ScreenToWorld(Point screen) { return new Point((screen.X - CenterX) / Scale, (CenterY - screen.Y) / Scale); }
    }
}
