using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;
using UnityEngine.Events;

public class Collectable : MonoBehaviour
{
    [SerializeField] bool _FollowPlayer = false;

    public UnityEvent OnPlayerCollectItemEvent;
    private Vector3 m_Velocity = Vector3.zero;
    private void Start()
    {
        if (OnPlayerCollectItemEvent == null)
            OnPlayerCollectItemEvent = new UnityEvent();
    }

    void DebugTest()
    {
        Debug.Log("Collect Test");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            OnPlayerCollectItemEvent.Invoke();
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_FollowPlayer)
        {
            transform.position = Vector3.SmoothDamp(transform.position, Player.Instance.transform.position, ref m_Velocity, 15 * Time.deltaTime);
        }
    }
}
