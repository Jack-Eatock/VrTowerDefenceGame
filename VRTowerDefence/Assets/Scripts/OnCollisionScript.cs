using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionScript : MonoBehaviour
{
    public int CollisionType;
    public bool IsColliding;
    public List<GameObject> ObjectsWithinCollider = new List<GameObject>(); 

    public void OnTriggerEnter(Collider other)
    {
        if (CollisionType == 1)
        {
            if (other.gameObject.name == "RightHand") // hand Interacted with Tower menu.
            {
                IsColliding = true;
            }

        }

        else if (CollisionType == 2) // MiniTower COlliding with Ground.
        {
            if (other.gameObject.name == "Ground") // hand Colliding with Tower menu.
            {
                IsColliding = true;
            }
        }

        else if (CollisionType == 3)
        {
            if (other.transform.parent.transform.tag == "Enemy")
            {
                ObjectsWithinCollider.Add(other.gameObject);
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
            if (other.transform.parent.transform.tag == "Enemy")
            {
                ObjectsWithinCollider.Remove(other.gameObject);
            }
        }
    }
}
