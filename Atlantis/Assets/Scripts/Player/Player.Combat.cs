using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

namespace MainCharacter
{

    /// <summary>
    /// 
    /// 
    /// Player.Movement.cs:
    /// Functions:
    /// Move();
    /// Gravity();
    /// 
    /// 
    /// </summary>

    public partial class Player : Entity
    {
        public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
        {
            entity.OnTakeDamage(damage, type);
        }

        public override void OnDeath()
        {
           
        }
    }

}