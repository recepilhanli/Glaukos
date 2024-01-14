using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Skilla Boss Class
/// </summary>
public class Skilla : Entity, IEnemyAI
{

    public enum SkillaStates
    {
        State_NONE,
        State_AttackNormal,
        State_Grabbing,
        State_Grabbed, //biting whilw grabbing
        State_AttackPoison,
        State_CallEnemies,
        State_SpellingHealth,
        State_SpellingFocus,
    }


    [SerializeField] float _Health = 300f;

    [SerializeField] Rigidbody2D _Rigidbody;
    [SerializeField] Animator _Animator;
    [SerializeField] Transform _AttachingBone;
    [SerializeField] Transform _HeadTransform;
    [Space(15), SerializeField] List<GameObject> _CallableEnemies = new List<GameObject>();
    [SerializeField] GameObject _PoisonPrefab;
    [SerializeField, ReadOnlyInspector] SkillaStates _CurrentState = SkillaStates.State_NONE;

    [SerializeField] GameObject _CallEnemyParticle;
    [SerializeField] GameObject _SpellingHealthParticle;
    [SerializeField] GameObject _SpellingFocusParticle;

    [SerializeField] GameObject _Canvas;

    [SerializeField] Image _SpellImage;

    [Space(15), SerializeField] GameObject GrabGO;
    [SerializeField] Slider _GrabBar;
    [SerializeField] TextMeshProUGUI _GrabText;

    [SerializeField] AudioClip _WelcomingClip;

    private float _GrabTime = 0f;
    private float _GrabDelay = 0f;

    private bool _isEntitySeen = false;
    private Vector2 m_Velocity = Vector2.zero;

    private int TotalCalledEnemies = 0;
    private float CallEnemyCooldown = 0;

    private float _SpellParticleCooldown = 0;

    private float _SpellFocusDelay = 0;

    private float _ChasingTime = 0;

    private float _SpellDelay = 0;


    void Start()
    {
        Init(null);
    }

