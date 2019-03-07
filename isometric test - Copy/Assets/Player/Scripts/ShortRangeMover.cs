using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortRangeMover : MonoBehaviour
{
    private Rigidbody rb;

    private GameObject player;
    public float speed;
    public float destroyTime;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.forward * speed;
        
        Destroy(this.gameObject, destroyTime);
        
    }
}
