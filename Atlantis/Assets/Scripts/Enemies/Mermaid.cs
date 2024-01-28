using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.AddressableAssets;


/// <summary>
/// Mermaid enemy boss class
/// </summary>
public class Mermaid : Entity, IEnemyAI
{

    /// <summary>
    /// Mermaid states
    /// </summary>
    public enum MermaidStates
    {
        State_None,
        State_AttackNormal,
        State_AttackPoison,
        State_GoingClone,
        State_AttackClone,
    }

    [Space, SerializeField, ReadOnlyInspector] bool _isRealMermaid = true;

    [SerializeField, Tooltip("This is where the mermaid can clone herself")] float _CloneYAxis = 0f;
    [SerializeField] GameObject DustParticle;

    [Space, SerializeField] Animator _Animator;
    [SerializeField] Rigidbody2D _RigidBody;

    [SerializeField] GameObject _MermaidCanvas;

    private static GameObject _WavePrefab;

    [SerializeField] GameObject _PoisonPrefab;

    [SerializeField] Transform _HeadTransform;

    [SerializeField] SpriteRenderer _HeadRenderer;
    [SerializeField] Sprite _MermaidRegularSprite;
    [SerializeField] Sprite _MermaidScreamSprite;

    [SerializeField] AudioSource _ScreamingSource;

    [SerializeField] float _Health = 200;
    [SerializeField, ReadOnlyInspector] MermaidStates _currentState = MermaidStates.State_AttackNormal;

    private Vector2 m_Velocity = Vector2.zero;

    private static List<Entity> _UnrealMermaids = new List<Entity>();

    private bool _isEntitySeen = false;

    private int _LastIndex = -1;

    private float _CloneDelay = 0f;


    void Start()
    {
        Init(null);

        Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Adressable/Wave.prefab").Completed += (handle) =>
        {
            _WavePrefab = handle.Result;
        };

    }

    void OnDestroy()
    {

        if (Player.Instance != null) Player.Instance._Spear.GetBackToThePlayer(false);

        if (_isRealMermaid)
        {
            Addressables.ReleaseInstance(_WavePrefab);
        }
    }

