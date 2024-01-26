using System.Collections;
using UnityEngine;

class EnemyController : MonoBehaviour
{
   [SerializeField]PlayerController player;
    public GameObject enemyWeapon;
    public Rigidbody2D selfRigidbody;
    public Vector2 spawnOffset;
    public Vector2 spawnRadius;
    public Animator actorAnimator;
    public bool isNearEnemy;
    public float theTimer = 1f;
   
    public AnimationClip hittingAnimationClip;
    public void Start()
    {
        isNearEnemy = false;
        
        Relocate();
    }

    public void Update()
    {
        theTimer -= Time.deltaTime;
        if(transform.position.x < 0)
        {
            if(player.transform.position.x - (-1*transform.position.x )>= 01f &&
                player.transform.position.x - (-1 * transform.position.x) >= 1.90f)
            {

                Invoke("AttackPlayer", 1f);
            }
        }
        else
        {
            if (player.transform.position.x - (transform.position.x) >= 0 &&
                player.transform.position.x - (transform.position.x) >= 1.90f)
            {

                Invoke("AttackPlayer", 1f);
            }
        }
        if(isNearEnemy)
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
            

            yield return new WaitForSeconds(0.8f);
            player.TakeDamage();
            actorAnimator.ResetTrigger("Attack");
            enemyWeapon.SetActive(false);
        }
        
        
    }
    
    void Relocate()
    {
        transform.position = Vector2.Scale(Random.insideUnitCircle, spawnRadius) + spawnOffset;
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