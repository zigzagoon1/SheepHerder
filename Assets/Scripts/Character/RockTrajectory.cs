using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RockTrajectory : MonoBehaviour
{
    LineRenderer line;
    public Vector3 startPosition;
    public Vector3 startVelocity;
    public Vector2 input;

    [SerializeField] float trajectoryVertDist = 0.25f;
    [SerializeField] float maxCurveLength = 5f;
    [Header("Debug")]
    [SerializeField] private bool debugAlwaysDrawTrajectory = false;


    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        startPosition = transform.position;
        startVelocity = transform.forward * GetComponent<LaunchRock>().launchVelocity;

    }

    private void Update()
    {
        if (debugAlwaysDrawTrajectory)
        {
            DrawTrajectory();
        }
    }

    public void DrawTrajectory()
    {
        var curvePoints = new List<Vector3>();
        startPosition = transform.position;
        startVelocity = transform.forward * GetComponent<LaunchRock>().launchVelocity;
        curvePoints.Add(startPosition);
        var currentPosition = startPosition;
        var currentVelocity = startVelocity;

        Ray ray = new Ray(currentPosition, currentVelocity.normalized);
        int i = 0;
        while (!Physics.Raycast(ray, out RaycastHit hit, trajectoryVertDist) && Vector3.Distance(startPosition, currentPosition) < maxCurveLength)
        {
            i++;
            var t = trajectoryVertDist / currentVelocity.magnitude;

            if (i > 0 && i < 10)
            {
                currentVelocity.x += input.x;
                currentVelocity.y += input.y;
            }

            currentVelocity += t * Physics.gravity;


            currentPosition += t * currentVelocity;

            curvePoints.Add(currentPosition);

            ray = new Ray(currentPosition, currentVelocity.normalized);

            if (hit.transform)
            {
                curvePoints.Add(hit.point);
            }


        }
        line.positionCount = curvePoints.Count;
        line.SetPositions(curvePoints.ToArray());
    }

    public void ClearTrajectory()
    {
        line.positionCount = 0;
    }
}
