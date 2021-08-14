using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioManager audioManager;

    void Awake()
    {
        audioManager.Play("Main music");
    }

    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    
    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}
