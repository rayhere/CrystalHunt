using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //NavMesh

[RequireComponent(typeof(NavMeshAgent))] //auto add NavMeshAgent component to the object with this script
[RequireComponent(typeof(LineRenderer))] //auto add LineRenderer component to the object with this script
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent myNavMeshAgent;
    private Animator myAnim;
    private LineRenderer myLineRenderer; 

    [SerializeField] private GameObject clickMarkerPrefab;
    
    // Created empty object call VisualObjects, set a transform equals to VisualObjects
    [SerializeField] private Transform visualObjectsParent;

    // Start is called before the first frame update
    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>(); // allow to control animations of the GameObject
        myLineRenderer = GetComponent<LineRenderer>();

        // Set Default value of LineRenderer
        myLineRenderer.startWidth = 0.15f;
        myLineRenderer.endWidth = 0.15f;
        myLineRenderer.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickToMove();
        }

        // Do if Object reached destination
        if (
            Vector3.Distance(myNavMeshAgent.destination, transform.position)
            <= myNavMeshAgent.stoppingDistance
        ) // calculate the distance, if reached destination, return true
        {
            // Created empty object call VisualObjects, set a transform equals to VisualObjects
            clickMarkerPrefab.transform.SetParent(transform);

            clickMarkerPrefab.SetActive(false);

            myAnim.SetBool("isRunning", false); //change condition for Animator
        }
        // Do if Object doesn't reached destination, and still moving
        else if (myNavMeshAgent.hasPath)
        {
            DrawPath();
        }
    }

    private void ClickToMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);
        if (hasHit)
        {
            SetDestination(hit.point);
        }
    }

    private void SetDestination(Vector3 target)
    {
        myAnim.SetBool("isRunning", true); // Change condition for Animator
        myNavMeshAgent.SetDestination(target);

        // Place clickMarker in position,  
        clickMarkerPrefab.SetActive(true);
        clickMarkerPrefab.transform.position = target;

        // Created empty object call VisualObjects, set a transform equals to VisualObjects
        clickMarkerPrefab.transform.SetParent(visualObjectsParent);
    }

    // Draws the path the player will take to reach its destination
    void DrawPath()
    {
        myLineRenderer.positionCount = myNavMeshAgent.path.corners.Length; 
        // Checks how many corners are in the path that the NavMesh is taking
        // so each time the NavMesh has to turn it's going to make a corner
        // and use those corners as points
        myLineRenderer.SetPosition(0, transform.position);

        if (myNavMeshAgent.path.corners.Length < 2)
        {
            return; // don't do anything for a straight line
        }

        // Record all vector for each corner
        for (int i = 1; i < myNavMeshAgent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(myNavMeshAgent.path.corners[i].x, myNavMeshAgent.path.corners[i].y, myNavMeshAgent.path.corners[i].z);
            myLineRenderer.SetPosition(i, pointPosition);
        }
    }
}