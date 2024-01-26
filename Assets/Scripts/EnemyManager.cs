using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    public List<Transform> possibleTargets; //the enemy may walk to any of the targets
    [SerializeField] GameObject enemyPrefab;

    List<GameObject> enemies = new List<GameObject>();
    List<Transform> currentTargets = new List<Transform>();

    void Start()
    {
        SpawnEnemy(); SpawnEnemy(); SpawnEnemy(); SpawnEnemy(); SpawnEnemy(); SpawnEnemy();
    }

    private void Update()
    {
        //fetch the targets of all enemies
        currentTargets.Clear();
        foreach (GameObject enemy in enemies)
        {
            currentTargets.Add(enemy.GetComponent<EnemyController>().currentTarget);
        }

        //Choose a new target for each enemy, which is the least populated of the three closest ones to it. Each enemy only has a 0.1 prob of doing this. (this should prevent oscillation)
        foreach (GameObject enemy in enemies)
        {
            if (Random.Range(1, 10) == 1)
            {
                //choose a target around the player
                //first sort the targets by distance:
                Vector2 playerPosition = player.transform.position;
                Vector3 thisPosition = enemy.transform.position;
                //sort by distance and take the first 3
                List<Transform> closest3 = possibleTargets.OrderBy(target => Vector2.Distance((Vector2)target.localPosition + playerPosition, thisPosition)).ToList().GetRange(0, 3);
                //sort by population and take the first one (least populated)
                enemy.GetComponent<EnemyController>().currentTarget = closest3.OrderBy(target => CountInTargetList(target)).First();
            }
        }
    }

    public int CountInTargetList(Transform t)
    {
        int count = 0;
        foreach (Transform trans in currentTargets)
        {
            if (Transform.Equals(t, trans)) { count++; }
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
        newEnemy.transform.position = new Vector2(Random.Range(-10f, 10f), Random.Range(-5f, 1f));
    }
}
