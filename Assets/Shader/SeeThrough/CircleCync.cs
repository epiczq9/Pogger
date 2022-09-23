using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCync : MonoBehaviour
{
    public static int PosID = Shader.PropertyToID("_Position");
    public static int SizeID = Shader.PropertyToID("_Size");

    public Material WallMaterial;
    public Camera Camera;
    public LayerMask Mask;
    GameObject lastGameObject = null;



    void Update()
    {
        var dir = Camera.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3000,  Mask))
        {
            lastGameObject = hit.collider.gameObject;
            lastGameObject.GetComponent<Renderer>().material.SetFloat(SizeID, 1);
        }
        else if(lastGameObject != null)
        {
            lastGameObject.GetComponent<Renderer>().material.SetFloat(SizeID, 0);
            lastGameObject = null;
        }

        var view = Camera.WorldToViewportPoint(transform.position);
        WallMaterial.SetVector(PosID, view);

    }
}
