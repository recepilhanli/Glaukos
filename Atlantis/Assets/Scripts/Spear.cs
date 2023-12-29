using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

/// <summary>
/// this class is for the spear weapon
/// </summary>
public class Spear : Weapons
{

    public ThrowStates ThrowState = ThrowStates.STATE_NONE;

    [SerializeField] GameObject _TornadoPrefab;

    [SerializeField] GameObject _SpearRainPrefab;
    private Vector2 _ThrowedPosition = Vector2.zero;
    private Vector2 _ThrowedPositionNormalized = Vector2.zero;

    [HideInInspector] public Vector2 SpearOffset = Vector2.zero;

    [SerializeField] GameObject BloodEffectPrefab;
    [SerializeField] GameObject BubbleEffect;

    [SerializeField] TrailRenderer _Trail;

    [SerializeField] LayerMask SpearLayerMask;

    float _ThrowingTime = 0f;

    [HideInInspector] public int Stuck = 0;



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
        _ThrowingTime = 0.75f + Time.time;
        _Trail.enabled = true;
        transform.SetParent(null);
        _ThrowedPosition = pos;
        _ThrowedPositionNormalized = (_ThrowedPosition - (Vector2)transform.position).normalized;

        transform.up = _ThrowedPositionNormalized;

        ThrowState = ThrowStates.STATE_THROWING;

        if (Player.Instance._Rigidbody.gravityScale == 0) BubbleEffect.SetActive(true);

        var hit = Physics2D.Raycast(Player.Instance.transform.position, _ThrowedPositionNormalized, 1.4f, SpearLayerMask);
        if(hit.collider != null)
        {
            CollisionReaction(hit.collider.gameObject);
            Debug.Log("Hit something while throwing");
        }
    }

    public void StopSpear()
    {
        var spear = Player.Instance._Spear;
        if (spear.ThrowState != ThrowStates.STATE_THROWING) return;
        spear.ThrowState = ThrowStates.STATE_OVERLAPPED;
        spear._ThrowingTime = 0;
        spear._ThrowedPositionNormalized = Vector2.zero;

        Debug.Log("Spear stopped. " + gameObject);
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
            Player.Instance.AttackState(2);
            Player.Instance.CameraShake(2, 0.5f, 0.001f);
            BubbleEffect.SetActive(false);
            _Trail.enabled = false;
            ThrowState = ThrowStates.STATE_NONE;
        }
        else
        {
            if (Stuck > 0)
            {
                UIManager.Instance.Fade(1, 1, 1, 3);
                Stuck--;
                transform.Rotate(0, 0, UnityEngine.Random.Range(-30, 30));
                return;
            }
            _Trail.enabled = true;
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

        if (other.CompareTag("Player") && ThrowState != ThrowStates.STATE_THROWING && Stuck <= 0)
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
        _Trail.enabled = false;

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

            var lerpedPos = Vector3.MoveTowards(transform.position, _ThrowedPosition, 0.8f);
            transform.position = lerpedPos;
            transform.Rotate(0, 0, Time.deltaTime * 1000);
            return;
        }


        if (ThrowState == ThrowStates.STATE_THROWING)
        {
            var lerpedPos = _ThrowedPositionNormalized * Time.fixedDeltaTime * 45;
            transform.position = (Vector2)transform.position + lerpedPos;
            if (_ThrowingTime < Time.time)
            {
                ThrowState = ThrowStates.STATE_FLOATING;
                Debug.Log("Set the spear state to floating.");
            }
        }
        else if (ThrowState == ThrowStates.STATE_FLOATING)
        {
            Debug.Log("Floating");
            transform.Rotate(0, 0, Time.deltaTime * 30);
        }

    }

    public void SendDamage(float damage, bool attach = false, Collider2D other = null, bool fromPlayer = false)
    {
        if (damage == 0) return;
        Debug.Log("overlapping");

        Vector3 pos = transform.position;
        if (fromPlayer)
        {
            Debug.Log("From Player");
            pos = Player.Instance.transform.position;
        }
        if (other == null) other = Physics2D.OverlapCircle(pos, 1.75f, Player.Instance.EnemyMask);
        if (other == null)
        {
            other = Physics2D.OverlapCircle(pos, 1.5f, Player.Instance.BossMask);
            if (other != null)
            {
                if (other.gameObject.CompareTag("Boss"))
                {
                    Kraken.Instance.OnTakeDamage(damage);
                    return;
                }
            }
        }
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

                if (attach && entity != null && ThrowState != ThrowStates.STATE_GETTING_BACK)
                {
                    ThrowState = ThrowStates.STATE_OVERLAPPED;
                    transform.SetParent(entity.transform);
                    if (entity.Type == Entity.EntityType.Type_JellyFish)
                    {
                        Stuck = 3;
                        UIManager.Instance.Fade(1, 1, 1, 4);
                    }
                    else Instantiate(BloodEffectPrefab, pos, transform.rotation);
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
