using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSymulatorView : MonoBehaviour
{
    [SerializeField] RectTransform networkSpace;
    [SerializeField] GameObject nodePrefab;

    protected List<GameObject> elementsInNetworkSpace = new List<GameObject> ();

    protected static float minRadius = 120f;
    protected static float maxRadius = 125f;
    protected static float minDistanceBetweenObjects = 50f;

    // Start is called before the first frame update
    void Start ()
    {

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
        }
    }
}
