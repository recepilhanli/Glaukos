
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

        [SerializeField] List<AudioClip> _Clips = new List<AudioClip>();

        [SerializeField] List<GameObject> _RageEffects = new List<GameObject>();

        public bool _Rage { get; private set; } = false;

        void RageCombat()
        {
            StartCoroutine(Rage());
        }

        IEnumerator Rage()
        {
            CameraShake(3, 0.8f, 0.01f);
            UIManager.Instance.Fade(0.35f, 1f, 0.9f);
            UIManager.Instance.StopFading = true;
            _LensSize = 12f;

            foreach (var _re in _RageEffects)
            {
                _re.SetActive(true);
            }
            _Rage = true;


            yield return new WaitForSeconds(10f);

            UIManager.Instance.StopFading = false;
            _Rage = false;
            foreach (var _re in _RageEffects)
            {
                _re.SetActive(false);
            }

            _LensSize = 8f;
            yield return null;
        }

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
                CameraShake(2.0f, 0.5f, 0.01f);
            }


            if (Input.GetKeyDown(_KeybindTable.Using_Item_2) && Focus >= 40)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                Focus -= 40;
                _Spear.CreateSpearRain();
                UIManager.Instance.Fade(1, 1, 1);
                CameraShake(2.0f, 0.5f, 0.01f);
            }


            if (Input.GetKeyDown(_KeybindTable.RageKey) && Focus >= 85)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                Focus -= 85;
                RageCombat();
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
                    CameraShake(2, 0.5f, 0.001f);
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
            float _dmg = (_Rage) ? damage * 1.5f : damage;
            entity.OnTakeDamage(_dmg, type);


            if (_Rage)
            {
                Health += damage;
                Mathf.Clamp(Health, 0, 100);
                CameraShake(2f, 0.6f, 0.025f);
            }
            else CameraShake(1.75f, 0.5f, 0.02f);
        }

        public override void OnDeath()
        {
            SceneManager.LoadScene("Death");
        }

        public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
        {
            if (_Rage) return;

            Health -= _h;
            if (Health <= 0) OnDeath();

            Focus -= _h / 2;
            Focus = Mathf.Clamp(Focus, 0, 100);

            StartCoroutine(DamageEffect());
            UIManager.Instance.Fade(1, 0f, 0, 4);
            CameraShake(2.5f, 0.8f, 0.03f);
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