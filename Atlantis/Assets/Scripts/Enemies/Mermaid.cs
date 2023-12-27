using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;
using UnityEngine.SceneManagement;

/// <summary>
/// Mermaid enemy boss class
/// </summary>
public class Mermaid : Entity, IEnemyAI
{

    /// <summary>
    /// Mermaid states
    /// </summary>
    enum MermaidStates
    {
        State_AttackNormal,
        State_AttackPoison,
        State_GoingClone,
        State_AttackClone,
    }

    [SerializeField, ReadOnlyInspector] bool _isRealMermaid = true;

    [SerializeField, Tooltip("This is where the mermaid can clone herself")] Vector2 _ClonePosition;
    [SerializeField] GameObject DustParticle;

    [Space, SerializeField] Animator _Animator;
    [SerializeField] Rigidbody2D _RigidBody;

    [SerializeField] GameObject _MermaidCanvas;



    private MermaidStates _currentState = MermaidStates.State_AttackNormal;

    private Vector2 m_Velocity = Vector2.zero;

    private static List<Entity> _UnrealMermaids = new List<Entity>();

    void Start()
    {
        Init(null);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _currentState = MermaidStates.State_GoingClone;
        }


        switch (_currentState)
        {
            case MermaidStates.State_AttackNormal:
                break;
            case MermaidStates.State_AttackPoison:
                break;
            case MermaidStates.State_GoingClone:
                {
                    if (Vector2.Distance(_ClonePosition, transform.position) < 1)
                    {
                        CreatUnrealMermaids();
                    }
                    else
                    {
                        Move(_ClonePosition);
                    }
                    break;
                }
            case MermaidStates.State_AttackClone:
                break;
            default:
                break;
        }
    }

    public void Init(EntityProperties _properties)
    {
        Type = EntityType.Type_Mermaid;
    }

    public void InitUnrealMermaid()
    {
        _isRealMermaid = false;
        Destroy(_MermaidCanvas);
    }



    public void OnDetected(Entity _entity)
    {
        _MermaidCanvas.SetActive(true);
    }

    /// <summary>
    /// Mermaid clone attack
    /// </summary>
    void CreatUnrealMermaids()
    {
        if (!_isRealMermaid || _UnrealMermaids.Count > 0) return;

        _currentState = MermaidStates.State_AttackClone;

        _UnrealMermaids.Clear();

        for (int i = 0; i < 5; i++)
        {
            var unrealmermaid = Instantiate(gameObject).GetComponent<Mermaid>();
            unrealmermaid.InitUnrealMermaid();
            _UnrealMermaids.Add(unrealmermaid);
            //making a circle with mermaids
            unrealmermaid.transform.position = transform.position + new Vector3(Mathf.Cos(i * 72 * Mathf.Deg2Rad), Mathf.Sin(i * 72 * Mathf.Deg2Rad), 0) * 5;
        }

        //exchange real mermaid to one of unreal mermaid's position
        var temp = _UnrealMermaids[Random.Range(0, 5)];
        var pos = temp.transform.position;
        temp.transform.position = transform.position;
        transform.position = pos;

        UIManager.Instance.Fade(1, 1, 1, 1f);
        Player.Instance.CameraShake(2, 1, 0.95f, true);


    }



    public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {

        if (!_isRealMermaid)
        {
            if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);
            UIManager.Instance.Fade(1, 1, 1, 2f);
            _UnrealMermaids.Remove(this);
            Destroy(Instantiate(DustParticle, transform.position, Quaternion.identity), 2f);
            Destroy(gameObject);
            return;
        }

        else if (_currentState == MermaidStates.State_AttackClone && _isRealMermaid)
        {
            if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);
            foreach (var unreal in _UnrealMermaids)
            {
                if (unreal != null)
                {
                    Destroy(Instantiate(DustParticle, unreal.transform.position, Quaternion.identity), 2f);
                    Destroy(unreal.gameObject);
                }
            }
            _UnrealMermaids.Clear();
            UIManager.Instance.Fade(1, 1, 1, 2f);
            _currentState = MermaidStates.State_AttackNormal;
        }


    }

    public override void OnDeath()
    {
        if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);

        Destroy(gameObject);
    }

    public override void Move(Vector2 pos)
    {
        _RigidBody.MovePosition(Vector2.SmoothDamp(transform.position, pos, ref m_Velocity, 0.5f));
    }

    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }
}
