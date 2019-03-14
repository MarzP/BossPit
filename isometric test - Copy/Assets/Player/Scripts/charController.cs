using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charController : MonoBehaviour
{

    [SerializeField]
    float moveSpeed = 4f;

    public Transform playerTransform;
    public GameObject gameController;
    public float dashDistance;
    public float dashRate;
    public float playerCurrentHealth;
    Vector3 forward, right;

    private float nextDash;


    // Start is called before the first frame update
    void Start()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("HorizontalKey") || Input.GetButton("VerticalKey"))
        {
            Move();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > nextDash)
        {
            nextDash = Time.time + dashRate;
            Dash();
        }
       
    }

    void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("HorizontalKey"), 0, Input.GetAxis("VerticalKey"));
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * Input.GetAxis("HorizontalKey");
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * Input.GetAxis("VerticalKey");

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }
    void Dash()
    {
        playerTransform.transform.position += playerTransform.transform.forward * dashDistance;
        //I should probably make the teleporting better to look at on the camera...
        //it should also raycast so you cant teleport through walls
        //maybe even a cooldown
    }
    public void TakeDamage(float amount)
    {
        // Reduce current health by the amount of damage done.
        playerCurrentHealth -= amount;
        Debug.Log("Player took " + amount);
        Debug.Log("Player's current health is " + playerCurrentHealth);
    }
}
