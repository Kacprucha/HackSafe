using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkNodeView : MonoBehaviour
{
    [SerializeField] Image nodeImage;
    [SerializeField] Text nodeNameLabel;
    [SerializeField] Text nodeIPLabel;

    [SerializeField] Button nodeButton;

    public Button NodeButton
    {
        get
        {
            return nodeButton;
        }
    }

    protected bool bigNode = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Inicialize (string nodeName, string nodeIP, bool bigNode, Color color)
    {
        nodeNameLabel.text = nodeName;
        nodeIPLabel.text = nodeIP;
        this.bigNode = bigNode;

        if (!bigNode)
        {
            nodeImage.rectTransform.sizeDelta = new Vector2 (50, 50);
        }

        nodeImage.color = color;
    }
}
