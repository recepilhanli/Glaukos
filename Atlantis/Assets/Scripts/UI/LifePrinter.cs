using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MainCharacter;
using UnityEditor.Localization.Editor;

public class LifePrinter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _TMP;
    [SerializeField] GameObject _ContinueButton;
    [SerializeField] GameObject _BackToMenuButton;
    void Start()
    {
        if (!PlayerPrefs.HasKey(PerfTable.perf_LastScene)) _ContinueButton.SetActive(true);
        _TMP.text = Translation.Translations["RemainingLife"].Get();
        if (Player.RemainingLifes <= 0)
        {
            Player.ResetRemainingLifes();
            _ContinueButton.SetActive(false);
            _BackToMenuButton.SetActive(true);
            _TMP.text = Translation.Translations["DeathMessage2"].Get();
            return;
        }
        string color = "<color=green>";
        if (Player.RemainingLifes <= 2)
        {
            color = "<color=red>";
        }
        if (Player.RemainingLifes == 3)
        {
            color = "<color=yellow>";
        }
        string extra = color + " " + Player.RemainingLifes.ToString();
        _TMP.text += extra;
        Destroy(this);
    }

}
