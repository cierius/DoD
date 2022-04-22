using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathRecapScreen : MonoBehaviour
{

    public void Awake() // Finds the main camera and sets the canvas size to it
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void OnRestart() // Restart Run button
    {
        Singleton.Instance.ResetRun();
    }


    public void OnMainMenu() // Main menu button
    {
        Singleton.Instance.ReturnToMenu();
    }
}
