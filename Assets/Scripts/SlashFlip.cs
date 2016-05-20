using UnityEngine;
using System.Collections;

public class SlashFlip : MonoBehaviour {


    private GameObject parent;
    private SpriteRenderer spRef;
    private Vector3 ogPos;
    // Use this for initialization
    void Start () {
        parent = gameObject.transform.parent.gameObject;
        spRef = GetComponent<SpriteRenderer>();
        ogPos = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {

        if (parent.GetComponent<SpriteRenderer>().flipX && !spRef.flipX)
        {
            Vector3 newPos = transform.localPosition;
            newPos.x = -newPos.x;
            transform.localPosition = newPos;
            spRef.flipX = true;
        }
        else if (!parent.GetComponent<SpriteRenderer>().flipX && spRef.flipX)
        {
            transform.localPosition = ogPos;
            spRef.flipX = false;
        }

	
	}
}
