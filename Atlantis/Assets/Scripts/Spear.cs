using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using Unity.VisualScripting;
using UnityEngine;

public class Spear : MonoBehaviour
{
    public Animator _Animator;

    public byte _ThrowState { get; private set; } = 0;

    private Vector2 _ThrowedPosition = Vector2.zero;

    [HideInInspector] public Vector2 SpearOffset = Vector2.zero;

    private void Start()
    {
        SpearOffset = transform.parent.localPosition;
    }

    public void Throw(Vector2 pos)
    {
        _ThrowedPosition = pos;

        var euler = transform.parent.eulerAngles;
        float tempY = (pos.x > 0) ? 0 : 180;
        euler.y = tempY;
        transform.parent.eulerAngles = euler;

        _ThrowState = 1;
        _Animator.ResetTrigger("Attack_Light");
        _Animator.ResetTrigger("Attack_Heavy");

    }

    public void GetBackToThePlayer(bool _instantly = false)
    {
        if (_instantly)
        {
            transform.parent.SetParent(Player.Instance.transform);
            transform.parent.localPosition = SpearOffset;
            _ThrowState = 0;
            var euler = transform.parent.eulerAngles;
            float TempY = Player.Instance.transform.eulerAngles.y;
            euler.y = TempY;
            transform.parent.eulerAngles = euler;
        }
        else
        {
            transform.parent.SetParent(null);
            _ThrowState = 5;

        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_ThrowState == 0) return;
        Debug.LogWarning(other.gameObject.tag);
        if (other.gameObject.CompareTag("Player") && _ThrowState == 1) return;
        else if (other.isTrigger) return;

        if (other.gameObject.CompareTag("Player"))
        {
            GetBackToThePlayer(true);
            return;
        }
        if (_ThrowState == 5) return;



        Debug.Log(other.gameObject + " -> throwing hit");
        _ThrowState = 2;


        if (other.gameObject.CompareTag("Puzzle"))
        {
            return;
        }

        SendDamage(20f, true);
    }


    void Update()
    {
        if (_ThrowState != 1 && _ThrowState != 3 && _ThrowState != 5) return;

        if (Vector2.Distance(Player.Instance.transform.position, transform.parent.position) <= 1.5f && _ThrowState == 5)
        {
            GetBackToThePlayer(true);
            return;
        }
        else if (_ThrowState == 5)
        {
            _ThrowedPosition = Player.Instance.transform.position;
            var euler = transform.parent.eulerAngles;
            float tempY = (_ThrowedPosition.x > 0) ? 180 : 0;
            euler.y = tempY;
            transform.parent.eulerAngles = euler;

            var lerpedPos = Vector3.MoveTowards(transform.parent.position, _ThrowedPosition, 0.5f);
            transform.parent.position = lerpedPos;
            return;
        }


        if (Vector2.Distance(_ThrowedPosition, transform.parent.position) > 1 && _ThrowState != 3)
        {
            var lerpedPos = Vector3.Lerp(transform.parent.position, _ThrowedPosition, Time.deltaTime * 3);
            transform.parent.position = lerpedPos;
        }
        else if (LevelManager.Instance.GravityScale != 0)
        {
            Debug.Log("Falling");
            _ThrowState = 3;
            var pos = transform.parent.position;
            pos -= new Vector3(0, 1 * Time.deltaTime * 4, 0);
            transform.parent.position = pos;
        }
        else
        {
            _ThrowState = 3;
        }

    }

    public void SendDamage(float damage, bool attach = false)
    {
        if (damage == 0) return;
        Debug.LogWarning("Damage");

        var other = Physics2D.OverlapCircle(transform.position, 1, Player.Instance.EnemyMask);

        if (!other) return;

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Puzzle"))
        {
            var entity = other.gameObject.GetComponent<Entity>();
            Debug.LogWarning("Damage object");
            if (entity != null)
            {
                Debug.LogWarning("Damage entity");
                Player.Instance.Attack(entity, damage, Entity.AttackTypes.Attack_Standart);

                if (attach && entity != null && _ThrowState != 5)
                {
                    _ThrowState = 2;
                    transform.parent.SetParent(entity.transform);
                }

            }
        }
    }


}
