using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public HealthBar healthBarHUD;
    public HealthBar healthBar;
    public World world;
    public Transform enemyContainerTransform;
    public Transform projectileContainerTransform;
    public MovementJoystick movementJoystick;
    public Button shootButton;
    public Rigidbody2D rb;
    public Animator animator;
    public Animator shieldAnimator;
    public Animator statusEffect_Net_Animator;
    public GameObject HUD;
    public GameObject playerHUD;
    public GameObject levelEndLosePanel;
    public AudioManager audioManager;
    public Camera cam;
    int health = 50;
    int shield = 3;
    float speed = 5;
    float speedSlowAmount;
    public Tilemap buildingTilemap;

    void Start()
    {
        healthBarHUD.SetMaxHealth(health);
        healthBarHUD.SetMaxShield(shield);
        healthBar.SetMaxHealth(health);
        healthBar.SetMaxShield(shield);
        levelEndLosePanel.SetActive(false);
    }

    void FixedUpdate() 
    {
        rb.MovePosition(rb.position + movementJoystick.joystickVec * speed * Time.fixedDeltaTime);

        if(movementJoystick.joystickVec != Vector2.zero)
        {
            rb.rotation = Mathf.LerpAngle(rb.rotation,
                                          Mathf.Atan2(movementJoystick.joystickVec.y, movementJoystick.joystickVec.x) * Mathf.Rad2Deg, 
                                          0.2f);
        }
    }

    public void ApplyStatus(string status, float[] statusArgs)
    {
        switch(status)
        {
            default:
                break;
            
            case "net":
                statusEffect_Net_Animator.SetBool("Netted", true);
                speedSlowAmount = speed * statusArgs[0];
                speed = speed - speedSlowAmount;
                break;
        }
    }

    public void TakeDamage(int damage) 
    {
        if(shield > 0)
        {
            --shield;
            if(shield == 0)
            {
                shieldAnimator.SetInteger("State", -1);
            }
            else
            {
                shieldAnimator.SetBool("Hit", true);
            }
            healthBar.UpdateShield(shield);
            healthBarHUD.UpdateShield(shield);
            return;
        }
        health -= damage;
        if(health <= 0)
        {
            health = 0;
            animator.SetBool("Alive", false);
        }
        healthBar.UpdateHealth(health);
        healthBarHUD.UpdateHealth(health);
    }

    public void OnDeathFinish()
    {
        foreach(MonoBehaviour obj in world.objectsToDisableOnLevelEnd)
        {
            obj.enabled = false;
        }
        levelEndLosePanel.SetActive(true);
        movementJoystick.GetComponent<EventTrigger>().enabled = false;
        movementJoystick.RevertToBasePosition();
        shootButton.interactable = false;
        audioManager.Play("Lose Music"); // Start game losing music
        foreach(Transform enemyTransform in enemyContainerTransform) // Enemy anim shift to happy
        {
            enemyTransform.gameObject.GetComponent<Animator>().SetBool("Happy", true);
        }
        foreach(Transform projectileTransform in projectileContainerTransform) // Free all projectiles
        {
            Destroy(projectileTransform.gameObject);
        }
        HUD.GetComponent<Animator>().SetInteger("Win", -1);// Hide HUD and show Level end lose panel
        playerHUD.GetComponent<Animator>().SetBool("Visible", false);// Hide player HUD
    }
    
    public List<Vector3Int> GetNearbyBuildings(float range)
    {
        range /= 10;
        List<Vector3Int> buildingTilesOnScreen = new List<Vector3Int>();
        float camHorizontalExtent = cam.orthographicSize * Screen.width/Screen.height * range;
        float camVerticalExtent = cam.orthographicSize * range;

        for(float posX = -camHorizontalExtent; posX <= camHorizontalExtent; posX++)
        {
            for(float posY = -camVerticalExtent; posY <= camVerticalExtent; posY++)
            {
                Vector3Int currentTilePos = buildingTilemap.WorldToCell(transform.position + new Vector3(posX, posY, 0));
                if(buildingTilemap.HasTile(currentTilePos) && !buildingTilesOnScreen.Contains(currentTilePos))
                {
                    buildingTilesOnScreen.Add(currentTilePos);
                }
            }
        }

        return buildingTilesOnScreen;
    }
    
    public void UpdateShield(int HUDshield)
    {
        shield = HUDshield;
        if(shield == 1)
        {
            shieldAnimator.SetInteger("State", 1);
        }
    }

    public void RestoreSlowedSpeed()
    {
        speed = speed + speedSlowAmount;
    }
}
