
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;

namespace MainCharacter
{
    /// <summary>
    /// 
    /// Player.Combat.cs:
    /// Functions:
    /// Movement();
    /// 
    /// </summary>

    public partial class Player : Entity
    {

        [Space]
        [Header("Combat")]

        public float Health = 100f;
        public float Focus = 0f;

        public Spear _Spear;
        public Transform _RightHand;
        void Combat()
        {

            if (Input.GetKeyDown(_KeybindTable.HealKey) && Focus >= 10 && Health != 100)
            {
                Focus -= 10;
                Health += 5;
                Health = Mathf.Clamp(Health, 0, 100);
                Focus = Mathf.Clamp(Focus, 0, 100);
            }

            if (_Spear._ThrowState != 0)
            {
                if (Input.GetKeyDown(_KeybindTable.HeavyAttack)) _Spear.GetBackToThePlayer(false);
                return;
            }

            if (Input.GetKeyDown(_KeybindTable.Attack))
            {
                if (Input.GetKey(_KeybindTable.HeavyAttack))
                {
                    Debug.Log("Throw Spear");
                    _Spear.transform.parent.SetParent(null);
                    _Animator.ResetTrigger("attack_throw");
                    _Animator.SetTrigger("attack_throw");
                    Vector3 mousePosition = Input.mousePosition;
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                    worldPosition.z = 0;
                    _Spear.Throw(worldPosition);

                    return;
                }
                _Animator.ResetTrigger("attack_standart");
                _Animator.SetTrigger("attack_standart");
                
                _Spear._Animator.SetTrigger("Attack_Light");
            }


            if (Input.GetKeyUp(_KeybindTable.HeavyAttack))
            {
                _Animator.ResetTrigger("attack_standart");
                _Animator.SetTrigger("attack_standart");

                _Spear._Animator.SetTrigger("Attack_Heavy");
            }


            if (Input.GetKeyUp(_KeybindTable.SpecialAttack))
            {
                Debug.Log("Special Attack");
            }


        }

        public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
        {
            if (damage == 0) return;
            entity.OnTakeDamage(damage, type);
        }

        public override void OnDeath()
        {
            SceneManager.LoadScene("Menu");
        }

        public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
        {
            Health -= _h;
            if (Health <= 0) OnDeath();

            Focus -= _h / 2;
            Focus = Mathf.Clamp(Health, 0, 100);

            StartCoroutine(DamageEffect());
        }


        IEnumerator DamageEffect()
        {
            _PlayerRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            _PlayerRenderer.color = Color.white;
            yield return null;
        }

    }

}