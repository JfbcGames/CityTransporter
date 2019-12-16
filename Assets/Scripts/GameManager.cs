using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manage all the game's behaviour and contains or can access all the runtime data. 
/// </summary>
public class GameManager : MonoBehaviour, ITransportListener {

    public static GameManager instance = null;
    private Plane auxPlane;
    public GameObject cam;
    public Sprite[] resourcesIcons;
    public GameObject navMeshGO;            //contains NavMeshSurface script
    private int[] screenDimensions;         //x, y
    private readonly int screenMargin = 10;
    private readonly float maxClosePointDistance = 1;
    public float cameraMovementSpeed;
    private Resources resources;
    private List<GameObject> gameBuildings;                     //builded buildings
    private List<GameObject> gameRoads;                         //builded roads
    private List<CarrierOfficeBehaviour> gameCarrierOffices;    //builded carrier office
    private List<WarehouseBehaviour> gameWarehouses;            //builded warehouses
    private Vector3 auxRoadStartPoint = Vector3.negativeInfinity, auxRoadEndPoint = Vector3.negativeInfinity;
    private Vector3 mouseLocation;

    #region "Buildables related variables"
    public Buildable[] allBuildables;       //all available buildables.
    public GameObject roadPrefab;
    #endregion

    #region "Vehicles related variables"
    public Vehicle[] allVehicles;           //types of vehicles
    private Pool vehiclesPool;
    public GameObject vehicleBase;          //it should change in the future
    #endregion

    #region Blueprint system related variables
    private Buildable somethingToBuild = null;
    public Material blueprintMaterial;
    private GameObject blueprint;
    private bool isBuildingSomething = false;
    private int freeBuilds = 10;
    #endregion

    #region GUIelements
    //BuildMenu items and prefabs
    private Type[] buildableTypes = { typeof(Road), typeof(Building) };
    public Sprite[] tabsIcons;
    public GameObject buildMenu;
    public GameObject tabBtnPref;
    public GameObject tabBtnPanel;
    public GameObject tabsContainerPanel;
    public GameObject tabPanel;
    public GameObject buildableObjectPanel;
    public GameObject descriptionText;
    public GameObject oneResourcePanel;
    public GameObject[] panelsTabsBuildables;   //the big panels displaying all the buildables of a type
    public GameObject[] buildablesBtns;         //the buildable btns that display each buildable object
    public GameObject[] tabsBtns;               //the btns for changing the displayed panelTabBuildable
    public GameObject resourcesIndicatorPanel;
    public GameObject pauseMenu;
    #endregion

    #region Initial escene
    //elements in the sample scene
    public bool chargeInitialScene = true;
    public GameObject warehouse;
    public GameObject carrierOffice;
    public GameObject road;
    #endregion

    void Awake() {

        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        if (false /*Partida cargada*/) {
            //resources = partidaCargada.res
            //vehicles pool = partidaCargada.vehicles
            //gameWarehouses = partidaCargada.gameWarehouses
            //gameCarrieroffices = partidaCargada.gameCarrierOfices
        } else if (chargeInitialScene) {

            Instantiate(road, navMeshGO.transform);
            Instantiate(warehouse);
            Instantiate(carrierOffice);

            road.GetComponent<RoadBehaviour>().startPoint = new Vector3(-40.7f, 0.2f, -18f);
            road.GetComponent<RoadBehaviour>().endPoint = new Vector3(-25.4f, 0.2f, -18.2f);
            gameRoads = new List<GameObject>();
            gameRoads.Add(road);
            gameBuildings = new List<GameObject>();
            gameBuildings.Add(warehouse);
            gameBuildings.Add(carrierOffice);
            gameWarehouses = new List<WarehouseBehaviour>();
            gameWarehouses.Add(warehouse.GetComponent<WarehouseBehaviour>());
            gameCarrierOffices = new List<CarrierOfficeBehaviour>();
            gameCarrierOffices.Add(carrierOffice.GetComponent<CarrierOfficeBehaviour>());
            vehiclesPool = new Pool(vehicleBase, 5);
            for (int i = 0; i < vehiclesPool.GetPoolSize(); i++) {
                carrierOffice.GetComponent<CarrierOfficeBehaviour>().StoreVehicle(vehiclesPool.GetElement(i).GetComponent<VehicleBehaviour>());
            }

        } else {
            resources = new Resources(new int[System.Enum.GetValues(typeof(IResources)).Length]);
            vehiclesPool = new Pool(vehicleBase, 5);
            gameBuildings = new List<GameObject>();
            gameRoads = new List<GameObject>();
            gameWarehouses = new List<WarehouseBehaviour>();
            gameCarrierOffices = new List<CarrierOfficeBehaviour>();
        }

    }

