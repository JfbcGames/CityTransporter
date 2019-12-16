using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the runtime behaviour of any road with it's particular data.
/// </summary>
public class RoadBehaviour : BuildableBehaviour {

    private int nRails;
    private float maxVelocity;
    public Vector3 startPoint;
    public Vector3 endPoint;

    /// <summary>
    /// Sets the road data from the scriptable object
    /// </summary>
    /// <param name="r">the road scriptable object</param>
    /// <param name="startPoint">the road start point</param>
    /// <param name="endPoint">the road end point</param>
    public void SetData(Road r, Vector3 startPoint, Vector3 endPoint) {

        this.nRails = r.nRails;
        this.maxVelocity = r.velocity;
        this.startPoint = startPoint;
        this.endPoint = endPoint;

    }

    public Vector3 GetStartPoint() {
        return startPoint;
    }
    public Vector3 GetEndPoint() {
        return endPoint;
    }
    public Vector3[] GetExtremes() {
        return new Vector3[] { startPoint, endPoint };
    }

    /// <summary>
    /// Get the points of the road to which other objects can join.
    /// </summary>
    /// <returns>A Vector3 array with all that points</returns>
    public Vector3[] GetRoadAtachPoints() {
        return GetExtremes();
    }

}
