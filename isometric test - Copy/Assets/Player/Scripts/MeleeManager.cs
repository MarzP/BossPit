using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeManager : MonoBehaviour
{
    public Camera fpsCam;
    public float range;
    public LayerMask myLayerMask;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            Attack();
        }
        
    }

    void Attack() {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, myLayerMask)) {
            Target target = hit.transform.gameObject.GetComponent<Target>();
            if (target != null)
            {
                Debug.Log("Enemy Hit");
                target.TakeDamage(damage);
            }
            
        }
    }
}
