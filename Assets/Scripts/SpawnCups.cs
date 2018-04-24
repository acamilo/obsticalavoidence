using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCups : MonoBehaviour {

    public Vector3 center;
    public Vector3 size;

    public int numberOfObjects = 40;

    public GameObject cupPrefab;  

	// Use this for initialization
	void Start () {
        for (int i = 0; i < numberOfObjects; i++) spawnCup();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void spawnCup()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2,size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
        Quaternion rot = Quaternion.Euler(
            Random.Range(0, 180),
            Random.Range(0, 180),
            Random.Range(0, 180)
            );

        Instantiate(cupPrefab, pos, rot);
    }

    private void OnDrawGizmosSelected()
    {
        center = transform.position;
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(center, size);
    }
}
