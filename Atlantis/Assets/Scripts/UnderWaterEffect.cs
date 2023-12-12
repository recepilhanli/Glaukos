using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UnderWaterEffect : MonoBehaviour
{

    [SerializeField] Volume _Volume;
    private LensDistortion _LensDistortion = null;

    float multiplier = 1;


    void Start()
    {
        _Volume.profile.TryGet(out _LensDistortion);
    }

    // Update is called once per frame
    void Update()
    {
        if (_LensDistortion == null)
        {
            Debug.LogError("Lens Distortion is null!");
            return;
        }
        Debug.Log("Pulse");

        float val = Mathf.PingPong(Time.time/6, 0.3f);

        _LensDistortion.intensity.value = val;
   

    }
}
