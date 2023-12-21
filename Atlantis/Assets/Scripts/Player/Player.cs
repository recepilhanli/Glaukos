
using UnityEditor;
using UnityEngine;

//this is the base player script
namespace MainCharacter
{
    /// <summary>
    /// This class is used to manage the player.
    /// </summary>
    public partial class Player : Entity
    {
        public static Player Instance { get; private set; }

        [Space]
        [Header("Player")]


        [SerializeField, Tooltip("Keybinding Table")] KeybindTable _KeybindTable;

        [SerializeField, Tooltip("Entity Flag")] EntityFlags _Flag;

        public bool CanMove = true;

        [Space]
        [Header("Visual")]
        [SerializeField, Tooltip("Renderer of the Player's Sprite")] SpriteRenderer _PlayerRenderer;
        [SerializeField] LayerMask PlayerMask;
        public LayerMask EnemyMask;
        public LayerMask BossMask;
        [SerializeField] Texture2D _CursorTexture;

        [ContextMenu("Call Start Function")]



        void Start()
        {
            Type = EntityType.Type_Player;
            Application.targetFrameRate = 60;
            Instance = this;
            _StartSpeed = _Speed;
            InitCamera();
            //Cursor.SetCursor(_CursorTexture,Vector2.zero,CursorMode.ForceSoftware);
        }

        void Update()
        {
            CameraUpdate();
            if (CanMove)
            {
                Movement();
                Combat();
            }
            else _Rigidbody.velocity = Vector2.zero;
        }

        /// <summary>
        /// This method is used to manage the physics of the player.
        /// </summary>
        void FixedUpdate()
        {

        }


        /// <summary>
        /// This method is used to manage the player's movement.
        /// </summary>
        /// <returns>Flag of the Entity</returns>
        public override EntityFlags GetEntityFlag()
        {
            return _Flag;
        }
    }

    /// <summary>
    /// this class is used to manage the editor of the player.
    /// </summary>

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
