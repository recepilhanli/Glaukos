using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Keybinds/Create Keybind Table", order = 1)]
public class KeybindTable : ScriptableObject
{

[Header("Actions")]
public KeyCode JumpKey = KeyCode.Space;
public KeyCode Attack = KeyCode.Mouse0;
public KeyCode HeavyAttack = KeyCode.Mouse1;
public KeyCode SpecialAttack = KeyCode.Mouse2;
public KeyCode HealKey = KeyCode.H;   
}
