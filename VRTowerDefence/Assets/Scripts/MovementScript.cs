using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{


    // General Variables \\
    public bool MovementControllsDisabled = false;

    // Controller Variables \\
    public static bool IsGrippingL = false;
    public static bool IsGrippingR = false;

    private Vector3 LastRHandPos;
    private Vector3 LastLHandPos;
    private Vector3 ScalingOffset = Vector3.zero;
    private Vector3 ControllerVelocityL = Vector3.zero;
    private Vector3 ControllerVelocityR = Vector3.zero;
    private Vector3 UpdatePosition = Vector3.zero;


    // Scaling the World \\
    private Vector3 Offset;
    private Vector3 TargetPos;
    private Vector3 Pivot;
    private Vector3 DistanceBetween;
    public static float LocalSF = 1;
    private float RS;

    [SerializeField]
    private float MaxHeight = 0;
    [SerializeField]
    private float MinHeight = 0;
    [SerializeField] private float WorldOffset = 0.5f;

    // References to GameObjects \\
    public GameObject GameWorld;

    [SerializeField] private GameObject LeftHandGO = null;
    [SerializeField] private GameObject RightHandGO = null;
    [SerializeField] private GameObject PlayerHead = null;


    // Adjustable Properties to be used in Editior. \\
    public AnimationCurve PullRatio;
    private float PullSpeed = 0;

    [SerializeField]
    private float MinVelocity = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        TargetPos = GameWorld.transform.position;
        Pivot = PlayerHead.transform.position;

        DistanceBetween = TargetPos - Pivot;
        RS = LocalSF / GameWorld.transform.localScale.x;
        Vector3 FP = new Vector3(Pivot.x + DistanceBetween.x, 0, Pivot.z + DistanceBetween.z) * RS;


        GameWorld.transform.localScale = new Vector3(LocalSF, LocalSF, LocalSF);
        GameWorld.transform.localPosition = FP + UpdatePosition + new Vector3(0, WorldOffset, 0);


        UpdatePosition = Vector3.zero;

        if (!MovementControllsDisabled)
        {
        
            if ((GameWorld.transform.position.y - PlayerHead.transform.position.y + Offset.y) >= MaxHeight)
            {
                Offset.y = (MaxHeight - (GameWorld.transform.position.y - PlayerHead.transform.position.y));
            }

            else if ((GameWorld.transform.position.y - PlayerHead.transform.position.y + Offset.y) <= MinHeight)
            {
                Offset.y = (MinHeight - (GameWorld.transform.position.y - PlayerHead.transform.position.y));
            }

            PullSpeed = this.PullRatio.Evaluate(LocalSF);
            LocalSF = GameWorld.transform.position.y - PlayerHead.transform.position.y + Offset.y;
            float TempPullSpeed = PullSpeed * LocalSF;

            ControllerVelocityR = (RightHandGO.transform.position - LastRHandPos) / Time.deltaTime;
            ControllerVelocityL = (LeftHandGO.transform.position - LastLHandPos) / Time.deltaTime;

            if (MovementScript.IsGrippingL && ControllerVelocityL.magnitude > MinVelocity)
            {
                if (new Vector3(0, ControllerVelocityL.y, 0).magnitude > MinVelocity / 2)
                {
                    ScalingOffset.y += ControllerVelocityL.y * Time.deltaTime * TempPullSpeed; // Possibly multiply by the SF ?
                }
                if (new Vector3(ControllerVelocityL.x, 0, ControllerVelocityL.z).magnitude > MinVelocity)
                {
                    UpdatePosition += new Vector3(ControllerVelocityL.x, 0, ControllerVelocityL.z) * TempPullSpeed * Time.deltaTime;
                }
            }

            if (MovementScript.IsGrippingR && ControllerVelocityR.magnitude > MinVelocity)
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

    }

    // Check if the controller is moving.
    //If the controller is moving, Work out the distance moved since clicked,
    // and then move world accordingly. ( transform.psoition ... * time.deltatime)

 }


