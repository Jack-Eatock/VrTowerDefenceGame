using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionScript : MonoBehaviour
{
    public int CollisionType;
    public bool IsColliding = false;
    public List<GameObject> ObjectsWithinCollider = new List<GameObject>();
    int Counter = 0;

    public void OnTriggerEnter(Collider other)
    {
        if (CollisionType == 1)
        {
            if (other.gameObject.name == "RightHand") // hand Interacted with Tower menu.
            {
                IsColliding = true;
            }

        }

        else if (CollisionType == 2) // MiniTower Colliding with Ground.
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

        else if (CollisionType == 4) // General Button press.
        {
            if (other.gameObject.name == "RightHand" || other.gameObject.name == "LeftHand") // Colliding with a hand.
            {
                Counter++;

                if (Counter != 0)
                {
                    IsColliding = true;
                    gameObject.GetComponent<ButtonScript>().IsCollidingWithHands = true;
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (CollisionType == 1)
        {
            if (other.gameObject.name == "RightHand")
            {
                IsColliding = false;
            }
        }

        else if (CollisionType == 2) // MiniTower COlliding with Ground.
        {
            if (other.gameObject.name == "Ground") // hand Colliding with Tower menu.
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
                    Counter = 0;
                    foreach (GameObject Obj in ObjectsWithinCollider)
                    {
                        if (Obj == other.gameObject)
                        {
                            ObjectsWithinCollider.RemoveAt(Counter);
                            break;
                        }
                        Counter++;
                    }
                }
            }

              
        }

        else if (CollisionType == 4) // General Button press.
        {
            if (other.gameObject.name == "RightHand" || other.gameObject.name == "LeftHand") // Colliding with a hand.
            {
                Counter--;

                if (Counter == 0)
                {
                    IsColliding = false;
                    gameObject.GetComponent<ButtonScript>().IsCollidingWithHands = false;
                }
            }
        }
    }
}
