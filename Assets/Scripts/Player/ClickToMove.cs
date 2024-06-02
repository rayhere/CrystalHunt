using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class ClickToMove : MonoBehaviour
{
    private NavMeshAgent myNavMeshAgent;
    private Animator myAnim;
    private LineRenderer myLineRenderer;
    private Rigidbody rb;

    [SerializeField] private GameObject clickMarkerPrefab;
    [SerializeField] private Transform visualObjectsParent;

    private Vector3 destination;

    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>();
        myLineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();

        myLineRenderer.startWidth = 0.15f;
        myLineRenderer.endWidth = 0.15f;
        myLineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                destination = hit.point;
                SetDestination(destination);
            }
        }

        if (Vector3.Distance(destination, transform.position) <= myNavMeshAgent.stoppingDistance)
        {
            StopMovement();
        }
        else if (myNavMeshAgent.hasPath)
        {
            DrawPath();
        }
    }

    private void SetDestination(Vector3 target)
    {
        myAnim.SetBool("isRunning", true);
        myNavMeshAgent.SetDestination(target);

        clickMarkerPrefab.SetActive(true);
        clickMarkerPrefab.transform.position = target;
        clickMarkerPrefab.transform.SetParent(visualObjectsParent);

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    private void StopMovement()
    {
        myAnim.SetBool("isRunning", false);
        myNavMeshAgent.isStopped = true;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        clickMarkerPrefab.SetActive(false);
    }

    void DrawPath()
    {
        myLineRenderer.positionCount = myNavMeshAgent.path.corners.Length;
        myLineRenderer.SetPosition(0, transform.position);

        if (myNavMeshAgent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < myNavMeshAgent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(myNavMeshAgent.path.corners[i].x, myNavMeshAgent.path.corners[i].y, myNavMeshAgent.path.corners[i].z);
            myLineRenderer.SetPosition(i, pointPosition);
        }
    }
}
