using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerObject;
    public Rigidbody2D playerRB;
    public Vector2 speed;
    public Slider hpBar;

    [Header("Stamina")]
    public Slider staminaBar;
    public float staminaIncreaseRate;

    [Header("Gun")]
    public GameObject theGun;
    public float staminaDecreaseRate;
    public bool isShooting;

    void Start()
    {
        staminaBar.value = 1;
        hpBar.value = 1;
        isShooting = false;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        movement = Vector2.Scale(movement, speed);
        playerRB.velocity = movement;

        if (Input.GetKey(KeyCode.P))
        {
            if (staminaBar.value != 0)
                theGun.SetActive(true);
            else
                theGun.SetActive(false);
            isShooting = true;
            staminaBar.value -= staminaDecreaseRate * Time.deltaTime;
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
                staminaBar.value += staminaIncreaseRate * Time.deltaTime;
            }
        }

        if (hpBar.value == 0)
        {
            SceneManager.LoadScene("GameOverScene");
        }
    }

    public void KillPlayer()
    {
        hpBar.value = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Stopper")
        {
            Debug.Log("Touched Wall");
        }
    }
}
