using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSphereAttackScript : MonoBehaviour
{
    public Rigidbody attackSphere;
    public float attackSphereSpeed;
    public float numOfSpheres;

    private float nextActionTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
       
        
    }

    // Update is called once per frame
    void Update()
    {
        nextActionTime += Time.deltaTime;

        if (nextActionTime > attackSphereSpeed) {
            Debug.Log("Should start fire method");
            FireSpheres();
            nextActionTime = 0.0f;
        }
    }

    private void FireSpheres()
    {

        Vector3 center = transform.position;
        for (int i = 0; i < numOfSpheres; i++)
        {
            float a = 360 / numOfSpheres * i;
            Vector3 pos = RandomCircle(center, 1.0f, a);
            Instantiate(attackSphere, pos, Quaternion.identity);


            //Vector3 center = transform.position;
            //for (int i = 0; i < numOfSpheres; i++)
            //{
            //    Vector3 pos = RandomCircle(center, 5.0f);
            //    Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
            //    Rigidbody attackSphereInstance = Instantiate(attackSphere, pos, rot) as Rigidbody;
            //    Debug.Log("Should have made instance");
            //}

            //Vector3 target = transform.position;
            //target = new Vector3(target.x, target.y, target.z);

            //Quaternion targetRotation = Quaternion.Euler(90, 0, 0);
            //Rigidbody attackSphereInstance = Instantiate(attackSphere, target, targetRotation) as Rigidbody;
            //Debug.Log("Should have made instance");
        }
    }

    Vector3 RandomCircle(Vector3 center, float radius, float a) {
        Debug.Log(a);
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z;
        return pos;
        //float ang = Random.value * 360;
        //Vector3 pos;
        //pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        //pos.y = center.y;
        //pos.z = center.z;
        //return pos;
    }
}
