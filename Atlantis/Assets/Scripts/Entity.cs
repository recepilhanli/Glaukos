using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public abstract class Entity : MonoBehaviour
{

    public enum AttackTypes
    {
        Attack_Standart,
        Attack_Rapid,
        Attack_Heavy,
        Attack_Special
    }

    public enum EntityFlags
    {
        Flag_None,
        Flag_Player,
        Flag_Enemy,
        Flag_Friendly,
        Flag_Neutral

    }

    public virtual void OnDeath()
    {
        Debug.Log(gameObject.name + " is death.");
        Destroy(gameObject);
    }

    public virtual void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {
        Debug.Log(gameObject.name + " took a damage.");
    }

    public virtual void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
    {
        Debug.Log($"{gameObject.name} attacked to {entity.gameObject} with {type}");
    }

    public virtual void Move(Vector2 pos)
    {
        Debug.Log($"{gameObject.name} moved to {pos}");
    }

    public abstract EntityFlags GetEntityFlag();
}




