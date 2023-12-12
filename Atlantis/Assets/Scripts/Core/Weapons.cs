using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapons : MonoBehaviour
{

    public enum Weapon_Types
    {
        Type_None,
        Type_Fists,
        Type_Spear,
        Type_Sword,
    }

    public Weapon_Types Type { get; protected set; } = Weapon_Types.Type_None;



}
