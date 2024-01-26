using UnityEngine;

class EnemyController : MonoBehaviour
{
    PlayerController player;
    public Rigidbody2D selfRigidbody;
    public Vector2 spawnOffset;
    public Vector2 spawnRadius;
    public void Start()
    {
        Relocate();
    }

    public void Update()
    {

    }

    void Relocate()
    {
        transform.position = Vector2.Scale(Random.insideUnitCircle, spawnRadius) + spawnOffset;
    }
}