    void Update()
    {
        if (CallEnemyCooldown != 0 && CallEnemyCooldown < Time.time)
        {
            CallEnemyCooldown = 0;
            TotalCalledEnemies = 0;
        }

        if (!_isEntitySeen)
        {
            if (Vector2.Distance(transform.position, Player.Instance.transform.position) <= 25f)
            {
                OnDetected(Player.Instance);
            }
            return;
        }

        if (isDeath) return;

        //??? bug (temp)
        if (!_HealthBar.gameObject.activeInHierarchy) _HealthBar.gameObject.SetActive(true);


        float dist = Vector2.Distance(transform.position, Player.Instance.transform.position);

        switch (_CurrentState)
        {
            case SkillaStates.State_NONE:
                {
                    Move(Player.Instance.transform.position);
                    if (dist <= 1.9f)
                    {
                        SetState(SkillaStates.State_AttackNormal);
                    }
                    else if (_ChasingTime < Time.time && _ChasingTime != 0)
                    {
                        Debug.Log("Set random state after endless chasing");
                        RandomState();
                    }
                    break;
                }
            case SkillaStates.State_AttackNormal:
                {
                    Move(Player.Instance.transform.position);
                    if (dist >= 1.9f)
                    {
                        SetState(SkillaStates.State_NONE);
                    }
                    break;
                }
            case SkillaStates.State_Grabbing:
                {
                    Move(Player.Instance.transform.position);

                    if (dist <= 1.9f)
                    {
                        SetState(SkillaStates.State_Grabbed);
                    }
                    break;
                }
            case SkillaStates.State_Grabbed:
                {
                    Player.Instance.SetLensSize(4, true);
                    Vector2 pos = transform.position;
                    pos.x = Mathf.PingPong(Time.time * 2, 2) - 1;
                    Move(pos);
                    Player.Instance.transform.localPosition = Vector3.zero;
                    _Animator.SetTrigger("Bite");

                    if (_GrabBar.value >= 0) _GrabBar.value -= Time.deltaTime / 4;

                    if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                    {
                        Player.Instance.CameraShake(2, 1, .3f);
                        _GrabBar.value += .175f;
                    }

                    if (_GrabBar.value >= 1) SetState(SkillaStates.State_NONE);

                    string mouse0 = "<sprite=2>";
                    string mouse1 = "<sprite=3>";


                    if (Time.time > _GrabTime)
                    {
                        _GrabText.text = (_GrabText.text == mouse0) ? mouse1 : mouse0;
                        _GrabTime = Time.time + .2f;
                    }


                    if (_SpellParticleCooldown < Time.time)
                    {
                        Instantiate(_SpellingHealthParticle, transform.position, Quaternion.identity);
                        _SpellParticleCooldown = Time.time + .6f;
                    }

                    break;
                }
            case SkillaStates.State_SpellingHealth:
                {
                    Vector2 pos = transform.position;
                    pos.y = Mathf.PingPong(Time.time * 2, 5);
                    Move(pos);
                    //ping pong spell image alpha black faster
                    _SpellImage.color = new Color(0, 0, 0, Mathf.PingPong(Time.time * 6, 2) - 1);


                    _Health += Time.deltaTime * 5;
                    _HealthBar.value = _Health / 100f;
                    Player.Instance.OnTakeDamage(Time.deltaTime * 3);

                    if (_SpellParticleCooldown < Time.time)
                    {
                        Instantiate(_SpellingHealthParticle, transform.position, Quaternion.identity);
                        _SpellParticleCooldown = Time.time + .6f;
                    }

                    if (_Health >= 200) SetState(SkillaStates.State_NONE);
                    break;
                }
            case SkillaStates.State_SpellingFocus:
                {
                    Vector2 pos = transform.position;
                    pos.y = Mathf.PingPong(Time.time * 2, 5);
                    Move(pos);
                    //ping pong spell image alpha black faster
                    _SpellImage.color = new Color(0, 0, 0, Mathf.PingPong(Time.time * 6, 2) - 1);

                    Player.Instance.PosionEffect();
                    _Health += Time.deltaTime * 5;
                    _HealthBar.value = _Health / 100f;
                    Player.Instance.Focus -= Time.deltaTime * 5;

                    if (_SpellParticleCooldown < Time.time)
                    {
                        Instantiate(_SpellingFocusParticle, transform.position, Quaternion.identity);
                        _SpellParticleCooldown = Time.time + .6f;
                    }

                    if (Player.Instance.Focus <= 10) SetState(SkillaStates.State_NONE);
                    break;
                }
            case SkillaStates.State_AttackPoison:
                {
                    break;
                }
            case SkillaStates.State_CallEnemies:
                {
                    break;
                }
            default: break;
        }


    }

    public void CreatePosion()
    {
        var wave = Instantiate(_PoisonPrefab, _HeadTransform.position, Quaternion.identity);
        wave.transform.up = (Player.Instance.transform.position - transform.position).normalized;
        Destroy(wave, 3f);
    }


    public void CallRandonEnemy()
    {
        if (_CallableEnemies.Count == 0) return;

        if (CallEnemyCooldown != 0 && CallEnemyCooldown > Time.time)
        {
            SetState(SkillaStates.State_AttackPoison);
            return;
        }

        int rand = Random.Range(0, _CallableEnemies.Count);
        var enemy = Instantiate(_CallableEnemies[rand], transform.position, Quaternion.identity).GetComponent<Entity>();
        if (enemy != null) StartCoroutine(EnemyCoroutine(enemy));
        StartCoroutine(CallEffect());

        //get all renderers of enemy and change color to alpha white
        var Renderers = enemy.GetComponentsInChildren<SpriteRenderer>();
        foreach (var _renderer in Renderers)
        {
            _renderer.color = new Color(.2f, 1f, .2f, 0.6f);
        }

        //change scale of the shark
        var shark = enemy.GetComponent<Shark>();
        if (shark != null)
        {
            enemy.transform.localScale = Vector3.one * 1.75f;
            shark.OnTakeDamage(20);
        }


        TotalCalledEnemies++;
        if (TotalCalledEnemies >= 3)
        {
            CallEnemyCooldown = Time.time + 15;
        }
        Instantiate(_CallEnemyParticle, transform.position, Quaternion.identity);
    }


