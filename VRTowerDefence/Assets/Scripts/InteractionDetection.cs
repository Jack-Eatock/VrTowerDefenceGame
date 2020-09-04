using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDetection : MonoBehaviour
{
    [SerializeField] private Color ColorWhenInteractable;

    [SerializeField] private OnCollisionScript LeftHand;
    [SerializeField] private OnCollisionScript RightHand;

    [SerializeField] private GameObject _leftHandDetectionBall;
    [SerializeField] private GameObject _rightHandDetectionBall;

    private OnCollisionScript _tempColScript;
    private IInteractable _tempInteractable;

    private bool _leftGlowing = false;
    private bool _rightGlowing = false;

    private ChangeColour _rightColour;
    private ChangeColour _leftColour;

    private bool _leftHandDisabled;
    private bool _rightHandDisabled;

    // Start is called before the first frame update
    void Start()
    {
        InputScripto.OnRightTriggerClick += RightClick;
        InputScripto.OnLeftTriggerClick +=  LeftClick;

        _rightColour = new ChangeColour();
        _leftColour  = new ChangeColour();
    }

    private void Update()
    {

        // Left Side

        if (_leftHandDisabled)
        {
            if (_leftGlowing == true)
            {
                _leftGlowing = false;
                _leftColour.Finished = true;
            }
        }

        else if (LeftHand.IsColliding)
        {
            if (!_leftGlowing)
            {
                _leftGlowing = true;
                _leftColour.Finished = false;
                StartCoroutine(_leftColour.ObjectBlinkColour(_leftHandDetectionBall, ColorWhenInteractable));
            }
        }
        else if (_leftGlowing)
        {
            if (_leftGlowing == true)
            {
                _leftGlowing = false;
                _leftColour.Finished = true;
            }
        }


        // Right side 


        if (_rightHandDisabled)
        {
            if (_rightGlowing == true)
            {
                _rightGlowing = false;
                _rightColour.Finished = true;
            }
        }


        else if (RightHand.IsColliding)
        {
            if (!_rightGlowing)
            {
                _rightGlowing = true;
                _rightColour.Finished = false;
                StartCoroutine(_rightColour.ObjectBlinkColour(_rightHandDetectionBall, ColorWhenInteractable));
            }
        }
        else if (_rightGlowing)
        {
            if (_rightGlowing == true)
            {
                _rightGlowing = false;
                _rightColour.Finished = true;
            }
        }
    }

    void OnClick(bool isleft)
    {
        if (isleft)   {     _tempColScript = LeftHand;   }
        else          {     _tempColScript = RightHand;  }

        if (_tempColScript._currentObj == null)
        {
            // Close any menus if open. 
            if (InteractionMenuDisplayer.CurrentInteractionMenu != null && InteractionMenuDisplayer.CurrentInteractionMenu.activeSelf)
            {
                InteractionMenuDisplayer.SetUpInteractionMenu(Vector3.zero, isleft, true);
            }

            return;
        }

        _tempInteractable = _tempColScript._currentObj.GetComponent<IInteractable>();

        // Check if it contains the function. 
        if (_tempInteractable != null)
        {
            _tempInteractable.Interact(isleft);
        }

        // If not check parents.
        else 
        {
            _tempInteractable = _tempColScript._currentObj.GetComponentInParent(typeof(IInteractable)) as IInteractable;

            if (_tempInteractable != null)
            {
                _tempInteractable.Interact(isleft);

            }
            else
            {
                Debug.Log("No interact found");

            }
        }

    }

    void LeftClick()
    {
        Debug.Log("LeftClick");

        if (!_leftHandDisabled)
        {
            OnClick(true);
        } 
    }

    void RightClick()
    {
        Debug.Log("RightClick");

        if (!_rightHandDisabled)
        {
            OnClick(false);
        }
    }


    public void DisableInteraction(bool isLeft, bool deactivate)
    {
        if (isLeft)
        {
            if (deactivate)
            {
                _leftHandDisabled = true;
            }
            else
            {
                _leftHandDisabled = false;
            }
        }

        else
        {
            if (deactivate)
            {
                _rightHandDisabled = true;
            }
            else
            {
                _rightHandDisabled = false;
            }
        }
    }

}

class ChangeColour
{
    public bool Finished = false;
    public  List<GameObject> ObjectsAffected = new List<GameObject>();

    public IEnumerator ObjectBlinkColour(GameObject theObject, Color theColour)
    {
        //Debug.Log(ObjectsAffected);

        bool flag = false;
        foreach (GameObject Obj in ObjectsAffected)
        {
            //Debug.Log(Obj + " : " + Object.name);

            if (theObject == Obj)
            {
                //Debug.Log("Same");
                flag = true;
            }
        }

        if (!theObject)
        {
            flag = true;
        }

        if (!flag)
        {
            // Debug.Log("Did it");
            ObjectsAffected.Add(theObject);

            List<Color> colourList = new List<Color>();
            Renderer[] objRenderers = theObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer Rend in objRenderers)
            {
                if (Rend.material.HasProperty("_Color")) // Checks the renderer actually has a colour property... Particle renderer does not ;)
                {
                    colourList.Add(Rend.material.color); // Saves the original Colours of the object.
                    Rend.material.color = theColour;           // Sets the object Color to the Color specificed (mostly Red)         
                }
                else
                {
                    colourList.Add(theColour); // If the renderer does not have a colour variable. Set that element in the list to the desired colour, so taht it is ignored.
                }

            }


            yield return new WaitUntil(() => Finished == true); // Waits the desired time. and then we set the colours back to normal.




            int counter = 0;
            foreach (Renderer rend in objRenderers)
            {
                if (colourList[counter] != theColour) // If the original colour is the same as the new colour then ignore.
                {
                    rend.material.color = colourList[counter];          // Sets the colours back to normal.
                    counter++;
                }

            }

            if (theObject)
            {
                foreach (GameObject currentObject in ObjectsAffected.ToArray()) // Creates a new list of the ObjectAffected List before looping through it. Other wise it would change the values "under the hood" Causing the Coroutine to possibly not be executable.
                {
                    if (currentObject == theObject)
                    {
                        ObjectsAffected.RemoveAt(ObjectsAffected.IndexOf(theObject)); // Removes the Current object from the currently being used objects array, as it is no longer needed.
                    }
                }

            }
        }

        yield return null;
    }

}
