using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;

/// <summary>
/// explosion class
/// </summary>
public class Explosion : MonoBehaviour
{

    
    void Start()
    {
        Explode();
    }


    public void Explode()
    {
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        transform.SetParent(null);

        var Entities = FindObjectsOfType<Entity>();
        foreach (var entity in Entities)
        {
            if (entity.isDeath) continue;

            float distance = Vector2.Distance(transform.position, entity.transform.position);
            if (distance < 10f)
            {
                entity.OnTakeDamage(2 + 1 / distance * 100);
                if (entity == Player.Instance) Player.Instance.PosionEffect(1.35f);
            }
        }
        float playerdist = Vector2.Distance(transform.position, Player.Instance.transform.position);
        if (playerdist <= 16) Player.Instance.CameraShake(0.6f + Mathf.Clamp((1 / playerdist * 6), 0, 4f), 1.5f, 0.9f, true);

        Destroy(gameObject, 5f);
    }
}
