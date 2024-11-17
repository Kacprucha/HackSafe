using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Canvas canvas;
    [SerializeField] LayoutElement layoutElement;

    public LayoutElement LayoutElemrntComponent { get { return layoutElement; } }

    protected Vector3 originalPosition;
    protected string keyName;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
        keyName = GetComponent<Label> ().GetText ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake ()
    {
        //originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag (PointerEventData eventData)
    {
        GetComponent<Image> ().enabled = true;
        GetComponent<Label> ().UpdateLabel (keyName);
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag (PointerEventData eventData)
    {
       rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag (PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<DropSlot> () == null)
        {
            layoutElement.ignoreLayout = false;
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
