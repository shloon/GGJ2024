using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlippingManager : MonoBehaviour
{
    public GameObject parent;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.tag);
        if (collider.tag == "Banana")
        {
            if (collider.GetComponent<BananaController>().isActive)
            {
                Debug.Log("It works!");
                parent.GetComponent<EnemyController>().StartCoroutine("FlipOnBanana");
                Destroy(collider.gameObject);
            }
        }
    }
}