    void Start() {

        //set screen dimensions
        screenDimensions = new int[2];
        screenDimensions[0] = Screen.width;
        screenDimensions[1] = Screen.height;

        //create blueprint
        blueprint = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blueprint.GetComponent<Renderer>().material = blueprintMaterial;
        blueprint.SetActive(false);

        //create de auxPlane
        auxPlane = new Plane(Vector3.up, new Vector3(0,0,0));

        //initialize the GUI elements
        BuildGUI();
        UpdateResourcesIndicator();

    }

    void Update() {

        mouseLocation = GetPlayerMouseLocation();
        float mouseWheel = Input.mouseScrollDelta.y;
        bool mouseClick = Input.GetMouseButtonDown(0);
        bool mouseSecClick = Input.GetMouseButtonDown(1);
        bool esc = Input.GetKeyDown(KeyCode.Escape);
        MoveCamera();

        if (somethingToBuild != null) {
            BlueprintSystem(mouseLocation, mouseWheel, mouseClick, esc || mouseSecClick);
        } else {
            if (mouseSecClick) {
                buildMenu.SetActive(!buildMenu.activeSelf);
            } else if (mouseClick) { 
                //building click mechanics
            } else if (buildMenu.activeSelf && esc) { //comprobar tambien si se hace click fuera del menu
                buildMenu.SetActive(false);
            } else if (esc) {
                TooglePause();
            }
            cam.transform.position += new Vector3(0, -mouseWheel * cameraMovementSpeed * Time.deltaTime, 0);
        }

    }

    private void BuildGUI() {
        BuildInfoGUI();
        BuildBuildMenu();
    }

