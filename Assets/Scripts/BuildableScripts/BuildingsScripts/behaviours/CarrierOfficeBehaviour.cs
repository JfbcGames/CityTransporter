using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierOfficeBehaviour : BuildingBehaviour {
    
    private List<VehicleBehaviour> storedVehicles;

    private void Start() {
        storedVehicles = new List<VehicleBehaviour>();
        //StartCoroutine(ConsumeResources());
    }

    public void StoreVehicle(VehicleBehaviour v) {
        storedVehicles.Add(v);
        v.gameObject.transform.position = roadAttachPoint;
        v.gameObject.SetActive(false);
    }

    public void ExitVehicle(VehicleBehaviour v) {
        storedVehicles.Remove(v);
    }

    public List<VehicleBehaviour> GetStoredVehicles() {
        return storedVehicles;
    }

}
