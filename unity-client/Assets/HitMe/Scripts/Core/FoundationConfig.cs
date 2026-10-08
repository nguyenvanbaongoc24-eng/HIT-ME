using System;

namespace HitMe.Core
{
    [Serializable]
    public sealed class FoundationConfig
    {
        public string arenaShape="ellipse";
        public double arenaWidth=2000,arenaHeight=3500,cornerRadius=300;
        [NonSerialized] IArenaGeometry cachedGeometry;
        [NonSerialized] string cachedShape; [NonSerialized] double cachedA,cachedB,cachedW,cachedH,cachedR;
        public IArenaGeometry ArenaGeometry
        {
            get
            {
                if(cachedGeometry==null||cachedShape!=arenaShape||cachedA!=arenaA||cachedB!=arenaB||cachedW!=arenaWidth||cachedH!=arenaHeight||cachedR!=cornerRadius)
                {
                    cachedGeometry=arenaShape=="roundedRectangle"?(IArenaGeometry)new RoundedRectangleArenaGeometry(arenaWidth,arenaHeight,cornerRadius):new EllipseArenaGeometry(arenaA,arenaB);
                    cachedShape=arenaShape;cachedA=arenaA;cachedB=arenaB;cachedW=arenaWidth;cachedH=arenaHeight;cachedR=cornerRadius;
                }
                return cachedGeometry;
            }
        }
        public double arenaA = 1000;
        public double arenaB = 1750;
        public double playerRadius = 90;
        public double projectileRadius = 30;
        public double placementSeconds = 5;
        public int maxHp = 3;
        public int minPlayers = 2;
        public int maxPlayers = 6;
        public double wallLogic = 140;
        public double hudFraction = 0.10;
        public double aimDragPixels = 12;
        public float revealSeconds = 0.3f;
        public float referenceWidth = 390;
        public float referenceHeight = 844;
        public void Validate()
        {
            if(arenaShape!="ellipse" && arenaShape!="roundedRectangle")throw new ArgumentException("Unsupported arena shape.");
            var bounds=ArenaGeometry.Bounds;
            if(playerRadius>=Math.Min(bounds.MaxX,bounds.MaxY)||projectileRadius>=Math.Min(bounds.MaxX,bounds.MaxY))throw new ArgumentException("Radius exceeds arena.");
            if (!Geometry.Finite(arenaA) || !Geometry.Finite(arenaB) || arenaA <= 0 || arenaB <= 0 ||
                !Geometry.Finite(playerRadius) || playerRadius < 0 || playerRadius >= Math.Min(arenaA, arenaB) ||
                !Geometry.Finite(projectileRadius) || projectileRadius < 0 || !Geometry.Finite(placementSeconds) || placementSeconds <= 0 ||
                !Geometry.Finite(wallLogic) || wallLogic < 0 || hudFraction <= 0 || hudFraction > 0.12 ||
                maxHp != 3 || minPlayers != 2 || maxPlayers != 6)
                throw new ArgumentException("Invalid foundation configuration.");
        }
    }
    public struct Point
    {
        public double X, Y;
        public Point(double x, double y) { X = x; Y = y; }
    }
}
