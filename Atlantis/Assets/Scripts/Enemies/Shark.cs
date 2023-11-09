using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;
using Unity.Mathematics;


public class Shark : Entity, IEnemyAI
{

    [SerializeField, Tooltip("Properties of the entity's")] EntityProperties _Properties;

    [SerializeField] SpriteRenderer _Renderer;
    [SerializeField] float _NoticeDistance = 20f;

    [SerializeField] float _SeenDuration = 1f;

    [SerializeField] float MaxRoamingDistance = 40f;




    Vector3 m_Velocity = Vector3.zero;

    Vector2 _RoamingPos = Vector2.zero;

    float _DamageForPerAttack = 1f;

    float _Health = 100f;

    float _IgnoreEntitesDuration = 0;

    bool _isEntitySeen = false;

    public void Init(EntityProperties _properties)
    {
        _Health = _properties.Health;
        _DamageForPerAttack = _properties.Damage;
    }


    void Start()
    {
        Init(_Properties);
    }


    void Update()
    {
        if (LevelManager.Instance.GravityScale != 0 && _Properties.CanSwim) OnDeath();

        var _entity = Player.Instance; //Could be changed with GetNearestEntity()

        if (!_isEntitySeen)
        {
            if (Mathf.Abs(transform.position.x - _entity.transform.position.x) < _NoticeDistance && _IgnoreEntitesDuration < Time.time) OnDetected(_entity);
            else
            {
                if (_RoamingPos != Vector2.zero)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, _RoamingPos, ref m_Velocity, 1 / _Properties.Speed);
                    if (Vector2.Distance(transform.position, _RoamingPos) < 2f) _RoamingPos = Vector2.zero;

                }
                else
                {
                    _RoamingPos = -transform.right * MaxRoamingDistance + transform.position;
                    Vector3 euler = transform.eulerAngles;
                    if ((_RoamingPos.x - transform.position.x) < 0) euler.y = 180;
                    else euler.y = 0;
                    transform.eulerAngles = euler;
                }
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
                if (Vector2.Distance(_entity.transform.position, transform.position) < 2f) Attack(_entity, _DamageForPerAttack);
                else
                {
                    Vector3 euler = transform.eulerAngles;
                    if ((_entity.transform.position.x - transform.position.x) < 0) euler.y = 180;
                    else euler.y = 0;
                    transform.eulerAngles = euler;

                    transform.position = Vector2.Lerp(transform.position, _entity.transform.position, _Properties.Speed / 2);

                }
            }
        }
    }


    public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
    {
        Vector3 euler = transform.eulerAngles;
        Vector2 force = new Vector2(0, 0);
        force.x = (euler.y == 0) ? 2000 : -2000;
        Player.Instance._Rigidbody.AddForce(force, ForceMode2D.Impulse);

        entity.OnTakeDamage(damage, type);
        _IgnoreEntitesDuration = 2f + Time.time;
        _isEntitySeen = false;
    }

    public void OnDetected(Entity _entity)
    {
        _SeenDuration = Time.time + 10f;
        _isEntitySeen = true;
        _RoamingPos = Vector2.zero;
    }

    public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {
        _IgnoreEntitesDuration -= 0.5f;
        Player.Instance.Focus += 15;
        _Health -= _h;
        if (_Health < 0) OnDeath();

        StartCoroutine(DamageEffect());
    }

    public override void OnDeath()
    {
        if(Player.Instance._Spear._ThrowState != 0) Player.Instance._Spear.GetBackToThePlayer(false);

        Destroy(gameObject);
    }

    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }

    IEnumerator DamageEffect()
    {
        _Renderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _Renderer.color = Color.white;
        yield return null;
    }
}
