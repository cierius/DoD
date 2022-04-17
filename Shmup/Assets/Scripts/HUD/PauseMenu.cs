using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    private Slider musicVolValue;
    private Slider fxVolValue;
    private InputField fpsLimitValue;


    private void Awake()
    {
        musicVolValue = GameObject.Find("Canvas/MusicVolumeSlider").GetComponent<Slider>();
        fxVolValue = GameObject.Find("Canvas/EffectsVolumeSlider").GetComponent<Slider>();
        fpsLimitValue = GameObject.Find("Canvas/FramerateInput").GetComponent<InputField>();

        LoadSettings();
    }


    public void OnDestroy()
    {
        SaveSettings();
    }

    public void OnResume()
    {
        print("Resuming Game");
        Time.timeScale = 1;
        GameObject.FindWithTag("Player").GetComponent<CharController>().isPaused = false;
        Destroy(gameObject);
    }


    public void OnMusicSliderChange()
    {
        Singleton.Instance.musicVol = musicVolValue.value;
        MusicManager.Instance.SetVolume(musicVolValue.value);
    }

    public void OnEffectsSliderChange()
    {
        Singleton.Instance.fxVol = fxVolValue.value;
    }

    public void OnFPSChange()
    {
        fpsLimitValue.text = Mathf.Clamp(int.Parse(fpsLimitValue.text), 15, 999).ToString();
        Singleton.Instance.frameRateLimit = Mathf.Clamp(int.Parse(fpsLimitValue.text), 20, 999);
        Application.targetFrameRate = Mathf.Clamp(int.Parse(fpsLimitValue.text), 20, 999);
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
        Singleton.Instance.SaveAllSettings();
    }

    private void LoadSettings()
    {
        musicVolValue.value = Singleton.Instance.musicVol;
        fxVolValue.value = Singleton.Instance.fxVol;
        fpsLimitValue.text = Singleton.Instance.frameRateLimit.ToString();
    }
}
