using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// cleared \\

public static class UtilitiesScript
{
    public static List<GameObject> ObjectsAffected = new List<GameObject>();

    public static int RandomiseByWeight(int[] weights)
    {
        int totalSum = 0;
        foreach (int weight in weights) // Adds all the Weights to generate the Total.
        {
            totalSum += weight;
        }

        int index = 0;
        int lastIndex = weights.Length - 1;

        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, totalSum) < weights[index])
            {
                return index;
            }

            // Remove the last item from the sum of total untested weights and try again.
            totalSum -= weights[index++];
        }
        // No Other index was selected.
        return index;

    }


    public static IEnumerator ObjectBlinkColour(GameObject theObject, Color theColour, float theBlinkTime)
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

            yield return new WaitForSeconds(theBlinkTime); // Waits the desired time. and then we set the colours back to normal.

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
