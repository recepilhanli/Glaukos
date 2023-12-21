using UnityEngine;

/// <summary>
/// For Tor
/// </summary>
public class EnemyDamager : MonoBehaviour
{
    [SerializeField] float _Damage = 5f;
    [SerializeField] Rigidbody2D _Body;
    [SerializeField] Entity.AttackTypes _Tyoe = Entity.AttackTypes.Attack_Tornado;


    private void Awake()
    {
        if (_Body != null) _Body.gravityScale = 4;
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
                enemy.OnTakeDamage(_Damage, _Tyoe);
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
