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
        Vector3 playerPosition = player.transform.position;
        Vector3 thisPosition = transform.position;
        Vector2 distanceToTarget = currentTarget.localPosition + playerPosition - thisPosition;

        //Walk towards the target
        Vector2 directionsToMove = vectorSigns(distanceToTarget);
        directionsToMove.y *= yMovementFactor;
        Vector2 nextVelocity = directionsToMove * speed;
        Vector2 step = Time.deltaTime * nextVelocity;
        if (Mathf.Abs(step.x) > Mathf.Abs(distanceToTarget.x)) { nextVelocity.x = 0; }
        if (Mathf.Abs(step.y) > Mathf.Abs(distanceToTarget.y)) { nextVelocity.y = 0; }
        selfRigidbody.velocity = nextVelocity;

        /////
        theTimer -= Time.deltaTime;
        if (transform.position.x < 0)
        {
            if (playerPosition.x - (-thisPosition.x) >= 01f &&
                playerPosition.x - (-thisPosition.x) >= 1.90f)
            {
                Invoke("AttackPlayer", 0.5f);
            }
        }
        else
        {
            if (playerPosition.x - transform.position.x >= 0 &&
                playerPosition.x - transform.position.x >= 1.90f)
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
        return new Vector2(Mathf.Sign(input.x), Mathf.Sign(input.y));
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

        isNearEnemy = true;
        Debug.Log("near player");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isNearEnemy = false;
    }
}