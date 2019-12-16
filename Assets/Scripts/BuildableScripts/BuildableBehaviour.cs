using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableBehaviour : MonoBehaviour {

    public void DestroyBuildable() {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    public virtual void SetData(Buildable buldableData) {

    }

}
