using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBuildingBehaviour : BuildingBehaviour {

    private Resources production;
    private bool isClearingOut;
    private bool isProducing;

    private void Start() {

        isProducing = true;
        StartCoroutine(ProduceResources());
        StartCoroutine(ConsumeResources());

    }

    private void AskForClearOut(bool force) {
        Debug.Log(gameObject.name + " is asking for clear out");
        if (force || !isClearingOut) {
            transportListener.OnExportNeeded(this);
        } 
    }

    public override void Exported() {
        if (!isProducing) { //si no esta produciendo, ha forzado el pedir mas vaciado. Por lo que isClearingOut seguira siendo true hasta que se lo vuelvan a recoger
            isProducing = true;
            StartCoroutine(ProduceResources());
        } else {
            isClearingOut = false;
        }
    }

    public override void SetDataToHinherits(Building b) {
        this.production = ((WorkBuilding)b).production;
    }

    IEnumerator ProduceResources() { 
        while (isProducing) {
            actualStock = Resources.Sum(actualStock, production);
            if (Resources.CompareResources(actualStock, maxCapacity)) {
                isProducing = false;
                AskForClearOut(true);
            } else if (Resources.CompareResources(actualStock, maxCapacity, 80)) {
                AskForClearOut(false);
            }
            yield return new WaitForSeconds(1);
        }
    }

}
