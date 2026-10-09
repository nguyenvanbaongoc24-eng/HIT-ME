using UnityEngine;
using UnityEngine.EventSystems;

namespace HitMe.UI
{
    public sealed class ArenaInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public BattleView view;
        int pointerId = int.MinValue;
        Vector2 down;
        public void OnPointerDown(PointerEventData e)
        {
            if (pointerId != int.MinValue || !view.CanPlace) return;
            pointerId = e.pointerId; down = e.position; view.Place(e.position);
        }
        public void OnDrag(PointerEventData e)
        {
            if (e.pointerId == pointerId && (e.position - down).sqrMagnitude >= view.DragThreshold * view.DragThreshold) view.Aim(e.position);
        }
        public void OnPointerUp(PointerEventData e) { if (e.pointerId == pointerId) { pointerId = int.MinValue; view.SnapAimVisual(); } }
    }
}
