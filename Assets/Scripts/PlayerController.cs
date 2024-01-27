using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("GUI")]
    public Slider hpBar;
    public Slider staminaBar;

    [Header("Player")]
    public GameObject playerObject;
    public Animator playerAnimator;
    public Rigidbody2D playerRB;
    public float speed;
    public float sprintSpeed;
    bool isSprinting;
    public float sprintStaminaConsumption;
    public bool hasBeenHit;
    public float timer = 2f;
    float tempTimer;
    public float yMovementFactor;

    [Header("Stamina")]
    public float staminaIncreaseRate;
    bool hasNoStamina;

    [Header("Gun")]
    public GameObject Kane;
    public GameObject theSpray;
    public float gunStaminaConsumption;
    public bool isShooting;

    [Header("Banana")] // bananas are used to make enemies fall so other enemies laugh (die)
    public GameObject bananaPrefab;
    public float bananaCooldown;
    private float bananaTimer;
    public Vector2 bananaTargetOffset;

    [Header("Sounds")]
    public AudioSource shootingAudioSource;
    public AudioClip sparyClip;
    public AudioClip gettingHurtSound1;
    public AudioClip gettingHurtSound2;
    public AudioClip gettingHurtSound3;
    public AudioSource vfxAudioSource;
    public AudioClip[] deathSounds;

    void Start()
    {
        staminaBar.value = 1;
        hpBar.value = 1;
        isShooting = false;
        isSprinting = false;
        hasNoStamina = false;
        hasBeenHit = false;
        tempTimer = timer;
        shootingAudioSource.Play();
        shootingAudioSource.Pause();
        vfxAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // detect sprinting
        if (isSprinting && !(horizontalInput == 0 && verticalInput == 0))
        {
            staminaBar.value -= sprintStaminaConsumption * Time.deltaTime;
            if (staminaBar.value <= 0)
            {
                hasNoStamina = true;
                isSprinting = false;
                playerAnimator.SetBool("isSprinting", true);
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
            hasNoStamina = false;
            playerAnimator.SetBool("isSprinting", false);
        }

        // gun attack
        if (Input.GetKey(KeyCode.P))
        {
            if (staminaBar.value != 0)
            {
                theSpray.transform.position = Kane.transform.position;
                isShooting = true;
                shootingAudioSource.UnPause();
                theSpray.SetActive(true);
            }
            else
            {
                shootingAudioSource.Pause();
                theSpray.gameObject.SetActive(false);
            }

            staminaBar.value -= gunStaminaConsumption * Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            theSpray.SetActive(false);
            shootingAudioSource.Pause();
            isShooting = false;
        }

        // banana throw
        if (bananaTimer > 0) { bananaTimer -= Time.deltaTime; }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            bananaTimer = bananaCooldown;
            GameObject banana = Instantiate(bananaPrefab);
            banana.transform.position = transform.position;
            Vector2 temp = (Vector2)transform.position + bananaTargetOffset;
            banana.GetComponent<BananaController>().target = new Vector2(transform.position.x + Mathf.Sign(transform.localScale.x) * bananaTargetOffset.x, transform.position.y + bananaTargetOffset.y);

        }

        // stamina increasing automagically
        if (staminaBar.value <= 1 && !(isShooting || isSprinting || hasNoStamina))
        {
            staminaBar.value += staminaIncreaseRate * Time.deltaTime;
        }

        //Face the direction of movement
        if (horizontalInput > 0)
        {
            Vector3 temp = this.transform.localScale;
            temp.x = 1;
            this.transform.localScale = temp;
        }

        else if (horizontalInput < 0)
        {
            Vector3 temp = this.transform.localScale;
            temp.x = -1;
            this.transform.localScale = temp;
        }

        // game over handling
        if (hpBar.value == 0)
        {
            vfxAudioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);
            SceneManager.LoadScene("GameOverScene");
        }
        if (hasBeenHit)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = tempTimer;
            }
        }

        // actual movement handling
        Vector2 movement = new Vector2(horizontalInput, verticalInput * yMovementFactor); //this vectror isn't normalized to emulate the movement in games like lf2 or "dad and I"
        Vector2 playerMovement;
        if (Input.GetKey(KeyCode.LeftShift) && !hasNoStamina)
        {
            playerMovement = movement * sprintSpeed;
            isSprinting = true;
            playerAnimator.SetBool("isSprinting", true);
        }
        else
        {
            playerMovement = movement * speed;
            //playerAudioScource.PlayOneShot(walkingSound);
        }
        playerRB.velocity = playerMovement;

    }

    public void KillPlayer()
    {
        hpBar.value = 0;
    }

    public void TakeDamage()
    {
        int rand = Random.Range(1, 4);
        if (rand == 1)
            vfxAudioSource.PlayOneShot(gettingHurtSound1);
        if (rand == 2)
            vfxAudioSource.PlayOneShot(gettingHurtSound2);
        if (rand == 3)
            vfxAudioSource.PlayOneShot(gettingHurtSound3);
        hpBar.value -= 0.1f;
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
