using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is used to manage the collectable items.
/// </summary>
public class Collectable : MonoBehaviour
{
    [SerializeField] bool _FollowPlayer = false;
    [SerializeField, Range(1, 250), Tooltip("Speed of the item")] float _Speed = 10f;
    [Space(15)]
    public UnityEvent OnPlayerCollectItemEvent;
    private Vector3 m_Velocity = Vector3.zero;


    private void Start()
    {
        if (OnPlayerCollectItemEvent == null)
            OnPlayerCollectItemEvent = new UnityEvent();


        if (!_FollowPlayer)
        {
            var newup = (transform.position - Player.Instance.transform.position).normalized;
            transform.up = newup;
            Destroy(gameObject, 10f);
        }
    }

    void DebugTest()
    {
        Debug.Log("Collect Test");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player c");
            OnPlayerCollectItemEvent.Invoke();
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_FollowPlayer)
        {
            var newpos = (transform.position - Player.Instance.transform.position).normalized;
            transform.position = transform.position - (newpos * Time.deltaTime * _Speed);
        }
        else
        {
            transform.position = transform.position - (transform.up * Time.deltaTime * _Speed);
        }
    }
}
