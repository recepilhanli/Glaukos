using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is for the properties of an entity.
///  Do not set variables in any script!
/// </summary>
[CreateAssetMenu(fileName = "Entity Properties Example", menuName = "Atlantis/Entity/Create a Entity's Properties", order = 1)]
public class EntityProperties : ScriptableObject
{
    public float Health = 100f;
    public float Speed = 10f;
    public float Damage = 10f;
    public bool CanSwim = false;
}
