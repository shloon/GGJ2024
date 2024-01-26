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
    public Vector2 sprintSpeed;
    bool isSprinting;
    public Slider hpBar;

    [Header("Stamina")]
    public Slider staminaBar;
    public float staminaIncreaseRate;
    // the oo in ooStamina is Out Of.
    //public float ooStaminaLockTimer = 1; 
    //float staminaLockTimer;
    bool hasNoStamina;
    [Header("Gun")]
    public GameObject theGun;
    public float staminaDecreaseRate;
    public bool isShooting;

    void Start()
    {
        staminaBar.value = 1;
        hpBar.value = 1;
        //staminaLockTimer = ooStaminaLockTimer;
        isShooting = false;
        isSprinting = false;
        hasNoStamina = false;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        if ((Input.GetKey(KeyCode.LeftShift)&&!hasNoStamina))
        {
            movement = Vector2.Scale(movement, sprintSpeed);
            isSprinting = true;
        }
        else
            movement = Vector2.Scale(movement, speed);
        playerRB.velocity = movement;
        if(isSprinting)
        {
            if (((Input.GetAxisRaw("Horizontal") != 0) || (Input.GetAxisRaw("Vertical") != 0)))
            {
                staminaBar.value -= staminaDecreaseRate * 1.2f * Time.deltaTime;
                if (staminaBar.value <= 0)
                {
                    hasNoStamina = true;
                    isSprinting = false;
                }
            }
        }
        //if(hasNoStamina)
        //{
        //    staminaLockTimer-= Time.deltaTime;
        //}
        //if(staminaLockTimer<=0)
        //{
        //    staminaLockTimer = ooStaminaLockTimer;
        //    hasNoStamina = false;
        //}
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
            if (hasNoStamina)
                hasNoStamina = false;
        }
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
            if (!isShooting&&!isSprinting&&!hasNoStamina)
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
