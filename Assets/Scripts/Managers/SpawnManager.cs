using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private int maxEnemyCount;
    [SerializeField] private GameObject enemyTypes;
    [SerializeField] private float spawnRadius;

    private int currentEnemyCount;
    
    Vector3 GetPointInCircle()
    {
        float angle = Random.Range(0f, 360f);
        float radius = Random.Range(0f, spawnRadius);

        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        return new Vector3(cos + sin, 0f, cos - sin) * radius;
    }

    public void RemoveEnemy(string id)
    {
        if (id == "Soldier")
        {
            currentEnemyCount -= 1;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
