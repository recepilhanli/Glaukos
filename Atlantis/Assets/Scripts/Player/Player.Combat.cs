
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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
        private static readonly string[] HIT_CLIPS = { "damage1", "damage2", "damage3", "damage4", "damage5", "damage6", "damage7", "damage8", "damage9", "damage10" };
        public static int RemainingLifes { private set; get; } = 5;

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

        [SerializeField] AudioClip _HealClip;
        public AudioClip RageNotificationClip;
        public AudioClip FocusClip;
        public AudioClip HarmClip;

        public AudioClip CanUseClip;
        [SerializeField] AudioClip _CannotUseClip;


        [SerializeField, Tooltip("Spear's way")] Transform _ThrowWay;
        [SerializeField, Tooltip("When the player's health low this source will be playing")] AudioSource _HeartBeat;
        [SerializeField, Tooltip("When the player's health low this source will be playing")] AudioSource _RageSource;




        [HideInInspector] public bool RewardSequence = false;
        public bool _Rage { get; private set; } = false;

        private bool _PunchState = false;

        private bool _Poisoned = false;


        public static void LoadRemaningLifes()
        {
            if (PlayerPrefs.HasKey(PerfTable.perf_RemainingLifes)) RemainingLifes = PlayerPrefs.GetInt(PerfTable.perf_RemainingLifes, 5);
        }
        public static void ResetRemainingLifes()
        {
            RemainingLifes = 5;

            PlayerPrefs.DeleteKey(PerfTable.perf_LastFocus);
            PlayerPrefs.DeleteKey(PerfTable.perf_LastHealth);
            PlayerPrefs.DeleteKey(PerfTable.perf_LastScene);
            PlayerPrefs.DeleteKey(PerfTable.perf_LastPosX);
            PlayerPrefs.DeleteKey(PerfTable.perf_LastPosY);
            PlayerPrefs.DeleteKey(PerfTable.perf_LastPosZ);


            PlayerPrefs.SetInt(PerfTable.perf_RemainingLifes, RemainingLifes);
            PlayerPrefs.Save();
        }

        public void GetSpearInteractionClip()
        {
            SoundManager.PlaySound2D(_Spear.SpearImpactSound2, .5f);
        }


        void RageCombat()
        {
            StartCoroutine(Rage());
        }

        public void PlayHitClip()
        {
            SoundManager.PlaySound2D(HIT_CLIPS[Random.Range(0, HIT_CLIPS.Length)], Random.Range(.25f, .35f));
        }

        public void PlayFocusClip()
        {
            SoundManager.PlaySound2D(Instance.FocusClip, .5f);
        }

        public void PlayHarmClip()
        {
            SoundManager.PlaySound2D(Instance.HarmClip, 1f);
        }


        void Deformation()
        {
            if (RemainingLifes < 3)
            {
                var DefColor = new Color(.7f, 1, .7f);

                if (RemainingLifes == 2) DefColor = new Color(.5f, 1, .5f);
                else DefColor = new Color(.3f, 1, .35f);

                //get all sprite renderers
                var _srs = gameObject.GetComponentsInChildren<SpriteRenderer>();
                foreach (var _sr in _srs)
                {
                    if (!_sr.gameObject.CompareTag("Weapon")) _sr.color = DefColor;
                }
            }
        }


        void Consumable()
        {

            if (Input.GetKeyDown(_KeybindTable.HealKey) && Focus >= 10 && Health != 100 && _Spear.ThrowState == Spear.ThrowStates.STATE_NONE)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                GiveFocusPoints(-10);
                Health += 5;
                Health = Mathf.Clamp(Health, 0, 100);
                UIManager.Instance.Fade(0, 0.9f, 0.1f);
                SoundManager.PlaySound2D(_HealClip, .6f);
            }
            else if (Input.GetKeyDown(_KeybindTable.HealKey)) SoundManager.PlaySound2D(_CannotUseClip, .05f);



            if (Input.GetKeyDown(_KeybindTable.Using_Item_1) && Focus >= 25 && _Spear.ThrowState == Spear.ThrowStates.STATE_NONE)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                GiveFocusPoints(-25);
                _Spear.CreateTornado();
                UIManager.Instance.Fade(1, 1, 1);
                AttackState(4);
                SoundManager.PlaySound2D(_Spear.SpearTalent1, .5f);
                CameraShake(3.0f, 0.6f, 0.01f);
            }
            else if (Input.GetKeyDown(_KeybindTable.Using_Item_1) && Focus >= 25) SoundManager.PlaySound2D(_CannotUseClip, .05f);


            if (Input.GetKeyDown(_KeybindTable.Using_Item_2) && Focus >= 40 && _Spear.ThrowState == Spear.ThrowStates.STATE_NONE)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                GiveFocusPoints(-40);
                _Spear.CreateSpearRain();
                UIManager.Instance.Fade(1, 1, 1);
                AttackState(5);
                SoundManager.PlaySound2D(_Spear.SpearTalent2, .5f);
                CameraShake(3.0f, 0.6f, 0.01f);
            }
            else if (Input.GetKeyDown(_KeybindTable.Using_Item_2) && Focus >= 40) SoundManager.PlaySound2D(_CannotUseClip, .05f);

            if (Input.GetKeyDown(_KeybindTable.RageKey) && ((Focus >= 85 && TutorialDialogHandler.SetFullFocus == false) || TutorialDialogHandler.SetFullFocus == true) && _Spear.ThrowState == Spear.ThrowStates.STATE_NONE)
            {
                Focus = Mathf.Clamp(Focus, 0, 100);
                GiveFocusPoints(-85);
                RageCombat();
            }
            else if (Input.GetKeyDown(_KeybindTable.RageKey) && ((Focus >= 85 && TutorialDialogHandler.SetFullFocus == false) || TutorialDialogHandler.SetFullFocus == true)) SoundManager.PlaySound2D(_CannotUseClip, .05f);

        }

        void Attacking()
        {

            if (_Spear.ThrowState != Spear.ThrowStates.STATE_NONE && !TutorialDialogHandler.TutBlockGetBack) //call back
            {
                if (Input.GetKeyDown(_KeybindTable.ThrowKey))
                {
                    if (_Spear.Stuck == 0)
                    {
                        _Source.clip = _Clips[2];
                        _Source.Play();
                    }
                    _Spear.GetBackToThePlayer(false);

                }

                if ((Input.GetKeyDown(_KeybindTable.Attack) || Input.GetKeyDown(_KeybindTable.HeavyAttack)) && !TutorialDialogHandler.TutBlockAttack1)
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
                if (Input.GetKey(_KeybindTable.ThrowKey) && !TutorialDialogHandler.TutBlockThrow)
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


            if (Input.GetKeyUp(_KeybindTable.HeavyAttack) && _Spear.ThrowState == Spear.ThrowStates.STATE_NONE && CanMove == true)
            {
                AttackState(3);
                _Source.clip = _Clips[0];
                _Source.Play();
                _LensSize = 8;
            }


        }

        public void FlowEffect(bool toggle)
        {
            if (toggle)
            {
                _ColorAdjustments.colorFilter.value = new Color(0.2f, 1f, 0.2f, 1);
                _ColorAdjustments.saturation.value = -20;
                _FilmGrain.intensity.value = 0.75f;
                _HeartBeat.Stop();

            }
            else
            {
                _ColorAdjustments.colorFilter.value = new Color(1, 1, 1, 1);
                _ColorAdjustments.saturation.value = 0;
                _FilmGrain.intensity.value = 0.3f;
                _HeartBeat.Stop();
            }

        }


        void Combat()
        {
            if (isDeath) return;

            if (_Rage)
            {
                //ping pong vignette smothless
                _Vignette.smoothness.value = Mathf.PingPong(Time.time, .5f) + 0.2f;
            }


            if (Health <= 25 && !_HeartBeat.isPlaying)
            {
                _ColorAdjustments.colorFilter.value = new Color(1, 0.5f, 0.5f, 1);
                _ColorAdjustments.saturation.value = -30;
                _FilmGrain.intensity.value = 0.8f;
                _HeartBeat.Play();
            }
            else if (Health > 25 && _HeartBeat.isPlaying)
            {
                _ColorAdjustments.colorFilter.value = new Color(1, 1, 1, 1);
                _ColorAdjustments.saturation.value = 0;
                _FilmGrain.intensity.value = 0.3f;
                _HeartBeat.Stop();
            }

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
            if (TutorialDialogHandler.TutBlockThrow) return;
            if (Input.GetKey(_KeybindTable.ThrowKey) && _Spear.ThrowState == Spear.ThrowStates.STATE_NONE && CanMove == true)
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
                Health += damage / 1.5f;
                Health = Mathf.Clamp(Health, 0, 100);
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
            _ColorAdjustments.saturation.value = -100;
        }



        public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
        {
            if (isDeath) return;

            else if (_Rage)
            {
                Health -= _h / 3.5f;
                Health = Mathf.Clamp(Health, 0, 100);
                if (Health <= 0) OnDeath();
                return;
            }

            Health -= _h;
            Health = Mathf.Clamp(Health, 0, 100);
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
            if (sound) SoundManager.PlaySound2D(_RewardClip, .4f);
            Health = 100;
            Focus = 100;
            LevelManager.Instance.SaveGame(false);
            PlayerPrefs.SetString(PerfTable.perf_LastScene, nextScene);
            PlayerPrefs.SetFloat(PerfTable.perf_LastPosX, 0);
            PlayerPrefs.Save();
            //   isDeath = true;
            RewardSequence = true;
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
            _FilmGrain.intensity.value = 0.75f;
            _ColorAdjustments.colorFilter.value = new Color(1, 0.75f, 0.75f, 1);
            _ColorAdjustments.saturation.value = -30;
            _LensSize = 9f;
            _RageSource.Play();
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

            _ColorAdjustments.colorFilter.value = new Color(1, 1, 1, 1);
            _ColorAdjustments.saturation.value = 0;

            if (SceneManager.GetActiveScene().name != PerfTable.perf_Level4)
            {
                _Vignette.intensity.value = .47f;
                _Vignette.smoothness.value = 0.18f;
                _FilmGrain.intensity.value = 0.3f;
            }
            else
            {
                _Vignette.smoothness.value = 0.353f;
                _Vignette.intensity.value = 0.537f;
                _FilmGrain.intensity.value = 0.9f;
            }


            _LensSize = 8f;
            _RageSource.Stop();
            yield return null;
        }


    }
}