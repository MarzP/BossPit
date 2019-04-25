using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public int health = 100; 
	//Method to damage a player or enemy and recalculate health.
    public void TakeDamage(int Amount) {
        health -= Amount;
        if (health <= 0)
        {
            Destroy();
        }
    }
	//Method to destory the gameobject.
    void Destroy() {
        Destroy(this.gameObject);
    }
}
