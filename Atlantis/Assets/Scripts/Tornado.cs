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


    private void OnCollisionEnter(Collision other)
    {

    }

    void Update()
    {
        foreach (var enemy in Enemies)
        {
            if (enemy == null) continue;
            _Body.velocity = Vector3.SmoothDamp(_Body.velocity, transform.right * Time.deltaTime * _TornadoSpeed, ref m_Velocity, .05f);

            if (Vector2.Distance(enemy.transform.position, transform.position) <= _AffectingDistance)
            {
                Vector2 toPos = enemy.transform.position - transform.position;
                toPos.y = 0;
                enemy.Move(-toPos * _TornadoSpeed);
                if (_TornadoDamageTime <= Time.time) enemy.OnTakeDamage(5, Entity.AttackTypes.Attakc_Tornado);
            }
        }
        if (_TornadoDamageTime < Time.time) _TornadoDamageTime = Time.time + 1f;

        if (_LifeTime < Time.time) Destroy(gameObject);

    }
}
