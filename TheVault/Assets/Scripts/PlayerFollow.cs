using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform PlayerTransform;
    public GameController gameController;
    public Vector3 cameraOffset;
    public bool LookAtPlayer = false;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
       PlayerTransform = gameController.getPlayerTransform();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = PlayerTransform.position + cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

        if (LookAtPlayer) {
            transform.LookAt(PlayerTransform);
        } 
    }
}
