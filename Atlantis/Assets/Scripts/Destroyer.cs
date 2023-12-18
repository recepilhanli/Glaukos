using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// destroy the gameobject after a specific time
/// </summary>
public class Destroyer : MonoBehaviour
{
    [SerializeField] float _DestroyTimeInSeconds = 5f;
    void Start()
    {
        Destroy(gameObject, _DestroyTimeInSeconds);
    }

}
