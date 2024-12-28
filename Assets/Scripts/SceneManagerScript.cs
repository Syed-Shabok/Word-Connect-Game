using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WordConnectPlayButton()
    {
        SceneManager.LoadScene(2);
    }

    public void WordConnectMenuButton()
    {
        SceneManager.LoadScene(1);
    }

    public void WordConnect()
    {
        SceneManager.LoadScene(1);
    }

    public void LobbyButton()
    {
        SceneManager.LoadScene(0);
    }

    public void WordSearch()
    {
        Debug.Log("Word-Search game button was clicked.");
    }
    public void FourLetters()
    {
        Debug.Log("Four-Letters game button was clicked.");
    }

    public void WordHunt()
    {
        Debug.Log("Word-Hunt game button was clicked.");
    }
}
