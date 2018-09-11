using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour {
	public Factory factoryPrefab;
	public Store storePrefab;
	public Silo siloPrefab;

	public Puff puffPrefab;
	
	public Water water;
	public Island island;

	public float startingCapital = 0.0f;

	public float factoryPrice = 0.0f;
	public float storePrice = 0.0f;
	public float siloPrice = 0.0f;
	
	public float maximumHeight = 0.0f;
	public float maximumSlope = 0.0f;

	public float heightPenalty = 1.0f;
	public float slopePenalty = 1.0f;

	public float factoryWeight = 0.0f;
	public float storeWeight = 0.0f;
	public float siloWeight = 0.0f;

	public float weightFactor = 1.0f;
	public float pollutionFactor = 1.0f;
	
	public float fogFactor = 1.0f;
	public Gradient fogColor;

	public float minimumDensity = 0.01f;
	public float maximumDensity = 0.05f;

	public Text capitalText;

	public Text factoryText;
	public Text storeText;
	public Text siloText;

	public Color activeToolColor = Color.white;
	public Color passiveToolColor = Color.white;

	public float rayCastDistance = 500.0f;

	public float selectedBuildingFade = 0.0f;
	public Color selectedBuildingBuildableColor = Color.white;
	public float selectedBuildingBuildableEffect = 0.0f;
	public Color selectedBuildingUnbuildableColor = Color.white;
	public float selectedBuildingUnbuildableEffect = 0.0f;
	
	private List<Factory> factories;
	private List<Store> stores;
	private List<Silo> silos;

	private enum Tool {
		Factory,
		Silo,
		Store,
		None
	}

	private Tool selectedTool;
	private Building selectedBuilding;

	private float capital;

	private float pollution;
	private float weight;


	private void Start() {
		factories = new List<Factory>();
		stores = new List<Store>();
		silos = new List<Silo>();

		selectedTool = Tool.None;
		selectedBuilding = null;

		capital = 0.0f;
		pollution = 0.0f;
		weight = 0.0f;

		funnel(startingCapital);

		factoryText.color = passiveToolColor;
		storeText.color = passiveToolColor;
		siloText.color = passiveToolColor;
	}
	

	private void Update() {
		foreach(Factory factory in factories.ToArray()) {
			if(factory.isFlooded(island, water)) {
				factory.evacuate();

				factories.Remove(factory);
			}
		}

		foreach(Store store in stores.ToArray()) {
			if(store.isFlooded(island, water)) {
				store.evacuate();

				stores.Remove(store);
			}
		}

		foreach(Silo silo in silos.ToArray()) {
			if(silo.isFlooded(island, water)) {
				silo.evacuate();

				silos.Remove(silo);
			}
		}


		foreach(Factory factory in factories) {
			factory.run(this);
		}
		
		foreach(Store store in stores) {
			store.run(this);
		}
		
		foreach(Silo silo in silos) {
			silo.run(this);
		}

		
		if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Z)) {
			selectTool(Tool.Factory);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.X)) {
			selectTool(Tool.Store);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.C)) {
			selectTool(Tool.Silo);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.V)) {
			selectTool(Tool.None);
		}


		Hexagon hexagon = null;
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, rayCastDistance, LayerMask.GetMask("Hexagons"))) {
			hexagon = hit.transform.GetComponent<Hexagon>();
		}

		
		if(selectedTool == Tool.None) {
			if(hexagon != null && !hexagon.isFlooded(island, water) && hexagon.isOccupied()) {
				// TODO Do the info stuff here.
			}
		}
		else {
			if(hexagon != null && !hexagon.isFlooded(island, water)) {
				selectedBuilding.transform.SetParent(hexagon.transform, false);
				selectedBuilding.gameObject.SetActive(true);
				
				bool unaffordableFactory = selectedTool == Tool.Factory && capital < factoryPrice;
				bool unaffordableStore = selectedTool == Tool.Store && capital < storePrice;
				bool unaffordableSilo = selectedTool == Tool.Silo && capital < siloPrice;
				bool occupied = hexagon.isOccupied();

				if(unaffordableFactory || unaffordableSilo || unaffordableStore || occupied) {
					selectedBuilding.setColor(selectedBuildingUnbuildableColor);
					selectedBuilding.setEffect(selectedBuildingUnbuildableEffect);
				}
				else {
					selectedBuilding.setColor(selectedBuildingBuildableColor);
					selectedBuilding.setEffect(selectedBuildingBuildableEffect);
				}
			}
			else {
				selectedBuilding.transform.SetParent(null, false);
				selectedBuilding.gameObject.SetActive(false);
			}
		}


		bool impact = false;
		float addedWeight = 0.0f;

		if(Input.GetMouseButtonDown(0) && selectedTool != Tool.None && hexagon != null && !hexagon.isFlooded(island, water) && !hexagon.isOccupied()) {
			if(selectedTool == Tool.Factory && capital >= factoryPrice) {
				Factory factory = build(factoryPrefab, hexagon, factoryPrice);

				factories.Add(factory);
				
				impact = true;
				addedWeight = factoryWeight;
			}
			else if(selectedTool == Tool.Store && capital >= storePrice) {
				Store store = build(storePrefab, hexagon, storePrice);
				
				stores.Add(store);
				
				impact = true;
				addedWeight = storeWeight;
			}
			else if(selectedTool == Tool.Silo && capital >= siloPrice) {
				Silo silo = build(siloPrefab, hexagon, siloPrice);
				
				silos.Add(silo);
				
				impact = true;
				addedWeight = siloWeight;
			}
		}


		water.setLevel(pollution * pollutionFactor);


		float smog = Mathf.Clamp01(pollution * fogFactor);

		Color color = fogColor.Evaluate(smog);
		RenderSettings.fogColor = color;
		Camera.main.backgroundColor = color;

		RenderSettings.fogDensity = Mathf.Lerp(minimumDensity, maximumDensity, smog);
		

		if(impact) {
			weight += addedWeight;

			island.setLevel(weight * -weightFactor);
			island.impact(addedWeight * -weightFactor);
		}
	}


	private void selectTool(Tool tool) {
		if(tool != Tool.Factory && selectedTool == Tool.Factory) {
			factoryText.color = passiveToolColor;
			
			disableSelectedTool();
		}

		if(tool != Tool.Store && selectedTool == Tool.Store) {
			storeText.color = passiveToolColor;
			
			disableSelectedTool();
		}

		if(tool != Tool.Silo && selectedTool == Tool.Silo) {
			siloText.color = passiveToolColor;
			
			disableSelectedTool();
		}

		if(tool == Tool.Factory && selectedTool != Tool.Factory) {
			selectedTool = Tool.Factory;

			factoryText.color = activeToolColor;

			enableSelectedTool(factoryPrefab);
		}

		if(tool == Tool.Store && selectedTool != Tool.Store) {
			selectedTool = Tool.Store;
			
			storeText.color = activeToolColor;

			enableSelectedTool(storePrefab);
		}
		
		if(tool == Tool.Silo && selectedTool != Tool.Silo) {
			selectedTool = Tool.Silo;
			
			siloText.color = activeToolColor;

			enableSelectedTool(siloPrefab);
		}

		if(tool == Tool.None && selectedTool != Tool.None) {
			selectedTool = Tool.None;
		}
	}

	private void enableSelectedTool(Building prefab) {
		Vector3 position = Vector3.zero;

		float angle = Random.Range(0.0f, 360.0f);
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

		selectedBuilding = Instantiate(prefab, position, rotation);
		selectedBuilding.name = "Selected " + prefab.name;

		selectedBuilding.usePreviewShader();
		selectedBuilding.setFade(selectedBuildingFade);

		selectedBuilding.enableRadiators();

		selectedBuilding.gameObject.SetActive(false);
	}

	private void disableSelectedTool() {
		Destroy(selectedBuilding.gameObject);

		selectedBuilding = null;
	}


	private T build<T>(T prefab, Hexagon hexagon, float price) where T : Building {
		hexagon.enableBasePlate();

		T building = Instantiate(prefab, selectedBuilding.transform.position, selectedBuilding.transform.rotation, hexagon.transform);
		building.name = prefab.name + " " + hexagon.name.Split(' ')[1];

		hexagon.occupy(building);

		channel(price);

		Instantiate(puffPrefab, selectedBuilding.transform.position, Quaternion.identity);

		selectTool(Tool.None);

		return building;
	}


	public Factory[] getFactories() {
		return factories.ToArray();
	}
	
	public Store[] getStores() {
		return stores.ToArray();
	}
	
	public Silo[] getSilos() {
		return silos.ToArray();
	}


	public void funnel(float money) {
		capital += money;

		capitalText.text = "$" + capital.ToString("n0");
	}

	public void channel(float money) {
		capital -= money;

		capitalText.text = "$" + capital.ToString("n0");
	}


	public void pollute(float pollution) {
		this.pollution += pollution;
	}


	public void restart() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void exit() {
		Application.Quit();
	}
}
