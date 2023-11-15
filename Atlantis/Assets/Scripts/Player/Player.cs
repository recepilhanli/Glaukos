
using UnityEditor;
using UnityEngine;

namespace MainCharacter
{
    public partial class Player : Entity
    {
        public static Player Instance { get; private set; }

        [Space]
        [Header("Player")]
        [SerializeField, Tooltip("Keybinding Table")] KeybindTable _KeybindTable;

        [SerializeField, Tooltip("Entity Flag")] EntityFlags _Flag;


        [Space]
        [Header("Visual")]
        [SerializeField, Tooltip("Renderer of the Player's Sprite")] SpriteRenderer _PlayerRenderer;
        [SerializeField] LayerMask PlayerMask;
        public LayerMask EnemyMask;

        [SerializeField] Texture2D _CursorTexture;

        [ContextMenu("Call Start Function")]

        void Start()
        {
            Type = EntityType.Type_Player;
            Application.targetFrameRate = 60;
            Instance = this;
            InitCamera();
            //Cursor.SetCursor(_CursorTexture,Vector2.zero,CursorMode.ForceSoftware);
        }

        void Update()
        {
            CameraUpdate();
            Movement();
            Combat();
        }


        void FixedUpdate()
        {

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
