using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilitiesScript
{

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
}
