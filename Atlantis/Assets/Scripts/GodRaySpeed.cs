using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodRaySpeed : MonoBehaviour
{

    void Start()
    {
        GetComponent<Animator>()?.SetFloat("Speed", Random.Range(0.2f, 2));
    }

}
