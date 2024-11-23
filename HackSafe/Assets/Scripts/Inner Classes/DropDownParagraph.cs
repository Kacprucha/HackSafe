using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownParagraph : MonoBehaviour
{
    [SerializeField] RectTransform scrollViewTransform;
    [SerializeField] Button dropDownButton;
    [SerializeField] Image dropDownButtonImage;
    [SerializeField] List<GameObject> paragraphContent;

    bool isDropDown = false;

    // Start is called before the first frame update
    void Start()
    {
        if (dropDownButton != null)
        {
            dropDownButton.onClick.AddListener (() => onDropDwonButtonClicked ());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable ()
    {
        if (isDropDown)
        {

            dropDownButtonImage.gameObject.transform.eulerAngles = Vector3.forward * 0;
            foreach (GameObject paragraph in paragraphContent)
            {
                paragraph.SetActive (true);
            }
        }
        else
        {
            dropDownButtonImage.gameObject.transform.eulerAngles = Vector3.forward * 90;
            foreach (GameObject paragraph in paragraphContent)
            {
                paragraph.SetActive (false);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate (scrollViewTransform);
    }

    private void OnDisable ()
    {
        foreach (GameObject paragraph in paragraphContent)
        {
            paragraph.SetActive (false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate (scrollViewTransform);
    }

    protected void onDropDwonButtonClicked ()
    {
        isDropDown = !isDropDown;

        if (isDropDown)
        {

            dropDownButtonImage.gameObject.transform.eulerAngles = Vector3.forward * 0;
            foreach (GameObject paragraph in paragraphContent)
            {
                paragraph.SetActive (true);
            }
        }
        else
        {
            dropDownButtonImage.gameObject.transform.eulerAngles = Vector3.forward * 90;
            foreach (GameObject paragraph in paragraphContent)
            {
                paragraph.SetActive (false);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate (scrollViewTransform);
    }
}
