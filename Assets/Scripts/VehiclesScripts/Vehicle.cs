using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Vehicle", menuName = "Vehicle")]
public class Vehicle : ScriptableObject {

    public string modelName;

    public Sprite artwork;

    public float maxVelocity;
    public float durability;
    public int resourcesCapacity;
    public int peopleCapacity;

}
