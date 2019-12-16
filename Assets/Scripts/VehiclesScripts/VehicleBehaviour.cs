using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleBehaviour : MonoBehaviour {
    
    public NavMeshAgent agent;
    private BuildingBehaviour[] destinations;   //the buildable objects to go.
    private Coroutine travelProcess;
    private Resources maxCharge;
    private Resources charge;
    private bool busy;
    private bool pointAchieved;
    private int mission;                        //the function/objective/mision to complete. (0: nothing/done/error 1: carry resources to building, 2: go to building to get resources

    private void Start() {
        SetData(new Resources(100, 100, 100, 100));
    }

    public void SetDestinations(BuildingBehaviour[] dests, int firsMission) {
        destinations = dests;
        mission = firsMission;
        travelProcess = StartCoroutine(DoTravel());
        busy = true;
    }

    private void ExecuteMission(BuildingBehaviour bb) {
        if (mission == 2) {
            bb.TakeResourcesOutOfBuilding(Resources.Sub(maxCharge, charge));
            bb.Exported();
        } else if (mission == 1) {
            bb.TakeResourcesFromVehicle(charge);
            bb.Imported();
            busy = false;
        } else if (mission == 0) {
            GameManager.instance.SaveVehicleOnCarrierOffice(this, (CarrierOfficeBehaviour)bb);
        } else {
            Debug.Log("Mision desconocida");
        }
    }

    public void ResetVehicle() {
        charge = new Resources(new int[System.Enum.GetValues(typeof(IResources)).Length]);
        destinations = null;
        busy = false;
        StopCoroutine(travelProcess);
        gameObject.SetActive(false);
    }

    public bool IsBusy() {
        return busy;
    }

    public void SetData(Resources maxCharge) {
        agent = GetComponent<NavMeshAgent>();
        this.maxCharge = maxCharge;
        busy = false;
    }

    IEnumerator DoTravel() {

        float waitTime = 1;

        for (int i = 0; i < destinations.Length; i++) {
            pointAchieved = false;
            agent.SetDestination(destinations[i].roadAttachPoint);
            while(!pointAchieved) {
                if (agent.pathStatus == NavMeshPathStatus.PathComplete) { //has arrived
                    ExecuteMission(destinations[i]);
                    mission--;
                    pointAchieved = true;
                } else { // has not arrived
                    waitTime = agent.remainingDistance / agent.speed;
                    if (waitTime < 1) {
                        waitTime = 1;
                    }
                }
                yield return new WaitForSeconds(waitTime);
            }
            
        }

    }

}
