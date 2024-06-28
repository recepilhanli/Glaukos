using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A class that manages the sounds in the game
/// Addition Date: 28/06/24
/// Note: This class added too late, so it is not used properly in the project
/// </summary>

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    private bool _dictioanryFoldout = false;
    public override void OnInspectorGUI()
    {
        if (!Application.isPlaying)
        {
            DrawDefaultInspector();
            return;
        }


        SoundManager soundManager = (SoundManager)target;

        _dictioanryFoldout = EditorGUILayout.Foldout(_dictioanryFoldout, "Active Sounds");



        if (soundManager.Sounds.Count > 0 && _dictioanryFoldout)
        {
            GUILayout.Space(10);
            GUILayout.Label("Sounds", EditorStyles.boldLabel);

            List<string> keys = new List<string>(soundManager.Sounds.Keys);

            foreach (var key in keys)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(key);
                soundManager.Sounds[key] = (AudioClip)EditorGUILayout.ObjectField(soundManager.Sounds[key], typeof(AudioClip), false);
                if (GUILayout.Button("Remove")) soundManager.Sounds.Remove(key);
                GUILayout.EndHorizontal();
            }
        }
    }
}
#endif

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    private struct AtlantisSound
    {
        public string name;
        public AudioClip clip;
    }

    public Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>();

    [SerializeField] private List<AtlantisSound> _SoundList = new List<AtlantisSound>();

    public static SoundManager Instance;

    private static AudioSource _audioSourceInstance = null;

    private void Awake()
    {
        Instance = this;

        foreach (var sound in _SoundList)
        {
            Sounds.Add(sound.name, sound.clip);
        }

        _SoundList = null;

        _audioSourceInstance = transform.GetComponentInChildren<AudioSource>(true);

        DontDestroyOnLoad(gameObject);
    }


    public static void PlaySound2D(AudioClip _clip, float _volume = 1f)
    {
        var tempSource = Instantiate(_audioSourceInstance, null);
        tempSource.volume = _volume;
        tempSource.clip = _clip;
        if (AudioListener.pause) tempSource.ignoreListenerPause = true;
        tempSource.gameObject.SetActive(true);
        tempSource.Play();
        Destroy(tempSource.gameObject, 5f);
    }

    public static void PlaySound2D(string _clipName, float _volume = 1f)
    {
        if (Instance.Sounds.ContainsKey(_clipName))
        {
            PlaySound2D(Instance.Sounds[_clipName], _volume);
        }
    }


}
