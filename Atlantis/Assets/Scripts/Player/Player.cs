using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Player
{
    public partial class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }
    
        [SerializeField,Tooltip("Table")]  KeybindTable _KeybindTable;
        
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
            Movement();
        }
    }

    [CustomEditor(typeof(Player))]
    public class EditorGUI : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Reset Player Variables"))
            {
                var player = Player.Instance;
                //TO DO
                Debug.Log("Player variables has ben reset.");
            }


        }
    }

} //namespace
