using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityOSC; // SJ

public class ResetKinect : MonoBehaviour {
	/****************************************
	****************************************/
	public GameObject KinectController; // Empty Object that has KinectManager.cs
	KinectManager KinectManager;
	
	
	/****************************************
	****************************************/
	// Use this for initialization
	void Start () {
		KinectManager = KinectController.GetComponent<KinectManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.P)){
			Debug.Log("Reset Kinect");
			
			/********************
			kinectは、quit後にすぐAwakeすると、Hardがついてこれなかった.
			1.0秒後にAwake()させる.
			********************/
			enabled = false;
			KinectManager.OnApplicationQuit();
			Invoke("RestartKinect", 1.0f);
		}
	}
	
	/******************************
	description
		最初、
			Application.LoadLevel("KinectAvatarsDemo1");
		によって、sceneをReloadしようと思ったが、OSCController側がうまくResetできず、Reload後に挙動がおかしくなってしまった。
		
		そこで、今回は、KinectのみResetさせることとした.
	******************************/
	void RestartKinect()
	{
		KinectManager.Awake();
		enabled = true;
	}
}
