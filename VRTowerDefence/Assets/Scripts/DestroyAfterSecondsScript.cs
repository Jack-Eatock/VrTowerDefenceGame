using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSecondsScript : MonoBehaviour
{

    [SerializeField] private float SecondsBeforeDestruction = 2;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(gameObject));
    }

    
    public IEnumerator DestroyAfterSeconds(GameObject obj)
    {
        yield return new WaitForSeconds(SecondsBeforeDestruction);

        if (obj != null)
        {
            Destroy(obj);
        }
    }
}
