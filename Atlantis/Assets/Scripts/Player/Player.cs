
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        [SerializeField] Slider _VolumeSlider;

        [ContextMenu("Call Start Function")]

        void Start()
        {
            Type = EntityType.Type_Player;
            Application.targetFrameRate = 60;
            Instance = this;
            _StartSpeed = _Speed;
            InitCamera();
            Cursor.visible = true;

            Physics2D.IgnoreLayerCollision(30, 6, true);
            Physics2D.IgnoreLayerCollision(6, 30, true);
            _VolumeSlider.value = PlayerPrefs.GetFloat(PerfTable.perf_Volume, 1f);

            if (PlayerPrefs.HasKey(PerfTable.perf_RemainingLifes))
            {
                RemainingLifes = PlayerPrefs.GetInt(PerfTable.perf_RemainingLifes);
            }
            else
            {
                PlayerPrefs.SetInt(PerfTable.perf_RemainingLifes, 5);
                RemainingLifes = 5;
                PlayerPrefs.Save();
            }

            Deformation();
            //Cursor.SetCursor(_CursorTexture,Vector2.zero,CursorMode.ForceSoftware);
        }

        void Update()
        {
            if (PauseMenu.instance != null)
            {
                if (PauseMenu.instance.isPaused) return;
            }

            CameraUpdate();

            Combat();

            if (CanMove) Movement();

            else _Rigidbody.velocity = Vector2.zero;

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Loading.LoadScene(PerfTable.perf_Level1);
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Loading.LoadScene(PerfTable.perf_Level2);
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                Loading.LoadScene(PerfTable.perf_Level3);
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                Loading.LoadScene(PerfTable.perf_Level4);
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SceneManager.LoadScene("Menu");
            }

#endif
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

        public void ChangeVolume()
        {
            AudioListener.volume = _VolumeSlider.value;
            PlayerPrefs.SetFloat(PerfTable.perf_Volume, AudioListener.volume);
            PlayerPrefs.Save();
        }
    }

#if UNITY_EDITOR
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
#endif

} //namespace
