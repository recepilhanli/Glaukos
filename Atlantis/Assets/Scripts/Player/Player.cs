using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Player
{
    public partial class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        [ContextMenu("Call Start Function")]
        void Start()
        {
            Application.targetFrameRate = 60;
            Instance = this;
        }

        void Update()
        {
        }


        void FixedUpdate()
        {
            Move();
        }
    }

    [CustomEditor(typeof(Player))]
    public class EditorGUI : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Reset Player"))
            {
                var player = Player.Instance;
                //TO DO
                Debug.Log("Player variables has ben reset.");
            }
        }
    }

} //namespace