    void Update()
    {
        float playerdist = Vector2.Distance(Player.Instance.transform.position, transform.position);

        if (!_isEntitySeen)
        {
            if (playerdist <= 12)
            {
                OnDetected(Player.Instance);
            }
            return;
        }





        switch (_currentState)
        {
            case MermaidStates.State_None:
                {
                    //add extra 10 seconds to clone delay
                    if (_Health <= 120 && _CloneDelay + 10 <= Time.time)
                    {
                        _CloneDelay = Time.time + 10;
                        SetState(MermaidStates.State_GoingClone);
                    }
                    if (playerdist <= 1.5f) SetState(MermaidStates.State_AttackNormal);
                    else Move(Player.Instance.transform.position);
                    break;
                }

            case MermaidStates.State_AttackNormal:
                {
                    if (playerdist <= 1.85f)
                    {
                        SetState(MermaidStates.State_AttackNormal);
                        Move(Player.Instance.transform.position);
                    }
                    else SetState(MermaidStates.State_None);
                    break;
                }

            case MermaidStates.State_AttackPoison:
                {
                    if ((transform.position.x - Player.Instance.transform.position.x) < 0) transform.localScale = new Vector3(-1.3f, 1.3f, 1.3f);
                    else transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

                    if (playerdist <= 2.4f)
                    {
                        SetState(MermaidStates.State_None);
                    }
                    break;
                }

            case MermaidStates.State_GoingClone:
                {
                    Vector2 _ClonePosition = new Vector2(transform.position.x, _CloneYAxis);

                    if (Vector2.Distance(_ClonePosition, transform.position) < 1.5f)
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
                {
                    if ((transform.position.x - Player.Instance.transform.position.x) < 0) transform.localScale = new Vector3(-1.3f, 1.3f, 1.3f);
                    else transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                    _CloneDelay = Time.time + 10;
                    break;
                }
            default:
                break;
        }
    }


    public void SetState(MermaidStates state)
    {
        if (_isRealMermaid)
        {
            if ((_currentState == state) || (_currentState == MermaidStates.State_GoingClone && state != MermaidStates.State_AttackClone)) return;
        }
        else if (state != MermaidStates.State_AttackClone) return; //unreal mermaid can only scream

        //animation states
        const int anim_move = 0;
        const int anim_scream = 1;
        const int anim_poison = 2;
        const int anim_normalattack = 3;

        _currentState = state;
        switch (state)
        {
            case MermaidStates.State_None:
                {
                    _Animator.SetInteger("State", anim_move);
                    break;
                }
            case MermaidStates.State_GoingClone:
                {
                    _Animator.SetInteger("State", anim_move);
                    break;
                }
            case MermaidStates.State_AttackNormal:
                {
                    _Animator.SetInteger("State", anim_normalattack);
                    break;
                }
            case MermaidStates.State_AttackPoison:
                {
                    _Animator.SetInteger("State", anim_poison);
                    break;
                }

            case MermaidStates.State_AttackClone:
                {
                    _Animator.SetInteger("State", anim_scream);
                    _HeadRenderer.sprite = _MermaidScreamSprite;
                    Debug.Log("Scream");
                    break;
                }
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

    public void CreateWave()
    {
        try
        {
            var wave = Instantiate(_WavePrefab, _HeadTransform.position, Quaternion.identity);
            wave.transform.up = (Player.Instance.transform.position - transform.position).normalized;
            Destroy(wave, 3f);
        }
        catch
        {
            Debug.Log("Error?");
        }
    }

    public void CreatePosion()
    {
        var wave = Instantiate(_PoisonPrefab, _HeadTransform.position, Quaternion.identity);
        wave.transform.up = (Player.Instance.transform.position - transform.position).normalized;
        Destroy(wave, 3f);
    }


    public void OnDetected(Entity _entity)
    {
        _isEntitySeen = true;
        // Player.Instance.LockLensSize = true;
        Player.Instance.CameraShake(1, .9f, 3f, true);
        if (_MermaidCanvas != null) _MermaidCanvas.SetActive(true);

    }

    /// <summary>
    /// Mermaid clone attack
    /// </summary>
    void CreatUnrealMermaids()
    {
        if (!_isRealMermaid || _UnrealMermaids.Count > 0 || _currentState != MermaidStates.State_GoingClone)
        {
            _currentState = MermaidStates.State_AttackPoison;
            return;
        }

        if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);

        SetState(MermaidStates.State_AttackClone);

        _UnrealMermaids.Clear();

        for (int i = 0; i < 5; i++)
        {
            var unrealmermaid = Instantiate(gameObject).GetComponent<Mermaid>();
            unrealmermaid.InitUnrealMermaid();
            unrealmermaid.SetState(MermaidStates.State_AttackClone);
            _UnrealMermaids.Add(unrealmermaid);

            //fix being red bug
            var renderers = unrealmermaid.GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.color = Color.white;
            }

            //making a circle with mermaids
            unrealmermaid.transform.position = transform.position + new Vector3(Mathf.Cos(i * 72 * Mathf.Deg2Rad), Mathf.Sin(i * 72 * Mathf.Deg2Rad), 0) * 5;
        }

        //exchange real mermaid to one of unreal mermaid's position
        int index = Random.Range(0, 5);
        while (index == _LastIndex)
        {
            index = Random.Range(0, 5);
        }
        _LastIndex = index;
        var temp = _UnrealMermaids[index];
        var pos = temp.transform.position;
        temp.transform.position = transform.position;
        transform.position = pos;

        UIManager.Instance.Fade(1, 1, 1, 1f);
        Player.Instance.CameraShake(2, 1, 0.95f, true);
        _ScreamingSource.Play();

    }



    public override void OnTakeDamage(float _h, AttackTypes type = AttackTypes.Attack_Standart)
    {
        if (isDeath) return;

        if (!_isEntitySeen)
        {
            OnDetected(Player.Instance);
            return;
        }


        if (!_isRealMermaid && type != AttackTypes.Attack_Rain)
        {
            if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);
            UIManager.Instance.Fade(1, 1, 1, 2f);
            _UnrealMermaids.Remove(this);
            if (type != AttackTypes.Attack_Tornado) Player.Instance.GiveFocusPoints(-5f);
            Destroy(Instantiate(DustParticle, transform.position, Quaternion.identity), 2f);
            Destroy(gameObject);
            return;
        }
        else if (!_isRealMermaid && type == AttackTypes.Attack_Rain)
        {
            StartCoroutine(DamageEffect());
            return;
        }



        else if (_currentState == MermaidStates.State_AttackClone && _isRealMermaid && type != AttackTypes.Attack_Tornado && type != AttackTypes.Attack_Rain)
        {
            _ScreamingSource.Stop();
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
            _HeadRenderer.sprite = _MermaidRegularSprite;
            SetState(MermaidStates.State_None);
            LevelManager.PlaySound2D(Player.Instance._Spear.SpearImpactSound2, .4f);
        }
        else StartCoroutine(DamageEffect());

        if (type == AttackTypes.Attack_Standart)
        {
            _Health -= _h / 8f;
            if (!Player.Instance._Rage) Player.Instance.GiveFocusPoints(5f);
        }
        else _Health -= _h / 12f;

        _HealthBar.value = _Health / 100;

        if (_Health <= 0) OnDeath();
        else if (_Health <= 160 && _CloneDelay <= Time.time)
        {
            _CloneDelay = Time.time + 12;
            SetState(MermaidStates.State_GoingClone);
        }
        else if (_Health <= 180 && _currentState == MermaidStates.State_None && Random.Range(0, 10) <= 4) SetState(MermaidStates.State_AttackPoison);
        else if (_currentState == MermaidStates.State_AttackPoison && Random.Range(0, 10) <= 5) SetState(MermaidStates.State_None);

    }

    public override void Attack(Entity entity, float damage, AttackTypes type = AttackTypes.Attack_Standart)
    {
        if (Vector2.Distance(transform.position, Player.Instance.transform.position) <= 1.9) Player.Instance.OnTakeDamage(damage, type);
    }

    public override void OnDeath()
    {
        isDeath = true;
        if (Player.Instance._Spear.ThrowState != Spear.ThrowStates.STATE_NONE) Player.Instance._Spear.GetBackToThePlayer(false);
        Destroy(gameObject);
        Player.Instance.BossKillReward(PerfTable.perf_Level2);
    }

    public override void Move(Vector2 pos)
    {
        if (transform.position.x - pos.x < 0) transform.localScale = new Vector3(-1.3f, 1.3f, 1.3f);
        else transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

        float speedMultiplier = 1f;
        if (_Health <= 50) speedMultiplier = 0.4f;

        _RigidBody.MovePosition(Vector2.SmoothDamp(transform.position, pos, ref m_Velocity, 0.3f * speedMultiplier));
    }

    IEnumerator DamageEffect()
    {
        // int randomindex = UnityEngine.Random.Range(1, _Clips.Count);
        // PlaySound(randomindex);

        var Renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var _renderer in Renderers)
        {
            _renderer.color = Color.red;
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var _renderer in Renderers)
        {
            _renderer.color = Color.white;
        }
        yield return null;
    }

    public override EntityFlags GetEntityFlag()
    {
        return EntityFlags.Flag_Enemy;
    }
}

