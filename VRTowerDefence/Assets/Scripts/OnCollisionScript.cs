using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionScript : MonoBehaviour
{
    public int CollisionType;
    public bool IsColliding;

    public void OnTriggerEnter(Collider other)
    {
        if (CollisionType == 1)
        {
            if (other.gameObject.name == "RightHand") // hand Colliding with Tower menu.
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
    }
}
