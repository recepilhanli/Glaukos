using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// this class is for the prologue cutscene
/// </summary>
public class PrologueCutscene : MonoBehaviour
{

    [SerializeField] Volume _PostProcessVolume;
    private Vignette _Vignette;
    private ChromaticAberration _ChromaticAberration;

    [SerializeField] PlayableDirector _Director;

    [SerializeField] float _Value;

    [SerializeField] Image _FadeFix;

    [SerializeField] Camera _Camera;

    [SerializeField] Transform _Player;


    private bool canSkip = false;

    public void SetSkip()
    {
        canSkip = true;
    }

    IEnumerator Start()
    {
        _PostProcessVolume.profile.TryGet(out _Vignette);
        _PostProcessVolume.profile.TryGet(out _ChromaticAberration);
        yield return new WaitForSeconds(.085f);
        _Director.Pause();
    }

    private void LateUpdate()
    {
        _Camera.transform.position = new Vector3(_Player.transform.position.x, _Camera.transform.position.y, _Camera.transform.position.z);
    }

    void Update()
    {
        

        if (_Director.time >= 5.3f && _ChromaticAberration.intensity.value == 0)
        {
            _ChromaticAberration.intensity.value = 1;
        }


        if (_Director.duration - _Director.time <= 0.11f && _Director.state == PlayState.Playing)
        {
            _Director.Pause();
            _FadeFix?.gameObject.SetActive(true);
            Loading.LoadScene(PerfTable.perf_LevelTutorial);
        }

      if(_Director.time > .25f)  _Vignette.intensity.value = _Value;
    }
}
