using UnityEngine;

namespace OscSimpl.Examples
{
	public class GettingStartedSendingPoseNet : MonoBehaviour
	{
		[SerializeField] OscOut _oscOut;

		OscMessage _message2; // Cached message.
       
        private int Nbr_portOut;
        //"nose", "leftShoulder", "rightShoulder", "leftElbow", "rightElbow", "leftWrist", "rightWrist",  "Hip", "leftKnee", "rightKnee", "leftAnkle", "rightAnkle"
        public string address1 = "/HeadX";
        public string address2 = "/HeadY";
        public string address3 = "/HeadZ";
        public string address4 = "/ShoulderLeftX";
        public string address5 = "/ShoulderLeftY";
        public string address6 = "/ShoulderLeftZ";
        public string address7 = "/ShoulderRightX";
        public string address8 = "/ShoulderRightY";
        public string address9 = "/ShoulderRightZ";
        public string address10 = "/ElbowLeftX";
        public string address11 = "/ElbowLeftY";
        public string address12 = "/ElbowLeftZ";
        public string address13 = "/ElbowRightX";
        public string address14 = "/ElbowRightY";
        public string address15 = "/ElbowRightZ";
        public string address16 = "/HandLeftX";
        public string address17 = "/HandLeftY";
        public string address18 = "/HandLeftZ";
        public string address19 = "/HandRightX";
        public string address20 = "/HandRightY";
        public string address21 = "/HandRightZ";
        public string address22 = "/HipX";
        public string address23 = "/HipY";
        public string address24 = "/HipZ";
        public string address25 = "/KneeLeftX";
        public string address26 = "/KneeLeftY";
        public string address27 = "/KneeLeftZ";
        public string address28 = "/KneeRightX";
        public string address29 = "/KneeRightY";
        public string address30 = "/KneeRightZ";
        public string address31 = "/AnkleLeftX";
        public string address32 = "/AnkleLeftY";
        public string address33 = "/AnkleLeftZ";
        public string address34 = "/AnkleRightX";
        public string address35 = "/AnkleRightY";
        public string address36 = "/AnkleRightZ";

        public string adresse37 = "/stopR";
       /* public string address38 = "/HandRightX2";
        public string address39 = "/HandRightY2";
        public string address40 = "/HandRightZ2";
        public string address41 = "/HandLeftX2";
        public string address42 = "/HandLeftY2";
        public string address43 = "/HandLeftZ2";
        public string address44 = "/HeadX2";
        public string address45 = "/HeadY2";
        public string address46 = "/HeadZ2";
        public string address47 = "/HipRightX2";
        public string address48 = "/HipRightY2";
        public string address49 = "/HipRightZ2";
        public string address50 = "/ShoulderRightX2";
        public string address51 = "/ShoulderRightY2";
        public string address52 = "/ShoulderRightZ2";
        public string address53 = "/ShoulderLeftX2";
        public string address54 = "/ShoulderLeftY2";
        public string address55 = "/ShoulderLeftZ2";
        public string address56 = "/ElbowRightX2";
        public string address57 = "/ElbowRightY2";
        public string address58 = "/ElbowRightZ2";
        public string address59 = "/ElbowLeftX2";
        public string address60 = "/ElbowLeftY2";
        public string address61 = "/ElbowLeftZ2";
        public string address62 = "/KneeRightX2";
        public string address63 = "/KneeRightY2";
        public string address64 = "/KneeRightZ2";
        public string address65 = "/KneeLeftX2";
        public string address66 = "/KneeLeftY2";
        public string address67 = "/KneeLeftZ2";
        public string address68 = "/AnkleRightX2";
        public string address69 = "/AnkleRightY2";
        public string address70 = "/AnkleRightZ2";
        public string address71 = "/AnkleLeftX2";
        public string address72 = "/AnkleLeftY2";
        public string address73 = "/AnkleLeftZ2";  */
        private string LocalIPTarget;  
        //public float floatValue;
        //public float floatValue2;
        public PoseEstimatorVid1 script2;
        public CaptureActivation script;
        public float fac1;
        public float fac2;
        void Start()
		{
            LocalIPTarget = _oscOut.remoteIpAddress;
            Nbr_portOut = _oscOut.port;
           // LocalIPTarget = "192.168.1.25";
            // Ensure that we have a OscOut component.
            if ( !_oscOut ) _oscOut = gameObject.AddComponent<OscOut>();

			// Prepare for sending messages locally on this device on port 7000.
			_oscOut.Open(Nbr_portOut, LocalIPTarget);

            // ... or, alternatively target remote devices with a IP Address.
            //oscOut.Open( 7000, "192.168.1.101" );

            // If you want to send a single value then you can use this one-liner.
            //_oscOut.Send( address1, 0.5f );

            // If you want to send a message with multiple values, then you
            // need to create a message, add your values and send it.
            // Always cache the messages you create, so that you can reuse them.
            //_message2 = new OscMessage( address2 );
            //_message2.Add( Time.frameCount ).Add( Time.time ).Add( Random.value );
            //_oscOut.Send( _message2 );
            // _oscOut.Send(address2, 0.6f);
            //fac1 = 10.0f / script.imageDims.x;
            //fac2 = 10.0f / script.imageDims.y;
        }


