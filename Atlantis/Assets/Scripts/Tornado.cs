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
                if (enemy.Type != Entity.EntityType.Type_Shark && enemy.Type != Entity.EntityType.Type_Mermaid && enemy.Type != Entity.EntityType.Type_SwordFish && enemy.Type != Entity.EntityType.Type_PufferFish) enemy.Move(-toPos * _TornadoSpeed);
                if (enemy.Type == Entity.EntityType.Type_SwordFish && !enemy.isDeath && dist < _AffectingDistance / 1.5f) enemy.Move((enemy.transform.position - transform.position).normalized);
                if (_TornadoDamageTime <= Time.time && enemy.Type != Entity.EntityType.Type_Mermaid) enemy.OnTakeDamage(5, Entity.AttackTypes.Attack_Tornado);
                else if (_TornadoDamageTime <= Time.time && dist < _AffectingDistance / 2) enemy.OnTakeDamage(30, Entity.AttackTypes.Attack_Tornado);
            }
        }
        if (_TornadoDamageTime < Time.time) _TornadoDamageTime = Time.time + 1f;



    }
}
