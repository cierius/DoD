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
        Singleton.Instance.ResetLevel();
    }


    public void OnMainMenu()
    {
        Singleton.Instance.ReturnToMenu();
    }
}
