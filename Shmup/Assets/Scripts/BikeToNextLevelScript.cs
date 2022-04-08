using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeToNextLevelScript : MonoBehaviour
{

    public void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.transform.tag == "Player")
        {
            Singleton.Instance.NextLevel();
        }
    }
}
