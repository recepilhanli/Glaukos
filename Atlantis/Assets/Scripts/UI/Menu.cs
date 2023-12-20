using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class is used to manage the menu scene.
/// </summary>
public class Menu : MonoBehaviour
{
    
    /// <summary>
    /// this is the continue button of the menu.
    /// </summary>
    [SerializeField,Tooltip("Continue Button Of The Menu")] Button ContinueButton;


    private void Start()
    {
        Time.timeScale = 1f;
        if (PlayerPrefs.HasKey("g_Scene"))
        {
            ContinueButton.interactable = true;
        }
    }

    /// <summary>
    /// This method is used to load the first level of the game.
    /// </summary>
    public void GetFirstLevel()
    {
        Debug.Log("New Level");
        SceneManager.LoadScene("CS_Prologue");
    }
    
    /// <summary>
    /// This method is used to load the second level of the game.
    /// </summary>
    public void GetDevLevel()
    {
        Loading.LoadScene("DeveloperScene");
    }

    /// <summary>
    /// This method is used to load the third level of the game.
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// This method is used to quit the game.
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// This method is used to load the last level played.
    /// </summary>
    public void Continue()
    {
        LevelManager.isLoadingGame = true;
        string levelName = PlayerPrefs.GetString("g_Scene");
        Loading.LoadScene(levelName);
    }



}
