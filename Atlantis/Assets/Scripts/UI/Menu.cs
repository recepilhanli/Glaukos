using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    [SerializeField] Button ContinueButton;


    private void Start()
    {
        Time.timeScale = 1f;
        if (PlayerPrefs.HasKey("g_Scene"))
        {
            ContinueButton.interactable = true;
        }
    }

    public void GetFirstLevel()
    {
        SceneManager.LoadScene("CS_Prologue");
    }

    public void GetDevLevel()
    {
        SceneManager.LoadScene("DeveloperScene");
    }


    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }


    public void Quit()
    {
        Application.Quit();
    }

    public void Continue()
    {
        LevelManager.isLoadingGame = true;
        string levelName = PlayerPrefs.GetString("g_Scene");
        SceneManager.LoadScene(levelName);
    }



}
