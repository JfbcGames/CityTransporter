using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Respresents the buildings destinated to resources production or consumption
/// </summary>
[CreateAssetMenu(fileName = "New Work Building", menuName = "Buildings/WorkBuilding")]
public class WorkBuilding : Building {

    public Resources production;

}
