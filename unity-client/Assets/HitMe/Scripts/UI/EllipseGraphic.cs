using UnityEngine;
using UnityEngine.UI;

namespace HitMe.UI
{
    // One UI mesh. Also used for circular foot hitboxes and portraits.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class EllipseGraphic : MaskableGraphic
    {
        [Range(0, 1)] public float innerRatio;
        const int Segments = 96;
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect r = rectTransform.rect;
            Vector2 center = r.center;
            if (innerRatio <= 0)
            {
                vh.AddVert(center, color, new Vector2(.5f, .5f));
                for (int i = 0; i <= Segments; i++)
                {
                    float angle = 2 * Mathf.PI * i / Segments;
                    vh.AddVert(center + new Vector2(Mathf.Cos(angle) * r.width / 2, Mathf.Sin(angle) * r.height / 2), color, Vector2.zero);
                    if (i > 0) vh.AddTriangle(0, i, i + 1);
                }
            }
            else
            {
                for (int i = 0; i <= Segments; i++)
                {
                    float angle = 2 * Mathf.PI * i / Segments;
                    Vector2 outer = new Vector2(Mathf.Cos(angle) * r.width / 2, Mathf.Sin(angle) * r.height / 2);
                    vh.AddVert(center + outer, color, Vector2.zero);
                    vh.AddVert(center + outer * innerRatio, color, Vector2.zero);
                    if (i > 0) { int k = i * 2; vh.AddTriangle(k - 2, k, k - 1); vh.AddTriangle(k, k + 1, k - 1); }
                }
            }
        }
        public override bool Raycast(Vector2 sp, Camera eventCamera)
        {
            if (!base.Raycast(sp, eventCamera)) return false;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out Vector2 p);
            Rect r = rectTransform.rect;
            return Mathf.Pow((p.x - r.center.x) / (r.width / 2), 2) + Mathf.Pow((p.y - r.center.y) / (r.height / 2), 2) <= 1;
        }
    }
}
