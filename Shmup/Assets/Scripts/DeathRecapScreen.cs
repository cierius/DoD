using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathRecapScreen : MonoBehaviour
{

    public void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void OnRestart()
    {
        print("Loading Level_1");
        SceneManager.LoadScene(1);
    }


    public void OnMainMenu()
    {
        print("Loading Main Menu");
        SceneManager.LoadScene("Menu");
    }
}
