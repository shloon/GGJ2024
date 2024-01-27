using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaController : MonoBehaviour
{
    public float timeToTarget;
    public Vector2 origin, target;
    float initialTime;
    public bool isActive = false; //can this banana cause someone to slip?

    void Start()
    {
        origin = transform.position;
        initialTime = Time.time;
    }


    void Update()
    {
        transform.position = Vector2.Lerp(origin, target, (Time.time - initialTime) / timeToTarget);
        if (Time.time - initialTime > timeToTarget) isActive = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(isActive);
        if (isActive && collider.tag == "Enemy Feet")
        {
            collider.GetComponent<EnemyController>().StartCoroutine("FlipOnBanana");
            Destroy(gameObject);
        }
    }
}
