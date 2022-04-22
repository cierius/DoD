using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public string objectiveName;
    public enum ObjectiveType
    {
        Null,
        Elimination,
        DamageDealt,
        CaptureAndHold,
        FindTarget,
        Survival,
        SearchAndDestroy
    }
    public enum WeaponType
    {
        Any,
        Rifle,
        Shotgun,
        Sniper,
        Launcher
    }
    public ObjectiveType objective;

    public bool isObjectiveComplete;


    //Elim data
    [Header("Elimination")]
    [Space(10)]
    [Range(0, 50)] public int elimsRequired;
    public int elimsCurrent;
    public WeaponType weaponUsed;

    //Damage dealt objective may also use the variable above
    [Header("Damage Dealt")]
    [Space(10)]
    [Range(0, 1000)] public int damageRequired;
    public int damageDealt;

    //Capture and Hold data
    [Header("Capture and Hold")]
    [Space(10)]
    public Transform captureTarget;
    [Range(0, 60)] public int holdTime;
    public int holdTimeCurr;


    //Find Target
    public enum TargetType
    {
        Car,
        House,
        Misc
    }
    [Header("Find Target")]
    [Space(10)]
    public TargetType targetType;
    public Transform target;


    //Survival Data
    [Header("Survival")]
    [Space(10)]
    [Range(0, 300)] public int survivalTime;
    public float survivalTimeCurr;


    //SnD Data
    [Header("Search and Destroy")]
    [Space(10)]
    public bool multipleObjects = false;
    public List<GameObject> objectsToDestroy;


    private void Update()
    {
        if (objective == ObjectiveType.Survival && !isObjectiveComplete)
            survivalTimeCurr += Time.deltaTime;
    }


    private void Awake()
    {
        // If find target objective
        List<GameObject> cars = new List<GameObject>();
        cars.AddRange(GameObject.FindGameObjectsWithTag("CarTarget"));

        if (targetType == TargetType.Car) // Need to put a circle around the target or something
        {
            target = cars[Random.Range(0, cars.Count)].GetComponent<Transform>();

            print(target);
        }
            

        RandomifyElimObjectiveReqs();
    }


    // Adds a bit of randomness to the objective requirements
    private void RandomifyElimObjectiveReqs()
    {
        if(objective == ObjectiveType.Elimination)
        {
            elimsRequired += Random.Range(-2, 5);
        }

    }


    // Called by the objective manager script to see if the objective has been completed
    public bool CheckObjective()
    {
        switch(objective)
        {
            case ObjectiveType.Null: return false;

            case ObjectiveType.Elimination:
                if (elimsCurrent >= elimsRequired)
                    return true;
                else
                    return false;

            case ObjectiveType.FindTarget:
                if (Mathf.Abs(Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, target.transform.position)) < 1.5f)
                    return true;
                else
                    return false;

            case ObjectiveType.Survival:
                if (survivalTimeCurr > survivalTime)
                    return true;
                else
                    return false;

        }
        return false;
    }


    // Returns a string value for the status of the objective
    public string CurrentObjective()
    {
        switch (objective)
        {
            case ObjectiveType.Null: return "Null";

            case ObjectiveType.Elimination:
                if (elimsCurrent >= elimsRequired) // Win Condition
                    return "COMPLETE";
                else
                    return elimsCurrent + "/" + elimsRequired;

            case ObjectiveType.FindTarget:
                if (Mathf.Abs(Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, target.transform.position)) < 1.5f) // Win Condition
                    return "COMPLETE";
                else
                    return "Keep Searchin'";

            case ObjectiveType.Survival:
                if (survivalTimeCurr > survivalTime)
                    return "COMPLETE";
                else
                    return Mathf.Round(survivalTimeCurr) + "/" + survivalTime;

        }
        return "Null";
    }
}

