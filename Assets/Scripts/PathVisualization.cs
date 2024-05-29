using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathVisualization : MonoBehaviour
{
    //public Transform targetDestination;
    public int numOfPaths = 3;
    public LineRenderer[] lineRenderers;

    private NavMeshPath[] paths;
    private NavMeshAgent agent;

    public Vector3 targetDestination;
    public bool hasDestination = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        paths = new NavMeshPath[numOfPaths];
        //if (hasDestination)
        for (int i = 0; i < numOfPaths; i++)
        {
            paths[i] = new NavMeshPath();
        }
        //if (hasDestination)
        ComputePaths();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasDestination)
        for (int i = 0; i < numOfPaths; i++)
        {
            DrawPath(paths[i], lineRenderers[i]);
        }

        if (Input.GetMouseButtonDown(0))
        {
            hasDestination = true;
            ClickToMove();
        }
    }

    void ComputePaths()
    {
        NavMesh.CalculatePath(transform.position, targetDestination, NavMesh.AllAreas, paths[0]);
        for (int i = 1; i < numOfPaths; i++)
        {
            NavMesh.CalculatePath(transform.position + Random.insideUnitSphere * 2f, targetDestination + Random.insideUnitSphere * 2f, NavMesh.AllAreas, paths[i]);
        }
    }

    void DrawPath2(NavMeshPath path, LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = path.corners.Length;
        lineRenderer.SetPositions(path.corners);
    }

    // Draws the path the player will take to reach its destination
    void DrawPath(NavMeshPath path, LineRenderer myLineRenderer)
    {
        myLineRenderer.positionCount = path.corners.Length; 
        // Checks how many corners are in the path that the NavMesh is taking
        // so each time the NavMesh has to turn it's going to make a corner
        // and use those corners as points
        myLineRenderer.SetPosition(0, transform.position);

        if (path.corners.Length < 2)
        {
            return; // don't do anything for a straight line
        }

        // Record all vector for each corner
        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(path.corners[i].x, path.corners[i].y, path.corners[i].z);
            myLineRenderer.SetPosition(i, pointPosition);
        }
    }

    private void ClickToMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);
        if (hasHit)
        {
            //SetDestination(hit.point);
            targetDestination = hit.point;
        }
    }
}
