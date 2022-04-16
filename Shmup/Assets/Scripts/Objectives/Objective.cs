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
        CaptureAndHold,
        FindTarget,
        Survival
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
    public int elimsRequired;
    public int elimsCurrent;
    public WeaponType weaponUsed;


    //Capture and Hold data
    [Header("Capture and Hold")]
    public Transform captureTarget;
    public int holdTime;
    public int holdTimeCurr;


    //Find Target
    public enum TargetType
    {
        Car,
        House,
        Person
    }
    [Header("Find Target")]
    public TargetType targetType;
    public Transform target;


    //Survival Data
    [Header("Survival")]
    public int survivalTime;
    public float survivalTimeCurr;


    private void Update()
    {
        if (objective == ObjectiveType.Survival && !isObjectiveComplete)
            survivalTimeCurr += Time.deltaTime;
    }


    private void Awake()
    {
        if (targetType == TargetType.Car)
            target = GameObject.FindGameObjectWithTag("CarTarget").GetComponent<Transform>();

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

