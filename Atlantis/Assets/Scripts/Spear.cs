using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Spear : Weapons
{

    public ThrowStates ThrowState { get; private set; } = ThrowStates.STATE_NONE;

    [SerializeField] GameObject _TornadoPrefab;

    [SerializeField] GameObject _SpearRainPrefab;
    private Vector2 _ThrowedPosition = Vector2.zero;

    [HideInInspector] public Vector2 SpearOffset = Vector2.zero;

    [SerializeField] GameObject BloodEffectPrefab;

    public enum ThrowStates
    {
        STATE_NONE,
        STATE_THROWING,
        STATE_OVERLAPPED,
        STATE_FLOATING,
        STATE_GETTING_BACK,
    }


    public void CreateSpearRain()
    {
        Transform _transform = Player.Instance.transform;
        var tornado = Instantiate(_SpearRainPrefab, _transform);
        tornado.transform.position = _transform.position;
        tornado.transform.SetParent(null);
    }


    public void CreateTornado()
    {
        Transform _transform = Player.Instance.transform;
        var tornado = Instantiate(_TornadoPrefab, _transform);
        tornado.transform.position = _transform.position;
        tornado.transform.SetParent(null);
    }

    private void Start()
    {
        Type = Weapon_Types.Type_Spear;
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

        ThrowState = ThrowStates.STATE_THROWING;
    }

    public void GetBackToThePlayer(bool _instantly = false)
    {
        if (_instantly)
        {
            transform.SetParent(Player.Instance._RightHand);
            transform.localPosition = SpearOffset;
            ThrowState = 0;
            var euler = Vector3.zero;
            float TempY = Player.Instance.transform.eulerAngles.y;
            euler.y = TempY;
            euler.z = -90;
            transform.eulerAngles = euler;
            Player.Instance.AttackState(1);
            Player.Instance.CameraShake(2, 0.5f, 0.001f);
        }
        else
        {
            transform.SetParent(null);
            ThrowState = ThrowStates.STATE_GETTING_BACK;
        }
    }



    void CollisionReaction(GameObject other)
    {
        if (ThrowState == 0) return;
        var _other = other.GetComponent<Collider2D>();
        if (_other == null) return;

        else if (_other.isTrigger) return;

        if (other.CompareTag("Player") && ThrowState != ThrowStates.STATE_THROWING)
        {
            GetBackToThePlayer(true);
            return;
        }
        else if (other.CompareTag("Player") && ThrowState == ThrowStates.STATE_THROWING) return;

        if (ThrowState == ThrowStates.STATE_GETTING_BACK)
        {
            SendDamage(2f, false, _other);
            return;
        }


        Debug.Log(other + " -> throwing hit");
        ThrowState = ThrowStates.STATE_OVERLAPPED;


        if (other.CompareTag("Puzzle"))
        {
            return;
        }

        SendDamage(20f, true, _other);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        CollisionReaction(other.gameObject);
    }


    void Update()
    {
        if (ThrowState != ThrowStates.STATE_THROWING && ThrowState != ThrowStates.STATE_FLOATING && ThrowState != ThrowStates.STATE_GETTING_BACK) return;

        if (Vector2.Distance(Player.Instance.transform.position, transform.position) <= 1.5f && ThrowState == ThrowStates.STATE_GETTING_BACK)
        {
            GetBackToThePlayer(true);
            return;
        }
        else if (ThrowState == ThrowStates.STATE_GETTING_BACK)
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


        if (Vector2.Distance(_ThrowedPosition, transform.position) > 1 && ThrowState != ThrowStates.STATE_FLOATING)
        {
            var lerpedPos = Vector3.Lerp(transform.position, _ThrowedPosition, Time.deltaTime * 3);
            transform.position = lerpedPos;
        }
        else
        {
            Debug.Log("Floating");
            ThrowState = ThrowStates.STATE_FLOATING;
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

        if (other.gameObject.CompareTag("Enemy"))
        {
            var entity = other.gameObject.GetComponent<Entity>();

            if (entity != null)
            {
                if (entity.isDeath) return;
                Debug.LogWarning("Damage entity");
                Player.Instance.Attack(entity, damage, Entity.AttackTypes.Attack_Standart);
                Instantiate(BloodEffectPrefab, transform.position, quaternion.identity);
                if (attach && entity != null && ThrowState != ThrowStates.STATE_GETTING_BACK)
                {
                    ThrowState = ThrowStates.STATE_OVERLAPPED;
                    transform.SetParent(entity.transform);
                }

            }
        }

        if (other.gameObject.CompareTag("Props"))
        {
            var prop = other.gameObject.GetComponent<Props>();
            if (prop != null) prop.Break();
        }


    }


}
