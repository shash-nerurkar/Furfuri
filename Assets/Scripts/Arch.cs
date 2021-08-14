using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Arch : MonoBehaviour
{
    public World world;
    public GameObject levelEndWinPanel;
    public Animator animator;
    public Animator playerAnimator;
    public Animator HUDAnimator;
    public Animator playerHUDAnimator;
    public MovementJoystick playerMovementJoystick;
    public Button playerShootButton;
    public AudioManager audioManager;
    public Transform enemyContainerTransform;
    public Transform projectileContainerTransform;

    void ArchEndTriggerStatus(bool isEndTrigger)
    {
        if(!isEndTrigger)
        {
            Destroy(gameObject.GetComponent<BoxCollider2D>());
        }
        else
        {
            animator = gameObject.GetComponent<Animator>();
            world = (World)FindObjectOfType(typeof(World));

            levelEndWinPanel = GameObject.Find("Level End Win Panel");
            levelEndWinPanel.SetActive(false);

            playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
            HUDAnimator = GameObject.Find("HUD").GetComponent<Animator>();
            playerHUDAnimator = GameObject.Find("Player HUD").GetComponent<Animator>();
            playerMovementJoystick = (MovementJoystick)FindObjectOfType(typeof(MovementJoystick));
            Transform[] transforms = (Transform[])FindObjectsOfType(typeof(Transform));
            foreach(Transform transform in transforms)
            {
                switch(transform.gameObject.name)
                {
                    case "Shoot Button":
                        playerShootButton = transform.gameObject.GetComponent<Button>();
                        break;
                    
                    case "Enemy Container":
                        enemyContainerTransform = transform;
                        break;
                    
                    case "Projectile Container":
                        projectileContainerTransform = transform;
                        break;
                }
            }
            audioManager = (AudioManager)FindObjectOfType(typeof(AudioManager));
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            foreach(MonoBehaviour obj in world.objectsToDisableOnLevelEnd)
            {
                obj.enabled = false;
            }
            levelEndWinPanel.SetActive(true);
            playerMovementJoystick.GetComponent<EventTrigger>().enabled = false;
            playerMovementJoystick.RevertToBasePosition();
            playerShootButton.interactable = false;
            animator.SetBool("Entered", true); // Arch animation
            audioManager.Play("Win Music"); // Start game winning music
            foreach(Transform childTransform in enemyContainerTransform) // Enemies anim to sad
            {
                childTransform.gameObject.GetComponent<Animator>().SetBool("Sad", true);
            }
            foreach(Transform childTransform in enemyContainerTransform) // Free all projectiles
            {
                Destroy(childTransform.gameObject);
            }
            HUDAnimator.SetInteger("Win", 1); // Hide HUD and Show level end win panel
            playerAnimator.SetBool("Won", true); // Player anim shift to happy and Silence player status effects
            playerHUDAnimator.SetBool("Visible", false);// Hide player HUD
        }
    }
}