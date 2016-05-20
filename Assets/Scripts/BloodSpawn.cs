using UnityEngine;
using System.Collections;

public class BloodSpawn : MonoBehaviour {

    private Vector3 ogPos;
    private Vector3 ogRot;
    // Use this for initialization
    void Start () {

        ogPos = transform.position;
        ogRot = transform.eulerAngles;

        ogPos.x += Random.Range(-0.4f, 0.4f);
        ogPos.y += Random.Range(-0.4f, 0.4f);
        ogRot.z += Random.Range(-180f, 180f);


        transform.position = ogPos;
        transform.eulerAngles = ogRot;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
