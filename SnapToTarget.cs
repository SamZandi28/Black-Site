using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SnapToTarget : MonoBehaviour
{
    public Transform targetObject; //the main obj
    public Transform targetOrientationPoint; // the point on the target object to snap
    public Transform snapOrientationPoint; // the point on the snappable object 
    public float snapDistance = 0.2f; // the max distance
    public float snapSpeed = 20.0f; // the snap speed

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grabInteractable.selectExited.AddListener(CheckForSnap);
    }
    private void CheckForSnap(SelectExitEventArgs args)
    {
        // calculate the distance between the snap points for snappable objects
        float distance = Vector3.Distance(snapOrientationPoint.position, targetOrientationPoint.position);
        if (distance <= snapDistance) // if the object within the snap distance
        {
            //calculate the offset to ensure the snap point to align with target
            Vector3 offset = snapOrientationPoint.position - transform.position;

            // used coroutine to smoother the snap
            StartCoroutine(SmoothSnap(targetOrientationPoint.position - offset));
            transform.position = targetOrientationPoint.position - offset;
            transform.rotation = targetOrientationPoint.rotation;

            transform.parent = targetObject;

            rb.isKinematic = true;
        }
    }

    private IEnumerator SmoothSnap(Vector3 targetPosition)
    {
        rb.isKinematic = true;

        // continue moving objec unitl it snaps to the the targret position
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetOrientationPoint.rotation, Time.deltaTime * snapSpeed);

            yield return null;
        }

        transform.SetPositionAndRotation(targetPosition, targetOrientationPoint.rotation);

        transform.SetParent(targetObject);

        grabInteractable.enabled = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}