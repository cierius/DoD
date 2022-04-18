using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollHUD : MonoBehaviour
{

    private SpriteRenderer rollHUDIcon;
    private SpriteRenderer rollHUDOutline;

    private bool onCooldown = false;

    private void Awake()
    {
        rollHUDIcon = GameObject.Find("Roll_Icon").GetComponent<SpriteRenderer>();
        rollHUDOutline = GameObject.Find("Roll_Icon_Outline").GetComponent<SpriteRenderer>();
    }


    public void CooldownStart(float dur)
    {
        if(!onCooldown)
            StartCoroutine(RollHUDFade(dur));
    }


    IEnumerator RollHUDFade(float dur) // Coroutine Function for fading in the rollHUDIcon based on its cooldown
    {
        float elapsedTime = 0f;
        float totalDur = dur;

        onCooldown = true;

        rollHUDOutline.enabled = false;
        rollHUDIcon.color = new Color(rollHUDIcon.color.r, rollHUDIcon.color.g, rollHUDIcon.color.b, 0f);

        Color c = rollHUDIcon.color;

        while (elapsedTime <= totalDur)
        {
            elapsedTime += Time.deltaTime;
            c.a = 1f * (elapsedTime / totalDur); // Transitions between 0f and 1f
            rollHUDIcon.color = c;
            yield return null;
        }

        c.a = 1f; // Since the transition doesn't fully get to 1f make it equal 1f
        rollHUDIcon.color = c;
        rollHUDOutline.enabled = true;

        onCooldown = false; 
    }
}
