using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// handle events when a specific object enters or exits the trigger
/// </summary>
public class TriggerBox : MonoBehaviour
{
    [SerializeField] string _Tag;

    [SerializeField] UnityEvent TriggerEvent;
    [SerializeField] UnityEvent ExitTriggerEvent;
    private void Start()
    {
        if (TriggerEvent == null) TriggerEvent = new UnityEvent();
    }

    private void OnDestroy()
    {
        TriggerEvent.RemoveAllListeners();
        ExitTriggerEvent.RemoveAllListeners();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var tags = _Tag.Split(';');
        foreach (var tag in tags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                TriggerEvent.Invoke();
                Debug.Log("Triggered by: " + other.gameObject + " - " + other.gameObject.tag);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        var tags = _Tag.Split(';');

        foreach (var tag in tags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                ExitTriggerEvent.Invoke();
                Debug.Log("Exit Triggered by: " + other.gameObject + " - " + other.gameObject.tag);
            }
        }
    }

    public void DestroyMe()
    {
        Destroy(gameObject, 0.05f);
    }

}
