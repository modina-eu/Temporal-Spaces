using UnityEngine;

namespace OscSimpl.Examples
{
    public class GettingStartedSendingVid : MonoBehaviour
    {
        [SerializeField] OscOut _oscOut;
        //   [SerializeField] OscOut _oscOut2;
        OscMessage _message2; // Cached message.

        private int Nbr_portOut;
        public string address1 = "/HandRightX";
        public string address2 = "/HandRightY";
        public string address3 = "/HandRightZ";
        public string address4 = "/HandLeftX";
        public string address5 = "/HandLeftY";
        public string address6 = "/HandLeftZ";
        public string address7 = "/HeadX";
        public string address8 = "/HeadY";
        public string address9 = "/HeadZ";
        public string address10 = "/Hip";
        public string address11 = "/Hip";
        public string address12 = "/Hip";
        /*public string address13 = "/HipLeftX";
        public string address14 = "/HipLeftY";
        public string address15 = "/HipLeftZ"; */
        public string address16 = "/ShoulderRightX";
        public string address17 = "/ShoulderRightY";
        public string address18 = "/ShoulderRightZ";
        public string address19 = "/ShoulderLeftX";
        public string address20 = "/ShoulderLeftY";
        public string address21 = "/ShoulderLeftZ";
        public string address22 = "/ElbowRightX";
        public string address23 = "/ElbowRightY";
        public string address24 = "/ElbowRightZ";
        public string address25 = "/ElbowLeftX";
        public string address26 = "/ElbowLeftY";
        public string address27 = "/ElbowLeftZ";
        public string address28 = "/KneeRightX";
        public string address29 = "/KneeRightY";
        public string address30 = "/KneeRightZ";
        public string address31 = "/KneeLeftX";
        public string address32 = "/KneeLeftY";
        public string address33 = "/KneeLeftZ";
        public string address34 = "/AnkleRightX";
        public string address35 = "/AnkleRightY";
        public string address36 = "/AnkleRightZ";
        public string address37 = "/AnkleLeftX";
        public string address38 = "/AnkleLeftY";
        public string address39 = "/AnkleLeftZ";
        private string LocalIPTarget;
        //public float floatValue;
        //public float floatValue2;
        public PoseEstimatorVid1 script2;
        // public computeRenderVid script2;
        public float fac1;
        public float fac2;
        void Start()
        {
            LocalIPTarget = _oscOut.remoteIpAddress;
            Nbr_portOut = _oscOut.port;
            // LocalIPTarget = "192.168.1.25";
            // Ensure that we have a OscOut component.
            if (!_oscOut) _oscOut = gameObject.AddComponent<OscOut>();

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
            // fac2 = 10.0f / script.imageDims.y;
        }


