using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// effect for underwater
/// </summary>
public class UnderWaterEffect : MonoBehaviour
{

    [SerializeField] Volume _Volume;
    private LensDistortion _LensDistortion = null;


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
        
        /// <summary>
        /// 0.3f is the max value of intensity
        /// </summary>
        float val = Mathf.PingPong(Time.time/6, 0.3f);

        _LensDistortion.intensity.value = val;
   

    }
}
