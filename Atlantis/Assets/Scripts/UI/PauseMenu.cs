using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;


    [SerializeField] private GameObject _QuitScreen;
    [SerializeField] AudioClip _ButtonSound;


    public bool isPaused { get; private set; } = false;
    void Awake()
    {
        instance = this;
        TogglePause(false);
    }

    public void TogglePause(bool toggle)
    {
        LevelManager.PlaySound2D(_ButtonSound, 1f);
        isPaused = toggle;
        if (toggle)
        {
            Time.timeScale = 0;
            gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
        _QuitScreen.SetActive(false);
    }

    public void QuitScreen(bool toggle)
    {
        LevelManager.PlaySound2D(_ButtonSound, 1f);
        _QuitScreen.SetActive(toggle);
    }

    public void Quit()
    {
        LevelManager.PlaySound2D(_ButtonSound, 1f);
        Application.Quit();
    }

}
