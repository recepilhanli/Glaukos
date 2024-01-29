
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using TMPro;
using System.Collections;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.VisualScripting;
public class Loading : MonoBehaviour
{

    private static string _LoadingSceneName = PerfTable.perf_Level1;
    private static string _LastScene = string.Empty;

    private bool _LoadingFade = false;

    [SerializeField] Image _FadeImage;
    [SerializeField] TextMeshProUGUI _LoadingText;

    private static AsyncOperationHandle<SceneInstance> loadHandle;


    void Awake()
    {
        Time.timeScale = 1f;

        if ((_LoadingSceneName == _LastScene)) _LoadingText.text = Translation.Translations["LoadingCompleted"].Get();
        else _LoadingText.text = "[" + Translation.Translations["Loading"].Get() + "..]";

        // StartCoroutine(LoadSceneAsync());

        StartCoroutine(LoadSceneAsync2());
    }


    IEnumerator LoadSceneAsync2()
    {
        while (_FadeImage.color.a > 0)
        {
            Color color = _FadeImage.color;
            color.a -= Time.deltaTime * 3;
            _FadeImage.color = color;
            yield return null;
        }

        _LoadingText.text = Translation.Translations["LoadingCompleted"].Get();

        while (!_LoadingFade)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _LoadingFade = true;
            }
            yield return null;
        }


        if (Input.GetKeyDown(KeyCode.F) && !_LoadingFade)
        {
            _LoadingFade = true;
        }

        if (_LoadingFade)
        {
            while (_FadeImage.color.a < 1)
            {
                Color color = _FadeImage.color;
                color.a += Time.deltaTime * 4;
                _FadeImage.color = color;
                yield return null;
            }
            _LastScene = _LoadingSceneName;
            SceneManager.LoadScene(_LoadingSceneName, LoadSceneMode.Single);
        }


        yield return null;
    }

    // IEnumerator LoadSceneAsync()
    // {

    //     while (_LoadingSceneName == _LastScene)
    //     {
    //         while (_FadeImage.color.a > 0)
    //         {
    //             Color color = _FadeImage.color;
    //             color.a -= Time.deltaTime * 3;
    //             _FadeImage.color = color;
    //             yield return null;
    //         }


    //         if (Input.GetKeyDown(KeyCode.F) && !_LoadingFade)
    //         {
    //             _LoadingFade = true;
    //         }

    //         if (_LoadingFade)
    //         {
    //             while (_FadeImage.color.a < 1)
    //             {
    //                 Color color = _FadeImage.color;
    //                 color.a += Time.deltaTime * 4;
    //                 _FadeImage.color = color;
    //                 yield return null;
    //             }

    //             yield return loadHandle = Addressables.LoadSceneAsync("Assets/Scenes/" + _LoadingSceneName + ".unity", LoadSceneMode.Additive, false);
    //             yield return loadHandle.Result.ActivateAsync();
    //             Debug.Log("Activated Last Scene: " + _LoadingSceneName);
    //             SceneManager.UnloadSceneAsync(PerfTable.perf_LevelLoading);
    //         }

    //         yield return null;
    //     }


    //     while (_FadeImage.color.a > 0)
    //     {
    //         Color color = _FadeImage.color;
    //         color.a -= Time.deltaTime;
    //         _FadeImage.color = color;
    //         yield return null;
    //     }


    //     if (loadHandle.IsValid())
    //     {
    //         Addressables.UnloadSceneAsync(loadHandle, true);
    //         Debug.Log("Unloading the old scene..");
    //     }

    //     while (loadHandle.IsValid())
    //     {
    //         yield return null;
    //     }

    //     Debug.Log("Loading the scene..");
    //     loadHandle = Addressables.LoadSceneAsync("Assets/Scenes/" + _LoadingSceneName + ".unity", LoadSceneMode.Additive, false);

    //     while (!loadHandle.IsDone)
    //     {
    //         _LoadingText.text = "[" + Translation.Translations["Loading"].Get() + " " + (loadHandle.PercentComplete * 100).ToString("F0") + "%]";
    //         yield return null;
    //     }

    //     _LoadingText.text = Translation.Translations["LoadingCompleted"].Get();

    //     while (!_LoadingFade)
    //     {
    //         if (Input.GetKeyDown(KeyCode.F))
    //         {
    //             _LoadingFade = true;
    //         }
    //         yield return null;
    //     }

    //     _LoadingText.text = string.Empty;

    //     while (_FadeImage.color.a < 1)
    //     {
    //         Color color = _FadeImage.color;
    //         color.a += Time.deltaTime;
    //         _FadeImage.color = color;
    //         yield return null;
    //     }


    //     yield return loadHandle.Result.ActivateAsync();
    //     Debug.Log("Activated Scene: " + _LoadingSceneName);
    //     SceneManager.UnloadSceneAsync(PerfTable.perf_LevelLoading);
    //     _LastScene = _LoadingSceneName;
    //     yield return null;

    // }


    public static void LoadScene(string _sceneName)
    {
        Debug.Log("Loading Scene: " + _sceneName);
        _LoadingSceneName = _sceneName;
        SceneManager.LoadScene(PerfTable.perf_LevelLoading);
    }

}
