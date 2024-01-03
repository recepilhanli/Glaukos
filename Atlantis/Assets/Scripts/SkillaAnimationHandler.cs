using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;
public class SkillaAnimationHandler : MonoBehaviour
{

    [SerializeField] Skilla _Skilla;

    public void Attack(float dmg)
    {
        _Skilla.Attack(Player.Instance, dmg/1.3f);
    }

    public void PoisonAttack()
    {
        _Skilla.CreatePosion();
    }

    public void CallEnemy()
    {
        _Skilla.CallRandonEnemy();
    }


}
