using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HUD : MonoBehaviour
{
    public GameObject pausePanel;
    public Animator animator;
    public AudioManager audioManager;
    public Image muteButtonImage;
    public TMP_Text time;
    public TMP_Text score;
    public Coroutine timeUpdateCoroutine;
    public Sprite mutedButtonSprite;
    public Sprite unmutedButtonSprite;
    public bool muted;

    void Start()
    {
        if(muted)
        {
            muteButtonImage.sprite = mutedButtonSprite;
            foreach(Sound sound in audioManager.sounds)
            {
                sound.audioSource.Pause();
                sound.audioSource.volume = 0;
            }
        }
        else
        {
            muteButtonImage.sprite = unmutedButtonSprite;
            foreach(Sound sound in audioManager.sounds)
            {
                sound.audioSource.Play();
                sound.audioSource.volume = 1;
            }
        }
        timeUpdateCoroutine = StartCoroutine(UpdateTime());
    }

    IEnumerator UpdateTime()
    {
        string[] timeParts = time.text.Split(':');
        int currentTime = int.Parse(timeParts[0]) * 60 + int.Parse(timeParts[1]);
        while(true)
        {
            yield return new WaitForSeconds(1f);
            ++currentTime;
            time.text = (currentTime/60 < 10 ? ("0" + (currentTime/60).ToString()) : (currentTime/60).ToString())
                      + ":"
                      + (currentTime%60 < 10 ? ("0" + (currentTime%60).ToString()) : (currentTime%60).ToString());
        }
    } 

    public void OnPauseButtonPressed()
    {
        audioManager.Play("Button Click");
        StopCoroutine(timeUpdateCoroutine);
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }
    
    public void OnMuteButtonToggled()
    {
        if(muted)
        {
            muteButtonImage.sprite = unmutedButtonSprite;
            foreach(Sound sound in audioManager.sounds)
            {
                sound.audioSource.Play();
                sound.audioSource.volume = 1;
            }
        }
        else
        {
            muteButtonImage.sprite = mutedButtonSprite;
            foreach(Sound sound in audioManager.sounds)
            {
                sound.audioSource.Pause();
                sound.audioSource.volume = 0;
            }
        }
        muted = !muted;
    }

    public void OnUnpauseButtonPressed()
    {
        audioManager.Play("Button Click");
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        timeUpdateCoroutine = StartCoroutine(UpdateTime());
    }

    public void OnExitToMenuButtonPressed()
    {
        audioManager.Play("Button Click");
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public void OnNextLevelButtonPressed()
    {
        // ADD LEVEL CHANGE LOGIC
        audioManager.Play("Button Click");
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public void OnRestartLevelButtonPressed()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void OnHUDHideFinished()
    {
        if(animator.GetBool("IsWinning"))
        {
            animator.SetInteger("Win", 1);
        }
        else
        {
            animator.SetInteger("Win", -1);
        }
    }
}