    public void Init(EntityProperties _properties)
    {
        Type = EntityType.Type_Skilla;
    }

    public void OnDetected(Entity _entity)
    {
        if (_isEntitySeen) return;
        LevelManager.PlaySound2D(_WelcomingClip, .6f);
        SetState(SkillaStates.State_NONE);
        Player.Instance.LockLensSize = true;
        _isEntitySeen = true;
        _Canvas.SetActive(true);
    }

    public void SetState(SkillaStates state)
    {
        if (_CurrentState == state) return;

        if (_CurrentState == SkillaStates.State_Grabbed)
        {
            _GrabDelay = Time.time + 10f;
            GrabGO.SetActive(false);
            _GrabBar.value = 0;
            Player.Instance.transform.SetParent(null);
            Player.Instance.CanMove = true;
            Player.Instance.transform.rotation = Quaternion.identity;
            Player.Instance.transform.transform.eulerAngles = Vector3.zero;
            Player.Instance.transform.localScale = new Vector3(.75F, .75F, .75F);
        }
        else if (_CurrentState == SkillaStates.State_SpellingHealth)
        {
            _SpellDelay = Time.time + 15f;
            _SpellImage.color = new Color(0, 0, 0, 0);
            Player.Instance.SetSlow(false);
        }
        else if (_CurrentState == SkillaStates.State_NONE) _ChasingTime = 0;


        if (state == SkillaStates.State_SpellingHealth && _Health > 200) state = SkillaStates.State_Grabbing;
        else if (state == SkillaStates.State_SpellingFocus && Player.Instance.Focus <= 15) state = SkillaStates.State_Grabbing;

        if (state == SkillaStates.State_Grabbed && _GrabDelay > Time.time) state = SkillaStates.State_AttackNormal;
        _CurrentState = state;


        //animation states
        const int anim_move = 0;
        const int anim_callenemy = 1;
        const int anim_normalattack = 3;
        const int anim_grabbing = 4;
        const int anim_grabbed = 5;
        const int anim_poisonattack = 6;

        switch (state)
        {
            case SkillaStates.State_NONE:
                {
                    _ChasingTime = Time.time + 12;
                    _Animator.SetInteger("State", anim_move);
                    break;
                }
            case SkillaStates.State_AttackNormal:
                {
                    _Animator.SetInteger("State", anim_normalattack);
                    break;
                }
            case SkillaStates.State_Grabbed:
                {
                    GrabGO.SetActive(true);
                    Player.Instance.transform.SetParent(_AttachingBone);
                    Player.Instance.CanMove = false;
                    Player.Instance.transform.localPosition = Vector3.zero;
                    Player.Instance.transform.eulerAngles = new Vector3(0, 0, 90);

                    _Animator.SetInteger("State", anim_grabbed);
                    break;
                }
            case SkillaStates.State_Grabbing:
                {
                    _Animator.SetInteger("State", anim_grabbing);
                    break;
                }
            case SkillaStates.State_CallEnemies:
                {
                    _Animator.SetInteger("State", anim_callenemy);
                    break;
                }
            case SkillaStates.State_AttackPoison:
                {
                    _Animator.SetInteger("State", anim_poisonattack);
                    break;
                }
            case SkillaStates.State_SpellingHealth:
                {
                    _Animator.SetInteger("State", anim_move);
                    Player.Instance.SetSlow(true);
                    break;
                }
            case SkillaStates.State_SpellingFocus:
                {
                    _Animator.SetInteger("State", anim_move);
                    Player.Instance.SetSlow(true);
                    break;
                }
            default: break;
        }
    }


