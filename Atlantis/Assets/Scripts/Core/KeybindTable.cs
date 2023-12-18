using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this class is for the keybind table
/// </summary>
[CreateAssetMenu(fileName = "Keybind Table Example", menuName = "Atlantis/Keybinds/Create Keybind Table", order = 1)]
public class KeybindTable : ScriptableObject
{

    [Header("Actions")]
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode Attack = KeyCode.Mouse0;
    public KeyCode HeavyAttack = KeyCode.Mouse1;
    public KeyCode SpecialAttack = KeyCode.Mouse2;
    public KeyCode HealKey = KeyCode.H;
    public KeyCode RageKey = KeyCode.R;
    public KeyCode Using_Item_1 = KeyCode.Alpha1;
    public KeyCode Using_Item_2 = KeyCode.Alpha2;
    public KeyCode Using_Item_3 = KeyCode.Alpha3;

}
