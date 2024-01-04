using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] int _Life = 0;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.3f);
        yield return null;
    }

}
