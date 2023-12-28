using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;


public class PufferFish : Entity, IEnemyAI
{

    //no need properties

    [SerializeField] SpriteRenderer _Renderer;
    [SerializeField] float _NoticeDistance = 20f;

    [SerializeField] float _MaxRoamingDistance = 40f;

    [Space, SerializeField] GameObject ExplosionPrefab;

    [SerializeField] TrailRenderer _TrailRenderer;

    [SerializeField] Sprite _ExplodingSprite;

    private Sprite _RegularSprite;

    Vector3 m_Velocity = Vector3.zero;

    Vector2 _RoamingPos = Vector2.zero;

    bool _isEntitySeen = false;

    bool _Exploding = false;

    public void Init(EntityProperties _properties)
    {
        Type = EntityType.Type_PufferFish;
    }


    void Start()
    {
        Init(null);
        _RegularSprite = _Renderer.sprite;
        _TrailRenderer.enabled = false;
    }


    void Update()
    {

        if (isDeath)
        {
            transform.Translate(0, -1 * Time.fixedDeltaTime, 0);
            var _color = _Renderer.color;
            _color.a -= Time.deltaTime;
            _Renderer.color = _color;
            if (transform.localScale.x >= 0) transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * 4;
            return;
        }

        else if (_Exploding)
        {
            transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * 1.5f;
            return;
        }

        transform.localScale = new Vector3(Mathf.PingPong(Time.time * 1.5f, 1) + 2.25f, Mathf.PingPong(Time.time * 1.5f, 1) + 2.25f, 1);

        if (LevelManager.Instance.GravityScale != 0) OnDeath();

        var _entity = Player.Instance; //Could be changed with GetNearestEntity()

        if (!_isEntitySeen)
        {
            if (Mathf.Abs(transform.position.x - _entity.transform.position.x) < _NoticeDistance) OnDetected(_entity);
            else
            {
                if (_RoamingPos != Vector2.zero)
                {
                    Move(_RoamingPos);
                    if (Vector2.Distance(transform.position, _RoamingPos) < 2f) _RoamingPos = Vector2.zero;
                }
                else
                {
                    Vector3 right = (_Renderer.flipX) ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
                    _RoamingPos = -right * _MaxRoamingDistance + transform.position;
                }
            }
        }
        else
        {
            if (Vector2.Distance(_entity.transform.position, transform.position) < 2f) Attack(_entity, 1);
            else Move(_entity.transform.position);
        }
    }


    public override void Move(Vector2 pos)
    {
        if ((pos.x - transform.position.x) < 0) _Renderer.flipX = true;
        else _Renderer.flipX = false;

        float speed = (_isEntitySeen) ? 0.15f * 25 : 0.15f * 2.5f;
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref m_Velocity, 1 / speed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 0) _RoamingPos = Vector2.zero;
    }


    public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
    {
        if (isDeath || _Exploding) return;
        Explode();
    }


    public void OnDetected(Entity _entity)
    {
        _isEntitySeen = true;
        _RoamingPos = Vector2.zero;
        _TrailRenderer.enabled = true;
    }


    IEnumerator ExplodeCoroutine()
    {
        _Exploding = true;
        yield return new WaitForSeconds(0.5f + Random.Range(0, 0.5f));
        _Renderer.sprite = _ExplodingSprite;
        _TrailRenderer.enabled = false;
        yield return new WaitForSeconds(0.5f + Random.Range(0, 1));
        OnDeath();
    }

    public void Explode()
    {
        if (_Exploding) return;
        StartCoroutine(ExplodeCoroutine());
    }

    public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {
        if (isDeath || _Exploding) return;

        if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);
        WarnOthers();
        Explode();

    }





    void WarnOthers()
    {
        var entities = FindObjectsOfType<Entity>();
        //warn other pufferfishes
        foreach (var entity in entities)
        {
            if (entity == this) continue;
            if (entity.Type != EntityType.Type_PufferFish) continue;
            if (Vector2.Distance(transform.position, entity.transform.position) <= 30)
            {
                entity.GetComponent<PufferFish>()?.OnDetected(Player.Instance);
            }
        }
    }

    public override void OnDeath()
    {
        if (isDeath) return;
        if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);

        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);

        _TrailRenderer.enabled = false;
        isDeath = true;
        gameObject.AddComponent<Destroyer>();
    }

    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }


}
