using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set up a game object with the information of a building scriptable object
/// </summary>
public class BuildingBuilder : MonoBehaviour {

    private Mesh mesh;
    private Material material;
    private MeshRenderer mr;
    private MeshFilter mf;

    public void SetupBuilder(MeshFilter meshf, MeshRenderer meshr, Mesh m, Material mat) {
        this.mf = meshf;
        this.mr = meshr;
        this.mesh = m;
        this.material = mat;
    }

    public void SetupBuilder(Building b) {
        GetComponent<MeshFilter>().mesh = b.GetMesh();
        //GetComponent<MeshRenderer>().material = b.GetMaterial();
        GetComponent<MeshRenderer>().materials = b.GetMaterials();
    }

    public void SetMesh(Mesh m) {
        this.mesh = m;
    }

}
