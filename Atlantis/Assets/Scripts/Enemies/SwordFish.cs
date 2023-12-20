using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;

/// <summary>
/// this class is responsible for the swordfish enemy
/// </summary>
public class SwordFish : Entity, IEnemyAI
{

    [SerializeField, Tooltip("Properties of the entity's")] EntityProperties _Properties;

    [SerializeField] SpriteRenderer _Renderer;

    [SerializeField] Animator _Animator;

    [SerializeField] float _NoticeDistance = 20f;

    [SerializeField] float _SeenDuration = 1f;

    [SerializeField] float MaxRoamingDistance = 40f;

    [Space, SerializeField] TrailRenderer _TrailRenderer;

    [SerializeField] AudioSource _Source;

    [SerializeField] List<AudioClip> _Clips = new List<AudioClip>();

    Vector3 m_Velocity = Vector3.zero;

    public Vector2 _RoamingPos = Vector2.zero;

    float _DamageForPerAttack = 1f;

    float _Health = 100f;

    float _IgnoreEntitesDuration = 0;

    bool _isEntitySeen = false;

    private Vector3 _MoveNormal = Vector3.zero;

    private float _StartY = 0;



    public void Init(EntityProperties _properties)
    {
        _Health = _properties.Health;
        _DamageForPerAttack = _properties.Damage;
        Type = EntityType.Type_SwordFish;
        _StartY = transform.position.y;
    }

    void Start()
    {
        Init(_Properties);
        _TrailRenderer.enabled = false;
    }




    void Update()
    {

        if (isDeath)
        {
            transform.Translate(0, -1 * Time.fixedDeltaTime, 0);
            var _color = _Renderer.color;
            _color.a -= Time.deltaTime / 4;
            _Renderer.color = _color;
            return;
        }
        if (LevelManager.Instance.GravityScale != 0 && _Properties.CanSwim) OnDeath();

        var _entity = Player.Instance; //Could be changed with GetNearestEntity()

        if (!_isEntitySeen)
        {
            if (Mathf.Abs(transform.position.x - _entity.transform.position.x) < _NoticeDistance && _IgnoreEntitesDuration < Time.time) OnDetected(_entity);
            else
            {
                if (_RoamingPos != Vector2.zero)
                {
                    Move((_RoamingPos - (Vector2)transform.position).normalized * 2);
                    if (Vector2.Distance(transform.position, _RoamingPos) < 2f) _RoamingPos = Vector2.zero;
                }
                else
                {
                    Debug.Log("New Roaming Pos");
                    Vector3 right = ((_RoamingPos - (Vector2)transform.position).normalized.x >= 0) ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
                    _RoamingPos = -right * MaxRoamingDistance + new Vector3(transform.position.x, _StartY, 0);
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
                Debug.Log($"Velocity: {m_Velocity.magnitude}");

                if (Vector2.Distance(_entity.transform.position, transform.position) < 2f) Attack(_entity, _DamageForPerAttack);
                else Move(_MoveNormal * 4);


            }
        }
    }


    public override void Move(Vector2 pos)
    {
        if (pos.x < 0) _Renderer.flipY = true;
        else _Renderer.flipY = false;

        transform.up = (Vector3)pos;

        float speed = (_isEntitySeen) ? _Properties.Speed * 40 : _Properties.Speed * 2.5f;
        transform.position = Vector3.SmoothDamp(transform.position, transform.position + (Vector3)pos, ref m_Velocity, 1 / speed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_isEntitySeen && other.gameObject.layer == 0)
        {
            _isEntitySeen = false;
            Debug.Log("Stop");
        }
        else if (other.gameObject.layer == 0) _RoamingPos = Vector2.zero;
    }


    public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
    {
        if (isDeath) return;
        Vector3 euler = transform.eulerAngles;
        Vector2 force = new Vector2(0, 0);
        force.x = (euler.y == 0) ? 2000 : -2000;
        Player.Instance._Rigidbody.AddForce(force, ForceMode2D.Impulse);

        entity.OnTakeDamage(damage, type);
        _IgnoreEntitesDuration = 2f + Time.time;
        Invoke("SetCalm", 3f);

        PlaySound(0);
    }

    /// <summary>
    /// Sets the enemy to calm state
    /// </summary>
    void SetCalm()
    {
        if (isDeath) return;
        _isEntitySeen = false;
        _TrailRenderer.enabled = false;
    }

    void PlaySound(int index)
    {
        if (isDeath) return;
        _Source.clip = _Clips[index];
        _Source.Play();
    }


    public void OnDetected(Entity _entity)
    {
        Debug.Log("Detected");
        _MoveNormal = (_entity.transform.position - transform.position).normalized;
        _SeenDuration = Time.time + 3;
        _isEntitySeen = true;
        _RoamingPos = Vector2.zero;
        _TrailRenderer.enabled = true;
    }


    public override void OnDeath()
    {
        if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);

        _Animator.enabled = false;
        _Renderer.flipY = false;
        transform.eulerAngles = Vector3.zero;
        transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        _Renderer.color = Color.gray;
        _TrailRenderer.enabled = false;
        isDeath = true;
        gameObject.AddComponent<Destroyer>();
        if (HealthBarCoroutine != null) StopCoroutine(HealthBarCoroutine);
        Destroy(_HealthBar.gameObject);
        GetComponent<PolygonCollider2D>().enabled = false;

    }

    public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {
        if (isDeath) return;
        _IgnoreEntitesDuration -= 0.5f;
        if (_IgnoreEntitesDuration < Time.time && type != AttackTypes.Attack_Tornado)
        {
            OnDetected(Player.Instance);
        }
        if (type != AttackTypes.Attack_Tornado && !Player.Instance._Rage) Player.Instance.Focus += 5;
        _Health -= _h;
        if (_Health < 0) OnDeath();

        _HealthBar.value = _Health / 100;

        var pos = Vector3.zero;
        pos = (_Renderer.flipX) ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
        transform.position -= pos * 1.5f;


        StartCoroutine(DamageEffect());
    }


    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }

    IEnumerator DamageEffect()
    {
        int randomindex = UnityEngine.Random.Range(1, _Clips.Count);
        PlaySound(randomindex);

        _Renderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _Renderer.color = Color.white;
        yield return null;
    }
}
