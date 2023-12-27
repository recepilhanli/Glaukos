using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MainCharacter;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// level manager class
/// </summary>
[ExecuteInEditMode]
public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    [Range(-2, 2)] public float GravityScale = 1f;

    public static bool isLoadingGame = false;
    public static string LastScene = "Menu";
    void Start()
    {
        Instance = this;

        OrientGravity();

        Physics2D.IgnoreLayerCollision(3, 6);
        Physics2D.IgnoreLayerCollision(6, 3);
        Physics2D.IgnoreLayerCollision(6, 6);
        Physics2D.IgnoreLayerCollision(3, 8);
        Physics2D.IgnoreLayerCollision(8, 3);
        Physics2D.IgnoreLayerCollision(6, 8);
        Physics2D.IgnoreLayerCollision(8, 6);
        Physics2D.IgnoreLayerCollision(7, 8);
        Physics2D.IgnoreLayerCollision(8, 8);

        if (isLoadingGame)
        {
            Invoke("LoadGame", 0.05f);
        }
        LastScene = SceneManager.GetActiveScene().name;
    }



    // Update is called once per frame
    void Update()
    {

    }

    public void OrientGravity()
    {
        var rigids = GameObject.FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
        foreach (var rigid in rigids)
        {
            if (GravityScale != 0)
            {
                if (rigid.gameObject.CompareTag("Enemy") && Application.isPlaying)
                {
                    var entity = rigid.gameObject.GetComponent<Entity>();
                    if (entity.Type != Entity.EntityType.Type_Kraken) rigid.gravityScale = GravityScale;
                }
                else if (rigid.gameObject.CompareTag("Enemy") && !Application.isPlaying) continue;
                else rigid.gravityScale = GravityScale;
            }
            else
            {
                if (rigid.gameObject.CompareTag("Props")) rigid.gravityScale = 0.05f;
                if (rigid.gameObject.CompareTag("Enemy") && Application.isPlaying)
                {
                    var entity = rigid.gameObject.GetComponent<Entity>();
                    if (entity.Type != Entity.EntityType.Type_Kraken) rigid.gravityScale = GravityScale;
                }
                else if (rigid.gameObject.CompareTag("Enemy") && !Application.isPlaying) continue;
                else rigid.gravityScale = GravityScale;
            }
        }
    }



    #region  Temp Save-Load System

    public void SaveGame()
    {
        var pos = Player.Instance.transform.position;
        PlayerPrefs.SetFloat(PerfTable.perf_LastPosX, pos.x);
        PlayerPrefs.SetFloat(PerfTable.perf_LastPosY, pos.y);
        PlayerPrefs.SetFloat(PerfTable.perf_LastPosZ, pos.z);

        PlayerPrefs.SetFloat(PerfTable.perf_LastHealth, Player.Instance.Health);
        PlayerPrefs.SetFloat(PerfTable.perf_LastFocus, Player.Instance.Focus);

        PlayerPrefs.SetString(PerfTable.perf_LastScene, SceneManager.GetActiveScene().name);

        PlayerPrefs.Save();
    }

    public void LoadScene(string sceneName)
    {
        Loading.LoadScene(sceneName);
    }

    public void LoadGame()
    {
        Vector3 pos = Vector3.zero;
        pos.x = PlayerPrefs.GetFloat(PerfTable.perf_LastPosX);
        pos.y = PlayerPrefs.GetFloat(PerfTable.perf_LastPosY);
        pos.z = PlayerPrefs.GetFloat(PerfTable.perf_LastPosZ);

        Player.Instance.Health = PlayerPrefs.GetFloat(PerfTable.perf_LastHealth);
        Player.Instance.Focus = PlayerPrefs.GetFloat(PerfTable.perf_LastFocus);

        Player.Instance.transform.position = pos;
        isLoadingGame = false;
    }

    #endregion

}


[CustomEditor(typeof(LevelManager))]
public class LevelEditorGUI : Editor
{
    public override void OnInspectorGUI()
    {

        GUILayout.Label("\nActive Level/Scene:  " + SceneManager.GetActiveScene().name + "\n", EditorStyles.boldLabel);

        base.OnInspectorGUI();

        if (LevelManager.Instance == null)
        {
            LevelManager.Instance = GameObject.FindAnyObjectByType<LevelManager>();
            Debug.Log("Reset level manager instance.");
        }

        if (GUILayout.Button("Under Water Mode"))
        {
            LevelManager.Instance.GravityScale = 0;
            LevelManager.Instance.OrientGravity();
        }

        if (GUILayout.Button("Surface or Inside Mode"))
        {
            LevelManager.Instance.GravityScale = 1;
            LevelManager.Instance.OrientGravity();
        }

        GUILayout.Space(2);
        GUILayout.Label("\nSave & Load System:");

        if (GUILayout.Button("Save Game"))
        {
            if (!Application.isPlaying) return;
            LevelManager.Instance.SaveGame();
        }

        if (GUILayout.Button("Load Game"))
        {
            if (!Application.isPlaying) return;
            LevelManager.Instance.LoadGame();
        }

    }
}