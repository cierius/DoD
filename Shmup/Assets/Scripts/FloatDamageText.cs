using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatDamageText : MonoBehaviour
{
    [SerializeField] private float maxTime = 2f; // Time in seconds for the floating damage text to be alive
    private float timer = 0;

    [Range(0f, 1f)]
    [SerializeField] private float force = 1f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer > maxTime)
            Destroy(gameObject);
    }

    public void SetText(float value, bool hasCrit)
    {
        if(hasCrit)
        {
            GetComponent<TextMesh>().color = new Color(255, 0, 90);
            GetComponent<TextMesh>().text = Mathf.Ceil(value).ToString() + "!";
            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            timer = 0;
        }
        else
        {
            GetComponent<TextMesh>().text = Mathf.Ceil(value).ToString();
            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            timer = 0;
        }     
    }
}
