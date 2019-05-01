//    ******** NOTE ********      //
//    everything works            //
//    theres just no health       //
//    so nothing visually happens //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class n3kHitBoxTutAttempt01 : MonoBehaviour
{

    public Collider[] attackHitboxes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            LaunchAttack(attackHitboxes[0]);
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
}
