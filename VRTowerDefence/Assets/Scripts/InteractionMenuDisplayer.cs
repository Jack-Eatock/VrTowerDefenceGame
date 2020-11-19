using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMenuDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject _interactionMenu;
    [SerializeField] private GameObject _playerHead;
    [SerializeField] private Transform _leftHandMenuPos;
    [SerializeField] private Transform _rightHandMenuPos;

    public static Transform LeftHandMenuPos;
    public static Transform RightHandMenuPos;

    public static GameObject InteractionMenu;
    public static GameObject CurrentInteractionMenu = null;

    

    // Start is called before the first frame update
    void Start()
    {
        LeftHandMenuPos  = _leftHandMenuPos;
        RightHandMenuPos = _rightHandMenuPos;

        InteractionMenu = _interactionMenu;
        GameScript.CleanSlate.AddListener(CleanSlateLocal);


    }




    // Update is called once per frame
    void Update()
    {
        if (CurrentInteractionMenu != null && CurrentInteractionMenu.activeSelf)
        {
            //CurrentInteractionMenu.transform.LookAt(_playerHead.transform.position);
        }
    }

    public void CleanSlateLocal()
    {
        if (CurrentInteractionMenu != null)
        {
            Destroy(CurrentInteractionMenu);
        }
    }

    public static void OnGrip()
    {
        if (CurrentInteractionMenu != null)
        {
            //SetUpInteractionMenu(Vector3.zero, true, true, 0);
        }
    }

    public static void SetUpInteractionMenu(Vector3 Pos , bool isLeftHand, bool Close = false, float heightOffset = 0)
    {
         
        float Sf = MovementScript.ScaleFactor;

        //heightOffset = heightOffset * Sf;

        if (Close)
        {
            CurrentInteractionMenu.SetActive(false);
            return;
        }

        else
        {
            if (CurrentInteractionMenu == null) // Create a new menu!
            {
                CurrentInteractionMenu = GameObject.Instantiate(InteractionMenu);
               // UtilitiesScript.AttachObjectToWorld(CurrentInteractionMenu, Vector3.zero, true);
            }

            else if (!CurrentInteractionMenu.activeSelf)
            {
                CurrentInteractionMenu.SetActive(true);
            }

        }

        //CurrentInteractionMenu.transform.position = Pos + new Vector3 (0,heightOffset,0);
        //CurrentInteractionMenu.transform.localScale = new Vector3(Sf, Sf, Sf);

        if (isLeftHand)
        {


            //CurrentInteractionMenu.transform.eulerAngles = new Vector3(0, RightHandMenuPos.eulerAngles.y, 0);

            CurrentInteractionMenu.transform.rotation = RightHandMenuPos.transform.rotation;
            CurrentInteractionMenu.transform.position = LeftHandMenuPos.transform.position; //+ new Vector3(0, heightOffset, 0);
            CurrentInteractionMenu.transform.SetParent(LeftHandMenuPos.transform);
           

        }

        else
        {
           // CurrentInteractionMenu.transform.eulerAngles = new Vector3(0, RightHandMenuPos.eulerAngles.y, 0);

            CurrentInteractionMenu.transform.rotation = RightHandMenuPos.transform.rotation;
            CurrentInteractionMenu.transform.position = RightHandMenuPos.transform.position; // + new Vector3(0, heightOffset, 0);
            CurrentInteractionMenu.transform.SetParent(RightHandMenuPos.transform);

        }

        



    }
}
