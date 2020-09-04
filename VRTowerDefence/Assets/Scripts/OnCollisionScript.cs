using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cleared \\

public class OnCollisionScript : MonoBehaviour
{
    public int  CollisionType;
    public bool IsColliding = false;
    public List<GameObject> ObjectsWithinCollider = new List<GameObject>();
    public GameObject _currentObj;

    public int BothLeftRightHandOnly = 1;

    private int _counter = 0;


    private void Update()
    {
        if (IsColliding && CollisionType == 5)
        {
            if (_currentObj != null)
            {
                if (!_currentObj.activeInHierarchy)
                {
                    _currentObj = null;
                    IsColliding = false;
                }
            }
           
        }
    }

    public void OnTriggerEnter(Collider other)
    {
    
        if (CollisionType == 2) // MiniTower COlliding with Ground.
        {
            if (other.gameObject.name == "Ground") 
            {
                IsColliding = true;
            }
        }

        else if (CollisionType == 3)
        {
            if (other.transform.parent)
            {
                if (other.transform.parent.transform.tag == "Enemy")
                {
                    ObjectsWithinCollider.Add(other.gameObject);
                }

            }
        }

        else if (CollisionType == 5)
        {
            if (other.gameObject.name != "RightHand" && other.gameObject.name != "LeftHand" )
            {
                if (other.gameObject.transform.tag == "Interactable")
                {
                    
                    IsColliding = true;
                    _currentObj = other.gameObject;
                   
                }
               
                   
            }

        }
    }

    public void OnTriggerExit(Collider other)
    {

        if (CollisionType == 2) // MiniTower COlliding with Ground.
        {
            if (other.gameObject.name == "Ground") 
            {
                IsColliding = false;
            }
        }

        else if (CollisionType == 3)
        {
            if (other.transform.parent)
            {

                if (other.transform.parent.transform.tag == "Enemy")
                {
                    _counter = 0;
                    foreach (GameObject Obj in ObjectsWithinCollider)
                    {
                        if (Obj == other.gameObject)
                        {
                            ObjectsWithinCollider.RemoveAt(_counter);
                            break;
                        }
                        _counter++;
                    }
                }
            }

              
        }

        else if (CollisionType == 5)
        {
            if (other.gameObject.name != "RightHand" && other.gameObject.name != "LeftHand") 
            {
                if (other.gameObject.transform.tag == "Interactable")
                {

                    if (_currentObj == other.gameObject)
                    {

                        IsColliding = false;
                        _currentObj = null;
                    }
       
                    
                }
              
                
            }

        }
    }
}
