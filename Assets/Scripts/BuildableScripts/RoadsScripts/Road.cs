using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the roads and store it's information in a scriptable object
/// </summary>
[CreateAssetMenu(menuName = "Road", fileName = "New Road")]
public class Road : Buildable {

    //This scriptable object contains the road data. Such as number of rails, velocity, etc.
    public int nRails;
    public float velocity;

}
