using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;

public class Drowned : Entity, IEnemyAI
{

    [SerializeField, Tooltip("Properties of the entity's")] EntityProperties _Properties;

    [SerializeField] GameObject DrownedSpriteParent;
    [SerializeField] float _NoticeDistance = 10f;

    [SerializeField] float _SeenDuration = 0f;

    [SerializeField] Rigidbody2D _Rigidbody;

    [SerializeField] Animator _Animator;

    [SerializeField] GameObject _ExplodingBubble;
    [SerializeField] GameObject _ExplodingPrefab;
    [SerializeField] float _Health = 100f;

    bool _isEntitySeen = false;

    float DamageDuration = 0f;

    float _DamageForPerAttack = 1f;


    float _Right = 1;

    private Vector3 m_Velocity = Vector3.zero;

    float _RotateTime = 0f;

    bool _Exploding = false;

    float damageDelay = 0f;

    public void Init(EntityProperties _properties)
    {
        _Health = _properties.Health;
        _DamageForPerAttack = _properties.Damage;
        Type = EntityType.Type_Drowned;
    }

    void Start()
    {
        Init(_Properties);
    }




    void Update()
    {


        if (_Exploding)
        {
            ExplodingBehaviour();
            return;
        }



        var _entity = Player.Instance; //Could be changed with GetNearestEntity()

        if (!_isEntitySeen)
        {
            if (Vector2.Distance(_entity.transform.position, transform.position) < _NoticeDistance) OnDetected(_entity);
            else
            {
                Move(new Vector2(_Right * _Properties.Speed * Time.fixedDeltaTime, 0));
            }
        }
        else
        {
            if (_SeenDuration < Time.time && Vector2.Distance(_entity.transform.position, transform.position) > _NoticeDistance)
            {
                _isEntitySeen = false;
            }
            else
            {
                if (Vector2.Distance(_entity.transform.position, transform.position) < 1.3f && DamageDuration < Time.time) Attack(_entity, _DamageForPerAttack);
                else if (Vector2.Distance(_entity.transform.position, transform.position) > 1.4f)
                {
                    Move(new Vector2(Mathf.Sign(_entity.transform.position.x - transform.position.x) * _Properties.Speed * Time.fixedDeltaTime, 0));
                }
            }
        }

    }



    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Ground")) return;
        else if (other.gameObject.CompareTag("Props")) return;

        if (_RotateTime < Time.time)
        {
            _Right = (_Right == 1) ? -1 : 1;
            _RotateTime = Time.time + 0.5f;
        }
    }



    public override void Move(Vector2 pos)
    {
        _Animator.SetBool("isSeen", _isEntitySeen);
        _Rigidbody.velocity = Vector3.SmoothDamp(_Rigidbody.velocity, pos, ref m_Velocity, .05f);
        _Rigidbody.velocity = Vector2.ClampMagnitude(_Rigidbody.velocity, 25f);
        if (pos.x != 0)
        {
            var euler = DrownedSpriteParent.transform.eulerAngles;
            euler.y = (pos.x < 0) ? 0 : 180;
            DrownedSpriteParent.transform.eulerAngles = euler;
        }

    }




    public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
    {
        Vector3 euler = transform.eulerAngles;
        Vector2 force = new Vector2(0, 0);
        force.x = (euler.y == 0) ? 2000 : -2000;
        Player.Instance._Rigidbody.AddForce(force, ForceMode2D.Impulse);

        entity.OnTakeDamage(damage, type);
        DamageDuration = Time.time + 1.5f;

        _Animator.ResetTrigger("attack");
        _Animator.SetTrigger("attack");

    }

    public void OnDetected(Entity _entity)
    {
        _SeenDuration = Time.time + 3f;
        _isEntitySeen = true;
    }

    public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {

        if (damageDelay > Time.time) return;
        damageDelay = Time.time + 0.3f;

        if (_Exploding)
        {
            OnDeath();
            return;
        }

        _Health -= _h;
        Debug.Log($"{gameObject.name} took a damage. Health: {_Health} -> {_h}");
        if (_Health < 0) OnDeath();

        _HealthBar.value = _Health / 100;

        if (type != AttackTypes.Attack_Tornado && !Player.Instance._Rage) Player.Instance.Focus += 4;

        StartCoroutine(DamageEffect());
        Player.Instance.PlayHitClip();
    }

    private void OnDestroy()
    {
        if (Player.Instance != null) Player.Instance._Spear.GetBackToThePlayer(false);
    }

    public override void OnDeath()
    {
        isDeath = true;

        if (_Exploding) Instantiate(_ExplodingPrefab, transform.position, Quaternion.identity);

        if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);

        Destroy(_HealthBar.gameObject);

        Destroy(gameObject);
    }

    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }

    public void Explode()
    {
        damageDelay = Time.time + 1.25f;
        _ExplodingBubble.SetActive(true);
        _Exploding = true;
        _HealthBar.gameObject.SetActive(false);
        transform.Rotate(0, 0, Random.Range(0, 360));
        _Animator.SetTrigger("explode");
        StartCoroutine(ExplodeEffect());
    }

    IEnumerator ExplodeEffect()
    {
        yield return new WaitForSeconds(Random.Range(3.5f, 6f));

        if (isDeath) yield return null;

        OnDeath();

        yield return null;
    }
    void ExplodingBehaviour()
    {

        transform.Rotate(0, 0, 60 * Time.deltaTime);
        Move(transform.up * 600 * Time.deltaTime);

    }

    IEnumerator DamageEffect()
    {
        var Renderers = DrownedSpriteParent.GetComponentsInChildren<SpriteRenderer>();
        foreach (var _renderer in Renderers)
        {
            if (_renderer != null) _renderer.color = Color.red;
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var _renderer in Renderers)
        {
            if (_renderer != null) _renderer.color = Color.gray;
        }
        yield return null;
    }

}
