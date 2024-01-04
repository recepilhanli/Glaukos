using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;

/// <summary>
/// Purpose: Shows deformation of the player
/// </summary>
public class PlayerLife : MonoBehaviour
{
    [SerializeField] int _Life = 0;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Life: " + _Life + " PlayerLife: " + Player.RemainingLifes);
        if (_Life < Player.RemainingLifes)
        {
            Destroy(gameObject);
        }
        yield return null;
    }

}
