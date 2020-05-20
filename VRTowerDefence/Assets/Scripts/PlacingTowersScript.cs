using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingTowersScript : MonoBehaviour
{

    [SerializeField] private GameObject _rightHandAttachmentPoint = null;
    [SerializeField] private GameObject _placedTowerStoragePrefab = null;

    private bool _canPlaceTowerAtCurrentPos = false;
    public static bool IsPlacing = false;


    private MenuManager _menuManager;
    private OnCollisionScript _onRightHandCollWithGround;
    private MenuTowerScript _menuTowerScript;

    private GameObject _newTower = null;

    // Locking Tower to Grid \\

    private int _gridWidth;
    private int _gridHeight;
    private float _posX;
    private float _posZ;
    private float _posY;
    private float _halfedGridSpacing;
    private Vector3 _currentPosition;
    private int _currentPositionPosX;
    private int _currentPositionPosY;
    private GameObject _placedTowersStorage;




    // Start is called before the first frame update
    void Start()
    {

        InputScripto.OnRightTriggerClick += RightTriggerClick;

        _menuManager = GetComponent<MenuManager>();
        _menuTowerScript = GetComponent<MenuTowerScript>();

        _onRightHandCollWithGround = _rightHandAttachmentPoint.GetComponent<OnCollisionScript>();

    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlacing)
        {
            _menuTowerScript.CurrentlyDisplayedTower.transform.rotation = Quaternion.LookRotation(new Vector3(1, 0, 0), Vector3.up); // Tower locks rotation looking up.

            if (_onRightHandCollWithGround.IsColliding) // If Mini Tower (Right hand Attachment point) is colliding with ground.
            {

                _gridHeight = GridGenerator.GridStatus.GetLength(1);
                _gridWidth = GridGenerator.GridStatus.GetLength(0);



                LockMinitureTowerToGrid();

                if (_newTower == null) // If it has not been generated yet. 
                {
                    Debug.Log("NewTower Being Generated.");
                    _newTower = GameObject.Instantiate(_menuTowerScript.Towers[_menuTowerScript.CurrentlySelectedTowerPositionInArray].TowerGO);
                    _newTower.transform.position = _currentPosition;
                    _newTower.transform.localScale = new Vector3(MovementScript.ScaleFactor, MovementScript.ScaleFactor, MovementScript.ScaleFactor);
                    _menuTowerScript.CurrentlyDisplayedTower.SetActive(false);

                }

                else // If the tower is generated make sure its position is updated and that it can stop be hidden and unhidden when the user collides with the ground etc.
                {
                    _newTower.transform.localScale = new Vector3(MovementScript.ScaleFactor, MovementScript.ScaleFactor, MovementScript.ScaleFactor);
                    _newTower.transform.position = _currentPosition;

                    if (_newTower.activeSelf == false)
                    {
                        _newTower.SetActive(true);
                        _menuTowerScript.CurrentlyDisplayedTower.SetActive(false);
                    }
                } 
            }

            else if (_newTower != null)
            {
                if (_newTower.activeSelf)
                {
                    _newTower.SetActive(false);
                    _menuTowerScript.CurrentlyDisplayedTower.SetActive(true);
                }
            }
        }
    }

    public void ResetPlacing()
    {
        _menuTowerScript.UpdateTowerMenu(true);
        _menuTowerScript.DisableTowerSwitch = false;
        _canPlaceTowerAtCurrentPos = false;
        IsPlacing = false;
        _newTower = null;
    }

    public void RightTriggerClick()
    {
        if (!_menuManager.IsMenuActive)
        {
            return;
        }

        if (_menuManager.IsMenuTowerPlacementMode) // Currently in tower placement mode.
        {


            if (_onRightHandCollWithGround.IsColliding) // If the Miniture Tower (Right Hand AttatchmentPoint) is colliding with the Ground.
            {
                if (IsPlacing)
                {
                    if (_canPlaceTowerAtCurrentPos)
                    {
                        // Place the Tower.
                        PlaceTower();
                    }
                    else
                    {
                        Debug.Log("Can Not Place");
                        StartCoroutine(UtilitiesScript.ObjectBlinkColour(_newTower, Color.red, 0.15f)); // Flash red , User cant place the tower.
                    }


                }
            }

            else if (_menuManager.MenuDisplayPoint.gameObject.GetComponent<OnCollisionScript>().IsColliding)  // If right hand is colliding with the menu.
            {
                if (IsPlacing)
                {
                    // Put tower back in Menu.
                    ResetPlacing();
                }

                else
                {
                    // Move Miniture Tower into Users hand.
                    MoveMinitureTowerToUsersHand();
                    _menuTowerScript.DisableTowerSwitch = true;
                    IsPlacing = true;
                }
            }

           


        }

        else // If in the user Prompt Menu and they right click.
        {
            if (_menuManager.MenuDisplayPoint.gameObject.GetComponent<OnCollisionScript>().IsColliding)  // If The user clicks on the user prompt Button.
            {
                _menuManager.UserPromptActivated();
            }
        }

    }

    public void MoveMinitureTowerToUsersHand()
    {
        GameObject currentTower = _menuTowerScript.CurrentlyDisplayedTower;
        currentTower.transform.position = _rightHandAttachmentPoint.transform.position;
        currentTower.transform.SetParent(_rightHandAttachmentPoint.transform);
    }

    public void LockMinitureTowerToGrid()
    {

        _posX =  _menuTowerScript.CurrentlyDisplayedTower.transform.position.x;
        _posZ =  _menuTowerScript.CurrentlyDisplayedTower.transform.position.z;
        _posY = GameObject.Find("World").transform.position.y;
        
        _halfedGridSpacing = GridGenerator.GridSpacing / 2f;

        //Debug.Log("Grid spacing for tower" + _halfedGridSpacing);


        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                Vector3 Point = GridGenerator.GridStatus[x, y].Tile.transform.position;
                if (Point.x < _posX + _halfedGridSpacing && Point.x > _posX - _halfedGridSpacing)
                {
                    if (Point.z < _posZ + _halfedGridSpacing && Point.z > _posZ - _halfedGridSpacing)
                    {
                        _currentPosition = new Vector3(Point.x, _posY, Point.z);

                        if (GridGenerator.GridStatus[x, y].Available)
                        {
                            _canPlaceTowerAtCurrentPos = true;
                            _currentPositionPosX = x;
                            _currentPositionPosY = y;
                        }
                        else if (_canPlaceTowerAtCurrentPos)
                        {
                            Debug.Log("NotPlaceable");
                            _canPlaceTowerAtCurrentPos = false;
                        }

                        break;
                    }
                }
            }
        }
    }

    public void PlaceTower()
    {
        Debug.Log("Placing Tower!");

        TowerScript TempTowerScript = _newTower.AddComponent<TowerScript>();
        TempTowerScript.TowerProperties = _menuTowerScript.Towers[_menuTowerScript.CurrentlySelectedTowerPositionInArray];

        Vector3 TilePosition = GridGenerator.GridStatus[_currentPositionPosX, _currentPositionPosY].Tile.transform.position;
        _newTower.transform.position = new Vector3(TilePosition.x, GameObject.Find("World").transform.position.y, TilePosition.z);

        if (_placedTowersStorage == null)
        {
            _placedTowersStorage = GameObject.Instantiate(_placedTowerStoragePrefab, GameObject.Find("World").transform);
        }

        _newTower.transform.SetParent(_placedTowersStorage.transform);
        UtilitiesScript.CircleRadius(new Vector2(_currentPositionPosX, _currentPositionPosY), 2);

        ResetPlacing();


    }

}
