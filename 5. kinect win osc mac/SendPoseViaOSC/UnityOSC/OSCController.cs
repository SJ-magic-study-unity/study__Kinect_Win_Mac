/************************************************************
************************************************************/


/************************************************************
************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityOSC; // SJ
using System.IO; // StreamWriter


/************************************************************
************************************************************/

public class OSCController : MonoBehaviour {
	/****************************************
	****************************************/
	// params for mac.
	/*
	public string serverId = "win";
	public string Ip_SendTo = "10.0.0.3";
	public int Port_SendTo = 12345;
	
	public string clientId = "mac";
	public int ReceivePort = 12345;
	*/
	
	// params for win.
	public string serverId = "mac";
	public string Ip_SendTo = "10.0.0.2";
	public int Port_SendTo = 12345;
	
	public string clientId = "win";
	public int ReceivePort = 12345;

	/* */
	// public KeyCode debugKey = KeyCode.S;
	public string  osc_dir = "/HumanPose";
	
	// private long latestTimeStamp = 0;
	private string label = "saijo";
	
	private Queue queue;
	
	getPose getPose;
	
	
	/****************************************
	****************************************/
	
	// Use this for initialization
	void Start () {
		/********************
		********************/
		OSCHandler.Instance.Init(this.clientId, this.Ip_SendTo, this.Port_SendTo, this.serverId, this.ReceivePort);
		
		queue = new Queue();
        queue = Queue.Synchronized(queue);

        // パケット受信時のイベントハンドラを登録
        OSCHandler.Instance.PacketReceivedEvent += OnPacketReceived_;
		
		getPose = GetComponent<getPose>();
		
		
		/********************
		********************/
		StreamWriter sw_Name;
		sw_Name = new StreamWriter("win_Name.csv", false);
		
		string[] muscleName = HumanTrait.MuscleName;
		int i = 0;
		while(i < HumanTrait.MuscleCount){
			sw_Name.Write(muscleName[i]);
			sw_Name.Write(",");
			i++;
		}
		sw_Name.Flush();
		sw_Name.Close();
	}
	
	void OnPacketReceived_(OSCServer server, OSCPacket packet) {
		/********************
		for safety
		********************/
		const int QueueLimitSize = 100;
		if(QueueLimitSize < queue.Count){
			queue.Clear();
		}
			
		/********************
		********************/
        queue.Enqueue(packet);
    }

	// Update is called once per frame
	void Update () {
		/********************
		send
		********************/
		// if (Input.GetMouseButtonDown(0))
		/*
		if (Input.GetKeyDown(this.debugKey))
		{
			Debug.Log("SendMessage");
			
			var sampleVals = new List<int>(){1, 2, 3}; // 2つ以上のparameterを送信(型は同じである必要あり)
			OSCHandler.Instance.SendMessageToClient(this.clientId, debugMessage, sampleVals);
		}
		*/
		
		/* 2つ以上のparameterを送信(型は同じである必要あり) */
		List<float> list = new List<float>();
		list.Add(getPose.HumanPose.bodyPosition.x);
		list.Add(getPose.HumanPose.bodyPosition.y);
		list.Add(getPose.HumanPose.bodyPosition.z);
		list.Add(getPose.HumanPose.bodyRotation.x);
		list.Add(getPose.HumanPose.bodyRotation.y);
		list.Add(getPose.HumanPose.bodyRotation.z);
		list.Add(getPose.HumanPose.bodyRotation.w);
		
		// list.Add(HumanTrait.muscles);
		list.Add(getPose.HumanPose.muscles.Length);
		
		for(int i = 0; i < HumanTrait.MuscleCount; i++){
			list.Add(getPose.HumanPose.muscles[i]);
		}
		OSCHandler.Instance.SendMessageToClient(this.clientId, osc_dir, list);
		
		/* */
		label = "(" + HumanTrait.MuscleCount + ", " + getPose.HumanPose.muscles.Length + ")";
		
		
		/********************
		receive
		********************/
		while (0 < queue.Count) {
            OSCPacket packet = queue.Dequeue() as OSCPacket;
            if (packet.IsBundle()) {
                // OSCBundleの場合
                OSCBundle bundle = packet as OSCBundle;
                foreach (OSCMessage msg in bundle.Data) {
                    // メッセージの中身にあわせた処理
                }
            } else {
                // OSCMessageの場合はそのまま変換
                OSCMessage msg = packet as OSCMessage;
				
				/*
                // メッセージの中身にあわせた処理
				if(msg.Address == "/mouse"){
					label = "(" + msg.Data[0] + ", " + msg.Data[1] + ")";
					
				}else if(msg.Address == "/test"){
					Debug.Log(	"Receive : "
								+ msg.TimeStamp
								+ ", "
								+ msg.Address
								+ ", "
								+ msg.Data[0]);
				}
				*/
			}
        }
	}
	
	void OnGUI()
	{
		GUI.color = Color.black;
		
		GUI.Label(new Rect(15, 15, 100, 30), label);
	}
}
