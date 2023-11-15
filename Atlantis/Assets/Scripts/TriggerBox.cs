using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerBox : MonoBehaviour
{
    [SerializeField] string _Tag;

    [SerializeField] UnityEvent TriggerEvent;
    [SerializeField] UnityEvent ExitTriggerEvent;
    private void Start()
    {
        if (TriggerEvent == null) TriggerEvent = new UnityEvent();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_Tag))
        {
            TriggerEvent.Invoke();
            Debug.Log("Triggered by: " + other.gameObject + " - " + other.gameObject.tag);
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_Tag))
        {
            ExitTriggerEvent.Invoke();
            Debug.Log("Exit by: " + other.gameObject + " - " + other.gameObject.tag);
        }
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

}
