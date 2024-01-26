using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //This script moves the camera to follow the player

    public Transform player;
    public float xmin, xmax;
    public float ymin, ymax;
    public float yMovementFraction;

    private float cameraZ;

    public void Start()
    {
        cameraZ = transform.position.z;
    }

    public void Update()
    {
        //Method 1: Follow directly on X, fractionally on Y
        transform.position = new Vector3(player.position.x, player.position.y * yMovementFraction, cameraZ);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, xmin, xmax), Mathf.Clamp(transform.position.y, ymin, ymax), cameraZ);
    }
}
