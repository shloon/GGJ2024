using UnityEngine;

class EnemyController : MonoBehaviour
{
   [SerializeField]PlayerController player;
    public GameObject enemyWeapon;
    public Rigidbody2D selfRigidbody;
    public Vector2 spawnOffset;
    public Vector2 spawnRadius;
    public void Start()
    {
        Relocate();
    }

    public void Update()
    {
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
    }

    public void AttackPlayer()
    {
        enemyWeapon.SetActive(true);
    }
    
    void Relocate()
    {
        transform.position = Vector2.Scale(Random.insideUnitCircle, spawnRadius) + spawnOffset;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Invoke("AttackPlayer",1);
    }
}