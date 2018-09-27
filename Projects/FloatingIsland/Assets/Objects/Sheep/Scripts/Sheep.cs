using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour {
	[Header("Sheep")]
	public float size;

	[Header("Head")]
	public GameObject headPrefab;
	
	public Vector3 headSize;
	public Vector3 headLocation;

	[Header("Body")]
	public GameObject bodyPrefab;
	
	public Vector3 bodySize;

	[Header("Legs")]
	public GameObject legPrefab;
	
	public Vector3 legSize;
	public Vector3 legLocation;

	[Header("Fluff")]
	public GameObject fluffPrefab;
	
	public Vector3 fluffSize;
	
	[Space]
	public int numberOfFluffs;
	
	public float interval;
	
	[Space]
	public float minimumDistance;
	public float maximumDistance;

	public Vector3 distanceAttenuation;
	
	[Space]
	public float minimumThickness;
	public float maximumThickness;

	public Vector3 thicknessAttenuation;
	
	[Space]
	public float minimumWoollyness;
	public float maximumWoollyness;

	public Vector3 woollynessAttenuation;


	private void Start() {
		createBody();

		createLeg(true, true);
		createLeg(true, false);
		createLeg(false, false);
		createLeg(false, true);

		createHead();

		createFluffs();
	}


	private void createBody() {
		Vector3 position = transform.position;

		GameObject body = Instantiate(bodyPrefab, position, Quaternion.identity, transform);
		body.name = "Body";

		body.transform.localScale = bodySize * size * minimumDistance;
	}

	private void createLeg(bool front, bool right) {
		Vector3 legPosition = Vector3.Scale(bodySize * size * minimumDistance, legLocation);

		legPosition.x *= front ? 1.0f : -1.0f;
		legPosition.z *= right ? 1.0f : -1.0f;
		
		Vector3 position = transform.position + legPosition;

		GameObject leg = Instantiate(legPrefab, position, Quaternion.identity, transform);

		string frontOrBackName = front ? "Front" : "Back";
		string rightOrLeftName = right ? "Right" : "Left";

		leg.name = frontOrBackName + " " + rightOrLeftName + " Leg";

		leg.transform.localScale = legSize * size;
	}

	private void createHead() {
		Vector3 headPosition = Vector3.Scale(bodySize * size * minimumDistance, headLocation);
		
		Vector3 position = transform.position + headPosition;

		GameObject head = Instantiate(headPrefab, position, Quaternion.identity, transform);
		head.name = "Head";

		head.transform.localScale = headSize * size;
	}

	private void createFluffs() {
		List<Vector3> positions = new List<Vector3>();

		for(int index = 0; index < numberOfFluffs; index += 1) {
			Vector3 normal = Random.onUnitSphere;

			Vector3 alignment = calculateAlignment(normal);
			
			float distance = calculateAttenuation(alignment, distanceAttenuation, minimumDistance, maximumDistance);
			
			Vector3 position = transform.position + Vector3.Scale(normal, bodySize * size * distance);

			bool outsideIntervals = true;
			foreach(Vector3 otherPosition in positions) {
				if(Vector3.Distance(position, otherPosition) < interval * size) {
					outsideIntervals = false;
				}
			}

			if(outsideIntervals) {
				Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);

				GameObject fluff = Instantiate(fluffPrefab, position, rotation, transform);
				fluff.name = "Fluff " + index;

				float thickness = calculateAttenuation(alignment, thicknessAttenuation, minimumThickness, maximumThickness);
				float woollyness = calculateAttenuation(alignment, woollynessAttenuation, minimumWoollyness, maximumWoollyness);

				Vector3 scale = fluffSize * size * thickness;
				scale.y *= woollyness;
				fluff.transform.localScale = scale;

				positions.Add(position);
			}
		}
	}


	private Vector3 calculateAlignment(Vector3 normal) {
		float alignmentX = Mathf.Abs(Vector3.Dot(normal, Vector3.right));
		float alignmentY = Mathf.Abs(Vector3.Dot(normal, Vector3.up));
		float alignmentZ = Mathf.Abs(Vector3.Dot(normal, Vector3.forward));

		return new Vector3(alignmentX, alignmentY, alignmentZ);
	}

	private float calculateAttenuation(Vector3 alignment, Vector3 attenuation, float minimum, float maximum) {
		Vector3 extension = Vector3.one - Vector3.Scale(alignment, attenuation);
		
		float attenuatedExtension = extension.x * extension.y * extension.z;
		float attenuatedMaximum = Mathf.Lerp(minimum, maximum, attenuatedExtension);

		return Random.Range(minimum, attenuatedMaximum);
	}
}
