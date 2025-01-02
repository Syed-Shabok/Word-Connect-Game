using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{   
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioSource audioSource;

    
    public void WordConnectMenuButton()
    {
        audioSource.clip = clickSound;
        audioSource.Play();

        StartCoroutine(LoadSceneAfterSound(1, clickSound.length));
    }
    
    public void WordConnectPlayButton()
    {
        audioSource.clip = clickSound;
        audioSource.Play();

        StartCoroutine(LoadSceneAfterSound(2, clickSound.length));
    }

    private IEnumerator LoadSceneAfterSound(int sceneNumber, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneNumber);
    }

    public void LobbyButton()
    {
        audioSource.clip = clickSound;
        audioSource.Play();

        StartCoroutine(LoadSceneAfterSound(0, clickSound.length));
    }

    public void WordSearch()
    {
        audioSource.clip = clickSound;
        audioSource.Play();
        
        Debug.Log("Word-Search game button was clicked.");
    }
    public void FourLetters()
    {
        audioSource.clip = clickSound;
        audioSource.Play();
        
        Debug.Log("Four-Letters game button was clicked.");
    }

    public void WordHunt()
    {
        audioSource.clip = clickSound;
        audioSource.Play();
        
        Debug.Log("Word-Hunt game button was clicked.");
    }
}
