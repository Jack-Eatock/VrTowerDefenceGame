using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MovementScript : MonoBehaviour
{
    // Reference to Steam Action's \\
    public SteamVR_Action_Boolean GripL;
    public SteamVR_Action_Boolean GripR;

  


    // References to Input devices \\
    public SteamVR_Input_Sources LeftHand; // Left Controller - Set in Engine.
    public SteamVR_Input_Sources RightHand; // Right Controller - Set in Engine.

    // General Variables \\


    // Controller Variables \\
    private Vector3 LastRHandPos;
    private Vector3 LastLHandPos;
    private bool IsGrabbingL = false;
    private bool IsGrabbingR = false;
    private bool IsGrippingL = false;
    private bool IsGrippingR = false;
    private Vector3 ScalingOffset = Vector3.zero;
    private Vector3 ControllerVelocityL = Vector3.zero;
    private Vector3 ControllerVelocityR = Vector3.zero;
    private Vector3 UpdatePosition = Vector3.zero;

    // Moving the World \\


    // Scaling the World \\
    public float SF = 1;
    private Vector3 Offset;
    private Vector3 TargetPos;
    private Vector3 Pivot;
    private Vector3 DistanceBetween;
    private float RS;

    // References to GameObjects \\
    public GameObject GameWorld;


    public GameObject LeftHandGO;
    public GameObject RightHandGO;
    public GameObject PlayerHead;


    // Adjustable Properties to be used in Editior. \\
    [SerializeField]
    private float PullSpeed = 26f;
    [SerializeField]
    private float MinVelocity = 0.5f;


    // Start is called before the first frame update
    void Start()
    {


        GripL.AddOnStateDownListener(GripDownL, LeftHand);
        GripL.AddOnStateUpListener(GripUpL, LeftHand);

        GripR.AddOnStateDownListener(GripDownR, RightHand);
        GripR.AddOnStateUpListener(GripUpR, RightHand);

      
    }

    // Update is called once per frame
    void Update()
    {
        TargetPos = GameWorld.transform.position;
        Pivot = PlayerHead.transform.position;

        DistanceBetween = TargetPos - Pivot;
        RS = SF / GameWorld.transform.localScale.x;
        Vector3 FP = new Vector3 (Pivot.x + DistanceBetween.x,0,Pivot.z + DistanceBetween.z) * RS;


        GameWorld.transform.localScale = new Vector3(SF, SF, SF);
        GameWorld.transform.localPosition = FP + UpdatePosition;


        UpdatePosition = Vector3.zero;
        SF = Mathf.Clamp(2 + GameWorld.transform.position.y - PlayerHead.transform.position.y  + Offset.y, 0.2f, 12f);
        

        ControllerVelocityR = (RightHandGO.transform.position - LastRHandPos) / Time.deltaTime;
        ControllerVelocityL = (LeftHandGO.transform.position - LastLHandPos) / Time.deltaTime;

        if (IsGrippingL && ControllerVelocityL.magnitude > MinVelocity)
        {
            if (new Vector3(0, ControllerVelocityL.y,0).magnitude > MinVelocity/2)
            {
                ScalingOffset.y += ControllerVelocityL.y * Time.deltaTime * PullSpeed;
            }
            if (new Vector3 (ControllerVelocityL.x,0,ControllerVelocityL.z).magnitude > MinVelocity)
            {
                UpdatePosition += new Vector3(ControllerVelocityL.x, 0, ControllerVelocityL.z) * PullSpeed * Time.deltaTime;
            }
        }

        if (IsGrippingR && ControllerVelocityR.magnitude > MinVelocity)
        {
            if (new Vector3(0, ControllerVelocityR.y, 0).magnitude > MinVelocity / 2)
            {
                ScalingOffset.y += ControllerVelocityR.y * Time.deltaTime * PullSpeed;
            }
            if (new Vector3(ControllerVelocityR.x, 0, ControllerVelocityR.z).magnitude > MinVelocity)
            {
                UpdatePosition += new Vector3(ControllerVelocityR.x, 0, ControllerVelocityR.z) * PullSpeed * Time.deltaTime;
            }
        }

        Offset += new Vector3(0, ScalingOffset.y / 4f, 0);
        ScalingOffset = Vector3.zero;

        LastRHandPos = RightHandGO.transform.position;
        LastLHandPos = LeftHandGO.transform.position;

    }





    // Check if the controller is moving.
    //If the controller is moving, Work out the distance moved since clicked,
    // and then move world accordingly. ( transform.psoition ... * time.deltatime)








    public void GripUpL(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        Debug.Log("Left Grip released");
        IsGrippingL = false;
    }
    public void GripDownL(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        Debug.Log("Left Grip Pressed");
        IsGrippingL = true;
    }
    public void GripUpR(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        Debug.Log("Right Grip released");
        IsGrippingR = false;
    }
    public void GripDownR(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        Debug.Log("Right Grip Pressed");
        IsGrippingR = true;
    }

}