using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    public PlayerController player;
    public List<Transform> possibleTargets; //the enemy may walk to any of the targets
    public GameObject enemyPrefab;
    public float initialStunTime;

    List<GameObject> enemies = new();
    List<Transform> currentTargets = new();

    public int initialEnemiesInWave;
    public float waveIncreaseRate;
    private int enemiesInWave;
    public float timeBetweenSpawns;
    public float timeBetweenWaves;
    public float waveTimer;
    public int enemyCounter, waveCounter;

    public AudioSource thisAudioSource;
    public AudioClip[] laughingSounds;


    public void Start()
    {
        enemiesInWave = NumberOfEnemies(0);
        thisAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //fetch the targets of all enemies
        currentTargets.Clear();
        foreach (GameObject enemy in enemies)
        {
            currentTargets.Add(enemy.GetComponent<EnemyController>().currentTarget);
        }

        Vector2 playerPosition = player.transform.position;
        //Choose a new target for each enemy, which is the least populated of the three closest ones to it. Each enemy only has a 0.1 prob of doing this. (this should prevent oscillation)
        foreach (GameObject enemy in enemies)
        {
            if (Random.Range(1, 10) == 1)
            {
                //first sort the targets by distance and take the first 3:
                Vector3 thisPosition = enemy.transform.position;
                List<Transform> closest3 = possibleTargets.OrderBy(target => Vector2.Distance((Vector2)target.localPosition + playerPosition, thisPosition)).ToList().GetRange(0, 3);

                //sort by population and take the first one (least populated)
                enemy.GetComponent<EnemyController>().currentTarget = closest3.OrderBy(target => CountInTargetList(target)).First();
            }
        }

        // advance wave/spawns
        waveTimer -= Time.deltaTime;
        if (enemyCounter == enemiesInWave && waveTimer <= 0)
        {
            enemyCounter = 0; waveTimer = timeBetweenWaves; enemiesInWave = NumberOfEnemies(waveCounter); waveCounter++;
        }
        if (enemyCounter < enemiesInWave && waveTimer <= 0)
        {
            enemyCounter++; waveTimer = timeBetweenSpawns; SpawnEnemy();
        }

        //destroy any laughing enemy when one is slipping
        if (enemies.Any(e => e.GetComponent<EnemyController>().nowSlipping))
        {
            var enemiesToDestroy = enemies
                .Select(e => (go: e, ctrl: e.GetComponent<EnemyController>()))
                .Where(e => e.ctrl.isLaughing)
                .Select(e => e.go)
                .ToList();

            foreach (var enemy in enemiesToDestroy)
            {
                DestroyEnemy(enemy);
            }
        }
    }

    int NumberOfEnemies(int wavenum)
    {
        Debug.Log(wavenum);
        return (int)(initialEnemiesInWave + wavenum * wavenum * waveIncreaseRate);
    }

    public int CountInTargetList(Transform t)
    {
        int count = 0;
        foreach (Transform trans in currentTargets)
        {
            if (t == trans) { count++; }
        }
        return count;
    }

    public void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab);
        enemies.Add(newEnemy);

        EnemyController ctrl = newEnemy.GetComponent<EnemyController>();
        ctrl.player = player;
        ctrl.possibleTargets = new List<Transform>(possibleTargets);
        ctrl.enemyManager = this;
        ctrl.stunTime = initialStunTime;

        newEnemy.transform.position = new Vector2(Random.Range(-10f, 10f), Random.Range(-5f, 1f));
    }

    public void DestroyEnemy(GameObject enemy)
    {
        GameObject.Destroy(enemy, 1f);
        enemies.Remove(enemy);
        thisAudioSource.PlayOneShot(laughingSounds[Random.Range(0, laughingSounds.Length)]);
    }
}
