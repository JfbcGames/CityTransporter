using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : BuildableBehaviour {

    public ITransportListener transportListener;
    public Resources maxCapacity;
    public Resources consumption;
    public Resources actualStock;
    public Vector3 roadAttachPoint;
    public int maxDurability;
    public int durability;
    public bool isFeeded;
    public bool isActive;

    public void SetTransportListener(ITransportListener tl) {
        this.transportListener = tl;
    }

    public Resources TakeResourcesOutOfBuilding(Resources vehicleCapacity) {
        Resources ret;
        if (Resources.CompareResources(vehicleCapacity, actualStock, 80)) { //cabe el 80% del stock en el vehiculo
            ret = Resources.ReducedRes(actualStock, 20);                    //returnea el 80% del stock 
            actualStock = Resources.Sub(actualStock, ret);
        } else {
            ret = vehicleCapacity;                                  //falta evitar que el vehiculo se coja materiales que el edificio no tiene(porque coge todos los que le quepan)
            actualStock = Resources.Sub(actualStock, ret);
        }
        return ret;
    }

    public Resources TakeResourcesFromVehicle(Resources vehicleTransported) {
        Resources ret;
        Resources canAccept = Resources.Sub(maxCapacity, actualStock);
        if (Resources.CompareResources(canAccept, vehicleTransported)) {
            actualStock = Resources.Sum(actualStock, vehicleTransported);
            ret = Resources.Zero;
        } else {
            ret = Resources.Sub(vehicleTransported, canAccept);
            actualStock = Resources.Sum(actualStock, canAccept);
        }
        return ret;
    }

    public virtual void Exported() {
        //override from child class;
    }

    public virtual void Imported() {
        //everride from child class
    }

    public Resources GetMaxCapacity() {
        return maxCapacity;
    }
    public Resources GetActualStock() {
        return actualStock;
    }
    public Resources GetConsumption() {
        return consumption;
    }

    public override void SetData(Buildable b) {
        
        this.maxCapacity = ((Building)b).maxCapacity;
        this.actualStock = ((Building)b).initialStock;
        this.consumption = ((Building)b).consumption;
        this.maxDurability = ((Building)b).maxDurability;
        this.durability = this.maxDurability;
        SetDataToHinherits((Building)b);

    }

    public virtual void SetDataToHinherits(Building b) {

    }

    public IEnumerator ConsumeResources() {

        while(isActive) {
            actualStock = Resources.Sub(actualStock, consumption);
            if (Resources.CompareResources(actualStock, Resources.Zero)) {
                if (durability < maxDurability) {
                    durability++; ;
                }
                if (!Resources.CompareResources(actualStock, maxCapacity, 10)) {
                    transportListener.OnImportNeeded(this);
                }
            } else {
                durability--;
                if (durability <= 0) {
                    DestroyBuildable();
                }
            }
            yield return new WaitForSeconds(1);
        }

    }

}
