using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    void Start()
    {
        var Entities = FindObjectsOfType<Entity>();
        foreach (var entity in Entities)
        {
            if(entity.isDeath) continue;

            float distance = Vector2.Distance(transform.position, entity.transform.position);
            if (distance < 10f)
            {
                entity.OnTakeDamage(2 + 1/distance * 100);
                if(entity == Player.Instance) Player.Instance.PosionEffect(1.3f);
            }
        }

       if(Vector2.Distance(transform.position, Player.Instance.transform.position) <= 15) Player.Instance.CameraShake(2.5f,1,1.25f,true);

        Destroy(gameObject,5f);
    }

}
