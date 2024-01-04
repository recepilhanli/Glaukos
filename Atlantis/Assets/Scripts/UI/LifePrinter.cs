using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MainCharacter;

public class LifePrinter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _TMP;
    [SerializeField] GameObject _ContinueButton;
    void Start()
    {
        if (!PlayerPrefs.HasKey(PerfTable.perf_LastScene)) _ContinueButton.SetActive(true);

        if (Player.RemainingLifes <= 0)
        {
            Player.ResetRemainingLifes();
            _ContinueButton.SetActive(false);
            _TMP.text = "<color=red>Geri kalan hayatinda artik bir baliksin!";
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
        string extra = color + Player.RemainingLifes.ToString();
        _TMP.text += extra;
        Destroy(this);
    }

}
