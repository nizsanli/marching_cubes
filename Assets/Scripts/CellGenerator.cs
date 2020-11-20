using UnityEngine;
using System.Collections;
using SimplexNoise;
using System.Collections.Generic;

public class CellGenerator : MonoBehaviour {

	private static int chunkSize = 16;
	private int numValuePoints;

	private Mesher.GridCell[,,] cells;
	private float[,,] valuePoints;

	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private Mesh mesh;

	public Material chunkMaterial;

	private Mesher.GridCell generateCell(int x, int y, int z)
	{
		Mesher.GridCell cell = new Mesher.GridCell();

		Vector3 point = new Vector3(x, y, z+1);
		cell.cornerPoints[0] = point;
		point.x += 1;
		cell.cornerPoints[1] = point;
		point.z -= 1;
		cell.cornerPoints[2] = point;
		point.x -= 1;
		cell.cornerPoints[3] = point;

		point = new Vector3(x, y+1, z+1);
		cell.cornerPoints[4] = point;
		point.x += 1;
		cell.cornerPoints[5] = point;
		point.z -= 1;
		cell.cornerPoints[6] = point;
		point.x -= 1;
		cell.cornerPoints[7] = point;

		cell.cornerValues[0] = valuePoints[x, y, z+1];
		cell.cornerValues[1] = valuePoints[x+1, y, z+1];
		cell.cornerValues[2] = valuePoints[x+1, y, z];
		cell.cornerValues[3] = valuePoints[x, y, z];
		cell.cornerValues[4] = valuePoints[x, y+1, z+1];
		cell.cornerValues[5] = valuePoints[x+1, y+1, z+1];
		cell.cornerValues[6] = valuePoints[x+1, y+1, z];
		cell.cornerValues[7] = valuePoints[x, y+1, z];

		return cell;
	}

	// Use this for initialization
	void Start () {
		numValuePoints = chunkSize + 1;
		valuePoints = new float[numValuePoints, numValuePoints, numValuePoints];
		for (int y = 0; y < numValuePoints; y++)
		{
			for (int z = 0; z < numValuePoints; z++)
			{
				for (int x = 0; x < numValuePoints; x++)
				{
					Vector3 realPos = new Vector3(x+transform.position.x, y+transform.position.y, z+transform.position.z);
					valuePoints[x, y, z] = Noise.Generate(realPos.x*0.02f, realPos.y*0.02f, realPos.z*0.02f);
				}
			}
		}

		meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshFilter = gameObject.AddComponent<MeshFilter>();
		mesh = new Mesh();

		meshRenderer.sharedMaterial = chunkMaterial;

		int initialSize = chunkSize*5*3;
		List<Vector3> verts = new List<Vector3>(initialSize);
		List<int> indices = new List<int>(initialSize);
		List<Vector2> uvs = new List<Vector2>(initialSize);

		int numVerts = 0;
		cells = new Mesher.GridCell[chunkSize, chunkSize, chunkSize];
		for (int y = 0; y < chunkSize; y++)
		{
			for (int z = 0; z < chunkSize; z++)
			{
				for (int x = 0; x < chunkSize; x++)
				{
					Mesher.GridCell currCell = generateCell(x, y, z);

					Mesher.Triangle[] tris = new Mesher.Triangle[5];
					int numTris = Mesher.Polygonize(currCell, 0.1f, tris);

					for (int i = 0; i < numTris; i++)
					{
						verts.Add(tris[i].triPoints[0]);
						verts.Add(tris[i].triPoints[1]);
						verts.Add(tris[i].triPoints[2]);
						
						indices.Add(numVerts);
						indices.Add(numVerts+1);
						indices.Add(numVerts+2);
						
						numVerts += 3;
						
						// uvs dont matter right now
						uvs.Add(Vector2.zero);
						uvs.Add(Vector2.zero);
						uvs.Add(Vector2.zero);
					}
				}
			}
		}

		mesh.vertices = verts.ToArray();
		mesh.triangles = indices.ToArray();
		mesh.uv = uvs.ToArray();
		
		mesh.Optimize();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		
		meshFilter.sharedMesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static int ChunkSize
	{
		get {return chunkSize;}
	}
}
