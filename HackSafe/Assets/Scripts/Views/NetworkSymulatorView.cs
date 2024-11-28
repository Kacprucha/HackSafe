using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NetworkSymulatorView : MonoBehaviour
{
    [SerializeField] RectTransform networkSpace;
    [SerializeField] GameObject nodePrefab;

    [SerializeField] Text networkTitle;

    public static string DefoultNetworkTitle = "\"Potato\"";

    protected static float minRadius = 120f;
    protected static float maxRadius = 125f;
    protected static float minDistanceBetweenObjects = 50f;

    public static string labelKey = "companyNameLabel_key";

    protected List<GameObject> elementsInNetworkSpace = new List<GameObject> ();

    // Start is called before the first frame update
    void Start ()
    {
        updateNetworkTitle ();
    }

    // Update is called once per frame
    void Update ()
    {
        
    }

    public void GenerateCommpanyLayOut ()
    {
        GameState gameState = GameState.instance;

        foreach (GameObject element in elementsInNetworkSpace)
        {
            Destroy (element);
        }

        elementsInNetworkSpace.Clear ();

        List<Vector2> spawnedPositions = new List<Vector2> ();

        foreach (Computer computer in gameState.ComapnysComputers)
        {
            Vector2 spawnPosition;
            bool positionIsValid;

            do
            {
                float angle = Random.Range (0f, Mathf.PI * 2f);

                float radius = Random.Range (minRadius, maxRadius);

                spawnPosition = new Vector2 (
                    networkSpace.transform.position.x + Mathf.Cos (angle) * radius,
                    networkSpace.position.y + Mathf.Sin (angle) * radius
                );

                positionIsValid = true;
                foreach (Vector2 existingPosition in spawnedPositions)
                {
                    if (Vector2.Distance (existingPosition, spawnPosition) < minDistanceBetweenObjects)
                    {
                        positionIsValid = false;
                        break;
                    }
                }

            } while (!positionIsValid);

            GameObject newNode = computer.IsMainComputer ? Instantiate (nodePrefab, networkSpace) : Instantiate (nodePrefab, spawnPosition, Quaternion.identity, networkSpace);
            spawnedPositions.Add (spawnPosition);

            Color color = computer.IsPasswordCracted ? Color.green : Color.red;
            color = computer.Password == "" ? Color.white : color;

            newNode.GetComponent<NetworkNodeView> ().Inicialize (computer, color);
            elementsInNetworkSpace.Add (newNode);

            updateNetworkTitle ();
        }
    }

    protected void updateNetworkTitle ()
    {
        if (GameState.instance != null)
        {
            GameState gameState = GameState.instance;
            string networkNameText = LocalizationManager.Instance.GetLocalizedValue (labelKey);

            string comapnyName = gameState.CompanyName;

            if (comapnyName.NullIfEmpty () == null)
                comapnyName = DefoultNetworkTitle;

            networkTitle.text = string.Format (networkNameText, gameState.CompanyName);
        }
    }
}
