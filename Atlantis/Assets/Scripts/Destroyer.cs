using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] float _DestroyTimeInSeconds = 5f;
    void Start()
    {
        Invoke("DestroyMe", _DestroyTimeInSeconds);
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }
}
