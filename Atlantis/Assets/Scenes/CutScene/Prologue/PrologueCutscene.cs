using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


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


    void Start()
    {
        _PostProcessVolume.profile.TryGet(out _Vignette);
        _PostProcessVolume.profile.TryGet(out _ChromaticAberration);

        _Director.stopped += OnStop;
    }

    private void OnStop(PlayableDirector director)
    {
        _FadeFix?.gameObject.SetActive(true);
        _Director.stopped -= OnStop;
        SceneManager.LoadScene("Presentation_Level_1");
    }

    void Update()
    {
        _Vignette.intensity.value = _Value;
        _ChromaticAberration.intensity.value = _Value * 10;
    }


    private void LateUpdate()
    {
        var pos = _MainCamera.transform.position;
        pos.x =_CamFollow.transform.position.x;
        _MainCamera.transform.position = pos;
    }

}
