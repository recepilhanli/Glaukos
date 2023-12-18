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

    [SerializeField] PlayableDirector _Director;

    [SerializeField] float _Value;

    [SerializeField] Image _FadeFix;

    void Start()
    {
        _PostProcessVolume.profile.TryGet(out _Vignette);
        _Director.stopped += OnStop;
    }

    private void OnStop(PlayableDirector director)
    {
        _Director.stopped -= OnStop;
        _FadeFix?.gameObject.SetActive(true);
        SceneManager.LoadScene("Presentation_Level_1");
    }

    void Update()
    {
        _Vignette.intensity.value = _Value;
    }
}
