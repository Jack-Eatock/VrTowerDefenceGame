using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    // Generating Grid Placement
    [SerializeField]
    private int GridWidth;
    [SerializeField]
    private int GridHeight;
    [SerializeField]

    public static Vector3[,] GridPoints;

    public static bool[] GridUsed;
    public static float GridSpacing = 0.25f;

  
    public void start()
    {
        GenerateGrid(GridGenerator.GridSpacing);
        GridGenerator.GridUsed = new bool[GridWidth * GridHeight];
    }

    public void Update()
    {
        GenerateGrid(GridGenerator.GridSpacing);

    }
    public void GenerateGrid(float GridSpacing)
    {
        GridGenerator.GridPoints = new Vector3[GridWidth, GridHeight];
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {

                Vector3 NewPoint = new Vector3(transform.position.x + (x * GridSpacing), transform.position.y, transform.position.z + (y * GridSpacing));
                GridGenerator.GridPoints[x, y] = NewPoint;
            }
        }
    }



    /* public void OnDrawGizmos()
     {
         Gizmos.color = Color.yellow;

         foreach (Vector3 Point in GridGenerator.GridPoints)
         {
             Gizmos.DrawSphere(Point, 0.1f);
         }

     }
     */
}
