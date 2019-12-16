using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents all the buildings and store it's information in a Scriptable Object
/// </summary>
public abstract class Building : Buildable {

    //maxCapacity, consumption, actualStock, maxDurability;
    public Resources maxCapacity;
    public Resources consumption;
    public Resources initialStock;
    public int maxDurability;

    /*public int peopleCapacity;
    public int peopleInside;

    public int EnterPeople(int n) {
        peopleInside += n;
        return peopleInside;
    }

    public int ExitPeople(int n) {
        peopleInside -= n;
        return peopleInside;
    }

    public int GetPeopleCapacity() {
        return peopleCapacity;
    }

    public int GetPeopleInside() {
        return peopleInside;
    }*/

}
