using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public AudioManager audioManager;
    public Animator animator;
    public TMP_Text loadingText;
    private Coroutine updateLoadingText;
    public PersistAcrossScenes persistAcrossScenes;
    
    void Awake()
    {
        persistAcrossScenes = (PersistAcrossScenes)GameObject.FindObjectOfType(typeof(PersistAcrossScenes));
    }

    IEnumerator UpdateLoadingText()
    {
        StartCoroutine(PauseLoadingAnimation());
        
        string originalText = loadingText.text;
        string dotText = ".";

        while(true)
        {
            loadingText.text = originalText + dotText;
            switch(dotText)
            {
                case ".":
                    dotText += ".";
                    break;
                
                case "..":
                    dotText += ".";
                    break;
                
                case "...":
                    dotText = ".";
                    break;
            }
            yield return new WaitForSeconds(0.6f);
        }

    }

    void OnLoadingScreenConverge()
    {
        audioManager.Play("Loading Thud");
        
        updateLoadingText = StartCoroutine(UpdateLoadingText());

        SceneManager.UnloadSceneAsync(persistAcrossScenes.transitionSceneToDelete);

    }

    IEnumerator PauseLoadingAnimation()
    {
        animator.speed = 0;

        yield return new WaitForSeconds(2);
        
        yield return SceneManager.LoadSceneAsync(persistAcrossScenes.transitionSceneToLoad, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(persistAcrossScenes.transitionSceneToLoad));
        animator.speed = 1;
        StopCoroutine(updateLoadingText);
    }

    void OnLoadingScreenDiverge()
    {
        SceneManager.UnloadSceneAsync("Transition");
    }
}
