using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Properties Example", menuName = "Atlantis/Enemies/Create a Enemy Properties", order = 1)]
public class EnemyProperties : ScriptableObject
{
    public float Health = 100f;
    public float Speed = 10f;
    public float Damage = 10f;
    public bool CanSwim = false;
}
