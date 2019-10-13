using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridPoint
{
    public bool Available = true;
    public Vector3 Position;
}

public class GridGenerator : MonoBehaviour
{
    // Generating Grid Placement
    [SerializeField]
    private int GridWidth;
    [SerializeField]
    private int GridHeight;
   

    public static GridPoint[,] GridStatus;


    public static float GridSpacing = 0.25f;

  
    public void Start()
    {
        GridGenerator.GridStatus = new GridPoint[GridWidth, GridHeight];
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y< GridHeight; y++)
            {
                GridStatus[x, y] = new GridPoint();
            }
        }

        GenerateGrid(GridGenerator.GridSpacing);
    }

    public void Update()
    {
        GenerateGrid(GridGenerator.GridSpacing);
    }

    public void GenerateGrid(float GridSpacing)
    {   
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                GridStatus[x, y].Position = new Vector3(transform.position.x + (x * GridSpacing), transform.position.y, transform.position.z + (y * GridSpacing));
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
