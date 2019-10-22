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
    private bool IsGrippingL = false;
    private bool IsGrippingR = false;

    private Vector3 LastRHandPos;
    private Vector3 LastLHandPos;
    private Vector3 ScalingOffset = Vector3.zero;
    private Vector3 ControllerVelocityL = Vector3.zero;
    private Vector3 ControllerVelocityR = Vector3.zero;
    private Vector3 UpdatePosition = Vector3.zero;

    // Moving the World \\


    // Scaling the World \\
    private Vector3 Offset;
    private Vector3 TargetPos;
    private Vector3 Pivot;
    private Vector3 DistanceBetween;
    public  float LocalSF = 1;
    private float RS;

    [SerializeField]
    private float MaxHeight;
    [SerializeField]
    private float MinHeight;

    // References to GameObjects \\
    public GameObject GameWorld;

    [SerializeField]
    private GameObject LeftHandGO;
    [SerializeField]
    private GameObject RightHandGO;
    [SerializeField]
    private GameObject PlayerHead;


    // Adjustable Properties to be used in Editior. \\
    public AnimationCurve PullRatio;
    private float PullSpeed;

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
        RS = LocalSF / GameWorld.transform.localScale.x;
        Vector3 FP = new Vector3 (Pivot.x + DistanceBetween.x,0,Pivot.z + DistanceBetween.z) * RS;


        GameWorld.transform.localScale = new Vector3(LocalSF, LocalSF, LocalSF);
        GameWorld.transform.localPosition = FP + UpdatePosition;


        UpdatePosition = Vector3.zero;
        if ((2 + GameWorld.transform.position.y - PlayerHead.transform.position.y + Offset.y) >= MaxHeight)
        {
            Offset.y = (MaxHeight - (2 + GameWorld.transform.position.y - PlayerHead.transform.position.y));
        }

        else if ((2 + GameWorld.transform.position.y - PlayerHead.transform.position.y + Offset.y) <= MinHeight)
        {
            Offset.y = (MinHeight - (2 + GameWorld.transform.position.y - PlayerHead.transform.position.y));
        }

        PullSpeed = this.PullRatio.Evaluate(LocalSF);
        LocalSF = 2 + GameWorld.transform.position.y - PlayerHead.transform.position.y  + Offset.y;
        float TempPullSpeed = PullSpeed * LocalSF;

        ControllerVelocityR = (RightHandGO.transform.position - LastRHandPos) / Time.deltaTime;
        ControllerVelocityL = (LeftHandGO.transform.position - LastLHandPos) / Time.deltaTime;

        if (IsGrippingL && ControllerVelocityL.magnitude > MinVelocity)
        {
            if (new Vector3(0, ControllerVelocityL.y,0).magnitude > MinVelocity/2)
            {
                ScalingOffset.y += ControllerVelocityL.y * Time.deltaTime * TempPullSpeed; // Possibly multiply by the SF ?
            }
            if (new Vector3 (ControllerVelocityL.x,0,ControllerVelocityL.z).magnitude > MinVelocity)
            {
                UpdatePosition += new Vector3(ControllerVelocityL.x, 0, ControllerVelocityL.z) * TempPullSpeed * Time.deltaTime;
            }
        }

        if (IsGrippingR && ControllerVelocityR.magnitude > MinVelocity)
        {
            if (new Vector3(0, ControllerVelocityR.y, 0).magnitude > MinVelocity / 2)
            {
                ScalingOffset.y += ControllerVelocityR.y * Time.deltaTime * TempPullSpeed;
            }
            if (new Vector3(ControllerVelocityR.x, 0, ControllerVelocityR.z).magnitude > MinVelocity)
            {
                UpdatePosition += new Vector3(ControllerVelocityR.x, 0, ControllerVelocityR.z) * TempPullSpeed * Time.deltaTime;
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