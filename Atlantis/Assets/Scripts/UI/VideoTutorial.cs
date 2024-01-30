using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class VideoTutorial : MonoBehaviour
{
    [SerializeField] Image _loadingImage;

    [SerializeField] VideoPlayer _videoPlayer;

    IEnumerator Start()
    {
        if (PlayerPrefs.GetInt(PerfTable.perf_LoadID) == 1) Destroy(gameObject);
        else PauseMenu.instance.TogglePause(true);

        while (!_videoPlayer.isPrepared && !_videoPlayer.isPlaying)
        {
            Color color = _loadingImage.color;
            float black = Mathf.PingPong(Time.realtimeSinceStartup, .5f) + .5f;
            color.r = black;
            color.g = black;
            color.b = black;
            _loadingImage.color = color;
            Debug.Log("Loading");
            yield return null;
        }
        yield return null;
    }

    public void UnPause()
    {
        PauseMenu.instance.TogglePause(false);
        Destroy(gameObject);
    }

}
