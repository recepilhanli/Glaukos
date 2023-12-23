using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





public abstract class Entity : MonoBehaviour
{

    [Header("Entity Presets")]
    [SerializeField] protected Slider _HealthBar = null;

    [HideInInspector] public bool isDeath { get; protected set; } = false;

    protected Coroutine HealthBarCoroutine = null;

    [HideInInspector] public EntityType Type { get; protected set; }


    public enum AttackTypes
    {
        Attack_Standart,
        Attack_Explosion,
        Attack_Rapid,
        Attack_Heavy,
        Attack_Tornado,
        Attack_Rain
    }

    public enum EntityFlags
    {
        Flag_None,
        Flag_Player,
        Flag_Enemy,
        Flag_Friendly,
        Flag_Neutral

    }
    public enum EntityType
    {
        Type_Player,
        Type_Drowned,
        Type_Shark,
        Type_Kraken,

        Type_JellyFish,
        Type_SwordFish,
    }



    public void OnHealthBarValueChanged()
    {
        if (HealthBarCoroutine != null) StopCoroutine(HealthBarCoroutine);
        HealthBarCoroutine = StartCoroutine(HealthBarEffect());

    }

    IEnumerator HealthBarEffect()
    {
        _HealthBar.transform.eulerAngles = Vector3.zero;
        _HealthBar.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        if (_HealthBar != null) _HealthBar.gameObject.SetActive(false);
        yield return null;
    }

    public virtual void OnDeath()
    {
        Debug.Log(gameObject.name + " is death.");
        Destroy(gameObject);
    }

    public virtual void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {
        Debug.Log(gameObject.name + " took a damage.");
    }

    public virtual void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
    {
        Debug.Log($"{gameObject.name} attacked to {entity.gameObject} with {type}");
    }

    public virtual void Move(Vector2 pos)
    {
        Debug.Log($"{gameObject.name} moved to {pos}");
    }

    public abstract EntityFlags GetEntityFlag();

}




