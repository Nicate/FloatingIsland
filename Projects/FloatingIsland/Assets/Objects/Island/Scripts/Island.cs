using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour {
	[Header("Subdivision")]
	public float height = 1.0f;
	
	[Space]
	public float centerVertexMean = 0.0f;
	public float centerVertexDeviation = 1.0f;
	
	[Space]
	public float edgeVertexMean = 0.0f;
	public float edgeVertexDeviation = 1.0f;
	
	[Space]
	public float innerVertexMean = 0.0f;
	public float innerVertexDeviation = 1.0f;

	[Space]
	public float damping = 1.0f;
	public float erosion = 0.0f;
	public float basis = 0.0f;

	[Space]
	public float[] attenuations = new float[0];

	[Header("Population")]
	public GameObject hexagonPrefab;
	
	[Space]
	public Material rockMaterial;
	public Material sandMaterial;
	public Material grassMaterial;
	public Material snowMaterial;
	
	[Space]
	public float minimumRockHeight = -4.0f;
	public float minimumSandHeight = 0.0f;
	public float minimumGrassHeight = 4.0f;
	public float minimumSnowHeight = 8.0f;
	
	[Space]
	public float minimumRockAngle = 90.0f;

	[Header("Gameplay")]
	public float sinkSpeed = 0.2f;


	private float targetLevel;


	private Stochast random;


	private struct Triangle {
		public int a;
		public int b;
		public int c;

		public Triangle(int a, int b, int c) {
			this.a = a;
			this.b = b;
			this.c = c;
		}
	}
	
	private List<Triangle> triangles;
	private List<Vector3> vertices;
	private List<Vector3> normals;


	private struct Edge {
		public int a;
		public int b;

		public Edge(int a, int b) {
			this.a = a;
			this.b = b;
		}

		public override bool Equals(object obj) {
			if(obj is Edge) {
				Edge pair = (Edge) obj;

				return a == pair.a && b == pair.b;
			}
			else {
				return false;
			}
		}

		public override int GetHashCode() {
			int hashCode = 2118541809;

			hashCode = hashCode * -1521134295 + a.GetHashCode();
			hashCode = hashCode * -1521134295 + b.GetHashCode();

			return hashCode;
		}
	}

	private Dictionary<Edge, int> subdividedEdges;
	

	void Start() {
		targetLevel = transform.position.y;

		random = new Stochast();

		initialize();
		generate();
	}


	private void initialize() {
		triangles = new List<Triangle>();
		vertices = new List<Vector3>();
		normals = new List<Vector3>();
		
		subdividedEdges = new Dictionary<Edge, int>();
	}


	private void generate() {
		createHexagon();
		subdivideHexagon();
		postProcessVertices();
		calculateNormals();
		createIsland();
	}


	private void createHexagon() {
		float radius = Mathf.Pow(2.0f, attenuations.Length - 1);

		vertices.Add(Vector3.zero);

		for(int index = 0; index < 6; index += 1) {
			// Clockwise because we do left handed cross products somewhere.
			float angle = -index / 3.0f * Mathf.PI;

			vertices.Add(new Vector3(Mathf.Cos(angle) * radius, 0.0f, Mathf.Sin(angle) * radius));
		}

		triangles.Add(new Triangle(0, 1, 2));
		triangles.Add(new Triangle(2, 3, 0));
		triangles.Add(new Triangle(4, 0, 3));
		triangles.Add(new Triangle(0, 4, 5));
		triangles.Add(new Triangle(5, 6, 0));
		triangles.Add(new Triangle(1, 0, 6));

		perturbVertex(0, 0, centerVertexMean, centerVertexDeviation, false);
		perturbVertex(1, 0, edgeVertexMean, edgeVertexDeviation, false);
		perturbVertex(2, 0, edgeVertexMean, edgeVertexDeviation, false);
		perturbVertex(3, 0, edgeVertexMean, edgeVertexDeviation, false);
		perturbVertex(4, 0, edgeVertexMean, edgeVertexDeviation, false);
		perturbVertex(5, 0, edgeVertexMean, edgeVertexDeviation, false);
		perturbVertex(6, 0, edgeVertexMean, edgeVertexDeviation, false);
	}


	private void subdivideHexagon() {
		for(int subdivision = 1; subdivision < attenuations.Length; subdivision += 1) {
			int numberOfTriangles = triangles.Count;
			
			for(int index = 0; index < numberOfTriangles; index += 1) {
				subdivideTriangle(index, subdivision);
			}
		}
	}

	private void subdivideTriangle(int index, int subdivision) {
		Triangle triangle = triangles[index];

		int a = triangle.a;
		int b = triangle.b;
		int c = triangle.c;
		
		int f = subdivideEdge(a, b, subdivision);
		int d = subdivideEdge(b, c, subdivision);
		int e = subdivideEdge(c, a, subdivision);

		// Remove the original triangle but don't mess up the list indices.
		triangles[index] = new Triangle(a, f, e);

		triangles.Add(new Triangle(f, b, d));
		triangles.Add(new Triangle(e, d, c));
		triangles.Add(new Triangle(d, e, f));
	}

	private int subdivideEdge(int a, int b, int subdivision) {
		Edge edge1 = new Edge(a, b);
		Edge edge2 = new Edge(b, a);

		if(subdividedEdges.ContainsKey(edge1)) {
			int f = subdividedEdges[edge1];

			subdividedEdges.Remove(edge1);
			subdividedEdges.Remove(edge2);

			return f;
		}
		else {
			int f = vertices.Count;

			vertices.Add(Vector3.Lerp(vertices[a], vertices[b], 0.5f));
			
			perturbVertex(f, subdivision, innerVertexMean, innerVertexDeviation, true);

			subdividedEdges[edge1] = f;
			subdividedEdges[edge2] = f;

			return f;
		}
	}

	private void perturbVertex(int f, int subdivision, float mean, float deviation, bool dampen) {
		Vector3 vertex = vertices[f];
		
		Vector3 projection = vertex;
		projection.y = 0;
		
		float radius = projection.magnitude;

		float minimumRadius = 0.0f;
		float maximumRadius = Mathf.Pow(2.0f, attenuations.Length - 1);

		float t = (radius - minimumRadius) / (maximumRadius - minimumRadius);

		float dampening = 1.0f - Mathf.Pow(t, 1.0f / damping);

		if(!dampen) {
			dampening = 1.0f;
		}

		vertex.y += random.nextGaussian(mean, deviation) * attenuations[subdivision] * height * dampening;

		vertices[f] = vertex;
	}


	private void postProcessVertices() {
		float minimumHeight = float.MaxValue;
		float maximumHeight = float.MinValue;

		for(int a = 0; a < vertices.Count; a += 1) {
			Vector3 vertex = vertices[a];

			if(vertex.y < minimumHeight) {
				minimumHeight = vertex.y;
			}

			if(vertex.y > maximumHeight) {
				maximumHeight = vertex.y;
			}
		}

		for(int a = 0; a < vertices.Count; a += 1) {
			Vector3 vertex = vertices[a];
			
			float height = (vertex.y - basis) / (maximumHeight - basis);

			if(height > 0.0f) {
				height = Mathf.Exp(-erosion) * height / Mathf.Exp(-erosion * height);

				vertex.y = basis + height * (maximumHeight - basis);

				vertices[a] = vertex;
			}
			else {

			}
		}
	}
	

	private void calculateNormals() {
		for(int a = 0; a < vertices.Count; a += 1) {
			Vector3 normal = Vector3.zero;

			int numberOfTriangles = 0;
		
			for(int index = 0; index < triangles.Count; index += 1) {
				Triangle triangle = triangles[index];

				if(triangle.a == a || triangle.b == a || triangle.c == a) {
					Vector3 vertexA = vertices[triangle.a];
					Vector3 vertexB = vertices[triangle.b];
					Vector3 vertexC = vertices[triangle.c];
					
					normal += Vector3.Cross(vertexB - vertexA, vertexC - vertexA).normalized;

					numberOfTriangles += 1;
				}
			}

			normals.Add(Vector3.Normalize(normal / numberOfTriangles));
		}
	}


	private void createIsland() {
		for(int a = 0; a < vertices.Count; a += 1) {
			Vector3 vertex = vertices[a];
			Vector3 normal = normals[a];

			float height = vertex.y;
			float angle = Mathf.Acos(normal.y) * Mathf.Rad2Deg;

			if(height > minimumRockHeight) {
				Material material = rockMaterial;

				if(height > minimumSnowHeight) {
					material = snowMaterial;
				}
				else if(height > minimumGrassHeight) {
					material = grassMaterial;
				}
				else if(height > minimumSandHeight) {
					material = sandMaterial;
				}

				if(angle > minimumRockAngle) {
					material = rockMaterial;
				}

				GameObject hexagon = Instantiate<GameObject>(hexagonPrefab, vertex, Quaternion.identity, transform);
				hexagon.GetComponent<MeshRenderer>().material = material;
				hexagon.name = "Hexagon " + a;
			}
		}
	}
	

	private void Update() {
		float delta = sinkSpeed * Time.deltaTime;
		
		Vector3 position = transform.position;
		
		float level = position.y;

		// We can only go down.
		if(level > targetLevel) {
			level -= delta;

			if(level < targetLevel) {
				level = targetLevel;
			}
		}

		position.y = level;

		transform.position = position;
	}


	public void sink(float level) {
		targetLevel = level;
	}
}
