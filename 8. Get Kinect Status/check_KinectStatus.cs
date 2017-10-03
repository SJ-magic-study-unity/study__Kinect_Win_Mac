/************************************************************
use case
	windows側でkinectの検出状態をcheck.
	これをOSCでmacに送る。
	
	本scriptのあるなし、に関わらず、macにはposeがsendされるが、
	本scriptによって、追加で、kinectの検出状態をsendする。
	mac側では、検出が外れている時は、
	送られてくるpose(T-pose)でなく、決めpose(例えば演出中にkey buttonで保存された)をmodelに反映させる、など。
	
	
How to use
	source model(Unity-chan)にadd.
	
	OSC Controller.csにて、
		GetComponent<check_KinectStatus>
	でgetし、
		b_IsTracked
		NumTracked
	にaccessして、OSC send.
************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************
************************************************************/

public class check_KinectStatus : MonoBehaviour {
	/****************************************
	****************************************/
	public GameObject KinectController; // Empty Object that has KinectManager.cs
	KinectManager KinectManager;
	
	public bool b_IsTracked = false;
	public int NumTracked = 0;

	private string label = "Nobuhiro";
	

	/****************************************
	****************************************/
	
	// Use this for initialization
	void Start () {
		KinectManager = KinectController.GetComponent<KinectManager>();
	}
	
	/******************************
	KinectManager.csでの言葉の定義
		index	:	0-
		ID		:	固有の値(乱数?).
					多分、detectされた時に与えられるのかな？
					一旦外れて、再detectされると違う値が入る。
	******************************/
	void Update () {
		b_IsTracked = KinectManager.IsUserDetected(0); // index.
		NumTracked = KinectManager.GetUsersCount();
		
		label = "IsTracked = " + b_IsTracked + ", UserCount = " + NumTracked;
	}
	
	void OnGUI()
	{
		GUI.color = Color.black;
		
		GUI.Label(new Rect(15, 60, 200, 200), label);
	}
}

