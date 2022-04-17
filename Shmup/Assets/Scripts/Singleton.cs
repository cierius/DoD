using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*  SINGLETON
 * The purpose of this script is as follows:
 * - Switch between scenes
 * - Keep character inventory and stats persistent between scenes
 * - Manage audio & video settings
 * - Manage playerInput state (controller / keyboard + mouse)
 * 
*/

public class Singleton : MonoBehaviour
{
    public static Singleton Instance { get; private set; } = null;

    public bool isResetting = false;

    // PlayerInput State
    public bool isUsingController = false; // Keyboard + mouse is default


    // Settings
    public int frameRateLimit = 120; // Default is 120 (60fps doesn't feel smooth enough)
    public float musicVol = 1.0f; // Default is 1.0f
    public float fxVol = 1.0f; // Default is 1.0f


    public List<GameObject> playerInventory = new List<GameObject>();
    private CharStatsContainer persistentStats = new CharStatsContainer();


    private void Awake()
    {
        // Singleton method, only one instance
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        LoadSettings();
        ApplySettings();
    }


    public void SaveSetting(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        print("Set " + key + " : " + value);
        PlayerPrefs.Save();
    }

    public void SaveSetting(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        print("Set " + key + " : " + value);
        PlayerPrefs.Save();
    }

    public void SaveSetting(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        print("Set " + key + " : " + value);
        PlayerPrefs.Save();
    }

    public void SaveAllSettings()
    {
        print("Saving Settings To Disk");
        PlayerPrefs.SetFloat("musicVol", musicVol);
        PlayerPrefs.SetFloat("fxVol", fxVol);
        PlayerPrefs.SetInt("frameRateLimit", frameRateLimit);
        PlayerPrefs.Save();
    }


    public void LoadSettings()
    {
        print("Loading Settings");
        frameRateLimit = PlayerPrefs.GetInt("frameRateLimit");
        musicVol = PlayerPrefs.GetFloat("musicVol");
        fxVol = PlayerPrefs.GetFloat("fxVol");
    }


    public void ApplySettings() // Sets the data for the settings
    {
        Application.targetFrameRate = frameRateLimit;
    }

    public void SwitchControlScheme()
    {
        if(isUsingController)
        {
            isUsingController = false;
        }
        else
        {
            isUsingController = true;
        }
        print("Using Controller: " + isUsingController);
    }


    public void ResetLevel()
    {
        print("Resetting Level");
        var currLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currLevel.buildIndex, LoadSceneMode.Single);
    }

    public void ReturnToMenu()
    {
        print("Returning To Menu");
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void NextLevel()
    {
        print("Switching To Next Level");
        SavePlayerStats();
        var currLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currLevel.buildIndex + 1, LoadSceneMode.Single);
    }


    public void SavePlayerStats() // Saves the current CharStats to a container
    {
        var charStats = GameObject.FindWithTag("Player").GetComponent<CharStats>();

        persistentStats.speedAdditive = charStats.speedAdditive;
        persistentStats.healthAdditive = charStats.healthAdditive;
        persistentStats.healthCurrent = charStats.healthCurrent;
        persistentStats.healthRegen = charStats.healthRegen;
        persistentStats.damageAdditive = charStats.damageAdditive;
        persistentStats.fireRatePercentage = charStats.fireRatePercentage;
        persistentStats.reloadSpeedPercentage = charStats.reloadSpeedPercentage;
        persistentStats.critMultiplierAdditive = charStats.critMultiplierAdditive;
        persistentStats.critChanceAdditive = charStats.critChanceAdditive;

        persistentStats.weapons = charStats.weapons;
        persistentStats.ammoInMag = charStats.ammoInMag;
        persistentStats.magsInInventory = charStats.magsInInventory;
        persistentStats.magCarryMax = charStats.magCarryMax;

        persistentStats.weaponEquipped = charStats.weaponEquipped;
        persistentStats.currWeaponIndex = charStats.currWeaponIndex;

        persistentStats.firstLoad = false;
    }

    public void LoadPlayerStats() // Loads all the CharStats variables to keep the stats persistent
    {
        var charStats = GameObject.FindWithTag("Player").GetComponent<CharStats>(); 

        charStats.speedAdditive = persistentStats.speedAdditive;
        charStats.healthAdditive = persistentStats.healthAdditive;
        charStats.healthCurrent = persistentStats.healthCurrent;
        charStats.healthRegen = persistentStats.healthRegen;
        charStats.damageAdditive = persistentStats.damageAdditive;
        charStats.fireRatePercentage = persistentStats.fireRatePercentage;
        charStats.reloadSpeedPercentage = persistentStats.reloadSpeedPercentage;
        charStats.critMultiplierAdditive = persistentStats.critMultiplierAdditive;
        charStats.critChanceAdditive = persistentStats.critChanceAdditive;

        charStats.weapons = persistentStats.weapons;
        charStats.ammoInMag = persistentStats.ammoInMag;
        charStats.magsInInventory = persistentStats.magsInInventory;
        charStats.magCarryMax = persistentStats.magCarryMax;

        charStats.weaponEquipped = persistentStats.weaponEquipped;
        charStats.currWeaponIndex = persistentStats.currWeaponIndex;
    }

    public void SavePlayerInventory()
    {

    }

    public void LoadPlayerInventory()
    {

    }

    public bool GetFirstLoad()
    {
        return persistentStats.firstLoad;
    }
}

class CharStatsContainer
{
    public float speedAdditive = 0f; // Used for speed boosts / bonus speed upgrades

    public float healthCurrent;
    public float healthAdditive = 0f;
    public float healthRegen = 0f; // Hp regen per sec (ticks 4 times per sec)
    public float damageReductionPercentage = 0f;

    public float damageAdditive = 0f;
    public float fireRatePercentage = 0;
    public float reloadSpeedPercentage = 0f;
    public int critMultiplierAdditive = 0;
    public int critChanceAdditive = 0;

    public List<WeaponBase> weapons = new List<WeaponBase>();
    public float[] weaponLastFired = new float[4] { 0f, 0f, 0f, 0f };
    public int[] ammoInMag = new int[] { 0, 0, 0, 0 }; 
    public int[] magsInInventory = new int[] { 0, 0, 0, 0 };
    public int[] magCarryMax = new int[] { 0, 0, 0, 0 };

    public WeaponBase weaponEquipped;
    public int currWeaponIndex = 0; // 0 is rifle

    public bool firstLoad = true;
}