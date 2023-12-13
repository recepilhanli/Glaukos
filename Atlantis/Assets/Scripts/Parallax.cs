using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    [SerializeField, Range(0.1f, 2)] float _Speed = 0.1f;

    private Material _Material;

    void Start()
    {
        _Material = GetComponent<Renderer>().material;
    }


    float dist = 0;
    void Update()
    {
        dist += Time.deltaTime * _Speed;
        _Material.SetTextureOffset("_MainTex", Vector2.right * dist);
    }
}
