using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveSphere : MonoBehaviour
{

    private Rigidbody rb;
    private GameObject boss;

    public float speed;
    public float destroyTime;
    public int damage;
    
    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.FindWithTag("Enemy");
        rb = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(12, 12, true);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 center = transform.position;
        Vector3 direction = (transform.position - center).normalized;
        transform.position += direction * speed;
        
        Destroy(this.gameObject, destroyTime);
    }
}
