using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HeightCalibrationMenuScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _heightText;
    [SerializeField] private TextMeshProUGUI _worldOffsetText;

    [SerializeField] private MovementScript _moveMentScript;

    private float _playerHeightInFeet;
    private float _playerHeightInCm;


    private float _worldFromPlayerOffsetInFeet;
    private float _worldFromPlayerOffsetInCm;

    private float _amountToChangeBy = 2f;

    private float _maxPlayerHeight = 242f; // 8 ft
    private float _minPlayerHeigt = 60f; // 2ft

    private float _maxWorldOffset = 180; // 8 ft
    private float _minWorldOffset = 0f; // 2ft


    // Start is called before the first frame update

    void OnEnable()
    {
        _maxWorldOffset = _playerHeightInCm;

        _playerHeightInCm = GameScript.PlayerHeight;
        _worldFromPlayerOffsetInCm = GameScript.WorldOffsetFromPlayerHeight;

        ConvertCmToFeet(true);
        ConvertCmToFeet(false);

        _heightText.text = _playerHeightInCm + " Cm \n" + _playerHeightInFeet + " Ft";
        _worldOffsetText.text = _worldFromPlayerOffsetInCm + " Cm \n" + _worldFromPlayerOffsetInFeet + " Ft";
    }



    public void PlayerHeightOnArrowButtonLeft(bool isLeft)
    {
        if (isLeft)
        {
            if (_playerHeightInCm - _amountToChangeBy > _minPlayerHeigt) // Checks not going to be below min height.
            {
                _playerHeightInCm -= _amountToChangeBy;
            }
        }

        else
        {
            if (_playerHeightInCm + _amountToChangeBy < _maxPlayerHeight) // Checks not going to be above max height.
            {
                _playerHeightInCm += _amountToChangeBy;
            }
        }


        ConvertCmToFeet(true);



        _heightText.text = _playerHeightInCm + " Cm \n" + _playerHeightInFeet + " Ft";
    }

    public void WorldOffsetOnArrowButtonLeft(bool isLeft)
    {

        if (isLeft)
        {
            if (_worldFromPlayerOffsetInCm - _amountToChangeBy > _minWorldOffset) // Checks not going to be below min height.
            {
                _worldFromPlayerOffsetInCm -= _amountToChangeBy;
            }
        }

        else
        {
            if (_worldFromPlayerOffsetInCm + _amountToChangeBy < _maxWorldOffset) // Checks not going to be above max height.
            {
                _worldFromPlayerOffsetInCm += _amountToChangeBy;
            }
        }

        ConvertCmToFeet(false);

        _worldOffsetText.text = _worldFromPlayerOffsetInCm + " Cm \n" + _worldFromPlayerOffsetInFeet + " Ft";
    }


    public void ConfirmHeight()
    {
        _maxWorldOffset = _playerHeightInCm;
        GameScript.PlayerHeight = _playerHeightInCm;
        _moveMentScript.OnUpdatePlayerHeight();
    }

    public void ConfirmOffset()
    {
        GameScript.WorldOffsetFromPlayerHeight = _worldFromPlayerOffsetInCm;
        _moveMentScript.OnUpdatePlayerHeight();
    }

    public void ConvertCmToFeet(bool playerHeight)
    {
        if (playerHeight)
        {
            _playerHeightInFeet = (_playerHeightInCm / 30.48f);

            _playerHeightInFeet = RoundToTheNearestPowerOfTen(_playerHeightInFeet, 1);
            //_playerHeightInCm = RoundToTheNearestPowerOfTen(_playerHeightInCm, 3);
        }
        else
        {
            _worldFromPlayerOffsetInFeet = (_worldFromPlayerOffsetInCm / 30.48f);
            _worldFromPlayerOffsetInFeet = RoundToTheNearestPowerOfTen(_worldFromPlayerOffsetInFeet, 1);
        }
      
    }



    public float RoundToTheNearestPowerOfTen(float value, int square)
    {
        float multiply = Mathf.Pow(10, square);

        int newValue = Mathf.RoundToInt(value * multiply);
        float result = newValue / multiply;

        return result;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
