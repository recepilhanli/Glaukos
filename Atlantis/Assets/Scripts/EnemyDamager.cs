using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamager : MonoBehaviour
{
    [SerializeField] float _Damage = 5f;
    [SerializeField] Rigidbody2D _Body;

    private void Awake()
    {
        if(_Body != null) _Body.gravityScale = 4;
        Invoke("DestroyMe", 10f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered");
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Entity>();
            if (enemy != null)
            {
                enemy.OnTakeDamage(_Damage, Entity.AttackTypes.Attakc_Tornado);
                DestroyMe();
            }
        }
    }

    void DestroyMe()
    {
        if (gameObject == null) return;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {

    }

}
