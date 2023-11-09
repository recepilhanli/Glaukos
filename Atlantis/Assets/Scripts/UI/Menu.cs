using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void GetFirstLevel()
    {
        SceneManager.LoadScene("Presentation_Level_1");
    }

    public void GetDevLevel()
    {
        SceneManager.LoadScene("DeveloperScene");
    }


    public void Quit()
    {
        Application.Quit();
    }
}
