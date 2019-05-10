using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldowns : MonoBehaviour
{
    public Image teleport;
    public Image bigAttack;
    public Image basicAttack;

    public float cooldownTeleport;
    public float cooldownBigAttack;
    public float cooldownBasicAttack;

    bool isCooldownTeleport;
    bool isCooldownBigAttack;
    bool isCooldownBasicAttack;

void Update()
{
    if(Input.GetKeyDown(KeyCode.Space))
    {
        isCooldownBasicAttack = true;
    }
    if(Input.GetKeyDown(KeyCode.G))
    {
        isCooldownBigAttack = true;
    }
    if(Input.GetKeyDown(KeyCode.LeftShift))
    {
    isCooldownTeleport = true;
    }
    
    // Change cooldown image
    if(isCooldownBasicAttack)
    {
        basicAttack.fillAmount += 1 / cooldownBasicAttack * Time.deltaTime;
        if(basicAttack.fillAmount >= 1)
        {
            basicAttack.fillAmount = 0;
            isCooldownBasicAttack = false;
        }
    }

    // Change cooldown image
    if(isCooldownTeleport)
    {
        teleport.fillAmount += 1 / cooldownTeleport * Time.deltaTime;
        if(teleport.fillAmount >= 1)
        {
            teleport.fillAmount = 0;
            isCooldownTeleport = false;
        }
    }
    
    
    
    // Change cooldown image
    if(isCooldownBigAttack)
    {
        bigAttack.fillAmount += 1 / cooldownBigAttack * Time.deltaTime;
        if(bigAttack.fillAmount >= 1)
        {
            bigAttack.fillAmount = 0;
            isCooldownBigAttack = false;
        }
    }
    
}
}
