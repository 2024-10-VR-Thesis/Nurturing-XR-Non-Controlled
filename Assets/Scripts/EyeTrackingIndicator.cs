using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingIndicator : MonoBehaviour
{
    [SerializeField]
    private float indicatorDistance = 1.0f;

    [SerializeField]
    private float indicatorWidth = 0.01f;

    [SerializeField]
    private LayerMask layersToInclude;

    [SerializeField]
    private Color indicatorColorDfState = Color.yellow;

    private LineRenderer lineRenderer;
    private bool askedAlready { get; set; }

    //private List<EyeInteractable> eyeInteractables = new List<EyeInteractable>();

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupIndicator();
    }

    void SetupIndicator()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = indicatorWidth;
        lineRenderer.endWidth = indicatorWidth;
        lineRenderer.startColor = indicatorColorDfState;
        lineRenderer.endColor = indicatorColorDfState;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y,
                                                     transform.position.z + indicatorDistance));
    }


    void FixedUpdate()
    {
        RaycastHit hit;

        Vector3 rayCastDirection = transform.TransformDirection(Vector3.forward) * indicatorDistance;

        if (Physics.Raycast(transform.position, rayCastDirection, out hit, 40f, layersToInclude) && !askedAlready)
        {
            //UnSelect();
            if (hit.transform != null)
            {
                var eyeInteractable = hit.transform.GetComponent<EyeInteractable>();
                //eyeInteractables.Add(eyeInteractable);
                eyeInteractable.IsHovered = true;
            }
        }
    }

    /*
    void UnSelect(bool clear = false)
    {
        Debug.Log(eyeInteractables.Count);
        if (eyeInteractables.Count > 0) // Check if list has elements before iterating
        {
            foreach (var interactable in eyeInteractables)
            {
                interactable.IsHovered = false;
            }
        }
        if (clear)
        {
            eyeInteractables.Clear();
        }
    }
    */

}