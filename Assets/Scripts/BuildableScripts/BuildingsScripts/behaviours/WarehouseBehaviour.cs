using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseBehaviour : BuildingBehaviour {

    private void Start() {
        StartCoroutine(ConsumeResources());
        //GameManager.instance.UpdateResourcesIndicator(); //no funciona porque cuando se instancia aun no tiene puestos los initial resources
    }

    public override void Imported() {
        base.Imported();
        GameManager.instance.UpdateResourcesIndicator();
    }
    public override void Exported() {
        base.Exported();
        GameManager.instance.UpdateResourcesIndicator();
    }

}
