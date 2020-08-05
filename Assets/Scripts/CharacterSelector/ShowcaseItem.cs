using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShowcaseItem : MonoBehaviour {
    public float angularSpeed = 2f;

    public List<Renderer> renderers = new List<Renderer>();
    void Start() {
        if( null == renderers) {
            renderers = GetComponentsInChildren<Renderer>().ToList();
        }
    }

    // Update is called once per frame
    void Update() {
        transform.Rotate(transform.up, angularSpeed * Time.deltaTime);
    }

    public void ChangeColor(Color color) {
        foreach (var renderer in renderers) {
            renderer.material.SetColor("_EmissionColor", color);
        }
    }

    public void ResetRotation() {
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
