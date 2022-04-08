using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlideBehavior : MonoBehaviour
{
    [SerializeField] private Transform onscreenPos;
    [SerializeField] private Transform offscreenPos;

    private bool moveOnScreen = false;

    public float lerpSpeed = 5f;


    void Update()
    {
        if (moveOnScreen)
        {
            MoveOnScreen();
        }
        else
        {
            MoveOffScreen();
        }
    }


    public void Toggle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!moveOnScreen)
            {
                moveOnScreen = true;
            }
            else
            {
                moveOnScreen = false;
            }
        }
    }


    private void MoveOnScreen()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, onscreenPos.transform.position.x, lerpSpeed * Time.deltaTime), transform.position.y, transform.position.z);
    }

    private void MoveOffScreen()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, offscreenPos.transform.position.x, lerpSpeed * Time.deltaTime), transform.position.y, transform.position.z);
    }
}
