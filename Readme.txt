/************************************************************
最終的にやりたいこと
************************************************************/
【Kinect】->【win:unity】-OSC->【mac:unity <-Syphon- mac:oF】

/************************************************************
steps
************************************************************/
1.	Kinect -> win:unity(unity-chan) using Asset

2.	study:Asset parameter
	Boneの位置取得について
		trace.cs
	として作成.
	
3.	win unity内:getpose->setpose
	CopyPose.cs として作成.

4.	osc:simple test:win unity->mac unity : send mouse pos
	
	Unity projectに対して、
	"4. OSC (simple)" dir内の、"UnityOSC"をdirごとAsset dir下にcopy(Finder上で行えば、自動的にUnityにも反映).
	これは、
		/Users/nobuhiro/Documents/source/Unity/Assets/UnityOSC/src
	と同じ内容.
	
	OSCController.cs をscene上のdummy objectにaddすれば通信する.
	
	詳細は、以下にまとめた
		https://github.com/SJ-magic-study-unity/study__OSC_btwnPC
	

5.	kinect -> win:getPose -OSC-> mac:setpose
	model = unity-chan.
		
		■操作
		win側
			・	SendPoseViaOSCをAsset下にCopy
			・	getPose.cs, UnityOSC/OSCController.cs をsource object modelにadd.
				Source AvatarをGUIから設定(prefabの元モデルから).
				
		mac側
			・	GetposeViaOSCをAsset下にCopy
			・	setPose.cs, UnityOSC/OSCController.cs をdst object modelにadd.
				Dst AvatarをGUIから設定(prefabの元モデルから).
				
		で、OK.
				
		
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
				これを考慮して受け側で上手く吸収するためのcodeを"WIN55_TO_MAC56"側(OSCController.cs)に書いた.
				もし、異なるversionでtryする時は、同様にsend/receive双方のMuscleNameを比較して調整のこと.
				
				
			さて、わざわざMac側に持ってきた理由は、syphonを使い、例えばoFで作成した複雑なtextureをmodelに適用するため.
			ところが、Unity上でsyphonは、5.3より新しいversionでは使えなかった.
				https://github.com/SJ-magic-study-unity/study__UnitySyphon
			なので、vj appのDesign開発においては、指先は諦めて開発を進めよう.
		
		
6.	use original 3D model
	3ds maxで作成するmodelの条件などは、
		https://github.com/SJ-magic-study-unity/study__Unity_max2018
	
	
	さて、
	受け側(mac)で、original modelをsceneに配置し、5. のunity-chanと同様に手順を踏めばOKと思われたが、なんとこれがクラッシュしてしまった.
	原因は、Unity 5.3のバグのようだ.
	5.4で修正されていた.
		https://unity3d.com/jp/unity/whats-new/unity-5.4.0
		[788132] Animation: サポートされていないヒエラルキーでヒューマノイドアバターとともに GetHumanPose を使用すると、クラッシュする不具合を修正
	3ds maxで作成したmodelのヒエラルキーが、サポートの形式(順序など?)と異なるためのようだ.
	
		
	ためしに、5.6でtestしてみたら、確かに問題なく動作した.
	しかし、syphonを使うため、5.3で動かす必要があるので、以下に対応策を記す.
	
	受け側(mac)でも一旦unity-chanで受け、
	これを3. で作成した"CopyPose.cs"でoriginal modelに、copyする.
	CopyPose.cs は、unity-chanに対して、GetHumanPose methodを呼んでいる(サポートされている形式)のみなので、クラッシュなし.


7.	Reset kinect from script
	Kinectの前を複数人が通過した後などに、IDがおかしくなるのか？認識が外れてしまうことがあった。
	アプリを再起動すると、問題なく動作する。
	
	vjなど、イベント中にこれが起きた時、button一つでresetできると便利なので、
	この機能を開発。
	
	
	修正 : Assets\KInectscripts\KinectManager.cs
		l2		:	#define USE_SINGLE_KM_IN_MULTIPLE_SCENES をコメントアウト.
		l2125	:	Awake() をpublicに.
		l2448	: 	OnApplicationQuit() をpublicに.
		
		
	手順
		7. Reset Kinect\ResetKinect.cs
		を例えばUnity-chanにaddし、Kinect Controller ObjectをGUI上からset.
		
		
8.	Get Kinect Status
	use case
		windows側でkinectの検出状態をcheck.
		これをOSCでmacに送る。
		
		本scriptのあるなし、に関わらず、macにはposeがsendされるが、本scriptによって、追加で、kinectの検出状態をsendする。
		mac側では、検出が外れている時は、送られてくるpose(T-pose)でなく、決めpose(例えば演出中にkey buttonで保存された)をmodelに反映させる、など。
		
		
	How to use
		source model(Unity-chan)にadd.
		
		OSC Controller.csにて、
			GetComponent<check_KinectStatus>
		でgetし、
			b_IsTracked
			NumTracked
		にaccessして、OSC send.
		
		
/************************************************************
other URLs
************************************************************/
■イベント関数の実行順
	https://docs.unity3d.com/jp/530/Manual/ExecutionOrder.html

■Unityで他のスクリプトの変数や関数を利用する
	http://hiroyukitsuda.com/archives/1702



