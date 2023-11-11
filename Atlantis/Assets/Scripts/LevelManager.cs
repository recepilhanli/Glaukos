using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    [Range(-2, 2)] public float GravityScale = 1f;

    void Start()
    {
        Instance = this;

        OrientGravity();

        Physics2D.IgnoreLayerCollision(3, 6);
        Physics2D.IgnoreLayerCollision(6, 3);

        Physics2D.IgnoreLayerCollision(3, 8);
        Physics2D.IgnoreLayerCollision(8, 3);
        Physics2D.IgnoreLayerCollision(6, 8);
        Physics2D.IgnoreLayerCollision(8, 6);
        Physics2D.IgnoreLayerCollision(7, 8);
        Physics2D.IgnoreLayerCollision(8, 8);

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
            if (GravityScale != 0) rigid.gravityScale = GravityScale;
            else
            {
                if (rigid.gameObject.CompareTag("Props")) rigid.gravityScale = 0.05f;
                else rigid.gravityScale = GravityScale;
            }
        }
    }
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


    }
}