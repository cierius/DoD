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
    private TextMesh[] objTexts = new TextMesh[10];

    public List<Objective> objectives = new List<Objective>();
    private int objectiveCount;

    private CharStats playerWeaponEquipped;

    public void Awake()
    {
        playerWeaponEquipped = GameObject.FindWithTag("Player").GetComponent<CharStats>();

        objectiveSlide = GameObject.FindWithTag("ObjectivesSlide");
        for(int i=0; i<objTexts.Length; i++)
        {
            objTexts[i] = objectiveSlide.transform.GetChild(i).GetComponent<TextMesh>();
        }

        UpdateObjectiveList();
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        CheckObjectivesCompleted(); // Checks to see if an objective has been completed yet
        UpdateObjectivesHUD(); // Updates the objective HUD display
    }


    // Finds and adds objectives to a list
    public void UpdateObjectiveList()
    {
        objectives.Clear();
        objectiveCount = 0;

        var objs = GameObject.FindGameObjectsWithTag("Objective");

        foreach(var obj in objs)
        {
            objectives.Add(obj.GetComponent<Objective>());
            objectiveCount++;
        }

        UpdateObjectivesHUD();
    }


    // Shows the objectives on the ObjectivesHUD
    private void UpdateObjectivesHUD()
    {
        int index = 1; // index 0 is the title Objectives

        foreach(var obj in objectives)
        {
            if (!obj.isObjectiveComplete)
            {
                objTexts[index].text = obj.objectiveName;
                index++;
                objTexts[index].text = obj.CurrentObjective();
                index++;
            }
            else
            {
                objTexts[index].text = obj.objectiveName;
                index++;
                objTexts[index].text = "Complete";
                index++;
            }
        }
    }


    private void CheckObjectivesCompleted()
    {
        if (objectiveCount > 0)
        {
            foreach (Objective obj in objectives)
            {
                if (obj.CheckObjective() && !obj.isObjectiveComplete) // Returns a bool based on the objective type and completion status
                {
                    obj.isObjectiveComplete = true;
                    SpawnItem();
                    print("Objective Finished - Spawning Item!");
                }
            }
        }
    }


    private void SpawnItem()
    {
        var item = GameObject.FindWithTag("ItemPool").GetComponent<ItemPool>().RandomItem();
        var playerPos = GameObject.FindWithTag("Player").transform.position;

        var instItem = Instantiate(item);
        instItem.transform.position = new Vector2(playerPos.x, playerPos.y + 1);
    }


    public void Elimination() // Any elimination type objective will refer to this function
    {
        foreach (Objective obj in objectives)
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
