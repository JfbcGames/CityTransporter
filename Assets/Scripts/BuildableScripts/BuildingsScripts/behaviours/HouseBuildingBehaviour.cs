using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuildingBehaviour : BuildingBehaviour {

    private void Start() {
        StartCoroutine(ConsumeResources());
    }

}
