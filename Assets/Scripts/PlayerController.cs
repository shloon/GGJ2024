using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public GameObject playerObject;
    public Rigidbody2D playerRB;
    public float speed;
    [SerializeField] public GameObject theGun;
    public Slider staminaBar;
    public bool isShooting;
    void Start()
    {
        staminaBar.value = 1;
        isShooting = false;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * speed;
        playerRB.velocity = movement;

        if (Input.GetKey(KeyCode.P))
        {
            if (staminaBar.value != 0)
                theGun.SetActive(true);
            else
                theGun.SetActive(false);
            isShooting = true;
            staminaBar.value -= 0.0003f;
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            theGun.SetActive(false);
            isShooting = false;
        }
        if (staminaBar.value != 1)
        {
            if (!isShooting)
            {
                staminaBar.value += 0.0003f;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Stopper")
        {
            Debug.Log("Touched Wall");
        }
    }
}