    public override void Move(Vector2 pos)
    {
        if (transform.position.x - pos.x < 0) transform.localScale = new Vector3(-6, 6, 6);
        else transform.localScale = new Vector3(6, 6, 6);

        float speedMultiplier = 1f;
        if (_Health <= 50) speedMultiplier = 0.4f;



        float multiplier = .275f;
        if (_CurrentState == SkillaStates.State_Grabbing) multiplier = .1f;
        else if (_CurrentState == SkillaStates.State_SpellingHealth) multiplier = .15f;

        _Rigidbody.MovePosition(Vector2.SmoothDamp(transform.position, pos, ref m_Velocity, multiplier * speedMultiplier));
    }

    public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
    {
        if (Vector2.Distance(transform.position, Player.Instance.transform.position) <= 1.9) Player.Instance.OnTakeDamage(damage, type);
    }

    public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {

        if (isDeath) return;
        if (!_isEntitySeen)
        {
            OnDetected(Player.Instance);
        }
        StartCoroutine(DamageEffect());
        if (type == AttackTypes.Attack_Standart && !Player.Instance._Rage) Player.Instance.GiveFocusPoints(5f);
        _Health -= _h / 8f;
        _HealthBar.value = _Health / 100f;
        if (_Health <= 0)
        {
            OnDeath();
            return;
        }

        else if (_CurrentState == SkillaStates.State_NONE && Vector2.Distance(Player.Instance.transform.position, transform.position) >= 3 && Random.Range(0, 3) <= 1) RandomState();
        if (_CurrentState == SkillaStates.State_AttackPoison && Random.Range(0, 5) <= 2) SetState(SkillaStates.State_NONE);
        if (_CurrentState == SkillaStates.State_Grabbing && Random.Range(0, 5) <= 2) SetState(SkillaStates.State_NONE);
        else if ((_CurrentState == SkillaStates.State_SpellingHealth || _CurrentState == SkillaStates.State_SpellingFocus) && Random.Range(0, 3) <= 1 && _SpellDelay <= Time.time) SetState(SkillaStates.State_NONE);
    }

    void RandomState()
    {
        //set random state

        if (_SpellDelay < Time.time && _CurrentState != SkillaStates.State_SpellingHealth && _Health <= 200)
        {
            _SpellDelay = Time.time + 1;
            SetState(SkillaStates.State_SpellingHealth);
            Debug.Log("Spelling Health");
            return;
        }

        int rand = Random.Range(0, 3);
        if (rand == 0) SetState(SkillaStates.State_CallEnemies);
        else if (rand == 1) SetState(SkillaStates.State_Grabbing);
        else if (rand == 2)
        {
            rand = Random.Range(0, 1);
            if (_SpellFocusDelay < Time.time && _CurrentState != SkillaStates.State_SpellingFocus && Player.Instance.Focus >= 40 && rand == 0)
            {
                _SpellDelay = Time.time + 2;
                _SpellFocusDelay = Time.time + 10;
                SetState(SkillaStates.State_SpellingFocus);
                Debug.Log("Spelling Focus");
            }
            else SetState(SkillaStates.State_AttackPoison);
        }

    }




    public override void OnDeath()
    {
        if (isDeath) return;
        isDeath = true;
        Player.Instance.BossKillReward("ToBe");

    }

    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }


    IEnumerator EnemyCoroutine(Entity enemy)
    {
        yield return new WaitForSeconds(12);
        if (enemy != null) enemy.OnDeath();
        yield return null;
    }

    IEnumerator DamageEffect()
    {
        // int randomindex = UnityEngine.Random.Range(1, _Clips.Count);
        // PlaySound(randomindex);

        var Renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var _renderer in Renderers)
        {
            _renderer.color = new Color(1f, .1f, .1f);
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var _renderer in Renderers)
        {
            _renderer.color = new Color(.85f, .85f, .85f);
        }
        yield return null;
    }

    IEnumerator CallEffect()
    {
        // int randomindex = UnityEngine.Random.Range(1, _Clips.Count);
        // PlaySound(randomindex);

        var Renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var _renderer in Renderers)
        {
            _renderer.color = new Color(.1f, 1f, .1f);
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var _renderer in Renderers)
        {
            _renderer.color = new Color(.85f, .85f, .85f);
        }
        yield return null;
    }

}
