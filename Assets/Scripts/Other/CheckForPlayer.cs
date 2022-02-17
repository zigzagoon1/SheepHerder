using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForPlayer : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float triggerDistance;
    [SerializeField] Camera mainCamera;

    private Collider thisCollider;
    private Vector3 closestPoint;
    private CameraFollow cam;
    private bool isNearRightEdge = false;
    private bool isNearLeftEdge = false;
    private bool isNearTopEdge = false;
    private bool isNearBottomEdge = false;

    private void Start()
    {
        player = GameObject.Find("PlayerParent").transform;
        cam = mainCamera.GetComponent<CameraFollow>();
        thisCollider = GetComponent<Collider>();
    }
    private void FixedUpdate()
    {
        closestPoint = thisCollider.ClosestPoint(player.position);
        if (Vector3.Distance(closestPoint, player.position) <= triggerDistance)
        {
            SetBoolTrue();
        }
        else
        {
            SetBoolFalse();
        }
    }
    void SetBoolTrue()
    {
        if (this.name == "Right")
        {
            isNearRightEdge = true;
            cam.IsNearRightEdge = isNearRightEdge;

        }
        else if (this.name == "Left")
        {
            isNearLeftEdge = true;
            cam.IsNearLeftEdge = isNearLeftEdge;
        }
        else if (this.name == "Top")
        {
            isNearTopEdge = true;
            cam.IsNearTopEdge = isNearTopEdge;
        }
        else if (this.name == "Bottom")
        {
            isNearBottomEdge = true;
            cam.IsNearBottomEdge = isNearBottomEdge;
        }
    }
    void SetBoolFalse()
    {
        if (this.name == "Right")
        {
            isNearRightEdge = false;
            cam.IsNearRightEdge = isNearRightEdge;
        }
        else if (this.name == "Left")
        {
            isNearLeftEdge = false;
            cam.IsNearLeftEdge = isNearLeftEdge;
        }
        else if (this.name == "Top")
        {
            isNearTopEdge = false;
            cam.IsNearTopEdge = isNearTopEdge;
        }
        else if (this.name == "Bottom")
        {
            isNearBottomEdge = false;
            cam.IsNearBottomEdge = isNearBottomEdge;
        }
    }
}
