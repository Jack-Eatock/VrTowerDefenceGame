using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
  
    public bool IsBuildingMenuActive = false; // When the menu is open, displaying towers.
    public bool IsPlacingActive = false;      // When a tower is in the users hand.
    public static bool IsBuidlingModeActive = false; // If the script should be active.
    private float SF;


    [Header ("Tower Variables")]

    [SerializeField] private TowerSO[] Towers;
    [SerializeField] private GameObject PlacedTowerStorageGO;


    private GameObject CurrentlyDisplayedMinitureTower = null;
    private int CurrentlyDisplayedMinitureTowerPosInList = 0;
    private GameObject[] MinitureTowers;
    private GameObject NewTower;



    private Vector3 CurrentPositionPhysical;
    private int CurrentVirtualPosX;
    private int CurrentVirtualPosY;
    private bool CanBePlaced = false;
    private GameObject PlacedTowerStorage;

    private MenuManagerScript MenuScripto;
    private MovementScript MovementScripto;
   

    // Start is called before the first frame update
    void Start()
    {
        MenuScripto = GetComponent<MenuManagerScript>();
        MovementScripto = GetComponent<MovementScript>();

        PreLoadMinitureTowers();

        InputScripto.OnRightTriggerClick += OnRightTriggerClicked;
        InputScripto.OnLeftDPClick += LeftDPClick;
        InputScripto.OnRightDPClick += RightDPClick;

        UpdateCurrentlyDisplayerMinitureTower();
    }

    // Update is called once per frame
    void Update()
    {
        SF = MovementScript.SF;

        if (!IsBuidlingModeActive) // If Build Mode is not active then the script should not react to inputs.
        {
            return;
        }

        if (IsPlacingActive)
        {
           

            if (IsCollidingWithGround())
            {
                AttemptToCalculateGridPositionForTower();
               

                if (NewTower == null)
                {
                    NewTower = GameObject.Instantiate(Towers[CurrentlyDisplayedMinitureTowerPosInList].TowerGO);
                }

                NewTower.transform.position = CurrentPositionPhysical;
                NewTower.transform.localScale = new Vector3(SF, SF, SF);

                if (CurrentlyDisplayedMinitureTower.activeSelf)
                {
                    CurrentlyDisplayedMinitureTower.SetActive(false);
                    NewTower.SetActive(true);
                }

            }
            else if (!CurrentlyDisplayedMinitureTower.activeSelf)
            {
                
                CurrentlyDisplayedMinitureTower.SetActive(true);
                NewTower.SetActive(false);
                

            }
        }



    }

    public bool IsCollidingWithGround()
    {
        if (MenuScripto.RightHandAttatchmentPoint.GetComponent<OnCollisionScript>().IsColliding) // If true then the Attatchment point is colliding with the Ground. Appears as if tower is colliding.
        {
            return true;
        }

        return false;
    }


    public void Interupt()
    {
        IsBuidlingModeActive = false;
        IsPlacingActive = false;
        CanBePlaced = false;
        Destroy(NewTower);
        MenuScripto = GetComponent<MenuManagerScript>();
        MenuScripto.ActivateCurrentDisplayedInteractable(true);
        MenuScripto.ActivateMenu(false);


    }


    public void LeftDPClick()
    {
        ShiftSelectedTower(false);

    }

    public void RightDPClick()
    {
        ShiftSelectedTower(true);
    }

    public void OnRightTriggerClicked()    // When the Right trigger is clicked
    {
        if (!IsBuidlingModeActive) // If Build Mode is not active then the script should not react to inputs.
        {
            return;
        }

        if (IsBuildingMenuActive)  // If Building Mode is active when clicked.
        {
            if (IsPlacingActive)  // If tower is already grabbed and in their hand. They will want to place it.
            {
                if (IsCollidingWithGround())       // If the tower is actually colliding with the ground.
                {
                    if (CanBePlaced)               // If the area they want to place is clear. Place the Tower;
                    {
                        PlaceTower();
                        IsPlacingActive = false;
                    }
                    else
                    {
                        StartCoroutine(UtilitiesScript.ObjectBlinkColour(NewTower, Color.red, 0.15f)); // Flash Red 
                    }
                }
            }

            else // Building is active but they are not currently placing. (Tower is still in the Menu).
            {
                if (IsCollidingWithMenuInteractable())  // If the users hand is colliding with the miniture tower in the interactable slot of the Menu.
                {
                    MenuScripto.MoveMinitureTowerFromMenuToHand(CurrentlyDisplayedMinitureTower);     // Moves miniture tower into the users hand.
                    IsPlacingActive = true;
                }
            }
        }

        else // Building is not Active
        {

        }
    }


    public void AttemptToCalculateGridPositionForTower()
    {
        // Locking the Tower to the grid.

        float PosX = CurrentlyDisplayedMinitureTower.transform.position.x;
        float PosZ = CurrentlyDisplayedMinitureTower.transform.position.z;
        float PosY = MovementScripto.GameWorld.transform.position.y;
        float GridSpacing = GridGenerator.GridSpacing / 2f;



        float GridHeight = GridGenerator.GridStatus.GetLength(1);
        float GridWidth = GridGenerator.GridStatus.GetLength(0);

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                Vector3 Point = GridGenerator.GridStatus[x, y].Tile.transform.position;
                if (Point.x < PosX + GridSpacing && Point.x > PosX - GridSpacing)
                {
                    if (Point.z < PosZ + GridSpacing && Point.z > PosZ - GridSpacing)
                    {
                        CurrentPositionPhysical = new Vector3(Point.x, PosY, Point.z);

                        if (GridGenerator.GridStatus[x, y].Available)
                        {
                            CanBePlaced = true;
                            CurrentVirtualPosX = x;
                            CurrentVirtualPosY = y;
                        }
                        else if (CanBePlaced)
                        {
                            Debug.Log("NotPlaceable");
                            CanBePlaced = false;
                        }

                        break;
                    }
                }
            }
        }

    }



    public void UpdateCurrentlyDisplayerMinitureTower()
    {
        CurrentlyDisplayedMinitureTower = MinitureTowers[CurrentlyDisplayedMinitureTowerPosInList];
        MenuScripto.ChangeInteractable(CurrentlyDisplayedMinitureTower);
        MenuScripto.ActivateCurrentDisplayedInteractable(true);
    }

    public void ShiftSelectedTower(bool ShiftRight)
    {
        if (IsPlacingActive || !IsBuildingMenuActive) // Enusres that the tower doesnt change while placing it.
        {
            return;
        }

        if (ShiftRight)
        {
            if (CurrentlyDisplayedMinitureTowerPosInList < Towers.Length - 1)  // Before shifting selected tower up in the list. Checks that it is not going be larger than the list.
            {
                CurrentlyDisplayedMinitureTowerPosInList += 1;
            }

            else
            {
                CurrentlyDisplayedMinitureTowerPosInList = 0; // If it will be out of the range. It resets at 0, appearing to loop to the user.
            }
        }

        else
        {
            if (CurrentlyDisplayedMinitureTowerPosInList != 0) // Before shifting the selected tower down in the list. Checks that it isnt already at the bottom. 
            {
                CurrentlyDisplayedMinitureTowerPosInList -= 1;
            }

            else
            {
                CurrentlyDisplayedMinitureTowerPosInList = Towers.Length - 1; // If it is then it is set to be at the top of the list which is equal to length -1.
            }
        }

        UpdateCurrentlyDisplayerMinitureTower();
    }


    public void PreLoadMinitureTowers()
    {
        MinitureTowers = new GameObject[Towers.Length]; 

        for (int SObject = 0; SObject < Towers.Length; SObject++)
        {
            MinitureTowers[SObject] = GameObject.Instantiate(Towers[SObject].MinitureVersion4Menu,MenuScripto.InteractableObject.transform.position, MenuScripto.InteractableObject.transform.rotation);
            MinitureTowers[SObject].transform.SetParent(MenuScripto.InteractableObject.transform);
            MinitureTowers[SObject].gameObject.SetActive(false);
        }
    }


    public bool IsCollidingWithMenuInteractable()
    {
        if (MenuScripto.InteractableObject.GetComponent<OnCollisionScript>().IsColliding)
        {
            return true;
        }

        return false;
       
    }

    public void PlaceTower()
    {
        GameModeScript.Points -= Towers[CurrentlyDisplayedMinitureTowerPosInList].Cost;

        SphereCollider SphereCol = NewTower.AddComponent<SphereCollider>();
        SphereCol.center = new Vector3(0, 0.5f, 0);
        SphereCol.isTrigger = true;

        OnCollisionScript TempColScript = NewTower.AddComponent<OnCollisionScript>();
        TempColScript.CollisionType = 3;

        TowerScript TempTowerScript = NewTower.AddComponent<TowerScript>();
        TempTowerScript.TowerProperties = Towers[CurrentlyDisplayedMinitureTowerPosInList];

        Vector3 TilePosition = GridGenerator.GridStatus[CurrentVirtualPosX, CurrentVirtualPosY].Tile.transform.position;
        NewTower.transform.position = new Vector3(TilePosition.x, MovementScripto.GameWorld.transform.position.y, TilePosition.z);

        if (PlacedTowerStorage == null)
        {
            PlacedTowerStorage = GameObject.Instantiate(PlacedTowerStorageGO, MovementScripto.GameWorld.transform);
        }

        NewTower.transform.SetParent(PlacedTowerStorage.transform);
        CanBePlaced = false;
        // Debug.Log(CurrentPositionPosX + " : " + CurrentPositionPosY);
        UtilitiesScript.CircleRadius(new Vector2(CurrentVirtualPosX, CurrentVirtualPosY), 2);

        NewTower = null;
        MenuScripto.MoveMinitureFromHandBackToMenu();
        MenuScripto.ActivateCurrentDisplayedInteractable(true);
    }





    /*

 

    // General Variables \\
    [Header("Tweekables")]

    public float SF;
    public float GridSize = 40;
    public bool Running = false;

    //

    [Header("Placing Towers")]

    [SerializeField] private GameObject CancelGO = null;
    public GameObject PlacedTowerStorageGO = null;

    private GameObject PlacedTowersStorage = null;
    private GameObject GameWorld = null;
    public GameObject NewTower = null;

    public bool TowerBeingPlaced = false;
    private bool NewTowerHidden = false;
    private bool CurrentlyDisplayedTowerHidden = true;
    public bool CanBePlaced = false;
    public static bool MenuControllsDisabled = true;

    //

    [Header("General References")]

    public GameObject Ground;
    public GameObject TowerMenuPos;
    public GameObject RightHandGO;

    private int CurrentPositionPosX;
    private int CurrentPositionPosY;
    private int GridWidth;
    private int GridHeight;

    private Vector3 CurrentPosition;

    //

    [Header("Menu Variables")]

    [SerializeField] private GameObject HandMenuGO = null;
    [SerializeField] private GameObject BuildingMenuGo = null;
    [SerializeField] private GameObject GeneralMenuGO = null;
    [SerializeField] private GameObject Text = null;
    [SerializeField] private GameObject PointsText = null;


    private GameObject[] MinitureTowers;
    private Text NameText;
    private Text PointsTextDisplayer;
    public bool BuildMenuActive = false;
    public bool GeneralMenuActive = false;
    public bool SufficientFunds = false;
    private int Points;

    //

    [Header("Tower Variables")]
    //Towers
    public TowerSO[] Towers;
    private GameObject CurrentlyDisplayedTower = null;
    private int CurrentlyDisplayedTowerPos = 0;

    //

    [Header("Ground Grid Variables")]

    // Switching grid ON/OFF Visual only \\
    [SerializeField] private Material GrassGridMat = null;
    [SerializeField] private Material GrassMat = null;

    private bool IsGroundGrid = false;


  

    public void InitatiateBuildingScript()
    {
        Debug.Log("Initiating BuidlingSCript.");

        GameWorld = this.GetComponent<MovementScript>().GameWorld;
        NameText = Text.GetComponent<Text>();
        PointsTextDisplayer = PointsText.GetComponent<Text>();


        SF = MovementScript.SF;

        MinitureTowers = new GameObject[Towers.Length];
        for (int SObject = 0; SObject < Towers.Length; SObject++)
        {
            MinitureTowers[SObject] = GameObject.Instantiate(Towers[SObject].MinitureVersion4Menu, TowerMenuPos.transform.position, TowerMenuPos.transform.rotation);
            MinitureTowers[SObject].transform.SetParent(TowerMenuPos.transform);
            MinitureTowers[SObject].gameObject.SetActive(false);
        }

        Running = true;
    }


    public void Update()
    {
        if (!Running)
        {
            return;
        }



        if (BuildMenuActive)
        {
            if (GameScript.Points != Points)
            {
                Points = GameScript.Points;
                UpdateBuildMenuText();
            }
        }

        SF = MovementScript.SF;
        GridGenerator.GridSpacing = ((Ground.transform.localScale.x * SF) / GridSize);

        if (TowerBeingPlaced)
        {
            if (!CancelGO.activeSelf)
            {
                CancelGO.SetActive(true);
            }

            CanBePlaced = false;

            CurrentlyDisplayedTower.transform.rotation = Quaternion.LookRotation(new Vector3(1, 0, 0), Vector3.up); // Tower locks rotation looking up.
            if (RightHandGO.GetComponent<OnCollisionScript>().IsColliding)
            {
                if (!CurrentlyDisplayedTowerHidden)
                {
                    CurrentlyDisplayedTower.SetActive(false);
                    CurrentlyDisplayedTowerHidden = true;
                }

                // Locking the Tower to the grid.

                float PosX = CurrentlyDisplayedTower.transform.position.x;
                float PosZ = CurrentlyDisplayedTower.transform.position.z;
                float PosY = GameWorld.transform.position.y;
                float GridSpacing = GridGenerator.GridSpacing / 2f;



                GridHeight = GridGenerator.GridStatus.GetLength(1);
                GridWidth = GridGenerator.GridStatus.GetLength(0);

                for (int x = 0; x < GridWidth; x++)
                {
                    for (int y = 0; y < GridHeight; y++)
                    {
                        Vector3 Point = GridGenerator.GridStatus[x, y].Tile.transform.position;
                        if (Point.x < PosX + GridSpacing && Point.x > PosX - GridSpacing)
                        {
                            if (Point.z < PosZ + GridSpacing && Point.z > PosZ - GridSpacing)
                            {
                                CurrentPosition = new Vector3(Point.x, PosY, Point.z);

                                if (GridGenerator.GridStatus[x, y].Available)
                                {
                                    CanBePlaced = true;
                                    CurrentPositionPosX = x;
                                    CurrentPositionPosY = y;
                                }
                                else if (CanBePlaced)
                                {
                                    Debug.Log("NotPlaceable");
                                    CanBePlaced = false;
                                }

                                break;
                            }
                        }
                    }
                }

                if (!NewTower)
                {
                    CurrentlyDisplayedTower.SetActive(false);
                    CurrentlyDisplayedTowerHidden = true;
                    NewTower = GameObject.Instantiate(Towers[CurrentlyDisplayedTowerPos].TowerGO);
                    NewTower.transform.position = CurrentPosition;
                    NewTower.transform.localScale = new Vector3(SF, SF, SF);

                }

                else
                {
                    NewTower.transform.localScale = new Vector3(SF, SF, SF);
                    NewTower.transform.position = CurrentPosition;

                    if (NewTowerHidden)
                    {
                        NewTower.SetActive(true);
                        NewTowerHidden = false;
                    }
                }



            }
            else if (NewTower)
            {
                NewTower.SetActive(false);
                NewTowerHidden = true;
                CurrentlyDisplayedTower.SetActive(true);
                CurrentlyDisplayedTowerHidden = false;
            }

        }
        else
        {
            if (CancelGO.activeSelf)
            {
                CancelGO.SetActive(false);
            }
        }
    }




    public void ActivateMenu(bool Activate, int MenuType)
    {
        if (Activate)
        {
            if (MenuType == 0) // Building Menu Set Active
            {
                HandMenuGO.SetActive(true);
                BuildingMenuGo.SetActive(true);
                GenerateRemoveMiniTowerFromMenu(CurrentlyDisplayedTowerPos, true);
                BuildMenuActive = true;
                GridSwitch();
            }

            else if (MenuType == 1) // Start Wave Menu. Active.
            {
                if (BuildMenuActive)
                {
                    ActivateMenu(false, 0);
                }
                HandMenuGO.SetActive(true);
                GeneralMenuGO.SetActive(true);
                GeneralMenuActive = true;

            }

        }
        else
        {
            if (MenuType == 0) // Building Menu Set Not active.
            {
                if (TowerBeingPlaced)
                {
                    SetTowerBeingPlacedTrueFalse(false);
                }
                HandMenuGO.SetActive(false);
                BuildingMenuGo.SetActive(false);
                GenerateRemoveMiniTowerFromMenu(0, false);
                BuildMenuActive = false;
                GridSwitch();
            }

            else if (MenuType == 1) // Start Wave Menu. Not Active.
            {
                HandMenuGO.SetActive(false);
                GeneralMenuGO.SetActive(false);
                GeneralMenuActive = false;
            }

        }

    }

    public void UpdateBuildMenuText()
    {
        NameText.text = Towers[CurrentlyDisplayedTowerPos].Name;
        int PointsAfter = GameScript.Points - Towers[CurrentlyDisplayedTowerPos].Cost;
        if (PointsAfter >= 0)
        {
            PointsTextDisplayer.text = "Points: " + Points + " (-" + Towers[CurrentlyDisplayedTowerPos].Cost + ") Purchasable!";
            SufficientFunds = true;
        }

        else
        {
            PointsTextDisplayer.text = "Points: " + Points + " (-" + Towers[CurrentlyDisplayedTowerPos].Cost + ") Insufficient Funds!";
            SufficientFunds = false;
        }
    }


    public void SetCurrentTowerPos(int Active = 0)
    {
        // Sets tower in the Menu. 
        if (Active == 0)
        {
            CurrentlyDisplayedTower.transform.SetParent(TowerMenuPos.transform);
            CurrentlyDisplayedTower.transform.position = TowerMenuPos.transform.position;
            CurrentlyDisplayedTower.transform.rotation = TowerMenuPos.transform.rotation;
        }

        //Sets tower as child of Hand.
        else if (Active == 1)
        {
            CurrentlyDisplayedTower.gameObject.transform.position = RightHandGO.transform.position;
            CurrentlyDisplayedTower.gameObject.transform.rotation = MinitureTowers[CurrentlyDisplayedTowerPos].transform.rotation;
            CurrentlyDisplayedTower.gameObject.transform.SetParent(RightHandGO.transform);
        }

    }




    public void OnPlaceOrCancel(bool Place)
    {
        if (Place)
        {
            GameScript.Points -= Towers[CurrentlyDisplayedTowerPos].Cost;

            SphereCollider SphereCol = NewTower.AddComponent<SphereCollider>();
            SphereCol.center = new Vector3(0, 0.5f, 0);
            SphereCol.isTrigger = true;

            OnCollisionScript TempColScript = NewTower.AddComponent<OnCollisionScript>();
            TempColScript.CollisionType = 3;

            TowerScript TempTowerScript = NewTower.AddComponent<TowerScript>();
            TempTowerScript.TowerProperties = Towers[CurrentlyDisplayedTowerPos];

            Vector3 TilePosition = GridGenerator.GridStatus[CurrentPositionPosX, CurrentPositionPosY].Tile.transform.position;
            NewTower.transform.position = new Vector3(TilePosition.x, GameWorld.transform.position.y, TilePosition.z);

            if (PlacedTowersStorage == null)
            {
                PlacedTowersStorage = GameObject.Instantiate(PlacedTowerStorageGO, GameWorld.transform);
            }

            NewTower.transform.SetParent(PlacedTowersStorage.transform);
            CanBePlaced = false;
            // Debug.Log(CurrentPositionPosX + " : " + CurrentPositionPosY);
            CircleRadius(new Vector2(CurrentPositionPosX, CurrentPositionPosY), 2);

        }
        else
        {
            Destroy(NewTower);
        }
        NewTower = null;
        TowerBeingPlaced = false;
        SetCurrentTowerPos(0);
        GenerateRemoveMiniTowerFromMenu(CurrentlyDisplayedTowerPos, true);
        //SetPlacingOnOff(true);
    }

    public void SetTowerBeingPlacedTrueFalse(bool Activate)
    {
        if (Activate)
        {
            TowerBeingPlaced = true;
            HandMenuGO.SetActive(true);
            GenerateRemoveMiniTowerFromMenu(CurrentlyDisplayedTowerPos, true);
            BuildMenuActive = true;
            SetCurrentTowerPos(1);
        }
        else
        {
            TowerBeingPlaced = false;
            if (NewTower)
            {
                Destroy(NewTower);
                NewTower = null;
            }
            SetCurrentTowerPos(0);
            GenerateRemoveMiniTowerFromMenu(0, false);
        }
    }


    public void SwitchDisplayedTower(bool Right)
    {
        CurrentlyDisplayedTower.SetActive(false);
        if (Right)
        {

            if (CurrentlyDisplayedTowerPos + 1 > MinitureTowers.Length - 1)
            {
                CurrentlyDisplayedTowerPos = 0;
            }
            else
            {
                CurrentlyDisplayedTowerPos += 1;
            }
        }
        else
        {
            if (CurrentlyDisplayedTowerPos - 1 < 0)
            {
                CurrentlyDisplayedTowerPos = MinitureTowers.Length - 1;
            }
            else
            {
                CurrentlyDisplayedTowerPos -= 1;
            }
        }
        GenerateRemoveMiniTowerFromMenu(CurrentlyDisplayedTowerPos, true);

    }

    public void GenerateRemoveMiniTowerFromMenu(int x, bool Generate)
    {
        if (CurrentlyDisplayedTower != null)
        {
            CurrentlyDisplayedTower.SetActive(false);
            CurrentlyDisplayedTower = null;
        }

        if (Generate)
        {
            MinitureTowers[x].gameObject.SetActive(true);
            CurrentlyDisplayedTower = MinitureTowers[x].gameObject;

            UpdateBuildMenuText();


        }
    }

    // Function called to switch the ground material from having a grid or not. 
    public void GridSwitch()
    {
        if (IsGroundGrid)
        {
            GridGenerator.OnLoadInUseTiles(false);
            Ground.GetComponent<Renderer>().material = GrassMat;
            IsGroundGrid = false;
        }
        else
        {
            GridGenerator.OnLoadInUseTiles(true);
            Ground.GetComponent<Renderer>().material = GrassGridMat;
            IsGroundGrid = true;
        }
    }

    // If the ground is made larger use this function to scale the grid along with it. If not the gird will look strange.
    public void UpdateGroundScale()
    {
        float TempGroundScaleX = Ground.transform.localScale.x;
        float TempGroundScaleZ = Ground.transform.localScale.z;
        GrassGridMat.mainTextureScale = new Vector2(TempGroundScaleX, TempGroundScaleZ);
    }

    public void RightTriggerClick()
    {
        if (!TowerBeingPlaced)
        {
            if (TowerMenuPos.GetComponent<OnCollisionScript>().IsColliding && BuildMenuActive) // Move the tower from menu to the users hand.
            {
                SetTowerBeingPlacedTrueFalse(true);
            }

            else if (TowerMenuPos.GetComponent<OnCollisionScript>().IsColliding && GeneralMenuActive && PathGenerator.PathGenerationComplete) // Start Next wave.
            {
                GameObject.Find("Local GameManager").GetComponent<EnemySpawner>().StartWave();
                ActivateMenu(false, 1);
                BuildingScript.MenuControllsDisabled = false;
            }
        }

        else if (CanBePlaced) // IF they can place the Tower, Place tower.
        {
            if (SufficientFunds)
            {
                OnPlaceOrCancel(true);
            }
            else
            {
                StartCoroutine(UtilitiesScript.ObjectBlinkColour(NewTower, Color.red, 0.15f)); // Flash red , User cant place the tower.
            }

        }

        else if (TowerBeingPlaced && TowerMenuPos.GetComponent<OnCollisionScript>().IsColliding) // If they click on the X and we know that the tower cant be placed. Cancel.
        {
            OnPlaceOrCancel(false);
        }

        else
        {
            StartCoroutine(UtilitiesScript.ObjectBlinkColour(NewTower, Color.red, 0.15f)); // Flash red , User cant place the tower.
        }

    }

    */

}
