using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;
using Unity.Mathematics;
using UnityEditor.Build.Player;


public class Kraken : Entity, IEnemyAI
{

    [SerializeField, Tooltip("Properties of the entity's")] EntityProperties _Properties;

    [SerializeField] float _NoticeDistance = 20f;

    [SerializeField] Sprite _DeathSprite;

    [SerializeField] AudioSource _Source;

    [SerializeField] List<AudioClip> _Clips = new List<AudioClip>();

    [SerializeField] GameObject _KrakenCanvas;


    Vector3 m_Velocity = Vector3.zero;



    float _DamageForPerAttack = 1f;

    float _Health = 100f;

    float _IgnoreEntitesDuration = 0;

    bool _isEntitySeen = false;

    public void Init(EntityProperties _properties)
    {
        _Health = _properties.Health;
        _DamageForPerAttack = _properties.Damage;
        Type = EntityType.Type_Kraken;
    }

    public void KrakenArea()
    {
        _isEntitySeen = true;
        Player.Instance.LockLensSize = true;
        _KrakenCanvas.SetActive(true);
    }


    void Start()
    {

    }


    void Update()
    {
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
