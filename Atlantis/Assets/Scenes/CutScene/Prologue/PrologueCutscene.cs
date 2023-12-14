using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PrologueCutscene : MonoBehaviour
{

    [SerializeField] Volume _PostProcessVolume;
    private Vignette _Vignette;
    private ChromaticAberration _ChromaticAberration;

    [SerializeField] PlayableDirector _Director;

    [SerializeField] float _Value;

    [SerializeField] Image _FadeFix;

    [SerializeField] Camera _MainCamera;

    [SerializeField] Transform _CamFollow;
    private Coroutine _LevelCoroutine;

    void Start()
    {
        _PostProcessVolume.profile.TryGet(out _Vignette);
        _PostProcessVolume.profile.TryGet(out _ChromaticAberration);
    }

 

    void Update()
    {
        
        if (_Director.state == PlayState.Playing && Mathf.Abs((float) _Director.time - (float)_Director.duration) <= 0.1f  && _LevelCoroutine == null)
        {
            _Director.Pause();
            _FadeFix?.gameObject.SetActive(true);
            _LevelCoroutine = StartCoroutine(LevelChanger());
        }

        _Vignette.intensity.value = _Value;
        _ChromaticAberration.intensity.value = _Value * 10;


    }


    IEnumerator LevelChanger()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Presentation_Level_1");
        yield return null;
    }


    private void LateUpdate()
    {
        var pos = _MainCamera.transform.position;
        pos.x =_CamFollow.transform.position.x;
        _MainCamera.transform.position = pos;
    }

}