        void Update()
        { }
    }
}

            /*   _oscOut.Send(address1, script.posePositions[10].x * fac1);
               _oscOut.Send(address2, script.posePositions[10].y * fac2);
               _oscOut.Send(address3, script.posePositions[10].z );
               _oscOut.Send(address4, script.posePositions[9].x * fac1);
               _oscOut.Send(address5, script.posePositions[9].y * fac2);
               _oscOut.Send(address6, script.posePositions[9].z);
               _oscOut.Send(address7, script.posePositions[0].x * fac1);
               _oscOut.Send(address8, script.posePositions[0].y * fac2);
               _oscOut.Send(address9, script.posePositions[0].z );
               _oscOut.Send(address10, script.posePositions[12].x * fac1);
               _oscOut.Send(address11, script.posePositions[12].y * fac2);
               _oscOut.Send(address12, script.posePositions[12].z );
               _oscOut.Send(address13, script.posePositions[11].x * fac1);
               _oscOut.Send(address14, script.posePositions[11].y * fac2);
               _oscOut.Send(address15, script.posePositions[11].z );
               _oscOut.Send(address16, script.posePositions[6].x * fac1);
               _oscOut.Send(address17, script.posePositions[6].y * fac2);
               _oscOut.Send(address18, script.posePositions[6].z );
               _oscOut.Send(address19, script.posePositions[5].x * fac1);
               _oscOut.Send(address20, script.posePositions[5].y * fac2);
               _oscOut.Send(address21, script.posePositions[5].z );
               _oscOut.Send(address22, script.posePositions[8].x * fac1);
               _oscOut.Send(address23, script.posePositions[8].y * fac2);
               _oscOut.Send(address24, script.posePositions[8].z );
               _oscOut.Send(address25, script.posePositions[7].x * fac1);
               _oscOut.Send(address26, script.posePositions[7].y * fac2);
               _oscOut.Send(address27, script.posePositions[7].z );
               _oscOut.Send(address28, script.posePositions[14].x * fac1);
               _oscOut.Send(address29, script.posePositions[14].y * fac2);
               _oscOut.Send(address30, script.posePositions[14].z );
               _oscOut.Send(address31, script.posePositions[13].x * fac1);
               _oscOut.Send(address32, script.posePositions[13].y * fac2);
               _oscOut.Send(address33, script.posePositions[13].z );
               _oscOut.Send(address34, script.posePositions[16].x * fac1);
               _oscOut.Send(address35, script.posePositions[16].y * fac2);
               _oscOut.Send(address36, script.posePositions[16].z );
               _oscOut.Send(address37, script.posePositions[15].x * fac1);
               _oscOut.Send(address38, script.posePositions[15].y * fac2);
               _oscOut.Send(address39, script.posePositions[15].z );        */
            //"nose", "leftShoulder", "rightShoulder", "leftElbow", "rightElbow", "leftWrist", "rightWrist", 
            //"leftHip", "rightHip", "leftKnee", "rightKnee", "leftAnkle", "rightAnkle"
           /*
            _oscOut.Send(address1, script2.pos0.x * 10);
            _oscOut.Send(address2, script2.pos0.y * 10);
            _oscOut.Send(address3, script2.pos0.z );
            _oscOut.Send(address4, script2.pos1.x * 10);
            _oscOut.Send(address5, script2.pos1.y * 10);
            _oscOut.Send(address6, script2.pos1.z );
            _oscOut.Send(address7, script2.pos2.x * 10);
            _oscOut.Send(address8, script2.pos2.y * 10);
            _oscOut.Send(address9, script2.pos2.z );
            _oscOut.Send(address10, script2.pos3.x * 10);
            _oscOut.Send(address11, script2.pos3.y * 10);
            _oscOut.Send(address12, script2.pos3.z);
            _oscOut.Send(address16, script2.pos4.x * 10);
            _oscOut.Send(address17, script2.pos4.y * 10);
            _oscOut.Send(address18, script2.pos4.z);
            _oscOut.Send(address19, script2.pos5.x * 10);
            _oscOut.Send(address20, script2.pos5.y * 10);
            _oscOut.Send(address21, script2.pos5.z);
            _oscOut.Send(address22, script2.pos6.x * 10);
            _oscOut.Send(address23, script2.pos6.y * 10);
            _oscOut.Send(address24, script2.pos6.z);
            _oscOut.Send(address25, script2.pos7.x * 10);
            _oscOut.Send(address26, script2.pos7.y * 10);
            _oscOut.Send(address27, script2.pos7.z);
            _oscOut.Send(address28, script2.pos8.x * 10);
            _oscOut.Send(address29, script2.pos8.y * 10);
            _oscOut.Send(address30, script2.pos8.z);
            _oscOut.Send(address31, script2.pos9.x * 10);
            _oscOut.Send(address32, script2.pos9.y * 10);
            _oscOut.Send(address33, script2.pos9.z);
            _oscOut.Send(address34, script2.pos10.x * 10);
            _oscOut.Send(address35, script2.pos10.y * 10);
            _oscOut.Send(address36, script2.pos10.z);
            _oscOut.Send(address37, script2.pos11.x * 10);
            _oscOut.Send(address38, script2.pos11.y * 10);
            _oscOut.Send(address39, script2.pos11.z);


        }
	}
}    */