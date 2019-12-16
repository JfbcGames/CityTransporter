using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents all the objects that the player can build and stores it's basic data in a scriptable object form.
/// </summary>
public class Buildable : ScriptableObject {

    private int buildableTypeID;
    public Mesh mesh;
    //public Material mat;
    public Material[] mats;
    public Sprite menuArtwork;
    public Vector3 BuildingPointOffset;
    public string buildingName;
    public string description;
    public Resources cost;


    /// <summary>
    /// Get the object mesh
    /// </summary>
    /// <returns>The object mesh</returns>
    public Mesh GetMesh() {
        return mesh;
    }

    /// <summary>
    /// It shouldn't be used. Change the stored object mesh
    /// </summary>
    /// <param name="m">the mesh to store</param>
    public void SetMesh(Mesh m) {
        this.mesh = m;
    }

    /// <summary>
    /// Get the object material
    /// </summary>
    /// <returns>The object material</returns>
    public Material[] GetMaterials() {
        return mats;
    }

    /// <summary>
    /// It shouldn't be used. Change the stored object material
    /// </summary>
    /// <param name="m">the material to store</param>
    public void SetMaterials(Material[] m) {
        this.mats = m;
    }

    /// <summary>
    /// Get the object generic name
    /// </summary>
    /// <returns>the object generic name</returns>
    public string GetName() {
        return this.buildingName;
    }

}
