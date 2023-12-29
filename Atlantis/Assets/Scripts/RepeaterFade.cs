using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeaterFade : MonoBehaviour
{
    
    [SerializeField] SpriteRenderer _Renderer;

    void Start()
    {
        _Renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //fade renderer
        if (_Renderer.color.a > 0)
        {
            var color = _Renderer.color;
            color.a -= Time.deltaTime * 2f;
            _Renderer.color = color;
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
