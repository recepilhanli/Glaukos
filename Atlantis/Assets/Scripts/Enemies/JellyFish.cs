using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// this class is responsible for the jellyfish enemy
/// </summary>
public class JellyFish : Entity, IEnemyAI
{

    private bool isPlayerInJellyFish = false;

    private Coroutine _JellyFishAttackCoroutine = null;

    [SerializeField] Rigidbody2D _RigidBody;

    private float _YPos;
    private float _Val;
    private bool _Pong = false;

    IEnumerator JellyFishAttack()
    {
        Player.Instance.SetSlow(true);
        while (isPlayerInJellyFish)
        {
            Player.Instance.OnTakeDamage(2);
            Player.Instance.PosionEffect();
            yield return new WaitForSeconds(1f);
        }

        StopCoroutine(_JellyFishAttackCoroutine);
        Player.Instance.SetSlow(false);
        _JellyFishAttackCoroutine = null;

    }

    
    void Start()
    {
        _YPos = transform.position.y;
        Init(null);
    }


    void Update()
    {
       if(_Pong)
        {
            _Val += Time.deltaTime;
            if (_Val > 4) _Pong = false;
        }
        else 
        {
            _Val -= Time.deltaTime;
            if (_Val < -4) _Pong = true;
        }

        transform.position = new Vector3(transform.position.x, _YPos + _Val, 0);

        if (Vector2.Distance(transform.position, Player.Instance.transform.position) < 2f && _JellyFishAttackCoroutine == null)
        {
            isPlayerInJellyFish = true;
            _JellyFishAttackCoroutine = StartCoroutine(JellyFishAttack());
        }
        else if (Vector2.Distance(transform.position, Player.Instance.transform.position) >= 2f)
        {
            isPlayerInJellyFish = false;
        }


    }

    //unused
    public void Init(EntityProperties _properties)
    {
        Type = EntityType.Type_JellyFish;
    }

    //unused
    public void OnDetected(Entity _entity)
    {

    }

    public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {
        if (type == AttackTypes.Attack_Tornado) Destroy(gameObject);
    }


    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }
}
