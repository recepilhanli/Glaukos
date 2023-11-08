using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MainCharacter
{
    public partial class Player : Entity
    {
        public static Player Instance { get; private set; }

        [SerializeField, Tooltip("Keybinding Table")] KeybindTable _KeybindTable;

        [SerializeField, Tooltip("Entity Flag")] EntityFlags _Flag;

        [SerializeField] LayerMask PlayerMask;
        [SerializeField] LayerMask EnemyMask;


        [ContextMenu("Call Start Function")]
        void Start()
        {
            Application.targetFrameRate = 60;
            Instance = this;
            Physics2D.IgnoreLayerCollision(3, 6);
             Physics2D.IgnoreLayerCollision(6, 3);    
        }

        void Update()
        {
        }


        void FixedUpdate()
        {
            Movement();
        }

        public override EntityFlags GetEntityFlag()
        {
            return _Flag;
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
