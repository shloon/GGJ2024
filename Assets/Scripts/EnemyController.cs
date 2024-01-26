using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class EnemyController : MonoBehaviour
{
    public PlayerController player;
    public GameObject enemyWeapon;
    public Rigidbody2D selfRigidbody;
    public List<Transform> possibleTargets; //the enemy may walk to any of the targets
    public List<Transform> corners; //the enemy will run towards the corners
    public Transform currentTarget; //the current target to which the enemy is walking
    public float yMovementFactor;
    public float speed;
    public bool isChasing = true;

    public Animator actorAnimator;
    public bool isNearEnemy;
    public float theTimer = 1f;

    public AnimationClip hittingAnimationClip;
    public void Start()
    {
        isNearEnemy = false;

        yMovementFactor = player.yMovementFactor;
        currentTarget = possibleTargets[0];
        selfRigidbody = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        Vector2 playerPosition = player.transform.position;
        Vector3 thisPosition = this.transform.position;
        Vector2 distanceToTarget = new Vector2();
        if (isChasing)
        {
            //ChooseNewTarget(); //This chooses the closest target
            distanceToTarget = currentTarget.localPosition + player.transform.position - thisPosition;
        }
        else
        {
            ////Choose a corner to run to. It is the one furthest away from the middle of the enemy and player, weighted by the sin of the angle
            //float biggestScore = -10000;
            //foreach (Transform corner in corners)
            //{
            //    Vector2 midpoint = ((Vector2)thisPosition + (Vector2)player.transform.position) / 2;
            //    float distance = Vector2.Distance(thisPosition, corner.position);
            //    float angle = Vector2.Angle((Vector2)player.transform.position - (Vector2)thisPosition, corner.position - thisPosition);
            //    if (angle > 180) { angle = 360 - angle; }
            //    float score = Mathf.Abs(angle);
            //    Debug.Log(angle);
            //    if (score > biggestScore) { currentTarget = corner; biggestScore = score; }
            //}
            //distanceToTarget = currentTarget.position - thisPosition;
        }
        //Walk towards the target
        Vector2 directionsToMove = vectorSigns(distanceToTarget);
        directionsToMove.y *= yMovementFactor;
        Vector2 nextVelocity = directionsToMove * speed;
        Vector2 step = Time.deltaTime * nextVelocity;
        if (Mathf.Abs(step.x) > Mathf.Abs(distanceToTarget.x)) { nextVelocity.x = 0; }
        if (Mathf.Abs(step.y) > Mathf.Abs(distanceToTarget.y)) { nextVelocity.y = 0; }
        selfRigidbody.velocity = nextVelocity;

        Debug.Log(distanceToTarget);

        /////

        theTimer -= Time.deltaTime;
        if (transform.position.x < 0)
        {
            if (player.transform.position.x - (-1 * transform.position.x) >= 01f &&
                player.transform.position.x - (-1 * transform.position.x) >= 1.90f)
            {

                Invoke("AttackPlayer", 0.5f);
            }
        }
        else
        {
            if (player.transform.position.x - (transform.position.x) >= 0 &&
                player.transform.position.x - (transform.position.x) >= 1.90f)
            {

                Invoke("AttackPlayer", 0.5f);
            }
        }
        if (isNearEnemy)
        {
            if (theTimer <= 0)
            {
                StartCoroutine("AnimataionCoroutine");
                theTimer = 1f;
            }
        }
    }

    public void AttackPlayer()
    {


    }

    IEnumerator AnimataionCoroutine()
    {
        if (isNearEnemy)
        {
            enemyWeapon.SetActive(true);

            actorAnimator.SetTrigger("Attack");


            yield return new WaitForSeconds(0.5f);
            player.TakeDamage();
            actorAnimator.ResetTrigger("Attack");
            enemyWeapon.SetActive(false);
        }


    }

    public Vector2 vectorSigns(Vector2 input)
    {
        Vector2 output = input;
        switch (input.x)
        {
            case > 0:
                output.x = 1;
                break;
            case < 0:
                output.x = -1;
                break;
        }
        switch (input.y)
        {
            case > 0:
                output.y = 1;
                break;
            case < 0:
                output.y = -1;
                break;
        }
        return output;
    }

    public void ChooseNewTarget()
    {
        //choose a target around the player
        //first sort the targets by distance:
        Vector2 playerPosition = player.transform.position;
        Vector3 thisPosition = this.transform.position;
        currentTarget = possibleTargets.OrderBy(target => Vector2.Distance((Vector2)target.localPosition + playerPosition, thisPosition)).First();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            isNearEnemy = true;
            Debug.Log("near player");


        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isNearEnemy = false;
    }
}