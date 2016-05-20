using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class BlurEnabler : MonoBehaviour {

    [SerializeField]
    private int blurAtFood;

    private Player pRef;
    private Blur blurRef;

	// Use this for initialization
	void Start () {

        pRef = gameObject.transform.parent.GetComponent<Player>();
        blurRef = GetComponent<Blur>();
	}
	
	// Update is called once per frame
	void Update () {

        if (pRef.food < blurAtFood)
        {
            blurRef.enabled = true;
        }
        else
        {
            blurRef.enabled = false;
        }
	}
}
