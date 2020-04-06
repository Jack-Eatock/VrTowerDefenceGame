using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Cleaned \\


public class MovementScript : MonoBehaviour
{


    // General Variables \\
    public static bool MovementControllsDisabled = false;

    // Controller Variables \\
    public static bool IsGrippingL = false;
    public static bool IsGrippingR = false;

    private Vector3 _lastRHandPos;
    private Vector3 _lastLHandPos;
    private Vector3 _updatePosition = Vector3.zero;
    private float   _lastDistBetweenHands;


    // Scaling the World \\
    public static float ScaleFactor = 1;

    [SerializeField] private float _scaleSpeed = 1;
    [SerializeField] private float _pullSpeed = 3;
    //[SerializeField] private float _maxScale = 2.2f; // 10
    //[SerializeField] private float _minScale = 0.2f;  // 0.3
    //[SerializeField] private float _playerHeight = 0.55f;

    // References to GameObjects \\
    public GameObject GameWorld;

    [SerializeField] private GameObject     _leftHandGO = null;
    [SerializeField] private GameObject     _rightHandGO = null;
    [SerializeField] private float          _minVelocity = 0.5f;

    private void Start()
    {
         //GameWorld.transform.localPosition = new Vector3(0,PlayerHeight,0);
    }

    // Update is called once per frame
    void Update()
    {
       
        if (!MovementScript.MovementControllsDisabled)
        {
            UpdateSF();
            GameWorld.transform.localPosition += _updatePosition;  
            _updatePosition = Vector3.zero;

            if (MovementScript.IsGrippingL && MovementScript.IsGrippingR) // Both Hands are Griping , Start Scalling the world if they pull apart.                [SCALE THE WORLD]
            {
                //ScaleWorldBasedOnHand(); /// ADD THIS AS AN OPTION IN CONTROLS.
            }

            else if (MovementScript.IsGrippingL || MovementScript.IsGrippingR)// If only one hand is gripping. Then move the world based on the Hand gripping.    [MOVE THE WORLD]
            {
                MoveWorldBasedOnHandVelocity();
            }

            _lastDistBetweenHands = (_leftHandGO.transform.position - _rightHandGO.transform.position).magnitude * 2;
            _lastRHandPos = _rightHandGO.transform.position; // Stores the last position of the controller, used to calculate the velocity.
            _lastLHandPos = _leftHandGO.transform.position;
        }
    }

    public void UpdateSF()
    {
        ScaleFactor =  GameWorld.transform.localScale.z;
    }

    public void MoveWorldBasedOnHandVelocity()
    {
        GameObject handGo;
        Vector3 lastHandPos;

        if (MovementScript.IsGrippingL) // Left hand gripping, So move based on the left hand velocity.
        {
            handGo = _leftHandGO;
            lastHandPos = _lastLHandPos;
        }

        else // Right hand Gripping, so mvoe based on the right hand velocity.
        {
            handGo = _rightHandGO;
            lastHandPos = _lastRHandPos;
        }

        Vector3 controllerVelocity = (handGo.transform.position - lastHandPos) / Time.deltaTime; // Keeps track of the velocity of the moving hand, calculated by the change in position over the time. [S = D /T]
        Vector2 horizontalVelocity = new Vector2(controllerVelocity.x, controllerVelocity.z); // Calculates only the Horizontal velocity of which ever hand is being moved.

        if (horizontalVelocity.magnitude > _minVelocity) // The Horizontal velocity of the controller is fast enough. Move the world.
        {
            Vector3 moveBY = controllerVelocity * _pullSpeed; //* PullSpeed * SF;
            _updatePosition += new Vector3(moveBY.x, 0, moveBY.z) * Time.deltaTime;
        }

        float verticalVelocity = controllerVelocity.y;

        if (Mathf.Abs(verticalVelocity) > _minVelocity)
        {
            float increaseScale = verticalVelocity * _scaleSpeed * Time.deltaTime * ScaleFactor;
            float finalScale = increaseScale + ScaleFactor;
            ScaleAround(GameWorld, gameObject.transform.position, new Vector3(finalScale, finalScale, finalScale));
        }

    }

    public void ScaleWorldBasedOnHand()
    {
        float distanceBetweenHands = (_leftHandGO.transform.position - _rightHandGO.transform.position).magnitude * 2;
        float changeInDist = distanceBetweenHands - _lastDistBetweenHands;
        Debug.Log(changeInDist);

        float increaseScale = changeInDist * _scaleSpeed * Time.deltaTime;
        float finalScale = increaseScale + ScaleFactor;

        ScaleAround(GameWorld, gameObject.transform.position, new Vector3(finalScale, finalScale, finalScale));
    }


    public void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 targetPosition = target.transform.localPosition;
        Vector3 pivotAroundPoint = pivot;

        Vector3 diffFromTargetToPivot = targetPosition - pivotAroundPoint; // diff from object pivot to desired pivot/origin

        float relativeScale = newScale.x / target.transform.localScale.x; // relative scale factor

        // calc final position post-scale
        Vector3 finalPosition = pivotAroundPoint + diffFromTargetToPivot * relativeScale;

        target.transform.localScale = newScale;
        target.transform.localPosition = finalPosition;

        /*
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
        */
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


