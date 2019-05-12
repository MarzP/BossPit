//    ******** NOTE ********      //
//    everything works            //
//    theres just no health       //
//    so nothing visually happens //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class n3kHitBoxTutAttempt01 : MonoBehaviour
{

    public Collider[] attackHitboxes;
    public Transform shotSpawn;
    public Transform bigShotSpawn;
    public float fireRate;
    public float bigFireRate;
    public charController charaController;
    public float speedBack;
    public Transform playerTransform;
    public ParticleSystem lightning;
    public Light attackLight;

    private float nextFire;
    private float nextBigFire;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && Time.time > nextBigFire)
        {
            attackLight.enabled = !attackLight.enabled;
            Vector3 tempPosition = playerTransform.position;
            playerTransform.position = tempPosition;
            nextBigFire = Time.time + bigFireRate;
            LaunchBigAttack(attackHitboxes[2]);
        }
        if (Input.GetKeyDown("space") && Time.time > nextFire)
        {
            Vector3 tempPosition = playerTransform.position;
            playerTransform.position = tempPosition;
            nextFire = Time.time + fireRate;
            LaunchShortRanged(attackHitboxes[1]);
        }

    }
    private void LaunchAttack(Collider col)
    {
        Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("Hitbox"));
        foreach (Collider c in cols)
        {
            if (c.transform.parent.parent == transform)
            {
                continue;
            }
            float damage = 0;
            switch (c.name)
            {
                case "Head":
                damage = 30;
                Debug.Log(c.name);
                break;
                case "Torso":
                damage = 10;
                Debug.Log(c.name);
                break;
                default:
                Debug.Log("Unable to identify body part, make sure the name matces the switch case");
                break;
            }
            
        }
    }
    private void LaunchShortRanged(Collider col)
    {
        Instantiate(attackHitboxes[1], shotSpawn.position, shotSpawn.rotation);
        //Instantiate(lightning, transform.position, transform.rotation);
        //lightning.transform.parent = this.transform;
        Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("Hitbox"));

        foreach (Collider c in cols)
        {
            if (c.transform.parent.parent == transform)
            {

                continue;

            }

            float damage = 0;

            switch (c.name)
            {
                case "enemy":
                    damage = 10;
                    Debug.Log(c.name);
                    Debug.Log(damage);
                    attackLight.enabled = !attackLight.enabled;
                    break;

                default:
                    Debug.Log("Unable to identify enemy. Make sure name matches switch case.");
                    break;

            }
        }
    }

    private void LaunchBigAttack(Collider col) {
        Instantiate(attackHitboxes[2], bigShotSpawn.position, bigShotSpawn.rotation);
        //Instantiate(lightning, transform.position, transform.rotation);
        //lightning.transform.parent = this.transform;
        Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("Hitbox"));

        foreach (Collider c in cols)
        {
            if (c.transform.parent.parent == transform)
            {

                continue;

            }

            float damage = 0;

            switch (c.name)
            {
                case "enemy":
                    damage = 10;
                    Debug.Log(c.name);
                    Debug.Log(damage);
                    attackLight.enabled = !attackLight.enabled;
                    break;

                default:
                    Debug.Log("Unable to identify enemy. Make sure name matches switch case.");
                    break;

            }
        }
    }
}

