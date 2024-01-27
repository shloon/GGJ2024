using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

class EnemyController : MonoBehaviour
{
    public EnemyManager enemyManager;
    public PlayerController player;
    public GameObject enemyWeapon;
    public Rigidbody2D selfRigidbody;
    public List<Transform> possibleTargets; //the enemy may walk to any of the targets
    public List<Transform> corners; //the enemy will run towards the corners
    public Transform currentTarget; //the current target to which the enemy is walking
    public float yMovementFactor;
    public float speed;

    public Animator actorAnimator;
    public Animator thisAnimator;
    public Slider theSlider;
    public bool isNearEnemy;
    public float attackTime = 1f;
    private float attackCounter; // time between attacks
    public float stunTime; // can't move or attack after being attacked
    public float stunDeterioration; // every subsequent stun is shorter by a fraction
    private float stunCounter;
    public bool isStunned = false;
    public bool isHurt;
    public float damageRate;
    public bool nowAttacking = false;
    public bool nowSlipping = false;
    public bool isLaughing = false;
    public float healthFillRate;

    public AnimationClip hittingAnimationClip;

    public AudioSource thisAudioSource;
    public AudioClip[] gigglingSounds;
    public AudioClip[] fallingSounds;

    public void Start()
    {
        isNearEnemy = false;
        isHurt = false;
        theSlider.value = 1f;
        yMovementFactor = player.yMovementFactor;
        currentTarget = possibleTargets[0];
        selfRigidbody = GetComponent<Rigidbody2D>();
        thisAnimator = GetComponent<Animator>();
        thisAudioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 thisPosition = transform.position;
        Vector2 distanceToTarget = currentTarget.localPosition + playerPosition - thisPosition;

        //Walk towards the target
        if (!nowAttacking && !isStunned && !nowSlipping && !isLaughing)
        {
            Vector2 directionsToMove = vectorSigns(distanceToTarget);
            directionsToMove.y *= yMovementFactor;
            Vector2 nextVelocity = directionsToMove * speed;
            Vector2 step = Time.deltaTime * nextVelocity;
            if (Mathf.Abs(step.x) > Mathf.Abs(distanceToTarget.x)) { nextVelocity.x = 0; }
            if (Mathf.Abs(step.y) > Mathf.Abs(distanceToTarget.y)) { nextVelocity.y = 0; }
            selfRigidbody.velocity = nextVelocity;
        }
        else
        {
            selfRigidbody.velocity = Vector2.zero;
        }

        //attack and stun cooldown
        attackCounter -= Time.deltaTime;
        if (stunCounter > 0) { stunCounter -= Time.deltaTime; isStunned = true; } else { isStunned = false; }

        if (isNearEnemy && !nowSlipping && !isLaughing)
        {
            if (attackCounter <= 0)
            {
                StartCoroutine("AnimataionCoroutine");
                attackCounter = attackTime;
            }
        }
        if (player.gameObject.transform.position.x - transform.position.x >= 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (player.gameObject.transform.position.x - transform.position.x <= 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (isHurt)
        {
            TakeDamage();
            //stunCounter = stunTime;
        }
        if (theSlider.value <= 0)
        {
            //enemyManager.DestroyEnemy(gameObject);
            if (!isLaughing) thisAudioSource.PlayOneShot(gigglingSounds[Random.Range(0, gigglingSounds.Length)]);
            isLaughing = true;
        }
        if (isLaughing)
        {
            theSlider.value += Time.deltaTime * healthFillRate;
            if (theSlider.value >= 1)
            {
                isLaughing = false;
            }
        }
    }

    public void TakeDamage()
    {
        theSlider.value -= Time.deltaTime * damageRate;
    }

    IEnumerator AnimataionCoroutine()
    {
        if (isNearEnemy && !isStunned)
        {
            enemyWeapon.SetActive(true);
            nowAttacking = true;

            actorAnimator.SetTrigger("Attack");

            yield return new WaitForSeconds(0.5f);

            if (isNearEnemy)
            {
                player.TakeDamage();
                actorAnimator.ResetTrigger("Attack");

            }
            enemyWeapon.SetActive(false);
            nowAttacking = false;
        }
    }

    IEnumerator FlipOnBanana()
    {
        thisAnimator.Play("spin");

        thisAudioSource.PlayOneShot(fallingSounds[Random.Range(0,fallingSounds.Length)]);

        nowSlipping = true;

        yield return new WaitForSeconds(1f);

        nowSlipping = false;
    }

    public Vector2 vectorSigns(Vector2 input)
    {
        return new Vector2(Mathf.Sign(input.x), Mathf.Sign(input.y));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isNearEnemy = true;
            //Debug.Log("near player");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isNearEnemy = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Spray"))
        {
            Debug.Log("Start Destroying");
            isHurt = true;
            if (!isStunned) { stunCounter = stunTime; isStunned = true; stunTime = stunTime * stunDeterioration; } // start stun and shorten the next one
        }
        if (collider.gameObject.CompareTag("Player"))
        {
            isNearEnemy = true;
            //Debug.Log("near player");
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Spray"))
        {
            isHurt = false;
        }
        if (collider.gameObject.CompareTag("Player"))
        {
            isNearEnemy = false;
        }
    }
}