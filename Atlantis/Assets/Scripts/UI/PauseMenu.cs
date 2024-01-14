using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;


    [SerializeField] private GameObject _QuitScreen;

    public bool isPaused {get; private set;} = false;
    void Awake()
    {
        instance = this;
        TogglePause(false);
    }

    public void TogglePause(bool toggle)
    {
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
        _QuitScreen.SetActive(toggle);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
