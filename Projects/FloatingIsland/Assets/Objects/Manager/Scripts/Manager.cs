using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour {
	public Factory factoryPrefab;
	public Silo siloPrefab;
	public Store storePrefab;

	public Puff puffPrefab;
	
	public Water water;
	public Island island;

	public float startingCapital = 0.0f;

	public float factoryPrice = 0.0f;
	public float siloPrice = 0.0f;
	public float storePrice = 0.0f;
	
	public float maximumHeight = 0.0f;
	public float maximumSlope = 0.0f;

	public float heightPenalty = 1.0f;
	public float slopePenalty = 1.0f;

	public float factoryWeight = 0.0f;
	public float siloWeight = 0.0f;
	public float storeWeight = 0.0f;

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

	public Color activeToolColor;
	public Color passiveToolColor;

	public float rayCastDistance = 500.0f;

	public float toolBuildingFade = 0.0f;


	private List<Factory> factories;
	private List<Silo> silos;
	private List<Store> stores;

	private enum Tool {
		Factory,
		Silo,
		Store,
		None
	}

	private Tool selectedTool;

	private Factory toolFactory;
	private Silo toolSilo;
	private Store toolStore;


	private float capital;

	private float pollution;
	private float weight;


	private void Start() {
		factories = new List<Factory>();
		silos = new List<Silo>();
		stores = new List<Store>();

		selectedTool = Tool.Factory;

		capital = 0.0f;
		pollution = 0.0f;
		weight = 0.0f;

		funnel(startingCapital);

		factoryText.color = activeToolColor;
		siloText.color = passiveToolColor;
		storeText.color = passiveToolColor;
	}
	

	private void Update() {
		foreach(Factory factory in factories.ToArray()) {
			if(factory.transform.position.y < water.getLevel()) {
				factory.evacuate();

				factories.Remove(factory);
			}
		}

		foreach(Silo silo in silos.ToArray()) {
			if(silo.transform.position.y < water.getLevel()) {
				silo.evacuate();

				silos.Remove(silo);
			}
		}

		foreach(Store store in stores.ToArray()) {
			if(store.transform.position.y < water.getLevel()) {
				store.evacuate();

				stores.Remove(store);
			}
		}

		foreach(Factory factory in factories) {
			factory.run(this);
		}
		
		foreach(Silo silo in silos) {
			silo.run(this);
		}
		
		foreach(Store store in stores) {
			store.run(this);
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Z)) {
			selectTool(Tool.Factory);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.C)) {
			selectTool(Tool.Silo);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.X)) {
			selectTool(Tool.Store);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.V)) {
			selectTool(Tool.None);
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, rayCastDistance, LayerMask.GetMask("Hexagons"))) {
			Transform transform = hit.collider.transform;
				
			if(transform.position.y > water.getLevel() && transform.childCount == 0) {
				float angle = Random.Range(0.0f, 360.0f);
				Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

				if(selectedTool == Tool.Factory) {
					if(toolFactory == null) {
						toolFactory = Instantiate(factoryPrefab, transform.position, rotation, transform);

						toolFactory.usePreviewShader();
						toolFactory.setFade(toolBuildingFade);

						toolFactory.enableRadiators();
					}
					else {
						toolFactory.transform.SetParent(transform, false);
					}
				}
				else if(selectedTool == Tool.Silo) {
					if(toolSilo == null) {
						toolSilo = Instantiate(siloPrefab, transform.position, rotation, transform);
						
						toolSilo.usePreviewShader();
						toolSilo.setFade(toolBuildingFade);

						toolSilo.enableRadiators();
					}
					else {
						toolSilo.transform.SetParent(transform, false);
					}
				}
				else if(selectedTool == Tool.Store) {
					if(toolStore == null) {
						toolStore = Instantiate(storePrefab, transform.position, rotation, transform);
						
						toolStore.usePreviewShader();
						toolStore.setFade(toolBuildingFade);

						toolStore.enableRadiators();
					}
					else {
						toolStore.transform.SetParent(transform, false);
					}
				}
			}
		}

		if(Input.GetMouseButtonDown(0)) {
			if(toolFactory != null && capital >= factoryPrice) {
				Hexagon hexagon = hit.collider.transform.GetComponent<Hexagon>();
				hexagon.enableBasePlate();

				Factory factory = Instantiate(factoryPrefab, toolFactory.transform.position, toolFactory.transform.rotation, toolFactory.transform.parent);

				factories.Add(factory);

				weight += factoryWeight;

				channel(factoryPrice);

				Instantiate(puffPrefab, toolFactory.transform.position, Quaternion.identity);
			}
			else if(toolSilo != null && capital >= siloPrice) {
				Hexagon hexagon = hit.collider.transform.GetComponent<Hexagon>();
				hexagon.enableBasePlate();

				Silo silo = Instantiate(siloPrefab, toolSilo.transform.position, toolSilo.transform.rotation, toolSilo.transform.parent);

				silos.Add(silo);

				weight += siloWeight;

				channel(siloPrice);

				Instantiate(puffPrefab, toolSilo.transform.position, Quaternion.identity);
			}
			else if(toolStore != null && capital >= storePrice) {
				Hexagon hexagon = hit.collider.transform.GetComponent<Hexagon>();
				hexagon.enableBasePlate();

				Store store = Instantiate(storePrefab, toolStore.transform.position, toolStore.transform.rotation, toolStore.transform.parent);

				stores.Add(store);

				weight += storeWeight;

				channel(storePrice);

				Instantiate(puffPrefab, toolStore.transform.position, Quaternion.identity);
			}
		}

		water.rise(pollution * pollutionFactor);

		float smog = Mathf.Clamp01(pollution * fogFactor);

		Color color = fogColor.Evaluate(smog);
		RenderSettings.fogColor = color;
		Camera.main.backgroundColor = color;

		RenderSettings.fogDensity = Mathf.Lerp(minimumDensity, maximumDensity, smog);

		island.sink(weight * -weightFactor);
	}


	private void selectTool(Tool tool) {
		if(tool != Tool.Factory && selectedTool == Tool.Factory) {
			factoryText.color = passiveToolColor;
			
			Destroy(toolFactory.gameObject);
			toolFactory = null;
		}

		if(tool != Tool.Silo && selectedTool == Tool.Silo) {
			siloText.color = passiveToolColor;

			Destroy(toolSilo.gameObject);
			toolSilo = null;
		}

		if(tool != Tool.Store && selectedTool == Tool.Store) {
			storeText.color = passiveToolColor;

			Destroy(toolStore.gameObject);
			toolStore = null;
		}

		if(tool == Tool.Factory && selectedTool != Tool.Factory) {
			selectedTool = Tool.Factory;

			factoryText.color = activeToolColor;
		}
		else if(tool == Tool.Silo && selectedTool != Tool.Silo) {
			selectedTool = Tool.Silo;
			
			siloText.color = activeToolColor;
		}
		else if(tool == Tool.Store && selectedTool != Tool.Store) {
			selectedTool = Tool.Store;
			
			storeText.color = activeToolColor;
		}
	}


	public Factory[] getFactories() {
		return factories.ToArray();
	}
	
	public Silo[] getSilos() {
		return silos.ToArray();
	}
	
	public Store[] getStores() {
		return stores.ToArray();
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
