using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : ScrollRect
{
    protected float mRadius = 0f;

    protected override void Start()
    {
        base.Start();
        mRadius = (transform as RectTransform).sizeDelta.x * 0.45f;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        var contentPosition = this.content.anchoredPosition;

        if (contentPosition.magnitude > mRadius)
        {
            contentPosition = contentPosition.normalized * mRadius;
            SetContentAnchoredPosition(contentPosition);
        }
    }
}
