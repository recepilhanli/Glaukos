using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fadeout : MonoBehaviour
{
   private Image _Image;

     private void Start()
    {
        _Image = GetComponent<Image>();
    }


    // Update is called once per frame
    void Update()
    {
        _Image.color = new Color(_Image.color.r, _Image.color.g, _Image.color.b, _Image.color.a - Time.deltaTime);
        if (_Image.color.a <= 0) Destroy(gameObject);
    }
}
