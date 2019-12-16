using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the methods to set up the road with the physical attributes that receives from the player in execution time.
/// </summary>
public class RoadBuilder : MonoBehaviour {

    public Vector3 startPoint, endPoint;
    public float roadLenght;

    /// <summary>
    /// Set up the road with the physical attributes that receives from the player in execution time.
    /// </summary>
    /// <param name="start">the road start point</param>
    /// <param name="end">the road end point</param>
    /// <param name="roadData">the road data from the scriptable object</param>
    public void Build(Vector3 start, Vector3 end, Road roadData) {

        float distance = Vector3.Distance(start, end);
        transform.position = new Vector3((start.x + end.x) / 2, 0, (start.z + end.z) / 2); //sustituir el 0 por la altura del terreno en ese punto
        transform.localScale = new Vector3(distance, 1, 1);
        //transform.GetChild(0).transform.GetChild(0).transform.localScale /= distance;
        transform.GetChild(0).transform.GetChild(0).transform.localScale = new Vector3(transform.GetChild(0).transform.GetChild(0).transform.localScale.x/distance, 1, 1);
        transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().size = new Vector2(distance, 1);
        transform.forward = Vector3.Cross(Vector3.up, end - start);
        GetComponent<RoadBehaviour>().SetData(roadData, start, end);

    }

}
