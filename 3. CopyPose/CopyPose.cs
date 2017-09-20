/************************************************************
description
	source側にattach.
	
	初期回転位置:
		合わせる必要なし(合わせてはいけない)
		なぜなら、例えば、初期位置を揃えるためにy軸中心に 180deg回転させたとする.
		この状態でscriptが走ると、scriptが反映された回転位置から、180deg回転してしまう。
************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************
************************************************************/
public class CopyPose : MonoBehaviour {
	
	/****************************************
	****************************************/
	public HumanPoseHandler SourcePoseHandler;
	public HumanPoseHandler DstPoseHandler;
	
	public Avatar SourceAvatar;
	public Avatar DstAvatar;
	
	public GameObject DstGameObject;

	public HumanPose HumanPose;
	

	
	/****************************************
	****************************************/
	
	void Start()
	{
		/********************
		"DstGameObject.transform"を渡してHandlerを作成するが、
		GetHumanPose/SetHumanPoseで"transform.position"は反映されない.
		
		つまり、modelを配置した初期位置から、同じoffset量の移動となる。
		姿勢はもちろん反映される。
		********************/
		SourcePoseHandler = new HumanPoseHandler(SourceAvatar, this.transform);
		DstPoseHandler = new HumanPoseHandler(DstAvatar, DstGameObject.transform);
		
		// DstGameObject.transform.position = new Vector3(-1, 0, 0); // こう触ると、位置を指定できる.
	}
	
	// Update is called once per frame
	void Update () {
		SourcePoseHandler.GetHumanPose(ref HumanPose);
		DstPoseHandler.SetHumanPose(ref HumanPose);		
	}
}



