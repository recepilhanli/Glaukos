using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// interface for the enemy AI
/// </summary>
public interface IEnemyAI
{
    public void OnDetected(Entity _entity);
    /// <summary>
    /// Do not set any variable of _properties in any script!
    /// </summary>
    /// <param name="_properties"></param>
    public void Init(EntityProperties _properties);
}

