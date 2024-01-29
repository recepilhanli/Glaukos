using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoTutorial : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt(PerfTable.perf_LoadID) == 1) Destroy(gameObject);
        else PauseMenu.instance.TogglePause(true);
    }

    public void UnPause()
    {
        PauseMenu.instance.TogglePause(false);
        Destroy(gameObject);
    }
}