    /// <summary>
    /// Builds the buildMenu with the available buildables objects
    /// </summary>
    private void BuildBuildMenu() {

        panelsTabsBuildables = new GameObject[buildableTypes.Length];
        buildablesBtns = new GameObject[allBuildables.Length];
        int[] numeros = new int[allBuildables.Length];
        int[] panelsIndexes = new int[buildableTypes.Length];
        GameObject auxResourcePanel;
        GameObject auxObj;

        buildMenu.SetActive(true);
        for (int i = 0; i < buildableTypes.Length; i++) {
            int aux1 = i;
            panelsIndexes[aux1] = aux1;
            panelsTabsBuildables[i] = Instantiate(tabPanel, tabsContainerPanel.transform);
            auxObj = Instantiate(tabBtnPref, tabBtnPanel.transform);
            auxObj.transform.GetChild(0).GetComponent<Text>().text = buildableTypes[i].FullName;
            auxObj.transform.GetChild(1).GetComponent<Image>().sprite = tabsIcons[i];
            auxObj.GetComponent<Button>().onClick.AddListener(() => ChangeBuildablesTypeTab(panelsIndexes[aux1]));
            for (int j = 0; j < allBuildables.Length; j++) {
                int aux2 = j;
                numeros[aux2] = aux2;
                if (buildableTypes[i].IsAssignableFrom(allBuildables[j].GetType())) {
                    buildablesBtns[j] = Instantiate(buildableObjectPanel, panelsTabsBuildables[i].transform);
                    buildablesBtns[j].transform.GetChild(0).GetComponent<Image>().sprite = allBuildables[j].menuArtwork;
                    for (int k = 0; k < System.Enum.GetValues(typeof(IResources)).Length; k++) {
                        auxResourcePanel = Instantiate(oneResourcePanel, buildablesBtns[j].transform.GetChild(1).transform);
                        auxResourcePanel.transform.GetChild(0).GetComponent<Image>().sprite = resourcesIcons[k];
                        auxResourcePanel.transform.GetChild(1).GetComponent<Text>().text = allBuildables[j].cost[k].ToString();
                    }
                    buildablesBtns[j].GetComponent<Button>().onClick.AddListener(() => BuildSomething(allBuildables[numeros[aux2]]));
                    buildablesBtns[j].GetComponent<EventTrigger>().triggers.Add(new EventTrigger.Entry {
                        eventID = EventTriggerType.PointerEnter
                    });
                    buildablesBtns[j].GetComponent<EventTrigger>().triggers.Add(new EventTrigger.Entry {
                        eventID = EventTriggerType.PointerExit
                    });
                    buildablesBtns[j].GetComponent<EventTrigger>().triggers[0].callback.AddListener((eventData) => ShowBuildableInfoMenu(allBuildables[numeros[aux2]]));
                    buildablesBtns[j].GetComponent<EventTrigger>().triggers[1].callback.AddListener((eventData) => ClearBuildableInfoMenu());
                }
            }
            panelsTabsBuildables[i].SetActive(false);
        }
        panelsTabsBuildables[0].SetActive(true);
        buildMenu.SetActive(false);
    }

    private void ChangeBuildablesTypeTab(int nTab) {
        //Debug.Log(nTab);
            for (int i = 0; i < panelsTabsBuildables.Length; i++) {
                panelsTabsBuildables[i].SetActive(false);
            }
        

        panelsTabsBuildables[nTab].SetActive(true);
    }

    private void ShowBuildableInfoMenu(Buildable b) {
        descriptionText.GetComponent<Text>().text = b.buildingName + ": " + b.description;
    }

    private void ClearBuildableInfoMenu() {
        descriptionText.GetComponent<Text>().text = "";
    }

    private void BuildInfoGUI() {
        for (int i = 0; i < System.Enum.GetValues(typeof(IResources)).Length; i++) {
            Instantiate(oneResourcePanel, resourcesIndicatorPanel.transform);
            resourcesIndicatorPanel.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = resourcesIcons[i];
        }
        UpdateResourcesIndicator();
    }

    public void UpdateResourcesIndicator() {
        resources = GetTotalResources();
        for (int i = 0; i < System.Enum.GetValues(typeof(IResources)).Length; i++) {
            resourcesIndicatorPanel.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = resources[i].ToString();
        }
    }

