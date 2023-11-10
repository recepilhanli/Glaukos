using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Spear : MonoBehaviour
{

    public ThrowStates _ThrowState { get; private set; } = ThrowStates.STATE_NONE;

    private Vector2 _ThrowedPosition = Vector2.zero;

    [HideInInspector] public Vector2 SpearOffset = Vector2.zero;


    public enum ThrowStates
    {
        STATE_NONE,
        STATE_THROWING,
        STATE_OVERLAPPED,
        STATE_FLOATING,
        STATE_GETTING_BACK,
    }

    private void Start()
    {
        SpearOffset = transform.localPosition;
    }

    public void Throw(Vector2 pos)
    {
        transform.SetParent(null);
        _ThrowedPosition = pos;

        var euler = transform.eulerAngles;
        float tempY = (pos.x > 0) ? 0 : 180;
        euler.y = tempY;
        transform.eulerAngles = euler;

        _ThrowState = ThrowStates.STATE_THROWING;
    }

    public void GetBackToThePlayer(bool _instantly = false)
    {
        if (_instantly)
        {
            transform.SetParent(Player.Instance._RightHand);
            transform.localPosition = SpearOffset;
            _ThrowState = 0;
            var euler = Vector3.zero;
            float TempY = Player.Instance.transform.eulerAngles.y;
            euler.y = TempY;
            euler.z = -90;
            transform.eulerAngles = euler;
        }
        else
        {
            transform.SetParent(null);
            _ThrowState = ThrowStates.STATE_GETTING_BACK;
        }
    }



    void CollisionReaction(GameObject other)
    {
        if (_ThrowState == 0) return;
        var _other = other.GetComponent<Collider2D>();
        if (_other == null) return;

        else if (_other.isTrigger) return;

        if (other.CompareTag("Player") && _ThrowState != ThrowStates.STATE_THROWING)
        {
            GetBackToThePlayer(true);
            return;
        }
        else if (other.CompareTag("Player") && _ThrowState == ThrowStates.STATE_THROWING) return;

        if (_ThrowState == ThrowStates.STATE_GETTING_BACK) return;


        Debug.Log(other + " -> throwing hit");
        _ThrowState = ThrowStates.STATE_OVERLAPPED;


        if (other.CompareTag("Puzzle"))
        {
            return;
        }

        SendDamage(20f, true,_other);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        CollisionReaction(other.gameObject);
    }


    void Update()
    {
        if (_ThrowState != ThrowStates.STATE_THROWING && _ThrowState != ThrowStates.STATE_FLOATING && _ThrowState != ThrowStates.STATE_GETTING_BACK) return;

        if (Vector2.Distance(Player.Instance.transform.position, transform.position) <= 1.5f && _ThrowState == ThrowStates.STATE_GETTING_BACK)
        {
            GetBackToThePlayer(true);
            return;
        }
        else if (_ThrowState == ThrowStates.STATE_GETTING_BACK)
        {
            _ThrowedPosition = Player.Instance.transform.position;
            var euler = transform.eulerAngles;
            float tempY = (_ThrowedPosition.x > 0) ? 180 : 0;
            euler.y = tempY;
            transform.eulerAngles = euler;

            var lerpedPos = Vector3.MoveTowards(transform.position, _ThrowedPosition, 0.5f);
            transform.position = lerpedPos;
            transform.Rotate(0, 0, Time.deltaTime * 1000);
            return;
        }


        if (Vector2.Distance(_ThrowedPosition, transform.position) > 1 && _ThrowState != ThrowStates.STATE_FLOATING)
        {
            var lerpedPos = Vector3.Lerp(transform.position, _ThrowedPosition, Time.deltaTime * 3);
            transform.position = lerpedPos;
        }
        else
        {
            Debug.Log("Floating");
            _ThrowState = ThrowStates.STATE_FLOATING;
            transform.Rotate(0, 0, Time.deltaTime * 30);
        }

    }

    public void SendDamage(float damage, bool attach = false, Collider2D other = null)
    {
        if (damage == 0) return;
        Debug.Log("overlapping");

        if (other == null) other = Physics2D.OverlapCircle(transform.position, 1.5f, Player.Instance.EnemyMask);

        if (!other) return;

        Debug.Log("Overallped");

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Puzzle"))
        {
            var entity = other.gameObject.GetComponent<Entity>();

            if (entity != null)
            {
                Debug.LogWarning("Damage entity");
                Player.Instance.Attack(entity, damage, Entity.AttackTypes.Attack_Standart);

                if (attach && entity != null && _ThrowState != ThrowStates.STATE_GETTING_BACK)
                {
                    _ThrowState = ThrowStates.STATE_OVERLAPPED;
                    transform.SetParent(entity.transform);
                }

            }
        }
    }


}
