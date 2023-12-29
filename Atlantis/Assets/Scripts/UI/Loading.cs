
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Loading : MonoBehaviour
{

    private static string _LoadingSceneName = "Loading";
    private bool _LoadingFade = false;

    [SerializeField] Image _FadeImage;

    public static void LoadScene(string _sceneName)
    {
        Debug.Log("Loading Scene: " + _sceneName);
        _LoadingSceneName = _sceneName;
        SceneManager.LoadScene(PerfTable.perf_LevelLoading);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Loading Scene: " + _LoadingSceneName);
            _LoadingFade = true;
        }


        if (_LoadingFade)
        {
            Color color = _FadeImage.color;
            color.a += Time.deltaTime;
            _FadeImage.color = color;
            if (color.a >= 1)
            {
                SceneManager.LoadScene(_LoadingSceneName);
            }
        }



    }


}
