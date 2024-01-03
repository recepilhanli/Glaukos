
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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

    /// <summary>
    /// This class is used to manage combat of the Player
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

        [SerializeField] AudioClip _RewardClip;

        [SerializeField, Tooltip("Spear's way")] Transform _ThrowWay;

        public bool _Rage { get; private set; } = false;

        bool _PunchState = false;

        bool _Poisoned = false;

        public static int RemainingLifes { private set; get; } = 5;



        public static void LoadRemaningLifes()
        {
            if (PlayerPrefs.HasKey(PerfTable.perf_RemainingLifes)) RemainingLifes = PlayerPrefs.GetInt(PerfTable.perf_RemainingLifes, 5);
        }
        public static void ResetRemainingLifes()
        {
            RemainingLifes = 5;
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt(PerfTable.perf_RemainingLifes, RemainingLifes);
            PlayerPrefs.Save();
        }

        void RageCombat()
        {
            StartCoroutine(Rage());
        }




        void Consumable()
        {
            if (_Spear.ThrowState != Spear.ThrowStates.STATE_NONE) return;

            if (Input.GetKeyDown(_KeybindTable.HealKey) && Focus >= 10 && Health != 100)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                GiveFocusPoints(-10);
                Health += 5;
                Health = Mathf.Clamp(Health, 0, 100);
                UIManager.Instance.Fade(0, 0.9f, 0.1f);
            }

            if (Input.GetKeyDown(_KeybindTable.Using_Item_1) && Focus >= 25)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                GiveFocusPoints(-25);
                _Spear.CreateTornado();
                UIManager.Instance.Fade(1, 1, 1);
                CameraShake(2.0f, 0.5f, 0.01f);
            }


            if (Input.GetKeyDown(_KeybindTable.Using_Item_2) && Focus >= 40)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                GiveFocusPoints(-40);
                _Spear.CreateSpearRain();
                UIManager.Instance.Fade(1, 1, 1);
                CameraShake(2.0f, 0.5f, 0.01f);
            }


            if (Input.GetKeyDown(_KeybindTable.RageKey) && ((Focus >= 85 && TutorialDialogHandler.SetFullFocus == false) || TutorialDialogHandler.SetFullFocus == true))
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                GiveFocusPoints(-85);
                RageCombat();
            }

        }

        void Attacking()
        {

            if (_Spear.ThrowState != Spear.ThrowStates.STATE_NONE && !TutorialDialogHandler.TutBlockGetBack) //call back
            {
                if (Input.GetKeyDown(_KeybindTable.HeavyAttack))
                {
                    if (_Spear.Stuck == 0)
                    {
                        _Source.clip = _Clips[2];
                        _Source.Play();
                    }
                    _Spear.GetBackToThePlayer(false);

                }

                if (Input.GetKeyDown(_KeybindTable.Attack) && !TutorialDialogHandler.TutBlockAttack1)
                {
                    _PunchState = !_PunchState;

                    AttackState((_PunchState) ? 10 : 20);

                    _Source.clip = _Clips[0];
                    _Source.Play();
                }
                return;
            }

            if (Input.GetKeyDown(_KeybindTable.Attack))
            {
                if (Input.GetKey(_KeybindTable.HeavyAttack) && !TutorialDialogHandler.TutBlockThrow)
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
                if (!TutorialDialogHandler.TutBlockAttack2) AttackState(1);


                _Source.clip = _Clips[1];
                _Source.Play();
            }


            if (Input.GetKeyUp(_KeybindTable.HeavyAttack) && _VirtualCamera.m_Lens.OrthographicSize < 8.75f)
            {
                AttackState(3);
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
            if (isDeath) return;


            if (CanMove)
            {
                Consumable();

                Attacking();

                if (_ChromaticAberration != null && !_Poisoned)
                {
                    if (Health <= 25) _ChromaticAberration.intensity.value = 1f;
                    else _ChromaticAberration.intensity.value = 0f;
                }
            }

            if (TutorialDialogHandler.TutBlockThrow == false) ThrowingWay();


        }


        void ThrowingWay()
        {
            if (_VirtualCamera.m_Lens.OrthographicSize >= 9 && Input.GetKey(_KeybindTable.HeavyAttack) && _Spear.ThrowState == Spear.ThrowStates.STATE_NONE && CanMove == true)
            {
                if (_ThrowWay.gameObject.activeInHierarchy == false) _ThrowWay.gameObject.SetActive(true);

                Vector3 mousePosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                _ThrowWay.up = ((Vector2)worldPosition - (Vector2)_ThrowWay.position).normalized;
                //_ThrowWay.eulerAngles = new Vector3(0, 0, _ThrowWay.eulerAngles.z);
            }
            else if (_ThrowWay.gameObject.activeInHierarchy == true) _ThrowWay.gameObject.SetActive(false);

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
            if (isDeath) return;

            RemainingLifes--;
            PlayerPrefs.SetInt(PerfTable.perf_RemainingLifes, RemainingLifes);
            PlayerPrefs.Save();
            //reset position of the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None;

            _ThrowWay.gameObject.SetActive(false);

            isDeath = true;
            AttackState(-1);
            StartCoroutine(DeathSequence());
        }



        public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
        {
            if (_Rage || isDeath) return;

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
            yield return new WaitForSeconds(0.2f);
            PlayerAnimator.SetInteger("attack_state", 0);
            yield return null;
        }




        public void PosionEffect(float time = 6f)
        {
            StartCoroutine(PoisonCoroutine(time));
        }

        public void GiveFocusPoints(float point)
        {
            Instance.Focus += point; //Using Instance for events
            Instance.Focus = Mathf.Clamp(Instance.Focus, 0, 100);
            if (point < 0) UIManager.Instance.Fade(0, 0f, 0f);

        }
        public void DamagePlayer(float dmg)
        {
            Instance.OnTakeDamage(dmg);
        }

        public void DamageAndPoisonPlayer(float dmg)
        {
            Instance.PosionEffect();
            Instance.SetSlow(true);
            Instance.OnTakeDamage(dmg);
            Instance.Invoke("SetDisableSlow", 2f);
        }

        void SetDisableSlow()
        {
            SetSlow(false);
        }

        IEnumerator DeathSequence()
        {
            var _g = GameObject.Find("Global Light 2D");
            if (_g != null) _g.GetComponent<Light2D>().intensity = 0.25f;
            _LensSize = 4f;
            _PlayerRenderer.color = new Color(0.3f, 0.3f, 0.3f);
            transform.eulerAngles = new Vector3(0, 0, 90);
            _Rigidbody.isKinematic = true;
            _Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            Time.timeScale = 0.4f;
            yield return new WaitForSeconds(2f);
            Time.timeScale = 1f;
            SceneManager.LoadScene(PerfTable.perf_LevelDeath);
            yield return null;
        }


        public void BossKillReward(string nextScene, bool sound = true)
        {
            if (sound) LevelManager.PlaySound2D(_RewardClip, .4f);
            Health = 100;
            Focus = 100;
            LevelManager.Instance.SaveGame(false);
            PlayerPrefs.SetString(PerfTable.perf_LastScene, nextScene);
            PlayerPrefs.SetFloat(PerfTable.perf_LastPosX, 0);
            PlayerPrefs.Save();
            isDeath = true;
            StartCoroutine(ThankSequence());
        }

        IEnumerator ThankSequence()
        {
            var _g = GameObject.Find("Global Light 2D");
            if (_g != null) _g.GetComponent<Light2D>().intensity = 0.25f;
            _LensSize = 4f;
            _Rigidbody.isKinematic = true;
            _Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            Time.timeScale = 0.4f;
            yield return new WaitForSeconds(2f);
            Time.timeScale = 1f;
            SceneManager.LoadScene(PerfTable.perf_LevelThank);
            yield return null;
        }



        IEnumerator PoisonCoroutine(float time)
        {
            _Poisoned = true;
            if (_ChromaticAberration != null) _ChromaticAberration.intensity.value = 1f;
            yield return new WaitForSeconds(time);
            _Poisoned = false;
            if (_ChromaticAberration != null) _ChromaticAberration.intensity.value = 0f;
            yield return null;
        }

        IEnumerator Rage()
        {
            CameraShake(3, 0.8f, 0.01f);
            UIManager.Instance.Fade(0.35f, 1f, 0.9f);
            UIManager.Instance.StopFading = true;
            _LensSize = 9f;

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


    }
}