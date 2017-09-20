/************************************************************
description
	receive側のunity(mac)で、Dstとなるmodelにadd.
	
	初期回転位置:
		win側のsource model objectに合わせる必要なし(合わせてはいけない)
		なぜなら、例えば、初期位置を揃えるためにy軸中心に 180deg回転させたとする.
		この状態でscriptが走ると、scriptが反映された回転位置から、180deg回転してしまう。
************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************
************************************************************/
public class setPose : MonoBehaviour {
	
	/****************************************
	****************************************/
	public HumanPoseHandler DstPoseHandler;
	public Avatar DstAvatar;
	public HumanPose HumanPose;
	
	/****************************************
	****************************************/
	
	void Start()
	{
		/********************
		"DstGameObject.transform"を渡してHandlerを作成するが、
		GetHumanPose/SetHumanPoseで"transform.position(global position)"は反映されない.
		
		つまり、modelを配置した初期位置から、同じoffset量(local position)の移動となる。
		姿勢はもちろん反映される。
		********************/
		DstPoseHandler = new HumanPoseHandler(DstAvatar, this.transform);
		
		// DstGameObject.transform.position = new Vector3(-1, 0, 0); // こう触ると、位置を指定できる.
		
		/********************
		HumanPoseについて、
		以下、二つの理由で、初期値をgetして設定しておく.
			GetHumanPose()内部で、musclesの配列サイズなど、初期化が行われている.
			OSC messageが届いてない時は、ここで取得しておいた初期parameterをsetすれば問題ない.
		********************/
		DstPoseHandler.GetHumanPose(ref HumanPose);
	}
	
	// Update is called once per frame
	void Update () {
		DstPoseHandler.SetHumanPose(ref HumanPose);		
	}
}



