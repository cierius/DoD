using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    
    public void OnDestroy()
    {
        SaveSettings();
    }

    public void OnMusicSliderChange()
    {

    }

    public void OnEffectsSliderChange()
    {

    }

    public void OnFPSChange()
    {
        
    }

    public void OnControls()
    {
        print("Showing controls");
    }

    public void OnResetRun()
    {
        print("Resetting run");
        Time.timeScale = 1;
        Singleton.Instance.ResetLevel();
    }

    public void OnExitRun()
    {
        print("Exitting run");
        Time.timeScale = 1;
        Singleton.Instance.ReturnToMenu();
    }

    private void SaveSettings()
    {

    }

    private void LoadSettings()
    {

    }
}
