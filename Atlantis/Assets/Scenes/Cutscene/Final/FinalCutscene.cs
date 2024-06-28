using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalCutscene : MonoBehaviour
{
  public void Ending()
  {
    SceneManager.LoadScene("Credits");
  }
}
