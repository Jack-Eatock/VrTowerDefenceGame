using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingTowersScript : MonoBehaviour
{

    // Tower range Variables

    [SerializeField] private GameObject _towerRangePrefab;
    private GameObject _towerRangeDisplayer;

    // others


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
    private float _towerPosX;
    private float _towerPosZ;
    private float _towerPosY;
    private float _halfedGridSpacing;
    private Vector3 _currentPosition;
    private int _currentPositionPosX;
    private int _currentPositionPosY;
    private GameObject _placedTowersStorage;
    private bool TowerLockedToGridSuccessfully;



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

                //Debug.Log("IsPlacing + Colliding with Groud");

                _gridHeight = GridGenerator.GridStatus.GetLength(1);
                _gridWidth = GridGenerator.GridStatus.GetLength(0);



                TowerLockedToGridSuccessfully = LockMinitureTowerToGrid();

                if (TowerLockedToGridSuccessfully)
                {
                    DisplayTowerRange(true);

                    if (_newTower == null) // If it has not been generated yet. 
                    {
                        Debug.Log("NewTower Being Generated.");

                        _newTower = GameObject.Instantiate(_menuTowerScript.Towers[_menuTowerScript.CurrentlySelectedTowerPositionInArray].TowerGO);
                        _newTower.transform.position = _currentPosition;
                        _newTower.transform.localScale = new Vector3(MovementScript.ScaleFactor, MovementScript.ScaleFactor, MovementScript.ScaleFactor); // Uses Global Scale because it is not child of the world yet.
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

                else
                {
                    DisplayTowerRange(false);

                    if (_newTower != null)
                    {
                        if (_newTower.activeSelf)
                        {
                            _newTower.SetActive(false);
                            _menuTowerScript.CurrentlyDisplayedTower.SetActive(true);
                        }
                    }
                }

              
            }

            else 
            {
                DisplayTowerRange(false);

                if (_newTower != null)
                {
                    if (_newTower.activeSelf)
                    {
                        _newTower.SetActive(false);
                        _menuTowerScript.CurrentlyDisplayedTower.SetActive(true);
                    }
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

    public void DisplayTowerRange(bool Activate)
    {
        // Check if you want to turn the range on or off.

        if (Activate)
        {
            // Check if the range is already been instantiated.
            
            if (_towerRangeDisplayer == null)
            {
                // Instantiate the range displayer.
                _towerRangeDisplayer = GameObject.Instantiate(_towerRangePrefab);
                
            }

            _towerRangeDisplayer.SetActive(true);

            float range = _menuTowerScript.Towers[_menuTowerScript.CurrentlySelectedTowerPositionInArray].Range * MovementScript.ScaleFactor;

            _towerRangeDisplayer.transform.position = _currentPosition;
            _towerRangeDisplayer.transform.localScale = new Vector3(range, _towerRangePrefab.transform.localScale.y * MovementScript.ScaleFactor , range);

            // If it has reset pos and scale.

            // if not Create it and set pos and scale.
        }
        else
        {
            // destroy / hide the range displayer
            if (_towerRangeDisplayer != null)
            {
                _towerRangeDisplayer.SetActive(false);
            }

        }
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
        Debug.Log("Moving tower from menu to hand.");
        GameObject currentTower = _menuTowerScript.CurrentlyDisplayedTower;
        currentTower.transform.position = _rightHandAttachmentPoint.transform.position;
        currentTower.transform.SetParent(_rightHandAttachmentPoint.transform);
    }

    public bool LockMinitureTowerToGrid()
    {
        //Debug.Log("Locking Tower to Grid.");

       

        GridGenerator.UpdateGridSpacing(_gridHeight);
        _halfedGridSpacing =  ( GridGenerator.LocalGridSpacing / 2f ) * GameObject.Find("World").transform.localScale.x; // Gridspacing is local. Multiply by  world scale to make it Global Gridspacing.

        //Debug.Log("Grid spacing for tower" + _halfedGridSpacing);

        bool towerWasAbleToLockToGrid = false;

        for (int actualGridPointX = 0; actualGridPointX < _gridWidth; actualGridPointX++)
        {
            for (int actualGridPointY = 0; actualGridPointY < _gridHeight; actualGridPointY++)
            {
                if (!GridGenerator.GridStatus[actualGridPointX, actualGridPointY].Inuse)
                {
                    continue;
                }

                // So the point at which the tower is hovering has a tile in use. Lock now
                _towerPosX = _menuTowerScript.CurrentlyDisplayedTower.transform.position.x;
                _towerPosZ = _menuTowerScript.CurrentlyDisplayedTower.transform.position.z;
                _towerPosY = GameObject.Find("World").transform.position.y;


                Vector3 posOfTile = GridGenerator.GridStatus[actualGridPointX, actualGridPointY].Tile.transform.position;
                //Debug.Log("Tile pos: " + posOfTile + " Tower Current Pos: " + new Vector3(_towerPosX, _towerPosY, _towerPosZ) + "Halved Grid Spacing:" + _halfedGridSpacing + " Scale Factor" + MovementScript.ScaleFactor);

                if (posOfTile.x < _towerPosX + _halfedGridSpacing && posOfTile.x > _towerPosX - _halfedGridSpacing)
                {
                    if (posOfTile.z < _towerPosZ + _halfedGridSpacing && posOfTile.z > _towerPosZ - _halfedGridSpacing)
                    {
                        towerWasAbleToLockToGrid = true;
                        _currentPosition = new Vector3(posOfTile.x, _towerPosY, posOfTile.z);

                        //Debug.Log("Position Updated");

                        if (GridGenerator.GridStatus[actualGridPointX, actualGridPointY].Available)
                        {
                            //Debug.Log("Tower can be placed at ths grid Point" + _currentPosition);
                            _canPlaceTowerAtCurrentPos = true;
                            _currentPositionPosX = actualGridPointX;
                            _currentPositionPosY = actualGridPointY;
                        }
                        else if (_canPlaceTowerAtCurrentPos)
                        {
                            //Debug.Log("Current Grid Point is not placeable. " + _currentPosition);
                            _canPlaceTowerAtCurrentPos = false;
                        }

                        break;
                    }
                }
            }
        }

        if (towerWasAbleToLockToGrid == false)
        {
            //Debug.Log("Tower Was unable to lock to the Grid. Tower was at position:" + new Vector3 (_towerPosX, _towerPosY, _towerPosZ ));

        }

        return (towerWasAbleToLockToGrid);
    }

    public void PlaceTower()
    {

        DisplayTowerRange(false); // Stop displaying the range

        Debug.Log("Placing Tower!");

        TowerScript TempTowerScript = _newTower.AddComponent<TowerScript>();
        TempTowerScript.TowerProperties = _menuTowerScript.Towers[_menuTowerScript.CurrentlySelectedTowerPositionInArray];

        Vector3 TilePosition = GridGenerator.GridStatus[_currentPositionPosX, _currentPositionPosY].Tile.transform.position;
   

        if (_placedTowersStorage == null)
        {
            Debug.Log("There is no placed tower storage. Generating one...");
            _placedTowersStorage = GameObject.Instantiate(_placedTowerStoragePrefab, GameObject.Find("World").transform);
        }



        _newTower.transform.SetParent(_placedTowersStorage.transform);
        _newTower.transform.position = new Vector3(TilePosition.x, GameObject.Find("World").transform.position.y, TilePosition.z);



        UtilitiesScript.CircleRadius(new Vector2(_currentPositionPosX, _currentPositionPosY), 2);

        ResetPlacing();


    }

}
