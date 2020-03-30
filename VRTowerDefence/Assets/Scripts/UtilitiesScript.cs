using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilitiesScript
{
    public static List<GameObject> ObjectsAffected = new List<GameObject>();

    public static void CircleRadius(Vector2 StartingCords, int Radius)
    {
        List<Vector2> Cords = new List<Vector2>();
        int Offset = 0;
        for (int Counter = 1; Counter <= (Radius + 1); Counter++)
        {
            if (Counter == Radius + 1)
            {
                Offset = 1;
            }

            for (int x = (int)StartingCords.x - Counter + Offset; x <= StartingCords.x + Counter - Offset; x++)
            {

                Cords.Add(new Vector2(x, StartingCords.y + ((Radius + 1) - Counter)));
                Cords.Add(new Vector2(x, StartingCords.y - ((Radius + 1) - Counter)));
            }
        }

        foreach (Vector2 Cord in Cords)
        {
            if (Cord.x < 40 && Cord.x >= 0 && Cord.y < 40 && Cord.y >= 0)
            {
                GridGenerator.SetGridPointAvailable(false, Cord);
            }

        }

    }

  


    public static int RandomiseByWeight(int[] Weights)
    {
        int TotalSum = 0;
        foreach (int Weight in Weights) // Adds all the Weights to generate the Total.
        {
            TotalSum += Weight;
        }

        int Index = 0;
        int LastIndex = Weights.Length - 1;

        while (Index < LastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, TotalSum) < Weights[Index])
            {
                return Index;
            }

            // Remove the last item from the sum of total untested weights and try again.
            TotalSum -= Weights[Index++];
        }
        // No Other index was selected.
        return Index;

    }


    public static IEnumerator ObjectBlinkColour(GameObject Object, Color Col, float BlinkTime)
    {
        //Debug.Log(ObjectsAffected);

        bool Flag = false;
        foreach (GameObject Obj in ObjectsAffected)
        {
            //Debug.Log(Obj + " : " + Object.name);

            if (Object == Obj)
            {
                //Debug.Log("Same");
                Flag = true;
            }
        }

        if (!Object)
        {
            Flag = true;
        }

        if (!Flag)
        {         
           // Debug.Log("Did it");
            ObjectsAffected.Add(Object);

            List<Color> ColourList = new List<Color>();
            Renderer[] ObjRenderers = Object.GetComponentsInChildren<Renderer>();

            foreach (Renderer Rend in ObjRenderers)
            {
                if (Rend.material.HasProperty("_Color")) // Checks the renderer actually has a colour property... Particle renderer does not ;)
                {
                    ColourList.Add(Rend.material.color); // Saves the original Colours of the object.
                    Rend.material.color = Col;           // Sets the object Color to the Color specificed (mostly Red)         
                }
                else
                {
                    ColourList.Add(Col); // If the renderer does not have a colour variable. Set that element in the list to the desired colour, so taht it is ignored.
                }
               
            }

            yield return new WaitForSeconds(BlinkTime); // Waits the desired time. and then we set the colours back to normal.

            int Counter = 0;
            foreach (Renderer Rend in ObjRenderers)
            {
                if (ColourList[Counter] != Col) // If the original colour is the same as the new colour then ignore.
                {
                    Rend.material.color = ColourList[Counter];          // Sets the colours back to normal.
                    Counter++;
                }
 
            }

            if (Object)
            {
                foreach (GameObject OBJ in ObjectsAffected.ToArray()) // Creates a new list of the ObjectAffected List before looping through it. Other wise it would change the values "under the hood" Causing the Coroutine to possibly not be executable.
                {
                    if (OBJ == Object)
                    {
                        ObjectsAffected.RemoveAt(ObjectsAffected.IndexOf(Object)); // Removes the Current object from the currently being used objects array, as it is no longer needed.
                    }
                }
                                             
            }
        }

        yield return null;
    }







}
