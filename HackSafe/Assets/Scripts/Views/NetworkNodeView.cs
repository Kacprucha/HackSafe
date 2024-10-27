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

    private Computer computer;

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
        if (computer != null)
        {
            if (computer.IsPasswordCracted)
            {
                nodeImage.color = Color.green;
            }
        }
    }

    public void Inicialize (Computer computer, Color color)
    {
        this.computer = computer;

        nodeNameLabel.text = computer.Username;
        nodeIPLabel.text = "IP: " + computer.IP;
        this.bigNode = computer.IsMainComputer;

        if (!bigNode)
        {
            nodeImage.rectTransform.sizeDelta = new Vector2 (50, 50);
        }

        nodeImage.color = color;
    }
}
