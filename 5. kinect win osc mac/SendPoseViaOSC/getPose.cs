/************************************************************
description
	send側のunity(win)でsourceとなるmodelにadd.
************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************
************************************************************/
public class getPose : MonoBehaviour {
	
	/****************************************
	****************************************/
	public HumanPoseHandler SourcePoseHandler;
	public Avatar SourceAvatar;
	public HumanPose HumanPose;
	
	/****************************************
	****************************************/
	
	void Start()
	{
		/********************
		********************/
		SourcePoseHandler = new HumanPoseHandler(SourceAvatar, this.transform);
		SourcePoseHandler.GetHumanPose(ref HumanPose);
	}
	
	// Update is called once per frame
	void Update () {
		SourcePoseHandler.GetHumanPose(ref HumanPose);
	}
}



