using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float Speed;
    public int Points;
    public string Name;
    public float Health;

    public List<Vector2> PathPoints = new List<Vector2>();
    public List<Vector3> LocalPathPoints = new List<Vector3>();
    private int CheckPoint = 0;

    private bool Loop = true;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x <= PathPoints.Count - 1; x++)
        {
            LocalPathPoints.Add(GridGenerator.GridStatus[(int)PathPoints[x].x, (int)PathPoints[x].y].Position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Loop)
        {
            if ( Health <= 0)
            {
                Debug.Log(name + " Is Dead");
                TowerScript.EnemiesInRange.Remove(gameObject.transform.GetChild(0).gameObject);
                EnemySpawner.EnemiesFinished++;
                Destroy(gameObject);          
            }

            if (CheckPoint < PathPoints.Count - 1)
            {
                if (Vector3.Distance(transform.localPosition, LocalPathPoints[CheckPoint + 1]) > 0.001f)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, LocalPathPoints[CheckPoint + 1], Speed * Time.deltaTime);
                }

                else
                {
                    CheckPoint++;
                }

            }
            else
            {
                Debug.Log("Enemy made it to the Finish");
                EnemySpawner.EnemiesFinished++;
                Loop = false;
                GameScript.PointPool += Points;
                Destroy(gameObject);
            }
        }
    }
} 
