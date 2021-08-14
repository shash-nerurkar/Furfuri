using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Player player;
    public Slider healthBarSlider;
    public Slider shieldBarSlider;
    float shieldLerpDuration = 5;
    float healthLerpDuration = 3;
    public bool isHUDElement;
    Coroutine  shieldRegenCoroutine;
    
    public void SetMaxHealth(int maxHealth)
    {
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = maxHealth;
    }

    public void SetMaxShield(int maxShield)
    {
        shieldBarSlider.maxValue = maxShield;
        shieldBarSlider.value = maxShield;
    }

    public void UpdateHealth(int health)
    {
        StartCoroutine(LerpStat(healthBarSlider, health, healthLerpDuration));
    }

    public void UpdateShield(int shield)
    {
        if(shieldRegenCoroutine != null)
        {
            StopCoroutine(shieldRegenCoroutine);
        }

        shieldBarSlider.value = shield;// StartCoroutine(LerpStat(shieldBarSlider, shield, shieldLerpDuration));

        shieldRegenCoroutine = StartCoroutine(LerpStat(shieldBarSlider, shield + 1, shieldLerpDuration));
    }

    IEnumerator LerpStat(Slider slider, int stat, float lerpDuration)
    {
        float timeElapsed = 0;
        float start_value = slider.value;
        
        while (timeElapsed < lerpDuration)
        {
            slider.value = Mathf.Lerp(start_value, stat, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            
            yield return null;
        }

        slider.value = stat;

        if(slider == shieldBarSlider)
        {
            if(isHUDElement)
            {
                player.UpdateShield(stat);
            }
            if(stat < slider.maxValue)
            {
                shieldRegenCoroutine = StartCoroutine(LerpStat(shieldBarSlider, stat + 1, shieldLerpDuration)); 
            }
        }
    }
}
