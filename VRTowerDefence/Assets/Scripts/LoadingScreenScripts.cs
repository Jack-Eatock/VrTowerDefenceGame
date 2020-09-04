using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LoadingScreenScripts : MonoBehaviour
{
    [SerializeField] private  GameObject _loadingScreen;

    // Start is called before the first frame update
    void Start()
    {
    }

    public  IEnumerator LoadingScreenFunc(bool useFunc)
    {
        _loadingScreen.SetActive(true);
        if (useFunc)
        {          
            yield return new WaitUntil(() => SteamVR_LoadLevel.PauseFuncFlag == true);
    
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        _loadingScreen.SetActive(false);
    }
}
