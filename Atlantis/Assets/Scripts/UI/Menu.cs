using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MainCharacter;
using TMPro;
using System;


/// <summary>
/// This class is used to manage the menu scene.
/// </summary>
public class Menu : MonoBehaviour
{

    /// <summary>
    /// this is the continue button of the menu.
    /// </summary>
    [SerializeField, Tooltip("Continue Button Of The Menu")] Button ContinueButton;

    [SerializeField] AudioClip _ButtonSound;

    [SerializeField, Tooltip("Can be null")] Slider _VolumeSlider;

    [SerializeField, Tooltip("Can be null")] GameObject _TutorialPanel;

    [SerializeField] LocalizationListener _LocalizationListener;

    [SerializeField] TMP_Dropdown _Dropdown;


    void Start()
    {
        Player.LoadRemaningLifes();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 1f;
        if (PlayerPrefs.HasKey(PerfTable.perf_LastScene))
        {
            ContinueButton.interactable = true;
        }

        if (_Dropdown != null)
        {
            if (PlayerPrefs.HasKey(PerfTable.perf_Language))
            {
                string lang = PlayerPrefs.GetString(PerfTable.perf_Language);
                if (lang == "TR")
                {
                    _Dropdown.value = 1;
                }
                else
                {
                    _Dropdown.value = 0;
                }
            }
            else
            {
                _Dropdown.value = 0;
            }
        }
    }


    /// <summary>
    /// This method is used to load the first level of the game.
    /// </summary>
    public void GetFirstLevel()
    {
        if (_ButtonSound != null) LevelManager.PlaySound2D(_ButtonSound, 1f);
        Debug.Log("New Level");
        SceneManager.LoadScene(PerfTable.perf_LevelPrologue);
    }

    /// <summary>
    /// This method is used to skip the tutorial.
    /// </summary>
    public void SkipTutorial()
    {
        Player.ResetRemainingLifes();
        PlayerPrefs.SetString(PerfTable.perf_LastScene, PerfTable.perf_Level1);

        if (_ButtonSound != null) LevelManager.PlaySound2D(_ButtonSound, 1f);
        Debug.Log("Skip New Level");
        Loading.LoadScene(PerfTable.perf_Level1);
    }

    /// <summary>
    /// This method is used to open the tutorial panel.
    /// </summary>
    public void OpenTutorialPanel()
    {
        if (_ButtonSound != null) LevelManager.PlaySound2D(_ButtonSound, 1f);
        _TutorialPanel.SetActive(true);
    }

    /// <summary>
    /// This method is used to close the tutorial panel.
    /// </summary>
    public void CloseTutorialPanel()
    {
        if (_TutorialPanel != null)
        {
            LevelManager.PlaySound2D(_ButtonSound, 1f);
            _TutorialPanel.SetActive(false);
        }
    }

    /// <summary>
    /// This method is used to load the second level of the game.
    /// </summary>
    public void GetDevLevel()
    {
        if (_ButtonSound != null) LevelManager.PlaySound2D(_ButtonSound, 1f);
        Loading.LoadScene("DeveloperScene");
    }

    /// <summary>
    /// This method is used to load the third level of the game.
    /// </summary>
    public void ReturnToMenu()
    {
        if (_ButtonSound != null) LevelManager.PlaySound2D(_ButtonSound, 1f);
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// This method is used to quit the game.
    /// </summary>
    public void Quit()
    {
        if (_ButtonSound != null) LevelManager.PlaySound2D(_ButtonSound, 1f);
        Application.Quit();
    }

    /// <summary>
    /// This method is used to load the last level played.
    /// </summary>
    public void Continue()
    {
        if (_ButtonSound != null) LevelManager.PlaySound2D(_ButtonSound, 1f);
        string levelName = PerfTable.perf_Level1;
        if (PlayerPrefs.HasKey(PerfTable.perf_LastScene))
        {
            levelName = PlayerPrefs.GetString(PerfTable.perf_LastScene);
            LevelManager.isLoadingGame = true;
        }
        Loading.LoadScene(levelName);
    }


    /// <summary>
    /// This method is used to change the language of the tutorial panel.
    /// </summary>
    public void ChangeVolume()
    {
        AudioListener.volume = _VolumeSlider.value;
    }

    /// <summary>
    /// This method is used to change the language of the tutorial panel.
    /// </summary>
    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void DropDownValueChanged()
    {
        int value = _Dropdown.value;
        if (value == 0)
        {
            PlayerPrefs.SetString(PerfTable.perf_Language, "EN");
            Debug.Log("EN");
        }
        else if (value == 1)
        {
            PlayerPrefs.SetString(PerfTable.perf_Language, "TR");
            Debug.Log("TR");
        }

        PlayerPrefs.Save();

        _LocalizationListener.Localize();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTutorialPanel();
        }
    }



}
