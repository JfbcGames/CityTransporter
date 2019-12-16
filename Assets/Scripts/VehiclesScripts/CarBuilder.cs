using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBuilder : MonoBehaviour {

    public Vehicle vehicle;

    public SpriteRenderer sRenderer;

    void Start() {

        sRenderer.sprite = vehicle.artwork;

    }

}
