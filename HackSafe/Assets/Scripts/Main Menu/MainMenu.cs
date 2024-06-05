using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button startGameButton;

    // Start is called before the first frame update
    void Start()
    {
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener (LoadScene);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadScene ()
    {
        SceneManager.LoadScene ("Loading");
    }
}
