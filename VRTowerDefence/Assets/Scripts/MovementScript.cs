using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{


    // General Variables \\
    public static bool MovementControllsDisabled = true;

    // Controller Variables \\
    public static bool IsGrippingL = false;
    public static bool IsGrippingR = false;

    private Vector3 LastRHandPos;
    private Vector3 LastLHandPos;
    private Vector3 UpdatePosition = Vector3.zero;

    private float LastDistBetweenHands;


    // Scaling the World \\
    public static float SF = 1;

    [SerializeField] private float ScaleSpeed = 40;
    [SerializeField] private float PullSpeed = 1;
    [SerializeField] private float MaxScale = 10;
    [SerializeField] private float MinScale = 0.3f;
    [SerializeField] private float PlayerHeight = 0.55f;

    // References to GameObjects \\
    public GameObject GameWorld;

    [SerializeField] private GameObject LeftHandGO = null;
    [SerializeField] private GameObject RightHandGO = null;
    [SerializeField] private float MinVelocity = 0.5f;

    private void Start()
    {
         //GameWorld.transform.localPosition = new Vector3(0,PlayerHeight,0);
    }

    // Update is called once per frame
    void Update()
    {
        SF = GameWorld.transform.localScale.z;
        if (!MovementScript.MovementControllsDisabled)
        {
            GameWorld.transform.localPosition += UpdatePosition;  
            UpdatePosition = Vector3.zero;

            if (MovementScript.IsGrippingL && MovementScript.IsGrippingR) // Both Hands are Griping , Start Scalling the world if they pull apart.                [SCALE THE WORLD]
            {
                //ScaleWorldBasedOnHand(); /// ADD THIS AS AN OPTION IN CONTROLS.
            }

            else if (MovementScript.IsGrippingL || MovementScript.IsGrippingR)// If only one hand is gripping. Then move the world based on the Hand gripping.    [MOVE THE WORLD]
            {
                MoveWorldBasedOnHandVelocity();
            }

            LastDistBetweenHands = (LeftHandGO.transform.position - RightHandGO.transform.position).magnitude * 2;
            LastRHandPos = RightHandGO.transform.position; // Stores the last position of the controller, used to calculate the velocity.
            LastLHandPos = LeftHandGO.transform.position;
        }
    }

    public void MoveWorldBasedOnHandVelocity()
    {
        GameObject HandGo;
        Vector3 LastHandPos;

        if (MovementScript.IsGrippingL) // Left hand gripping, So move based on the left hand velocity.
        {
            HandGo = LeftHandGO;
            LastHandPos = LastLHandPos;
        }

        else // Right hand Gripping, so mvoe based on the right hand velocity.
        {
            HandGo = RightHandGO;
            LastHandPos = LastRHandPos;
        }

        Vector3 ControllerVelocity = (HandGo.transform.position - LastHandPos) / Time.deltaTime; // Keeps track of the velocity of the moving hand, calculated by the change in position over the time. [S = D /T]
        Vector2 HorizontalVelocity = new Vector2(ControllerVelocity.x, ControllerVelocity.z); // Calculates only the Horizontal velocity of which ever hand is being moved.

        if (HorizontalVelocity.magnitude > MinVelocity) // The Horizontal velocity of the controller is fast enough. Move the world.
        {
            Vector3 MoveBY = ControllerVelocity * PullSpeed; //* PullSpeed * SF;
            UpdatePosition += new Vector3(MoveBY.x, 0, MoveBY.z) * Time.deltaTime;
        }

        float VerticalVelocity = ControllerVelocity.y;

        if (Mathf.Abs(VerticalVelocity) > MinVelocity)
        {
            float IncreaseScale = VerticalVelocity * ScaleSpeed * Time.deltaTime * SF;
            float FinalScale = IncreaseScale + SF;
            ScaleAround(GameWorld, gameObject.transform.position, new Vector3(FinalScale, FinalScale, FinalScale));
        }

    }

    public void ScaleWorldBasedOnHand()
    {
        float DistanceBetweenHands = (LeftHandGO.transform.position - RightHandGO.transform.position).magnitude * 2;
        float ChangeInDist = DistanceBetweenHands - LastDistBetweenHands;
        Debug.Log(ChangeInDist);

        float IncreaseScale = ChangeInDist * ScaleSpeed * Time.deltaTime;
        float FinalScale = IncreaseScale + SF;

        ScaleAround(GameWorld, gameObject.transform.position, new Vector3(FinalScale, FinalScale, FinalScale));
    }


    public void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = target.transform.localPosition;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / target.transform.localScale.x; // relative scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        if (newScale.x < MaxScale && newScale.x > MinScale)
        {
            // finally, actually perform the scale/translation
            target.transform.localScale = newScale;
            target.transform.localPosition = FP;
        }
        else
        {
            Debug.Log("Scaling out of bounds.");
        }

    }
}




/*

 *      [OLD MOVEMENT SYSTEM]
 *  - Scales world with head movement.
 *  - Movement and scaling can be done with one hand instead of one for movement and two for scale.
 * 
 * 
 * 
 *     // Check if the controller is moving.
    //If the controller is moving, Work out the distance moved since clicked,
    // and then move world accordingly. ( transform.psoition ... * time.deltatime)


TargetPos = GameWorld.transform.position;
Pivot = PlayerHead.transform.position;

DistanceBetween = TargetPos - Pivot;
RS = LocalSF / GameWorld.transform.localScale.x;
Vector3 FP = new Vector3(Pivot.x + DistanceBetween.x, 0, Pivot.z + DistanceBetween.z) * RS;


GameWorld.transform.localScale = new Vector3(LocalSF, LocalSF, LocalSF);
GameWorld.transform.localPosition = FP + UpdatePosition + new Vector3(0, WorldOffset, 0); 



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


*/


