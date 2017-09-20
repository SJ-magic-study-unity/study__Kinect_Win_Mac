/************************************************************
■send(win)側、receive(mac)側のUnity versionについて
	send側(win)のunityは5.5を使用.
	
	receive側について
	Unity 5.3
		HumanPoseをsetする際、指先の動きが反映されなかった.
		
	Unity 5.6
		問題なく反映された.
		ただし、HumanPose.musclesの定義が5.3から変更(追加)されていた.
		(実際には、5.3が92個に対し、5.6は95個.)
		
		そこで、HumanTrait.MuscleNameをLogFileに出力し、両vetsionを比較、追加された項目をcheck.
		これを考慮して受け側で上手く吸収するためのcodeを"WIN55_TO_MAC56"側に書いた.
		もし、異なるversionでtryする時は、同様にsend/receive双方のMuscleNameを比較して調整のこと.
		
		
	さて、わざわざMac側に持ってきた理由は、syphonを使い、例えばoFで作成した複雑なtextureをmodelに適用するため.
	ところが、Unity上でsyphonは、5.3より新しいversionでは使えなかった.
		https://github.com/SJ-magic-study-unity/study__UnitySyphon
	なので、vj appのDesign開発においては、指先は諦めて開発を進めよう.
************************************************************/
// #define WIN55_TO_MAC56

/************************************************************
************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SJ
using UnityOSC;
using System.IO;

/************************************************************
************************************************************/

public class OSCController : MonoBehaviour {
	/****************************************
	****************************************/
	// params for mac.
	public string serverId = "win";
	public string Ip_SendTo = "10.0.0.3";
	public int Port_SendTo = 12345;
	
	public string clientId = "mac";
	public int ReceivePort = 12345;
	
	// params for win.
	/*
	public string serverId = "mac";
	public string Ip_SendTo = "10.0.0.2";
	public int Port_SendTo = 12345;
	
	public string clientId = "win";
	public int ReceivePort = 12345;
	*/

	/* */
	// public KeyCode debugKey = KeyCode.S;
	public string  osc_dir = "/HumanPose";
	
	// private long latestTimeStamp = 0;
	private string label = "saijo";
	
	private Queue queue;
	
	setPose setPose;
	
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
		
		setPose = GetComponent<setPose>();
		
		/********************
		********************/
		StreamWriter sw_Name;
		sw_Name = new StreamWriter("Name.csv", false); //true=追記 false=上書き
		
		string[] muscleName = HumanTrait.MuscleName;
        int i = 0;
        while (i < HumanTrait.MuscleCount) {
            // Debug.Log(muscleName[i]);
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
		
		/*
		// var sampleVals = new List<float>(){Input.mousePosition.x, Input.mousePosition.y}; // 2つ以上のparameterを送信(型は同じである必要あり)
		var sampleVals = new List<float>(){(int)(Input.mousePosition.x), (int)(Input.mousePosition.y)}; // 2つ以上のparameterを送信(型は同じである必要あり)
		OSCHandler.Instance.SendMessageToClient(this.clientId, debugMessage, sampleVals);
		*/
		
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
				
				if(msg.Address == "/HumanPose"){
#if !WIN55_TO_MAC56
					if(setPose.HumanPose.muscles.Length == System.Convert.ToInt16(msg.Data[7])){
#endif
						setPose.HumanPose.bodyPosition.x = (float)( System.Convert.ToDouble(msg.Data[0]) );
						setPose.HumanPose.bodyPosition.y = (float)( System.Convert.ToDouble(msg.Data[1]) );
						setPose.HumanPose.bodyPosition.z = (float)( System.Convert.ToDouble(msg.Data[2]) );
						setPose.HumanPose.bodyRotation.x = (float)( System.Convert.ToDouble(msg.Data[3]) );
						setPose.HumanPose.bodyRotation.y = (float)( System.Convert.ToDouble(msg.Data[4]) );
						setPose.HumanPose.bodyRotation.z = (float)( System.Convert.ToDouble(msg.Data[5]) );
						setPose.HumanPose.bodyRotation.w = (float)( System.Convert.ToDouble(msg.Data[6]) );
						
						for(int i = 0; i < setPose.HumanPose.muscles.Length; i++){
#if WIN55_TO_MAC56
							if(i < 6){ // 0 - 5:同じ
								setPose.HumanPose.muscles[i] = (float)( System.Convert.ToDouble(msg.Data[8 + i]) );
							}else if(i < 9){ // 6 - 8:additionnal param
								/* skip(not zero) */
							}else{ // 以降 : 間はさんで後は同じ.
								setPose.HumanPose.muscles[i] = (float)( System.Convert.ToDouble(msg.Data[8 + i - 3]) );
							}
#else
							setPose.HumanPose.muscles[i] = (float)( System.Convert.ToDouble(msg.Data[8 + i]) );
#endif
							
						}
#if !WIN55_TO_MAC56
					}
#endif
					
					/* */
					label = "(" + setPose.HumanPose.muscles.Length + ", " + msg.Data[7] + ", " + HumanTrait.MuscleCount + ")";
				}
			}
        }
	}
	
	void OnGUI()
	{
		GUI.color = Color.black;
		
		GUI.Label(new Rect(15, 15, 100, 30), label);
	}
}
