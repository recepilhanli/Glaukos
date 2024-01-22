using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimizer : MonoBehaviour
{

    void Awake()
    {

#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif

        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }


}
