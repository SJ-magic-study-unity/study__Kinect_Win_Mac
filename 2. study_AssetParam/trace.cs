using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trace : MonoBehaviour {
	public GameObject target; // e.g : Character1_LeftHand
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void LateUpdate(){
		transform.position = target.transform.position;
	}
}
