using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour {
	[Header("Sheep")]
	public float size;

	[Header("Body")]
	public GameObject bodyPrefab;
	
	public Vector3 bodySize;
	public Vector3 bodyLocation;
	public Vector3 bodyRotation;

	[Header("Head")]
	public GameObject headPrefab;
	
	public Vector3 headSize;
	public Vector3 headLocation;
	public Vector3 headRotation;

	[Header("Leg")]
	public GameObject legPrefab;
	
	public Vector3 legSize;
	public Vector3 legLocation;
	public Vector3 legRotation;

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

		createHead();

		createLeg(true, true);
		createLeg(true, false);
		createLeg(false, false);
		createLeg(false, true);

		createFluffs();
	}


	private void createBody() {
		Vector3 bodyPosition = Vector3.Scale(bodySize * size * minimumDistance, bodyLocation);

		Vector3 position = transform.position + bodyPosition;

		Quaternion rotation = Quaternion.Euler(bodyRotation);

		GameObject body = Instantiate(bodyPrefab, position, rotation, transform);
		body.name = "Body";

		body.transform.localScale = bodySize * size * minimumDistance;
	}

	private void createHead() {
		Vector3 headPosition = Vector3.Scale(bodySize * size * minimumDistance, headLocation);
		
		Vector3 position = transform.position + headPosition;

		Quaternion rotation = Quaternion.Euler(headRotation);

		GameObject head = Instantiate(headPrefab, position, rotation, transform);
		head.name = "Head";

		head.transform.localScale = headSize * size;
	}

	private void createLeg(bool right, bool front) {
		Vector3 legPosition = Vector3.Scale(bodySize * size * minimumDistance, legLocation);
		
		legPosition.x *= right ? 1.0f : -1.0f;
		legPosition.z *= front ? 1.0f : -1.0f;
		
		Vector3 position = transform.position + legPosition;

		Vector3 thisLegRotation = legRotation;

		// First flip the rotation along the X axis, then along the Y axis.
		thisLegRotation.y = right ? thisLegRotation.y : 0.0f - thisLegRotation.y;
		thisLegRotation.y = front ? thisLegRotation.y : 180.0f - thisLegRotation.y;

		Quaternion rotation = Quaternion.Euler(thisLegRotation);

		GameObject leg = Instantiate(legPrefab, position, rotation, transform);
		
		string rightOrLeftName = right ? "Right" : "Left";
		string frontOrBackName = front ? "Front" : "Back";

		leg.name = frontOrBackName + " " + rightOrLeftName + " Leg";
		
		leg.transform.localScale = legSize * size;
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
