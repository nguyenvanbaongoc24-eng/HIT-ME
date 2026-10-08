using System;
namespace HitMe.Core
{
    public struct ArenaBounds
    {
        public readonly double MinX,MinY,MaxX,MaxY;
        public ArenaBounds(double a,double b){MinX=-a;MaxX=a;MinY=-b;MaxY=b;}
    }
    public interface IArenaGeometry
    {
        ArenaBounds Bounds {get;}
        bool PointInside(Point point);
        bool PlayerPlacementValidation(Point point,double radius);
        Point ClampPosition(Point point,double clearance);
        Point RayBoundaryIntersection(Point origin,Point direction,double clearance=0);
        Point ProjectileCollision(Point origin,Point direction,double radius);
    }
    public sealed class EllipseArenaGeometry : IArenaGeometry
    {
        readonly double a,b;
        public EllipseArenaGeometry(double a,double b){Geometry.Contains(new Point(),a,b);this.a=a;this.b=b;}
        public ArenaBounds Bounds=>new ArenaBounds(a,b);
        public bool PointInside(Point p)=>Geometry.Contains(p,a,b);
        public bool PlayerPlacementValidation(Point p,double r)=>Geometry.ValidPlacement(p,a,b,r);
        public Point ClampPosition(Point p,double r)=>Geometry.Clamp(p,a,b,r);
        public Point RayBoundaryIntersection(Point o,Point d,double clearance=0)=>Geometry.RayWallPoint(o,d,a-clearance,b-clearance);
        public Point ProjectileCollision(Point o,Point d,double r)=>RayBoundaryIntersection(o,d,r);
    }
    public sealed class RoundedRectangleArenaGeometry : IArenaGeometry
    {
        public readonly double Width,Height,CornerRadius;
        public RoundedRectangleArenaGeometry(double width,double height,double cornerRadius)
        {
            if(!Geometry.Finite(width)||!Geometry.Finite(height)||!Geometry.Finite(cornerRadius)||width<=0||height<=0||cornerRadius<0||cornerRadius>Math.Min(width,height)/2)throw new ArgumentException("Invalid rounded arena dimensions.");
            Width=width;Height=height;CornerRadius=cornerRadius;
        }
        public ArenaBounds Bounds=>new ArenaBounds(Width/2,Height/2);
        void Offset(double clearance,out double a,out double b,out double r)
        {
            if(!Geometry.Finite(clearance)||clearance<0||clearance>=Math.Min(Width,Height)/2)throw new ArgumentException("Invalid clearance.");
            a=Width/2-clearance;b=Height/2-clearance;r=Math.Max(0,CornerRadius-clearance);
        }
        static bool Finite(Point p)=>Geometry.Finite(p.X)&&Geometry.Finite(p.Y);
        static bool Inside(Point p,double a,double b,double r)
        {
            if(!Finite(p))return false;
            double x=Math.Abs(p.X)-(a-r),y=Math.Abs(p.Y)-(b-r);
            return Math.Sqrt(Math.Max(x,0)*Math.Max(x,0)+Math.Max(y,0)*Math.Max(y,0))+Math.Min(Math.Max(x,y),0)-r<=1e-8;
        }
        public bool PointInside(Point p)=>Inside(p,Width/2,Height/2,CornerRadius);
        public bool PlayerPlacementValidation(Point p,double clearance){Offset(clearance,out var a,out var b,out var r);return Inside(p,a,b,r);}
        public Point ClampPosition(Point p,double clearance)
        {
            if(!Finite(p))throw new ArgumentException("Finite point required.");
            Offset(clearance,out var a,out var b,out var r);if(Inside(p,a,b,r))return p;
            double x=Math.Max(-(a-r),Math.Min(a-r,p.X)),y=Math.Max(-(b-r),Math.Min(b-r,p.Y));
            double dx=p.X-x,dy=p.Y-y,length=Math.Sqrt(dx*dx+dy*dy);
            return r==0?new Point(x,y):new Point(x+dx*r/length,y+dy*r/length);
        }
        public Point RayBoundaryIntersection(Point o,Point d,double clearance=0)
        {
            Offset(clearance,out var a,out var b,out var r);
            if(!Inside(o,a,b,r)||!Finite(d))throw new ArgumentException("Ray must start inside arena.");
            double length=Math.Sqrt(d.X*d.X+d.Y*d.Y);if(!Geometry.Finite(length)||length<1e-12)throw new ArgumentException("Nonzero direction required.");
            double dx=d.X/length,dy=d.Y/length,best=-1;
            // A convex arena has one positive exit, plus t=0 when starting on a boundary.
            for(int sign=-1;sign<=1;sign+=2)
            {
                if(Math.Abs(dx)>1e-12){double t=(sign*a-o.X)/dx;if(t>=-1e-8&&Math.Abs(o.Y+dy*t)<=b-r+1e-8)best=Math.Max(best,t);}
                if(Math.Abs(dy)>1e-12){double t=(sign*b-o.Y)/dy;if(t>=-1e-8&&Math.Abs(o.X+dx*t)<=a-r+1e-8)best=Math.Max(best,t);}
            }
            if(r>0)for(int sx=-1;sx<=1;sx+=2)for(int sy=-1;sy<=1;sy+=2)
            {
                double cx=sx*(a-r),cy=sy*(b-r),ox=o.X-cx,oy=o.Y-cy;
                double projection=ox*dx+oy*dy,disc=projection*projection-(ox*ox+oy*oy-r*r);
                if(disc < -1e-8)continue;double root=Math.Sqrt(Math.Max(0,disc));
                for(int sign=-1;sign<=1;sign+=2)
                {
                    double t=-projection+sign*root,x=o.X+dx*t,y=o.Y+dy*t;
                    if(t>=-1e-8&&sx*x>=a-r-1e-8&&sy*y>=b-r-1e-8)best=Math.Max(best,t);
                }
            }
            if(best< -1e-8)throw new InvalidOperationException("No arena exit.");
            return new Point(o.X+dx*Math.Max(0,best),o.Y+dy*Math.Max(0,best));
        }
        public Point ProjectileCollision(Point o,Point d,double radius)=>RayBoundaryIntersection(o,d,radius);
    }
}
