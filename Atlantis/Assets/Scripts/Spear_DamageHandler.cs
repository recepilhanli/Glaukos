using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;

public class Spear_DamageHandler : MonoBehaviour
{
    public void HandleDamage(float dmg)
    {
        Player.Instance._Spear.SendDamage(dmg);
    }
}
