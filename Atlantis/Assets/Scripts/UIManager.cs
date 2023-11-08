using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;
using UnityEngine.UI;
using MainCharacter;

public class UIManager : MonoBehaviour
{
    [SerializeField] Slider HealthBar;
    [SerializeField] Slider FocusBar;


    void Update()
    {

        FocusBar.value = 100 / Player.Instance.Focus;
        HealthBar.value = 100 / Player.Instance.Health;


    }
}
