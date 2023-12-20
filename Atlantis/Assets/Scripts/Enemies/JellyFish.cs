using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;

/// <summary>
/// this class is responsible for the jellyfish enemy
/// </summary>
public class JellyFish : Entity, IEnemyAI
{

    private bool isPlayerInJellyFish = false;

    private Coroutine _JellyFishAttackCoroutine = null;

    IEnumerator JellyFishAttack()
    {
        while (isPlayerInJellyFish)
        {
            Player.Instance.OnTakeDamage(2);
            Player.Instance.PosionEffect();
            yield return new WaitForSeconds(1f);
        }

        StopCoroutine(_JellyFishAttackCoroutine);
        _JellyFishAttackCoroutine = null;

    }


    void Start()
    {
        Init(null);
    }

    void Update()
    {


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
