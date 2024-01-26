using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerObject;
    public Rigidbody2D playerRB;
    public float speed;
    public Vector2 sprintSpeed;
    bool isSprinting;
    public Slider hpBar;
    public bool hasBeenHit;
    public float timer = 2f;
    float tempTimer;
    public float yMovementFactor;

    [Header("Stamina")]
    public Slider staminaBar;
    public float staminaIncreaseRate;
    bool hasNoStamina;

    [Header("Gun")]
    public GameObject theGun;
    public float staminaDecreaseRate;
    public bool isShooting;

    void Start()
    {
        staminaBar.value = 1;
        hpBar.value = 1;
        isShooting = false;
        isSprinting = false;
        hasNoStamina = false;
        hasBeenHit = false;
        tempTimer = timer;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput * yMovementFactor); //this vectror isn't normalized to emulate the movement in games like lf2 or "dad and I"
        movement *= speed;
        playerRB.velocity = movement;

        Vector2 playerMovement;
        if (Input.GetKey(KeyCode.LeftShift) && !hasNoStamina)
        {
            playerMovement = Vector2.Scale(movement, sprintSpeed);
            isSprinting = true;
        }
        else
        {
            playerMovement = movement * speed;
        }
        playerRB.velocity = playerMovement;

        if (isSprinting)
        {
            if ((Input.GetAxisRaw("Horizontal") != 0) || (Input.GetAxisRaw("Vertical") != 0))
            {
                staminaBar.value -= staminaDecreaseRate * 1.2f * Time.deltaTime;
                if (staminaBar.value <= 0)
                {
                    hasNoStamina = true;
                    isSprinting = false;
                }
            }
        }

        // detect sprinting
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
            if (hasNoStamina)
                hasNoStamina = false;
        }

        // gun attack
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

        // stamina increasing
        if (staminaBar.value != 1)
        {
            if (!isShooting && !isSprinting && !hasNoStamina)
            {
                staminaBar.value += staminaIncreaseRate * Time.deltaTime;
            }
        }

        //Face the direction of movement
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            Vector3 temp = this.transform.localScale; temp.x = 1;
            this.transform.localScale = temp;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            Vector3 temp = this.transform.localScale; temp.x = -1;
            this.transform.localScale = temp;
        }

        // game over handling
        if (hpBar.value == 0)
        {
            SceneManager.LoadScene("GameOverScene");
        }
        if (hasBeenHit)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Debug.Log("You need to take damage");
                //TakeDamage();
                timer = tempTimer;
            }
        }
    }

    public void KillPlayer()
    {
        hpBar.value = 0;
    }

    public void TakeDamage()
    {
        hpBar.value -= 0.1f;
        Debug.Log(hpBar.value);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Stopper")
        {
            Debug.Log("Touched Wall");
        }

        if (collision.gameObject.tag == "Enemy")
        {
            hasBeenHit = true;

        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (hasBeenHit)
        {
            hasBeenHit = false;
            timer = tempTimer;

        }
    }

}
