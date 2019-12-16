using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestObjectNavMesh : MonoBehaviour {

    public NavMeshAgent agent;
    public Vector3 targetPoint; 

    void Start() {

        agent.SetDestination(new Vector3(-4.5f, 1, 0));

    }

    /*private void Update() {
        
        if (Input.GetMouseButtonDown(0)) {
            targetPoint = new Vector3(-0.5f, 0.5f, -6.5f); 
            agent.SetDestination(targetPoint);
        }

    }*/

}
