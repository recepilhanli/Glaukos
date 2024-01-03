using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;

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
    }


    [SerializeField] float _Health = 300f;

    [SerializeField] Rigidbody2D _Rigidbody;
    [SerializeField] Animator _Animator;
    [SerializeField] Transform _AttachingBone;
    [SerializeField] Transform _HeadTransform;
    [Space(15), SerializeField] List<GameObject> _CallableEnemies = new List<GameObject>();
    [SerializeField] GameObject _PoisonPrefab;
    [SerializeField, ReadOnlyInspector] SkillaStates _CurrentState = SkillaStates.State_NONE;
    private bool _isEntitySeen = false;
    private Vector2 m_Velocity = Vector2.zero;

    void Start()
    {

    }


    void Update()
    {

        if (!_isEntitySeen)
        {
            if (Vector2.Distance(transform.position, Player.Instance.transform.position) <= 25f)
            {
                OnDetected(Player.Instance);
            }
            return;
        }

        if (isDeath) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            SetState(SkillaStates.State_CallEnemies);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            SetState(SkillaStates.State_Grabbing);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            SetState(SkillaStates.State_NONE);
        }

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
        int rand = Random.Range(0, _CallableEnemies.Count);
        var enemy = Instantiate(_CallableEnemies[rand], transform.position, Quaternion.identity).GetComponent<Entity>();
        if (enemy != null) StartCoroutine(EnemyCoroutine(enemy));
    }

    IEnumerator EnemyCoroutine(Entity enemy)
    {
        yield return new WaitForSeconds(12);
        if (enemy != null) enemy.OnDeath();
        yield return null;
    }

    public void Init(EntityProperties _properties)
    {

    }

    public void OnDetected(Entity _entity)
    {
        _isEntitySeen = true;
    }

    public void SetState(SkillaStates state)
    {
        if (_CurrentState == state) return;

        if (_CurrentState == SkillaStates.State_Grabbed)
        {
            Player.Instance.transform.SetParent(null);
            Player.Instance.CanMove = true;
            Player.Instance.transform.rotation = Quaternion.identity;
            Player.Instance.transform.transform.eulerAngles = Vector3.zero;
            Player.Instance.transform.localScale = new Vector3(.75F, .75F, .75F);
        }

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

            default: break;
        }
    }


    public override void Move(Vector2 pos)
    {
        if (transform.position.x - pos.x < 0) transform.localScale = new Vector3(-6, 6, 6);
        else transform.localScale = new Vector3(6, 6, 6);

        float speedMultiplier = 1f;
        if (_Health <= 50) speedMultiplier = 0.4f;

        _Rigidbody.MovePosition(Vector2.SmoothDamp(transform.position, pos, ref m_Velocity, 0.3f * speedMultiplier));
    }

    public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
    {
        if (Vector2.Distance(transform.position, Player.Instance.transform.position) <= 1.9) Player.Instance.OnTakeDamage(damage, type);
    }

    public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {


        if (isDeath) return;
        _Health -= _h / 8;
        if (_Health <= 0)
        {
            OnDeath();
        }

    }

    public override void OnDeath()
    {
        isDeath = true;
    }

    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }
}
