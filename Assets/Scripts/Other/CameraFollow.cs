using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraFollow : MonoBehaviour
{
    public float camSpeed;
    public GameObject player;
    public Vector3 offset;
    private Vector3 offset2;
    Camera mainCamera;
    float distance;
    Bounds playerBounds;
    Vector3 playerPrevPos, playerMoveDir;

    private bool _isNearRightEdge = false;
    private bool _isNearLeftEdge = false;
    private bool _isNearTopEdge = false;
    private bool _isNearBottomEdge = false;

    public bool IsNearRightEdge { get { return _isNearRightEdge; } set { _isNearRightEdge = value; } }
    public bool IsNearLeftEdge { get { return _isNearLeftEdge; } set { _isNearLeftEdge = value; } }
    public bool IsNearTopEdge { get { return _isNearTopEdge; } set { _isNearTopEdge = value; } }
    public bool IsNearBottomEdge { get { return _isNearBottomEdge; } set { _isNearBottomEdge = value; } }





    /*    BoxCollider[] triggerColliders;
        BoxCollider rightCollider;
        BoxCollider leftCollider;
        BoxCollider topCollider;
        BoxCollider bottomCollider;*/
    private void Start()
    {
        
/*        triggerColliders = GetComponentsInChildren<BoxCollider>();
        rightCollider = triggerColliders.ToList().Find(x => x.name == "Right");
        leftCollider = triggerColliders.ToList().Find(x => x.name == "Left");
        topCollider = triggerColliders.ToList().Find(x => x.name == "Top");
        bottomCollider = triggerColliders.ToList().Find(x => x.name == "Bottom");*/
        /*        mainCamera = GetComponent<Camera>();
                Collider playerCollider = player.GetComponent<Collider>();
                playerBounds = playerCollider.bounds
                transform.LookAt(player.transform.position);
                offset2 = transform.position - player.transform.position;
                distance = offset2.magnitude;
                playerPrevPos = player.transform.position;*/
    }
    private void LateUpdate()
    {
        //Debug.Log($"Right = {IsNearRightEdge}; Left = {IsNearLeftEdge}; Top = {IsNearTopEdge}; Bottom = {IsNearBottomEdge}.");
        if (IsNearRightEdge)
        {
            transform.position += Vector3.right * camSpeed *  Time.deltaTime;
        }
        if (IsNearLeftEdge)
        {
            transform.position += Vector3.left * camSpeed * Time.deltaTime;
        }
        if (IsNearTopEdge)
        {
            transform.position += Vector3.forward * camSpeed * Time.deltaTime;

        }
        if (IsNearBottomEdge)
        {
            transform.position += Vector3.back * camSpeed * Time.deltaTime;
        }
        /*        Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(mainCamera);
                if (GeometryUtility.TestPlanesAABB(frustum, playerBounds))
                {

                }*/
        /*        playerMoveDir = player.transform.position - playerPrevPos;
                playerMoveDir = playerMoveDir.normalized;
                if (playerMoveDir != Vector3.zero)
                {
                    //transform.position = Vector3.Slerp(transform.position, player.transform.position - (playerMoveDir * distance) + offset, 0.1f);
                    transform.LookAt(player.transform.position);
                    playerPrevPos = player.transform.position;
                }*/
    }
    public void MoveUp()
    {
    }
    public void MoveDown()
    {
    }
    public void MoveRight()
    {
    }
    public void MoveLeft()
    {
    }
}
