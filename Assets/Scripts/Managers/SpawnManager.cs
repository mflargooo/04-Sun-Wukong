using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private int baseTotalEnemyCount;
    // [SerializeField] private int[] maxEnemyTypeCount;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float minDistBetweenSpawns;

    private int waveCount = 0;

    private int currentEnemyCount = 0;
    List<Vector3> spawnLocs;

    private void Start()
    {
        StartCoroutine(SpawnWave());
    }
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
        if (id == "Enemy")
        {
            currentEnemyCount -= 1;
        }

        print(currentEnemyCount);

        if (currentEnemyCount <= 0)
        {
            StartCoroutine(SpawnWave());
        }
    }

    int NumTotalEnemiesBasedOnWave(int wave)
    {
        return baseTotalEnemyCount + (int) (wave * wave / 49 + .75f);
    }
    
    private IEnumerator SpawnWave()
    {
        waveCount++;
        /* Display wave number or something */
        yield return null;
        currentEnemyCount = NumTotalEnemiesBasedOnWave(waveCount);
        StartCoroutine(SpawnEnemies(currentEnemyCount));
    }

    IEnumerator SpawnEnemies(int numEnemies)
    {
        spawnLocs = new List<Vector3>();
        for (int i = 0; i < numEnemies; i++)
        {
            int enemyType = Random.Range(0, enemies.Length);
            Vector3 newLoc = transform.position + GetPointInCircle();

            int attempts = 0;
            while(attempts < 250)
            {
                yield return null;
                attempts++;
                foreach (Vector3 v in spawnLocs)
                {
                    yield return null;
                    if ((newLoc - v).sqrMagnitude < minDistBetweenSpawns * minDistBetweenSpawns)
                    {
                        newLoc = transform.position + GetPointInCircle();
                        break;
                    }
                    else attempts = 250;
                }
            }

            spawnLocs.Insert(i, transform.position + GetPointInCircle());
            Instantiate(enemies[enemyType], spawnLocs[i], enemies[enemyType].transform.rotation);
            yield return new WaitForSeconds(.5f);
        }
    }
}
