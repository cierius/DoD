using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script will be used in conjunction with DeathRecapScreen for displaying after run stats - May be used for achievement tracking down the line as well
public class GameStatTracker : MonoBehaviour
{
    public static GameStatTracker GST;

    public int enemyKills;
    public int damageDealt;

    public int healingGained;
    public int damageTaken;

    public int ammoPickUps;
    public int ammoSpent;

    //public List<Item>() itemsCollected = new List<Item>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Don't want this to be deleted since it tracks the whole run's stats
    }


}
