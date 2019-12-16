using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransportListener {

    void OnExportNeeded(BuildingBehaviour destination);
    void OnImportNeeded(BuildingBehaviour destination);

}
