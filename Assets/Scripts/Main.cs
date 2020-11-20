using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	public Transform chunk;

	private int numChunks = 4;

	// Use this for initialization
	void Start () {
		for (int y = 0; y < numChunks; y++)
		{
			for (int z = 0; z < numChunks*8; z++)
			{
				for (int x = 0; x < numChunks*8; x++)
				{
					Transform currChunk = (Transform) Instantiate(chunk, new Vector3(x * CellGenerator.ChunkSize, y * CellGenerator.ChunkSize, z * CellGenerator.ChunkSize), Quaternion.identity);
					currChunk.parent = transform;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
