using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    public List<GameObject> pool = new List<GameObject>();


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    public GameObject RandomItem() // Returns a random item from the pool
    {
        return pool[Random.Range(0, pool.Count)];
    }

    public void RemoveItem(GameObject item) // Removes an item from the pool
    {
        pool.Remove(item);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightControl))
            SpawnItem();
    }

    private void SpawnItem() // DEBUG PURPOSES / CHEATING FOR FUN
    {
        var item = RandomItem();
        var playerPos = GameObject.FindWithTag("Player").transform.position;

        var instItem = Instantiate(item);
        instItem.transform.position = new Vector2(playerPos.x, playerPos.y + 1);
    }
}
