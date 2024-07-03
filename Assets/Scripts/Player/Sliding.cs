using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    [SerializeField, Tooltip("Player")]
    public Transform playerObj;
    private Rigidbody rb;
    private WASDController pm;
    private StaminaManager sm;
    public DarknessStatsSO playerStats;

    [Header("Sliding")]
    public float maxSlideTime =2f;
    public float slideForce = 200f;
    private float slideTimer;

    public float slideYScale = .5f;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    public bool isActive = true; // Flag to control whether script is active


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<WASDController>();
        sm = GetComponent<StaminaManager>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey))
        {
            if ((playerStats.currentSP >= playerStats.slideSPCost) && (horizontalInput != 0 || verticalInput != 0) && !pm.jumping && !pm.falling && !pm.aboutlanding && !pm.landedonground)
                StartSlide();
        }
        

        if (Input.GetKeyUp(slideKey) && pm.sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        //if (!(playerStats.currentSP >= playerStats.slideSPCost)) return;
        pm.sliding = true;
        if (sm != null) sm.isSliding = true;
        Debug.Log("Sliding playerStats.currentSP >= playerStats.slideSPCost " + playerStats.currentSP + " >= " +playerStats.slideSPCost + " TIme.time " + Time.time);
        //playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // sliding normal
        if(!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        // sliding down a slope
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        pm.sliding = false;
        if (sm != null) sm.isSliding = false;
        //playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
