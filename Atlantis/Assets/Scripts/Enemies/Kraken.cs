using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;
using Unity.Mathematics;
using UnityEditor.Build.Player;
using UnityEngine.SceneManagement;


public class Kraken : Entity, IEnemyAI
{

    public static Kraken Instance { get; private set; } = null;


    public enum Kraken_AnimStates
    {
        State_Idle,
        State_Attack1,
        State_Attack2,
        State_Attack3,
        State_Attack4,
        State_Attacking,
        State_Grab,
        State_InkAttack,
        State_GrabAndShake,
    }

    private Kraken_AnimStates _LastState = Kraken_AnimStates.State_Idle;

    [SerializeField, Tooltip("Properties of the entity's")] EntityProperties _Properties;

    [SerializeField] float _NoticeDistance = 20f;

    [SerializeField] Sprite _DeathSprite;

    [SerializeField] AudioSource _Source;

    [SerializeField] GameObject _KrakenCanvas;

    [SerializeField] Animator _Animator;

    [SerializeField] GameObject GrabParent;
    [SerializeField] GameObject InkPrefab;
    [SerializeField] List<AudioClip> _Clips = new List<AudioClip>();
    Vector3 m_Velocity = Vector3.zero;


    float _Health = 100f;

    float _IgnoreEntitesDuration = 0;

    bool _isEntitySeen = false;

    bool SpearDamage = false;

    float AnimWaitDuration = 0;

    float GrabDuration = 0;
    float InkDuration = 0;

    public void Init(EntityProperties _properties)
    {
        _Health = _properties.Health;
        Type = EntityType.Type_Kraken;
    }

    void Start()
    {
        Instance = this;
        Init(_Properties);
    }


    void Update()
    {
        if (!_isEntitySeen) return;
        if (SpearDamage && Player.Instance._Spear.ThrowState == Spear.ThrowStates.STATE_NONE) SpearDamage = false;


        float dist = Vector2.Distance(transform.position, Player.Instance.transform.position);

        if (dist < 20 && AnimWaitDuration < Time.time && GrabDuration < Time.time)
        {
            SetAnimState(Kraken_AnimStates.State_Grab);
        }

        else if (dist <= 22.5f && AnimWaitDuration < Time.time)
        {
            AnimWaitDuration = Time.time + 2;
            int randomindex = UnityEngine.Random.Range(0, 5);
            if (randomindex == 1) SetAnimState(Kraken_AnimStates.State_Attack2);
            if (randomindex == 2) SetAnimState(Kraken_AnimStates.State_Attack3);
            if (randomindex == 3) SetAnimState(Kraken_AnimStates.State_Attack4);
            else SetAnimState(Kraken_AnimStates.State_Attack1);
        }

        else if (dist > 22.5f && AnimWaitDuration < Time.time)
        {
            AnimWaitDuration = Time.time + 2;
            SetAnimState(Kraken_AnimStates.State_InkAttack);
        }

        Debug.Log("Player & Kraken dist: " + dist);
    }

    public void SetAnimState(Kraken_AnimStates state)
    {
        if (_LastState == state && state != Kraken_AnimStates.State_InkAttack)
        {
            AnimWaitDuration -= Time.deltaTime * 3;
            return;
        }

        _LastState = state;

        switch (state)
        {
            case Kraken_AnimStates.State_Attack1:
                {
                    _Animator.ResetTrigger("Attack_1");
                    _Animator.SetTrigger("Attack_1");
                    break;
                }

            case Kraken_AnimStates.State_Attack2:
                {
                    _Animator.ResetTrigger("Attack_2");
                    _Animator.SetTrigger("Attack_2");
                    break;
                }

            case Kraken_AnimStates.State_Attack3:
                {
                    _Animator.ResetTrigger("Attack_3");
                    _Animator.SetTrigger("Attack_3");
                    break;
                }

            case Kraken_AnimStates.State_Attack4:
                {
                    _Animator.ResetTrigger("Attack_4");
                    _Animator.SetTrigger("Attack_4");
                    break;
                }

            case Kraken_AnimStates.State_Grab:
                {
                    _Animator.ResetTrigger("grab");
                    _Animator.SetTrigger("grab");
                    break;
                }

            case Kraken_AnimStates.State_InkAttack:
                {
                    _Animator.ResetTrigger("Attack_ink");
                    _Animator.SetTrigger("Attack_ink");
                    if (InkDuration < Time.time)
                    {
                        Instantiate(InkPrefab, transform.position, Quaternion.identity);
                        InkDuration = Time.time + 3f;
                    }
                    break;
                }

            case Kraken_AnimStates.State_GrabAndShake:
                {
                    _Animator.ResetTrigger("Attack_shake");
                    _Animator.SetTrigger("Attack_shake");
                    break;
                }



            default: break;
        }
    }

