using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private int baseTotalEnemyCount;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject nezha;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float minDistBetweenSpawns;

    private int waveCount = 0;

    private int currentEnemyCount = 0;
    List<Vector3> spawnLocs;

    private Transform player;

    [SerializeField] private int lastWave;
    [SerializeField] private TMP_Text waveText;

    private void Start()
    {
        player = FindObjectOfType<IsometricPlayerController3D>().transform;
        StartCoroutine(SpawnWave(false));
    }
    Vector3 GetPointInCircle()
    {
        float angle = Random.Range(0f, 360f);
        float radius = Random.Range(0f, spawnRadius);

        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        return new Vector3(cos - sin, 0f, cos + sin) * radius;
    }

    public void RemoveEnemy(string id)
    {
        if (id == "Enemy")
        {
            currentEnemyCount -= 1;
            if (currentEnemyCount <= 0)
            {
                StartCoroutine(SpawnWave(true));
            }
        }
        else if (id == "Nezha")
        {
            Debug.Log("CONGRATS U WIN!");
        }
    }

    int NumTotalEnemiesBasedOnWave(int wave)
    {
        return baseTotalEnemyCount + (int) (wave * wave / 49 + .75f);
    }
    
    private IEnumerator SpawnWave(bool playEnd)
    {
        if (playEnd)
        {
            SoundManager.PlayCheer();
            yield return new WaitForSeconds(3f);
        }
        if (waveCount == lastWave)
        {
            anim.Play("fade_out");
        }
        else
        {
            yield return new WaitForSeconds(.2f);
            waveCount++;
            waveText.text = "V@UD " + waveCount.ToString();
            waveText.gameObject.SetActive(true);
            SoundManager.PlayGong();
            yield return new WaitForSeconds(3f);
            waveText.gameObject.SetActive(false);
            yield return null;
            StartCoroutine(SpawnEnemies(waveCount));
        }
    }

    IEnumerator SpawnEnemies(int wave)
    {
        currentEnemyCount = NumTotalEnemiesBasedOnWave(waveCount);

        if (wave == 10)
        {
            currentEnemyCount = 1;
            Instantiate(nezha, transform.position, nezha.transform.rotation);
        }
        else
        {
            int numEnemies = currentEnemyCount;
            spawnLocs = new List<Vector3>();
            for (int i = 0; i < numEnemies; i++)
            {
                int enemyType = Random.Range(0, enemies.Length);
                Vector3 newLoc = transform.position + GetPointInCircle();

                int attempts = 0;
                while (attempts < 250)
                {
                    attempts++;
                    foreach (Vector3 v in spawnLocs)
                    {
                        if ((newLoc - v).sqrMagnitude < minDistBetweenSpawns * minDistBetweenSpawns)
                        {
                            newLoc = transform.position + GetPointInCircle();
                            break;
                        }
                        else attempts = 250;
                    }
                }

                spawnLocs.Insert(i, newLoc);
                Instantiate(enemies[enemyType], newLoc, enemies[enemyType].transform.rotation);
                yield return new WaitForSeconds(.5f);
            }
        }
    }
}
