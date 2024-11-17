using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableOverlay : MonoBehaviour, /*IPointerDownHandler,*/ IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] Button closeButton;

    virtual public void Start ()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener (() => CloseButtonClicked ());
        }
    }

    virtual public void ShowOverlay ()
    {
        gameObject.SetActive (true);
    }

    virtual public void CloseOverlay ()
    {
        gameObject.SetActive (false);
    }

    void CloseButtonClicked ()
    {
        this.gameObject.SetActive (false);
    }

    //public void OnPointerDown (PointerEventData eventData)
    //{
    //    // This can be used to bring the window to the front
    //    transform.SetAsLastSibling ();
    //}

    public void OnBeginDrag (PointerEventData eventData)
    {
        // Make the window slightly transparent while dragging
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag (PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag (PointerEventData eventData)
    {
        // Reset the transparency
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }
}

