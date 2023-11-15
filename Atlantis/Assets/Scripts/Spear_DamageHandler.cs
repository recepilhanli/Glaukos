using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;

public class Spear_DamageHandler : MonoBehaviour
{
    public void HandleDamage(float dmg)
    {
        if (dmg < 0)
        {
            Player.Instance._Spear.SendDamage(-dmg, false, null, true);
            Debug.Log("Punch");
        }
        else Player.Instance._Spear.SendDamage(dmg, false, null, false);
    }
}
