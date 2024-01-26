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
    // Start is called before the first frame update
    void Start()
    {
        staminaBar.value = 1;
        isShooting = false;
    }

    // Update is called once per frame
    void Update()
    
    {
        //if(Input.GetKeyDown(KeyCode.A))
        //  {
        //      Debug.Log("Pressed A");
        //      playerRB.AddForce(Vector2.left * speed ,ForceMode2D.Force);
        //  }
        //  if (Input.GetKeyDown(KeyCode.D))
        //  {
        //      playerRB.AddForce(Vector2.right * speed, ForceMode2D.Force);
        //  }
        //  if (Input.GetKeyDown(KeyCode.W))
        //  {
        //      playerRB.AddForce(Vector2.up * speed, ForceMode2D.Force); ;
        //  }
        //  if (Input.GetKey(KeyCode.S))
        //  {
        //      playerRB.AddForce(Vector2.down * speed, ForceMode2D.Force);
        //  }

        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * speed;
        playerRB.velocity = movement;

        if(Input.GetKey(KeyCode.P))
        {
            if (staminaBar.value != 0)
                theGun.SetActive(true);
            else
                theGun.SetActive(false);
            isShooting = true;
            staminaBar.value -= 0.0003f;
        }
        if(Input.GetKeyUp(KeyCode.P))
        {
            theGun.SetActive(false);
            isShooting = false;
        }
        if(staminaBar.value != 1)
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
