using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class will be used to manage the objectives during each run.
 * The Objective script will be the main way of handling the actual mechanics of the objective system.
 */


public class ObjectiveManager : MonoBehaviour
{
    private GameObject objectiveSlide;
    private List<TextMesh> objTexts = new List<TextMesh>();

    public List<Objective> objectivePool = new List<Objective>();
    [Tooltip("Selected Objectives")] public List<Objective> objectivesSelected = new List<Objective>();

    private CharStats playerWeaponEquipped;

    private AudioSource completionSound;

    public void Awake()
    {
        playerWeaponEquipped = GameObject.FindWithTag("Player").GetComponent<CharStats>();
        objectiveSlide = GameObject.FindWithTag("ObjectivesSlide");

        for(int i=1; i <= 8; i++) // Refs for ObjectiveSlide texts
            objTexts.Add(objectiveSlide.transform.GetChild(i).GetComponent<TextMesh>());

        objTexts.Reverse();

        objectivesSelected = RandomObjectiveSelection();

        completionSound = GetComponent<AudioSource>();
    }

    public void FixedUpdate()
    {
        CheckObjectivesCompleted(); // Checks to see if an objective has been completed yet
        UpdateObjectivesHUD(); // Updates the objective HUD display
    }


    private List<Objective> RandomObjectiveSelection() // Selects 4 random objectives from the pool
    {
        List<Objective> objList = new List<Objective>();

        for (int i = 0; i < 4; i++)
        {
            var rand = Random.Range(0, objectivePool.Count);
            var objInstance = Instantiate(objectivePool[rand], gameObject.transform);

            objList.Add(objInstance.GetComponent<Objective>());
        }

        return objList;
    }


    // Shows the objectives on the ObjectivesHUD
    private void UpdateObjectivesHUD()
    {
        int index = 0; // index 0 is the title Objectives

        foreach(var obj in objectivesSelected)
        {
            if (!obj.isObjectiveComplete)
            {
                objTexts[index].text = obj.CurrentObjective();
                index++;
                objTexts[index].text = obj.objectiveName;
                index++;
            }
            else
            {
                objTexts[index].text = "Complete";
                index++;
                objTexts[index].text = obj.objectiveName;
                index++;
            }
        }
    }


    private void CheckObjectivesCompleted()
    {
        foreach (Objective obj in objectivesSelected)
        {
            if (obj.CheckObjective() && !obj.isObjectiveComplete) // Returns a bool based on the objective type and completion status
            {
                completionSound.volume = Singleton.Instance.fxVol;
                completionSound.Play();

                obj.isObjectiveComplete = true;
                SpawnItem();
                print("Objective Finished - Spawning Item!");
            }
        }
    }


    private void SpawnItem() // Called when CheckObjectivesCompleted has a completed objective
    {
        var item = GameObject.FindWithTag("ItemPool").GetComponent<ItemPool>().RandomItem();
        var playerPos = GameObject.FindWithTag("Player").transform.position;

        var instItem = Instantiate(item);
        instItem.transform.position = new Vector2(playerPos.x, playerPos.y + 1);
    }


    public void Elimination() // Any elimination type objective will refer to this function
    {
        foreach (Objective obj in objectivesSelected)
        {
            if (obj.objective == Objective.ObjectiveType.Elimination && obj.weaponUsed == Objective.WeaponType.Any) // If any weapon can be used
            {
                obj.elimsCurrent++;
            }
            else if(obj.objective == Objective.ObjectiveType.Elimination && obj.weaponUsed != Objective.WeaponType.Any) // Specific weapon used for kills
            {
                switch (obj.weaponUsed)
                {
                    case Objective.WeaponType.Rifle:
                        if (playerWeaponEquipped.weaponEquipped.name == "Rifle")
                            obj.elimsCurrent++;
                        break;
                    case Objective.WeaponType.Shotgun:
                        if (playerWeaponEquipped.weaponEquipped.name == "Shotgun")
                            obj.elimsCurrent++;
                        break;
                    case Objective.WeaponType.Sniper:
                        if (playerWeaponEquipped.weaponEquipped.name == "Sniper")
                            obj.elimsCurrent++;
                        break;
                    case Objective.WeaponType.Launcher:
                        if (playerWeaponEquipped.weaponEquipped.name == "Launcher")
                            obj.elimsCurrent++;
                        break;
                }
            }
        }
    }
}
