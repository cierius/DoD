using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is a very simple spawner handling currently only 1 enemy type
 */


public class EnemySpawnerScript : MonoBehaviour
{
    [Header("Enemy Types To Spawn")]
    public List<GameObject> enemyTypes; // Enemies that could be spawned - Currently only 1 enemy type available
    
    [Header("Spawn Timing & Position")]
    [Range(1f, 10f)] public float minSpawnDelay = 2f;
    [Range(1f, 30f)] public float maxSpawnDelay = 5f;
    private float spawnTimer = 0;
    private float randSpawnDelay = 0; // Random number between the min and max delay

    [Range(1f, 5f)] public float spawnRadius = 1f;
    private Vector2 randSpawnPos;
    public bool showSpawnRadius = false;

    [Header("Total Enemies To Spawn - 0 Is Infinite")]
    [Range(0, 100)] public int totalEnemiesToSpawn = 0; // 0 = never ending
    private int spawnCounter = 0;
    public bool destroyAfterSpawnCompletion = false;

    [Header("Runes")]
    [SerializeField] private bool requireRunes = true; // If false then the spawner can't be destroyed by player, NEEDS TO BE IMPLEMENTED STILL
    public List<GameObject> runes = new List<GameObject>();

    private void OnDrawGizmos()
    {
        if(showSpawnRadius)
        {
            Gizmos.color = Color.black;

            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }


    private void Update()
    {
        if (totalEnemiesToSpawn != 0 && spawnCounter < totalEnemiesToSpawn && runes.Count > 0 || totalEnemiesToSpawn == 0 && runes.Count > 0)
        {
            if (randSpawnDelay == 0) // Sets the random delay and the random position for the next enemy spawn
            {
                randSpawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
                print(randSpawnDelay);
                randSpawnPos = new Vector2(transform.position.x + Random.Range(-spawnRadius, spawnRadius), transform.position.y + Random.Range(-spawnRadius, spawnRadius));
            }

            if (spawnTimer >= randSpawnDelay)
            {
                randSpawnDelay = 0;
                spawnTimer = 0;
                spawnCounter++;

                var spawnedEnemy = Instantiate(enemyTypes[0], GameObject.Find("AIManager").transform);
                spawnedEnemy.transform.position = randSpawnPos;

                GameObject.Find("AIManager").GetComponent<AIManager>().RefreshEnemyList(); // Could just add the enemy to the list instead of updating the whole list - this works for now
            }

            spawnTimer += Time.deltaTime;
        }
        else if(destroyAfterSpawnCompletion || runes.Count < 1)
        {
            Destroy(gameObject, 1f);
        }

        CheckRunes();
    }


    private void CheckRunes()
    {
        if(runes.Count > 0)
        {
            foreach(GameObject rune in runes)
            {
                if(rune == null)
                {
                    runes.Remove(rune);
                }
            }
        }
    }
}
