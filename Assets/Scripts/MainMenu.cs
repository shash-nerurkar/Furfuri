using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public AudioManager audioManager;
    public PersistAcrossScenes persistAcrossScenes;

    void Awake()
    {
        audioManager.Play("Main music");
        persistAcrossScenes = (PersistAcrossScenes)GameObject.FindObjectOfType(typeof(PersistAcrossScenes));
    }

    public void OnPlayButtonPressed()
    {
        persistAcrossScenes.transitionSceneToLoad = "Main";
        persistAcrossScenes.transitionSceneToDelete = "Main Menu";
        SceneManager.LoadScene("Transition", LoadSceneMode.Additive);
    }
    
    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}