    public void TooglePause() {
        if (pauseMenu.activeSelf) {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        } else {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ShowBuildMenu() {
        buildMenu.SetActive(true);
    }

    /// <summary>
    /// Manage the blueprint system, which is active before building whatever that inherit from Buildable class.
    /// </summary>
    /// <remarks>
    /// Depending on the type of object to build, the method will actuate in a different way.
    /// </remarks>
    /// <param name="mouseLocation">The world space position that the player is pointing to.</param>
    /// <param name="mouseWheel">The rotation value of the mouse wheel.</param>
    /// <param name="mouseClick">Did the player clicked?</param>
    /// <param name="cancel">Need to cancel the building process?</param>
    public void BlueprintSystem(Vector3 mouseLocation, float mouseWheel, bool mouseClick, bool cancel) {

        if (cancel) {
            ResetBlueprintSystem();
            return;
        }

        Vector3 aux = IsCloseToABuildPoint(mouseLocation + somethingToBuild.BuildingPointOffset);
        if (!aux.Equals(Vector3.negativeInfinity)) {
            mouseLocation = aux - somethingToBuild.BuildingPointOffset;
        }

        ShowBuildBlueprint(mouseLocation, mouseWheel);

        if (mouseClick) {
            if (somethingToBuild is Building) {
                if (PayBuildingCost(somethingToBuild.cost)) {
                    gameBuildings.Add(BuildBuildingGameObject((Building)somethingToBuild));
                    gameBuildings[gameBuildings.Count - 1].transform.position = mouseLocation;
                    gameBuildings[gameBuildings.Count - 1].transform.rotation = blueprint.transform.rotation;
                    if (somethingToBuild is WarehouseBuilding) {
                        gameWarehouses.Add(gameBuildings[gameBuildings.Count - 1].GetComponent<WarehouseBehaviour>());
                        UpdateResourcesIndicator();
                    } else if (somethingToBuild is CarrierOfficeBuilding) {
                        gameCarrierOffices.Add(gameBuildings[gameBuildings.Count - 1].GetComponent<CarrierOfficeBehaviour>());
                    }
                    ResetBlueprintSystem();
                } else {
                    CantAfford();
                }
            } else if (somethingToBuild is Road) {
                if (auxRoadStartPoint.Equals(Vector3.negativeInfinity)) {
                    auxRoadStartPoint = mouseLocation;
                } else if (PayBuildingCost(somethingToBuild.cost)) {
                    auxRoadEndPoint = mouseLocation;
                    gameRoads.Add(InstantiateNewRoad((Road)somethingToBuild, auxRoadStartPoint, auxRoadEndPoint));
                    RebakeNavMesh();
                    ResetBlueprintSystem();
                } else {
                    CantAfford();
                }
            }
        }
    }

    /// <summary>
    /// Render the blueprint object.
    /// </summary>
    /// <param name="mouseLocation">The point to render the blueprint</param>
    /// <param name="mouseWheel">The mouse wheel rotation for blueprint rotation purposes. 0 if not rotation needed</param>
    public void ShowBuildBlueprint(Vector3 mouseLocation, float mouseWheel) {

        if (!isBuildingSomething) {
            blueprint.GetComponent<MeshFilter>().mesh = somethingToBuild.mesh;
            Material[] materials = new Material[somethingToBuild.mats.Length];
            for (int i = 0; i < materials.Length; i++) {
                materials[i] = blueprintMaterial;
            }
            blueprint.GetComponent<MeshRenderer>().materials = materials;
            blueprint.SetActive(true);
            isBuildingSomething = true;
        }

        if (somethingToBuild is Building) {
            blueprint.transform.position = mouseLocation;
        } else if (somethingToBuild is Road) {
            if (auxRoadStartPoint.Equals(Vector3.negativeInfinity)) {
                blueprint.transform.position = mouseLocation;
            } else {
                blueprint.transform.position = (auxRoadStartPoint + mouseLocation) / 2;
                blueprint.transform.localScale = new Vector3(Vector3.Distance(auxRoadStartPoint, mouseLocation), 1, 1);
                blueprint.transform.forward = Vector3.Cross(Vector3.up, mouseLocation - auxRoadStartPoint);
            }
        }

        if (mouseWheel != 0 && somethingToBuild is Building) { //las carreteras no se pueden girar
            RotateBlueprint(mouseWheel > 0);
        }

    }

    /// <summary>
    /// Rotates the orientation of the blueprint
    /// </summary>
    /// <param name="direction">The direction of the rotation. True-->positive. False-->negative</param>
    public void RotateBlueprint(bool direction) {
        int n;
        if (direction)
            n = 10;
        else
            n = -10;
        blueprint.transform.Rotate(0, n, 0);
    }

    /// <summary>
    /// Resets the information that the blueprint system uses.
    /// </summary>
    private void ResetBlueprintSystem() {
        isBuildingSomething = false;
        auxRoadStartPoint = Vector3.negativeInfinity;
        auxRoadEndPoint = Vector3.negativeInfinity;
        somethingToBuild = null;
        blueprint.transform.position = Vector3.zero;
        blueprint.transform.localScale = new Vector3(1, 1, 1);
        blueprint.transform.rotation = Quaternion.identity;
        //blueprint.GetComponent<Renderer>().materials = new Material[] { blueprintMaterial };
        blueprint.SetActive(false);
    }

    /// <summary>
    /// Recalculate navegable surface
    /// </summary>
    private void RebakeNavMesh() {
        float startTime = Time.time;
        navMeshGO.GetComponent<NavMeshSurface>().BuildNavMesh();
        Debug.Log("Bake Time: " + (Time.time - startTime)); //no funciona porque time.time es en segundos.
    }


    /// <summary>
    /// check if the given point is close to some other point where is recommended to build.
    /// </summary>
    /// <param name="point">The point to look around</param>
    /// <returns>The point which is close to or Vector3.negativeInfinity if dont find any point</returns>
    private Vector3 IsCloseToABuildPoint(Vector3 point) {

        Vector3[] auxBuildPoints;
        for (int i = 0; i < gameRoads.Count; i++) {
            auxBuildPoints = gameRoads[i].GetComponent<RoadBehaviour>().GetRoadAtachPoints();
            for (int j = 0; j < auxBuildPoints.Length; j++) { //check each point of road
                if (Vector3.Distance(point, auxBuildPoints[j]) <= maxClosePointDistance) {
                    return auxBuildPoints[j];
                }
            }
        }
        return Vector3.negativeInfinity;

    }

    /// <summary>
    /// Moves the camera 
    /// </summary>
    public void MoveCamera() {

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 v = Input.mousePosition;

        if (v.x < screenMargin) {
            cam.transform.position += new Vector3(-cameraMovementSpeed * Time.deltaTime, 0, 0);
        } else if (v.x > screenDimensions[0] - screenMargin) {
            cam.transform.position += new Vector3(cameraMovementSpeed * Time.deltaTime, 0, 0);
        }
        if (v.y < screenMargin) {
            cam.transform.position += new Vector3(0, 0, -cameraMovementSpeed * Time.deltaTime);
        } else if (v.y > screenDimensions[1] - screenMargin) {
            cam.transform.position += new Vector3(0, 0, cameraMovementSpeed * Time.deltaTime);
        }

        transform.position += new Vector3(horizontalInput * cameraMovementSpeed * Time.deltaTime, 0, verticalInput * cameraMovementSpeed * Time.deltaTime);

    }

    /// <summary>
    /// Plays the media to indicate a lack of resources
    /// </summary>
    public void CantAfford() {
        Debug.Log("Cant afford");
        ResetBlueprintSystem();
        //cant afford it. Blink resources indicator. Play wrong sound. 
    }

    /// <summary>
    /// Calculate the world space position that the player is pointing to.
    /// </summary>
    /// <returns>The world space position that the player is pointing to</returns>
    public Vector3 GetPlayerMouseLocation() {
        Vector3 v = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(v);
        auxPlane.Raycast(ray, out float distance);
        return ray.GetPoint(distance);
    }

    /// <summary>
    /// Public method to start a build process with the blueprint system.
    /// </summary>
    /// <param name="something">The scriptable object from which to take the build information</param>
    public void BuildSomething(Buildable something) {

        this.buildMenu.SetActive(false);
        this.somethingToBuild = something;

    }

    /// <summary>
    /// Check if can afford some cost
    /// </summary>
    /// <param name="cost">The cost to check</param>
    /// <returns>True if can afford it. False otherwise</returns>
    public bool CanPayBuildingCost(Resources cost) {

        for (int i = 0; i < resources.length; i++) {
            if (resources[i] - cost[i] < 0) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Pay some cost if can afford it
    /// </summary>
    /// <param name="cost">The cost to pay</param>
    /// <returns>True if have paied the cost. False if can't afford it</returns>
    private bool PayBuildingCost(Resources cost) {

        bool paied = false;
        if (freeBuilds > 0) {
            freeBuilds--;
            return true;
        } else if (CanPayBuildingCost(cost)) {
            Resources.Sub(resources, cost);
            for (int i = 0; i < gameWarehouses.Count && !paied; i++) {
                if (Resources.CompareResources(gameWarehouses[i].actualStock, cost)) {
                    gameWarehouses[i].actualStock = Resources.Sub(gameWarehouses[i].actualStock, cost);
                    paied = true;
                } else {
                    Resources diff = Resources.Sub(cost, gameWarehouses[i].actualStock);
                    gameWarehouses[i].actualStock = Resources.Sub(gameWarehouses[i].actualStock, diff);
                    cost = Resources.Sub(cost, diff);
                }
            }
            return true;
        } else {
            return false;
        }

    }

    /// <summary>
    /// Set up a new GameObject representing a new building
    /// </summary>
    /// <param name="b">The building data from a scriptable object</param>
    /// <returns>The Building GameObject</returns>
    private GameObject BuildBuildingGameObject(Building b) {

        Component bb = null;
        GameObject go = new GameObject(gameBuildings.Count.ToString() + "_" + b.GetName(), typeof(MeshFilter), typeof(MeshRenderer), typeof(BuildingBuilder));
        go.GetComponent<BuildingBuilder>().SetupBuilder(b);
        if (b is WorkBuilding) {
            bb = go.AddComponent(typeof(WorkBuildingBehaviour));
        } else if (b is HouseBuilding) {
            bb = go.AddComponent(typeof(HouseBuildingBehaviour));
        } else if (b is WarehouseBuilding) {
            bb = go.AddComponent(typeof(WarehouseBehaviour));
        } else if (b is CarrierOfficeBuilding) {
            bb = go.AddComponent(typeof(CarrierOfficeBehaviour));
        }
        ((BuildingBehaviour)bb).SetTransportListener(this);
        ((BuildingBehaviour)bb).SetData(b);
        return go;

    }

    /// <summary>
    /// Set up a new runtime copy of RoadPrefab with a particular data and position.
    /// </summary>
    /// <param name="r">The road data from a scriptable object</param>
    /// <param name="startPoint">The road start point</param>
    /// <param name="endPoint">The Road end point</param>
    /// <returns>The instantiated road GameObject</returns>
    private GameObject InstantiateNewRoad(Road r, Vector3 startPoint, Vector3 endPoint) {

        GameObject go = Instantiate(roadPrefab);
        go.name = gameRoads.Count.ToString() + "_" + r.GetName();
        go.transform.SetParent(navMeshGO.transform);
        go.GetComponent<RoadBuilder>().Build(startPoint, endPoint, r);
        return go;

    }

    private GameObject SearchForAvailableVehicle() {
        GameObject vehicle;
        vehicle = vehiclesPool.GetPooledObject();
        if (vehicle == null) {
            List<GameObject> actives = vehiclesPool.GetActiveObjects();
            for (int i = 0; i < actives.Count; i++) {
                if (!actives[i].GetComponent<VehicleBehaviour>().IsBusy()) {
                    return actives[i];
                }
            }
            return null;
        } else {
            return vehicle;
        }
    }

    public void OnExportNeeded(BuildingBehaviour dest) {

        GameObject vehicle = SearchForAvailableVehicle();
        if (vehicle != null) {
            BuildingBehaviour[] dests = new BuildingBehaviour[3];
            dests[0] = dest;
            dests[1] = FindClosestWarehosue(dests[0].roadAttachPoint);
            dests[2] = FindClosestCarrierOffice(dests[1].roadAttachPoint);
            vehicle.GetComponent<VehicleBehaviour>().SetDestinations(dests, 2);
        }

    }

    public void OnImportNeeded(BuildingBehaviour dest) {

        GameObject vehicle = SearchForAvailableVehicle();
        if (vehicle != null) {
            BuildingBehaviour[] dests = new BuildingBehaviour[3];
            dests[0] = FindClosestWarehosue(vehicle.transform.position);
            dests[1] = dest;
            dests[2] = FindClosestCarrierOffice(dest.roadAttachPoint);
            vehicle.GetComponent<VehicleBehaviour>().SetDestinations(dests, 2);
        }

    }

    public void SaveVehicleOnCarrierOffice(VehicleBehaviour vehicle, CarrierOfficeBehaviour carrierOffice) {

        vehicle.ResetVehicle();
        carrierOffice.StoreVehicle(vehicle);

    }

    /*private void FindClosestBuildingOfType(Type t) {
        
    }*/

    private WarehouseBehaviour FindClosestWarehosue(Vector3 startPosition) {
        /*NavMeshPath shortestPath = new NavMeshPath();
        NavMeshPath auxPath = new NavMeshPath();
        for (int i = 0; i < gameWarehouses.Count; i++) {
            NavMesh.CalculatePath(startPosition, gameWarehouses[i].roadAttachPoint, NavMesh.AllAreas, auxPath);
            if () {
                //compare the paths by sum(distances between corners)
            }
        }*/

        WarehouseBehaviour closest = gameWarehouses[0];
        float aux = Vector3.Distance(startPosition, gameWarehouses[0].roadAttachPoint);
        float minDistance = aux;
        for (int i = 1; i < gameWarehouses.Count; i++) {
            aux = Vector3.Distance(startPosition, gameWarehouses[i].roadAttachPoint);
            if (aux < minDistance) {
                closest = gameWarehouses[i];
                minDistance = aux;
            }
        }
        return closest;

    }

    private Resources GetTotalResources() {
        Resources result = new Resources(new int[System.Enum.GetValues(typeof(IResources)).Length]);
        for (int i = 0; i < gameWarehouses.Count; i++) {
            Debug.Log("Getting resources from warehouse " + i + ". Actual stock: " + gameWarehouses[i].actualStock.ToString());
            result = Resources.Sum(result, gameWarehouses[i].actualStock);
        }
        return result;
    }

    private CarrierOfficeBehaviour FindClosestCarrierOffice(Vector3 startPosition) {

        CarrierOfficeBehaviour closest = gameCarrierOffices[0];
        float aux = Vector3.Distance(startPosition, gameCarrierOffices[0].roadAttachPoint);
        float minDistance = aux;
        for (int i = 1; i < gameWarehouses.Count; i++) {
            aux = Vector3.Distance(startPosition, gameCarrierOffices[i].roadAttachPoint);
            if (aux < minDistance) {
                closest = gameCarrierOffices[i];
                minDistance = aux;
            }
        }
        return closest;

    }

    /// <summary>
    /// Get the gold's icon
    /// </summary>
    /// <returns>The gold icon</returns>
    public Sprite getGoldIcon() {
        return resourcesIcons[(int)IResources.GOLD];
    }
    /// <summary>
    /// Get the wood's icon
    /// </summary>
    /// <returns>The wood icon</returns>
    public Sprite getWoodIcon() {
        return resourcesIcons[(int)IResources.WOOD];
    }
    /// <summary>
    /// Get the iron's icon
    /// </summary>
    /// <returns>The iron icon</returns>
    public Sprite getIronIcon() {
        return resourcesIcons[(int)IResources.IRON];
    }
    /// <summary>
    /// Get the food's icon
    /// </summary>
    /// <returns>The food icon</returns>
    public Sprite getFoodIcon() {
        return resourcesIcons[(int)IResources.FOOD];
    }
    //implementar getResourceIcon(resource) para sustituir los 4 metodos anteriores y permitir escalar a mas tipos de recursos

    public GameObject InstantiateObject(GameObject original) {
        return Instantiate(original);
    }

}