		void Update()
		{
             /*
            _oscOut.Send(address1, script2.pos0.x * 10);
            _oscOut.Send(address2, script2.pos0.y * 10);
            _oscOut.Send(address3, script2.pos0.z);
            _oscOut.Send(address4, script2.pos1.x * 10);
            _oscOut.Send(address5, script2.pos1.y * 10);
            _oscOut.Send(address6, script2.pos1.z);
            _oscOut.Send(address7, script2.pos2.x * 10);
            _oscOut.Send(address8, script2.pos2.y * 10);
            _oscOut.Send(address9, script2.pos2.z);
            _oscOut.Send(address10, script2.pos3.x * 10);
            _oscOut.Send(address11, script2.pos3.y * 10);
            _oscOut.Send(address12, script2.pos3.z);
            _oscOut.Send(address13, script2.pos4.x * 10);
            _oscOut.Send(address14, script2.pos4.y * 10);
            _oscOut.Send(address15, script2.pos4.z);
            _oscOut.Send(address16, script2.pos5.x * 10);
            _oscOut.Send(address17, script2.pos5.y * 10);
            _oscOut.Send(address18, script2.pos5.z);
            _oscOut.Send(address19, script2.pos6.x * 10);
            _oscOut.Send(address20, script2.pos6.y * 10);
            _oscOut.Send(address21, script2.pos6.z);
            _oscOut.Send(address22, script2.pos7.x * 10);
            _oscOut.Send(address23, script2.pos7.y * 10);
            _oscOut.Send(address24, script2.pos7.z);
            _oscOut.Send(address25, script2.pos8.x * 10);
            _oscOut.Send(address26, script2.pos8.y * 10);
            _oscOut.Send(address27, script2.pos8.z);
            _oscOut.Send(address28, script2.pos9.x * 10);
            _oscOut.Send(address29, script2.pos9.y * 10);
            _oscOut.Send(address30, script2.pos9.z);
            _oscOut.Send(address31, script2.pos10.x * 10);
            _oscOut.Send(address32, script2.pos10.y * 10);
            _oscOut.Send(address33, script2.pos10.z);
            _oscOut.Send(address34, script2.pos11.x * 10);
            _oscOut.Send(address35, script2.pos11.y * 10);
            _oscOut.Send(address36, script2.pos11.z);        
            if (script.stop == 1)
            {
                _oscOut.Send(adresse37, 1.0f);
            }
            /*_oscOut.Send(address38, script2.posb0.x * 10);
            _oscOut.Send(address39, script2.posb0.y * 10);
            _oscOut.Send(address40, script2.posb0.z);
            _oscOut.Send(address41, script2.posb1.x * 10);
            _oscOut.Send(address42, script2.posb1.y * 10);
            _oscOut.Send(address43, script2.posb1.z);
            _oscOut.Send(address44, script2.posb2.x * 10);
            _oscOut.Send(address45, script2.posb2.y * 10);
            _oscOut.Send(address46, script2.posb2.z);
            _oscOut.Send(address47, script2.posb3.x * 10);
            _oscOut.Send(address48, script2.posb3.y * 10);
            _oscOut.Send(address49, script2.posb3.z);
            _oscOut.Send(address50, script2.posb4.x * 10);
            _oscOut.Send(address51, script2.posb4.y * 10);
            _oscOut.Send(address52, script2.posb4.z);
            _oscOut.Send(address53, script2.posb5.x * 10);
            _oscOut.Send(address54, script2.posb5.y * 10);
            _oscOut.Send(address55, script2.posb5.z);
            _oscOut.Send(address56, script2.posb6.x * 10);
            _oscOut.Send(address57, script2.posb6.y * 10);
            _oscOut.Send(address58, script2.posb6.z);
            _oscOut.Send(address59, script2.posb7.x * 10);
            _oscOut.Send(address60, script2.posb7.y * 10);
            _oscOut.Send(address61, script2.posb7.z);
            _oscOut.Send(address62, script2.posb8.x * 10);
            _oscOut.Send(address63, script2.posb8.y * 10);
            _oscOut.Send(address64, script2.posb8.z);
            _oscOut.Send(address65, script2.posb9.x * 10);
            _oscOut.Send(address66, script2.posb9.y * 10);
            _oscOut.Send(address67, script2.posb9.z);
            _oscOut.Send(address68, script2.posb10.x * 10);
            _oscOut.Send(address69, script2.posb10.y * 10);
            _oscOut.Send(address70, script2.posb10.z);
            _oscOut.Send(address71, script2.posb11.x * 10);
            _oscOut.Send(address72, script2.posb11.y * 10);
            _oscOut.Send(address73, script2.posb11.z);  */
        }
	}
}