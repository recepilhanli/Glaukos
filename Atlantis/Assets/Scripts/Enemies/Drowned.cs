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


    bool _isEntitySeen = false;

    float DamageDuration = 0f;

    float _DamageForPerAttack = 1f;

    float _Health = 100f;

    float _Right = 1;

    private Vector3 m_Velocity = Vector3.zero;

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


        _Right = (_Right == 1) ? -1 : 1;

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


        _Health -= _h;
        if (_Health < 0) OnDeath();

        _HealthBar.value = _Health / 100;

        if (type != AttackTypes.Attack_Tornado && !Player.Instance._Rage) Player.Instance.Focus += 4;

        StartCoroutine(DamageEffect());
    }

    public override void OnDeath()
    {

        if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);

        Destroy(_HealthBar.gameObject);

        Destroy(gameObject);
    }

    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }


    IEnumerator DamageEffect()
    {
        var Renderers = DrownedSpriteParent.GetComponentsInChildren<SpriteRenderer>();
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
