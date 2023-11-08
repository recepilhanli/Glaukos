using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;

public class Drowned : Entity, IEnemyAI
{
    // Start is called before the first frame update
    public bool isPlayerSeen = false;
    public float NoticeDistance = 10f;

    public float SeenDuration = 0f;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var player = Player.Instance;
        if (!isPlayerSeen)
        {
            if (Vector2.Distance(player.transform.position, transform.position) < NoticeDistance) OnPlayerDetected();
        }
        else
        {
            if (SeenDuration < Time.time && Vector2.Distance(player.transform.position, transform.position) > NoticeDistance)
            {
                isPlayerSeen = false;
            }
            else
            {
                if (Vector2.Distance(player.transform.position, transform.position) < 1.3f) Attack(player, 10f);
                else transform.Translate(-(transform.position - player.transform.position) * Time.deltaTime * 5);
            }
        }
    }


    public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
    {
        entity.TakeDamage(damage, type);

    }

    public void OnPlayerDetected()
    {
        SeenDuration = Time.time + 10f;
        isPlayerSeen = true;
    }

    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }
}
