using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] bool isKeySlot = true;
    [SerializeField] bool isFormulaSlot = false;
    [SerializeField] KeyAsoscietedSite keyAsoscietedSite = KeyAsoscietedSite.Non;
    [SerializeField] Text label;

    public string StoredKeyName { get { return actualKeyName; } }

    protected string actualKeyName = "";

    public delegate void ElementDropedHandler (string keyName, KeyAsoscietedSite keyAsoscietedSite, bool isKeySlot, bool isFromulaSlot);
    public event ElementDropedHandler OnElementDroped;

    // Start is called before the first frame update
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void OnDrop (PointerEventData eventData)
    {
        Draggable draggable = eventData.pointerDrag.GetComponent<Draggable> ();

        if (draggable != null)
        {
            draggable.LayoutElemrntComponent.ignoreLayout = true;
            draggable.GetComponent<RectTransform> ().position = GetComponent<RectTransform> ().position;
            draggable.GetComponent<Image> ().enabled = false;

            actualKeyName = draggable.GetComponent<Label> ().GetText ();

            if (OnElementDroped != null)
            {
                OnElementDroped (actualKeyName, keyAsoscietedSite, isKeySlot, isFormulaSlot);
            }

            if (label != null && isFormulaSlot)
            {
                label.text = actualKeyName;
                draggable.GetComponent<Label> ().UpdateLabel ("");
            }
        }
    }
}