    public void KrakenArm(Transform _transform, Transform _hitTransform, string tag)
    {
        if (tag == "Weapon")
        {
            Debug.Log("Arm Wep");
            if (Player.Instance._Spear.ThrowState == Spear.ThrowStates.STATE_NONE || SpearDamage) return;
            Player.Instance._Spear.ThrowState = Spear.ThrowStates.STATE_OVERLAPPED;
            Player.Instance._Spear.transform.SetParent(_transform);
            OnTakeDamage(15);
            SpearDamage = true;
        }
        else if (tag == "Harmful")
        {
            Destroy(_hitTransform.gameObject);
            OnTakeDamage(2f);
            Debug.Log("harmful");
        }
        else //player
        {
            if (_LastState == Kraken_AnimStates.State_Grab)
            {

                Player.Instance.transform.position = GrabParent.transform.position;
                Player.Instance.transform.SetParent(GrabParent.transform);
                Player.Instance.CanMove = false;
                SetAnimState(Kraken_AnimStates.State_GrabAndShake);
                GrabDuration = Time.time + 20;
            }
            else Player.Instance.OnTakeDamage(5f);
        }

    }




    public void KrakenArea()
    {
        _isEntitySeen = true;
        Player.Instance.LockLensSize = true;
        _KrakenCanvas.SetActive(true);
    }



    public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
    {
        Vector3 euler = transform.eulerAngles;
        Vector2 force = new Vector2(0, 0);
        force.x = (euler.y == 0) ? 2000 : -2000;
        Player.Instance._Rigidbody.AddForce(force, ForceMode2D.Impulse);

        entity.OnTakeDamage(damage / 4, type);
        _IgnoreEntitesDuration = 2f + Time.time;
        _isEntitySeen = false;
        PlaySound(0);
    }

    void PlaySound(int index)
    {
        if (isDeath) return;
        _Source.clip = _Clips[index];
        _Source.Play();
    }


    public void OnDetected(Entity _entity)
    {
        _isEntitySeen = true;

    }

    public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {
        if (type == AttackTypes.Attakc_Tornado) return;
        Debug.Log("Kraken Takes damage");
        if (isDeath) return;
        _IgnoreEntitesDuration -= 0.5f;
        if (_IgnoreEntitesDuration < Time.time && type != AttackTypes.Attakc_Tornado)
        {
            OnDetected(Player.Instance);
        }
        if (type != AttackTypes.Attakc_Tornado && !Player.Instance._Rage) Player.Instance.Focus += 5;
        _Health -= _h / 3;
        if (_Health < 0) OnDeath();

        _HealthBar.value = _Health / 100;

        StartCoroutine(DamageEffect());
    }

    public override void OnDeath()
    {
        if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);

        isDeath = true;
        gameObject.AddComponent<Destroyer>();
        if (HealthBarCoroutine != null) StopCoroutine(HealthBarCoroutine);
        Destroy(_HealthBar.gameObject);
        SceneManager.LoadScene("Thank");
    }

    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }

    IEnumerator DamageEffect()
    {
        int randomindex = UnityEngine.Random.Range(1, _Clips.Count);
        PlaySound(randomindex);

        var Renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var _renderer in Renderers)
        {
            _renderer.color = Color.red;
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var _renderer in Renderers)
        {
            _renderer.color = Color.white;
        }
        yield return null;
    }
}
