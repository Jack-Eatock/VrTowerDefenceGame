using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class VRPointerInputModule : BaseInputModule
{

    public Camera m_Camera;
    public SteamVR_Input_Sources m_TargetSource;
    public SteamVR_Action_Boolean m_ClickAction;

    private GameObject m_CurrentGameObject = null;
    private PointerEventData m_Data  = null ;


    protected override void Awake()
    {
        base.Awake();

        m_Data = new PointerEventData(eventSystem);
    }


    public override void Process()
    {
        // Reset Data, set camera
        m_Data.Reset();
        m_Data.position = new Vector2(m_Camera.pixelWidth / 2, m_Camera.pixelHeight / 2);


        // Raycast
        eventSystem.RaycastAll(m_Data, m_RaycastResultCache);
        m_Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        m_CurrentGameObject = m_Data.pointerCurrentRaycast.gameObject;

        // Clear Raycast
        m_RaycastResultCache.Clear();

        // Hover
        HandlePointerExitAndEnter(m_Data, m_CurrentGameObject);

        // Press
        if (m_ClickAction.GetStateDown(m_TargetSource))
        {
            ProcessPress(m_Data);
        }

        // Release
        if (m_ClickAction.GetStateUp(m_TargetSource))
        {
            ProcessRelease(m_Data);
        }

    }

    public PointerEventData Getdata()
    {
        return m_Data;
    }

    private void ProcessPress(PointerEventData data)
    {
        // Set Raycast
        data.pointerPressRaycast = data.pointerCurrentRaycast;


        // check for object hit, Get the Down Handler, call it.
        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(m_CurrentGameObject, data, ExecuteEvents.pointerDownHandler);

        // if no Down Handler, try and get click handler.
        if (newPointerPress == null)
        {
            newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentGameObject);
        }

        // Set Data.

        data.pressPosition = data.position;
        data.pointerPress = newPointerPress;
        data.rawPointerPress = m_CurrentGameObject;

    }

    private void ProcessRelease(PointerEventData data)
    {
        // Execute Pointer up
        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);

        // Check for click handler
        GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentGameObject);

        // Check if actual
        if ( data.pointerPress == pointerUpHandler)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
        }

        // Clear selected gameObjects.
        eventSystem.SetSelectedGameObject(null);

        // reset data.
        data.pressPosition = Vector2.zero;
        data.pointerPress = null;
        data.rawPointerPress = null;
    }
}

