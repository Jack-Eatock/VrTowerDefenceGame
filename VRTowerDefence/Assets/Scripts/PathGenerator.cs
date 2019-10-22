using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTile
{
    public static List<Vector2> Cords;
    public int StartDirection; // 1, North, 2 East, 3 South, 4 west.
}



public class PathGenerator : MonoBehaviour
{
    [SerializeField]
    private Vector2 StartingCords;
    private Vector2 CurrentCord;
    private int LastDirection;
    private int MaxIterations = 80;
    private int Counter = 0;
    private bool Loop = false;

    public static List<Vector2> Cords = new List<Vector2>();


    private void Start()
    {
        Cords.Add(StartingCords);
        CurrentCord = StartingCords;
    }

    // Update is called once per frame
    void Update()
    {
        if (Loop)
        {
            if (Counter >= MaxIterations)
            {
                Cords.Clear();
                Cords.Add(StartingCords);
                CurrentCord = StartingCords;
                Counter = 0;
            }

            else if (CurrentCord.y != 40)
            {
                Debug.Log(CurrentCord);
                Worm();
            }
            else
            {
                Loop = false;
                foreach (Vector2 cord in Cords)
                {
                    Debug.Log(cord);
                }
                
            }
            Counter++;
        }

    }

    public void Worm()
    {
        int Direction = Random.Range(1, 5);

        switch (Direction)
        {
            case 1: // Up 
                if (!BackOnSelfChecker(CurrentCord + new Vector2(0, 1)))
                {
                    AddCord(new Vector2(0, 1));
                }
                else
                {
                    Debug.Log("Back on self");
                }
              
                break;

            case 2:  // Right
                if (!BackOnSelfChecker(CurrentCord + new Vector2(1, 0)))
                {
                    AddCord(new Vector2(1, 0));
                }
                else
                {
                    Debug.Log("Back on self");
                }
                break;

            case 3: // Down 
                if (!BackOnSelfChecker(CurrentCord + new Vector2(0, -1)))
                {
                    AddCord(new Vector2(0, -1));
                }
                else
                {
                    Debug.Log("Back on self");
                }
                break;

            case 4: // left 
                if (!BackOnSelfChecker(CurrentCord + new Vector2(-1, 0)))
                {
                    AddCord(new Vector2(-1, 0));
                }
                else
                {
                    Debug.Log("Back on self");
                }
                break;

            case 5: // Up 
                if (!BackOnSelfChecker(CurrentCord + new Vector2(0, 1)))
                {
                    AddCord(new Vector2(0, 1));
                }
                else
                {
                    Debug.Log("Back on self");
                }

                break;

        }
    }

    public void AddCord(Vector2 Offset)
    {
        Cords.Add(CurrentCord + Offset);
        CurrentCord = (CurrentCord + Offset);

    }

    public bool BackOnSelfChecker(Vector2 NewCord)
    {
        bool Flag = false;
        foreach (Vector2 Cord in Cords)
        {
            if (Cord == NewCord)
            {
                Flag = true;
            }
        }
        if (NewCord.x > 40 || NewCord.x < 0 || NewCord.y < 0)
        {
            Flag = true;
        }
        Debug.Log(NewCord);
        return Flag;

    }
}


