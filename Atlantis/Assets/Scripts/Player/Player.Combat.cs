
using System.Collections;
using System.Collections.Generic;
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

        public AudioSource _Source;

        public List<AudioClip> _Clips = new List<AudioClip>();

  

        void CameraSize()
        {
            _VirtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(_VirtualCamera.m_Lens.OrthographicSize, _LensSize, Time.deltaTime * 10);

            if (Input.GetKey(_KeybindTable.HeavyAttack)) _LensSize = 12;
            else _LensSize = 8;
        }


        void Consumable()
        {
            if (Input.GetKeyDown(_KeybindTable.HealKey) && Focus >= 10 && Health != 100)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                Focus -= 10;
                Health += 5;
                Health = Mathf.Clamp(Health, 0, 100);
                UIManager.Instance.Fade(0, 0.9f, 0.1f);
            }


            if (Input.GetKeyDown(_KeybindTable.Using_Item_1) && Focus >= 25)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                Focus -= 25;
                _Spear.CreateTornado();
                UIManager.Instance.Fade(1, 1, 1);
            }


            if (Input.GetKeyDown(_KeybindTable.Using_Item_2) && Focus >= 40)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                Focus -= 40;
                _Spear.CreateSpearRain();
                UIManager.Instance.Fade(1, 1, 1);
            }

        }


        void Attacking()
        {

            if (_Spear.ThrowState != Spear.ThrowStates.STATE_NONE) //call back
            {
                if (Input.GetKeyDown(_KeybindTable.HeavyAttack))
                {
                    _Spear.GetBackToThePlayer(false);
                    _Source.clip = _Clips[2];
                    _Source.Play();
                }
                return;
            }

            if (Input.GetKeyDown(_KeybindTable.Attack))
            {
                if (Input.GetKey(_KeybindTable.HeavyAttack))
                {
                    Debug.Log("Throw Spear");
                    AttackState(2);

                    Vector3 mousePosition = Input.mousePosition;
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.lockState = CursorLockMode.None;
                    worldPosition.z = 0;
                    _Spear.Throw(worldPosition);
                    _LensSize = 8;
                    _Source.clip = _Clips[0];
                    _Source.Play();
                    return;
                }
                AttackState(1);


                _Source.clip = _Clips[1];
                _Source.Play();
            }


            if (Input.GetKeyUp(_KeybindTable.HeavyAttack) && _VirtualCamera.m_Lens.OrthographicSize < 8.75f)
            {
                AttackState(1);
                _Source.clip = _Clips[0];
                _Source.Play();
                _LensSize = 8;
            }


            if (Input.GetKeyUp(_KeybindTable.SpecialAttack))
            {
                Debug.Log("Special Attack");
            }

        }


        void Combat()
        {
            CameraSize();

            Consumable();

            Attacking();
        }

        public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
        {
            if (damage == 0) return;
            entity.OnTakeDamage(damage, type);
        }

        public override void OnDeath()
        {
            SceneManager.LoadScene("Death");
        }

        public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
        {
            Health -= _h;
            if (Health <= 0) OnDeath();

            Focus -= _h / 2;
            Focus = Mathf.Clamp(Focus, 0, 100);

            StartCoroutine(DamageEffect());
            UIManager.Instance.Fade(1, 0f, 0, 4);
        }


        IEnumerator DamageEffect()
        {
            _PlayerRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            _PlayerRenderer.color = Color.white;
            yield return null;
        }

        public void AttackState(int state)
        {
            StartCoroutine(AttackAnim(state));
        }
        IEnumerator AttackAnim(int state)
        {
            PlayerAnimator.SetInteger("attack_state", state);
            yield return new WaitForSeconds(0.5f);
            PlayerAnimator.SetInteger("attack_state", 0);
            yield return null;
        }
    }

}