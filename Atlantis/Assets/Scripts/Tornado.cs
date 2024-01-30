using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;

public class Tornado : MonoBehaviour
{

    [SerializeField] Rigidbody2D _Body;
    [SerializeField, Range(0, 100)] float _LifeTimeInSeconds;

    [SerializeField, Range(0, 500)] float _TornadoSpeed = 100;

    [SerializeField, Range(0, 50)] float _AffectingDistance = 10;

    [SerializeField] ParticleSystem _Particle;

    [SerializeField] SpriteRenderer _Renderer;

    private float _LifeTime;
    private Vector3 m_Velocity = Vector3.zero;

    private float _TornadoDamageTime = 0f;

    Entity[] Enemies = null;

    List<GameObject> _Mines = new List<GameObject>();


    private void Awake()
    {
        _LifeTime = Time.time + _LifeTimeInSeconds;
        Enemies = GameObject.FindObjectsOfType<Entity>();
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (!Enemies[i].gameObject.CompareTag("Enemy")) Enemies[i] = null;
        }

        var player = Player.Instance;
        transform.eulerAngles = player.transform.eulerAngles;

        var minegos = GameObject.FindGameObjectsWithTag("Props");
        foreach (var mine in minegos)
        {
            if (mine.name.Contains("Mine")) _Mines.Add(mine);
        }

    }


    void Update()
    {
        if (_LifeTime < Time.time)
        {
            var main = _Particle.main;
            main.startColor = Color.Lerp(main.startColor.color, Color.clear, Time.deltaTime * 2);
            if (main.startColor.color.a <= 0.1f) _Particle.Stop();

            //fade out and destroy
            _Renderer.color = Color.Lerp(_Renderer.color, Color.clear, Time.deltaTime * 2);
            if (_Renderer.color.a <= 0.1f) Destroy(gameObject);
            return;
        }

        Player.Instance.CameraShake(1, 0.5f);
        _Body.velocity = Vector3.SmoothDamp(_Body.velocity, transform.right * Time.deltaTime * _TornadoSpeed, ref m_Velocity, .085f);


        foreach (var enemy in Enemies)
        {
            if (enemy == null) continue;
            float dist = Vector2.Distance(enemy.transform.position, transform.position);
            if (dist <= _AffectingDistance)
            {
                Vector2 toPos = enemy.transform.position - transform.position;
                toPos.y = 0;
                if (_TornadoDamageTime <= Time.time && dist < _AffectingDistance / Random.Range(1.5f, 2.5f)) enemy.OnTakeDamage(15, Entity.AttackTypes.Attack_Tornado);

                switch (enemy.Type)
                {

                    case Entity.EntityType.Type_SwordFish:
                        {
                            enemy.Move((enemy.transform.position - transform.position).normalized);
                            break;
                        }
                    case Entity.EntityType.Type_Drowned:
                        {
                            enemy.Move(-toPos * _TornadoSpeed * 1.25f);
                            break;
                        }

                    case Entity.EntityType.Type_JellyFish:
                        {

                            break;
                        }

                    case Entity.EntityType.Type_Mermaid:
                        {
                            if (_TornadoDamageTime <= Time.time) enemy.OnTakeDamage(5, Entity.AttackTypes.Attack_Tornado);
                            enemy.Move(transform.position);
                            break;
                        }

                    default: break;

                }
            }
        }

        foreach (var mine in _Mines)
        {
            if (mine == null) continue;
            float dist = Vector2.Distance(mine.transform.position, transform.position);
            if (dist <= _AffectingDistance * 2)
            {
                Vector2 toPos = mine.transform.position - transform.position;
                mine.transform.position -= (Vector3)toPos.normalized * Time.deltaTime * 10;
            }
        }



        if (_TornadoDamageTime < Time.time) _TornadoDamageTime = Time.time + 1f;


    }
}

