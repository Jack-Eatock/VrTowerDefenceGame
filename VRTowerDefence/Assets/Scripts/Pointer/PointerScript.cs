using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerScript : MonoBehaviour
{

    public float DefaultLength = 5.0f;
    public GameObject Dot;

    private LineRenderer _lineRenderer = null;

    public VRPointerInputModule VRInputModule;





    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateLine();
    }

    
    private void UpdateLine()
    {
        // use Default or Distance.
        PointerEventData data = VRInputModule.Getdata();

        float targetlength = data.pointerCurrentRaycast.distance == 0 ? DefaultLength : data.pointerCurrentRaycast.distance;

        // Raycast
        RaycastHit hit = CreateRayCast(targetlength);

        // Default
        Vector3 endpos = transform.position + (transform.forward * targetlength);

        // Or based on Hit.
        if (hit.collider != null && targetlength == DefaultLength)
        {
            endpos = hit.point;
        }

        // Set pos of dot
        Dot.transform.position = endpos;

        // Set line renderer
        _lineRenderer.SetPosition(0, transform.position); // Line starts at pointer :)
        _lineRenderer.SetPosition(1, endpos); // Line ends at end pos . Either defualt or hit pos.

    }

    private RaycastHit CreateRayCast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, DefaultLength);

        return hit;
    }

}
