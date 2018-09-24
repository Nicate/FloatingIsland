using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour {
	[Header("Fluff")]
	public GameObject fluffPrefab;

	public int numberOfFluffs;
	
	public float interval;

	[Header("Size")]
	public Vector3 size;

	public float scale;

	[Header("Wool")]
	public float minimumDistance;
	public float maximumDistance;

	public float minimumThickness;
	public float maximumThickness;

	public float minimumWoollyness;
	public float maximumWoollyness;

	public Vector3 distanceAttenuation;
	public Vector3 thicknessAttenuation;
	public Vector3 woollynessAttenuation;


	private void Start() {
		List<Vector3> positions = new List<Vector3>();

		for(int index = 0; index < numberOfFluffs; index += 1) {
			Vector3 normal = Random.onUnitSphere;

			Vector3 alignment = calculateAlignment(normal);
			
			float distance = calculateAttenuation(alignment, distanceAttenuation, minimumDistance, maximumDistance);
			
			Vector3 position = transform.position + Vector3.Scale(normal, size) * distance * scale;

			bool outsideIntervals = true;
			foreach(Vector3 otherPosition in positions) {
				if(Vector3.Distance(position, otherPosition) < interval * scale) {
					outsideIntervals = false;
				}
			}

			if(outsideIntervals) {
				Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);

				GameObject fluff = Instantiate(fluffPrefab, position, rotation, transform);
				fluff.name = "Fluff " + index;

				float thickness = calculateAttenuation(alignment, thicknessAttenuation, minimumThickness, maximumThickness);
				float woollyness = calculateAttenuation(alignment, woollynessAttenuation, minimumWoollyness, maximumWoollyness);

				Vector3 localScale = fluff.transform.localScale;
				localScale *= thickness * scale;
				localScale.y *= woollyness;
				fluff.transform.localScale = localScale;

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
