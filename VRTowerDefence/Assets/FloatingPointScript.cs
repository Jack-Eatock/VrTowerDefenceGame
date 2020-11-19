using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingPointScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _floatingPointText;
    [SerializeField] private Animator _floatingAnim;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
               
    }

    public void StartFloating(int points)
    {
        _floatingPointText.text = "+ " + points.ToString();
        _floatingAnim.SetTrigger("Active");
    }
}
