using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Slider HealthBar;
    [SerializeField] Slider FocusBar;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("Menu");
        
        FocusBar.value = Player.Instance.Focus / 100;
        HealthBar.value = Player.Instance.Health / 100;

    }
}
