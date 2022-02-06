using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraFollow : MonoBehaviour
{
    public float camSpeed;
    public GameObject player;
    public Vector3 offset;
    
    private bool _isNearRightEdge = false;
    private bool _isNearLeftEdge = false;
    private bool _isNearTopEdge = false;
    private bool _isNearBottomEdge = false;

    public bool IsNearRightEdge { get { return _isNearRightEdge; } set { _isNearRightEdge = value; } }
    public bool IsNearLeftEdge { get { return _isNearLeftEdge; } set { _isNearLeftEdge = value; } }
    public bool IsNearTopEdge { get { return _isNearTopEdge; } set { _isNearTopEdge = value; } }
    public bool IsNearBottomEdge { get { return _isNearBottomEdge; } set { _isNearBottomEdge = value; } }

    private void LateUpdate()
    {
        if (IsNearRightEdge)
        {
            transform.position += camSpeed * Time.deltaTime * Vector3.right;
        }
        if (IsNearLeftEdge)
        {
            transform.position += camSpeed * Time.deltaTime * Vector3.left;
        }
        if (IsNearTopEdge)
        {
            transform.position += camSpeed * Time.deltaTime * Vector3.forward;

        }
        if (IsNearBottomEdge)
        {
            transform.position += camSpeed * Time.deltaTime * Vector3.back;
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
}
