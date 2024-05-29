using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathVisualization2 : MonoBehaviour
{
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
        for (int i = 0; i < numOfPaths; i++)
        {
            paths[i] = new NavMeshPath();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasDestination)
        {
            for (int i = 0; i < numOfPaths; i++)
            {
                ComputePath(paths[i], i);
                DrawPath(paths[i], lineRenderers[i]);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            hasDestination = true;
            ClickToMove();
        }
    }

    void ComputePath(NavMeshPath path, int index)
    {
        if(index == 0)
        {
            NavMesh.CalculatePath(transform.position, targetDestination, NavMesh.AllAreas, path);
        }
        else
        {
            NavMesh.CalculatePath(transform.position + Random.insideUnitSphere * 2f, targetDestination + Random.insideUnitSphere * 2f, NavMesh.AllAreas, path);
        }
    }

    void DrawPath(NavMeshPath path, LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = path.corners.Length;
        lineRenderer.SetPositions(path.corners);
    }

    private void ClickToMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);
        if (hasHit)
        {
            targetDestination = hit.point;
        }
    }
}
