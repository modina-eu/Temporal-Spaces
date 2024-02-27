using UnityEngine;

namespace OscSimpl.Examples
{
	public class GettingStartedSending : MonoBehaviour
	{
        [SerializeField] OscOut _oscOut;
        OscMessage _message2; // Cached message.

        private int Nbr_portOut;
        //"nose", "leftShoulder", "rightShoulder", "leftElbow", "rightElbow", "leftWrist", "rightWrist",  "Hip", "leftKnee", "rightKnee", "leftAnkle", "rightAnkle"
        public string address1 = "/P1HeadX";
        public string address2 = "/P1HeadY";
        public string address3 = "/P1HeadS";
        public string address4 = "/P1ShoulderLeftX";
        public string address5 = "/P1ShoulderLeftY";
        public string address6 = "/P1ShoulderLeftS";
        public string address7 = "/P1ShoulderRightX";
        public string address8 = "/P1ShoulderRightY";
        public string address9 = "/P1ShoulderRightS";
        public string address10 = "/P1ElbowLeftX";
        public string address11 = "/P1ElbowLeftY";
        public string address12 = "/P1ElbowLeftS";
        public string address13 = "/P1ElbowRightX";
        public string address14 = "/P1ElbowRightY";
        public string address15 = "/P1ElbowRightS";
        public string address16 = "/P1HandLeftX";
        public string address17 = "/P1HandLeftY";
        public string address18 = "/P1HandLeftS";
        public string address19 = "/P1HandRightX";
        public string address20 = "/P1HandRightY";
        public string address21 = "/P1HandRightS";
        public string address22 = "/P1HipX";
        public string address23 = "/P1HipY";
        public string address24 = "/P1HipS";
        public string address25 = "/P1KneeLeftX";
        public string address26 = "/P1KneeLeftY";
        public string address27 = "/P1KneeLeftS";
        public string address28 = "/P1KneeRightX";
        public string address29 = "/P1KneeRightY";
        public string address30 = "/P1KneeRightS";
        public string address31 = "/P1AnkleLeftX";
        public string address32 = "/P1AnkleLeftY";
        public string address33 = "/P1AnkleLeftS";
        public string address34 = "/P1AnkleRightX";
        public string address35 = "/P1AnkleRightY";
        public string address36 = "/P1AnkleRightS";

        public string adresse37 = "/stopR";

        public string address38 = "/P0X";
        public string address39 = "/P0Y";
        public string address40 = "/P1X";
        public string address41 = "/P1Y";
        public string address42 = "/P2X";
        public string address43 = "/P2Y";
        public string address44 = "/P3X";
        public string address45 = "/P3Y";
        public string address46 = "/P4X";
        public string address47 = "/P4Y";

        public string address48 = "/P1HeadZ";        
        public string address49 = "/P1ElbowLeftZ";
        public string address50 = "/P1ElbowRightZ";
        public string address51 = "/P1HandLeftZ";
        public string address52 = "/P1HandRightZ";
        public string address53 = "/P1HipZ";
        public string address54 = "/P1KneeLeftZ";
        public string address55 = "/P1KneeRightZ";
        public string address56 = "/P1AnkleLeftZ";
        public string address57 = "/P1AnkleRightZ";

        public string address58 = "/P2HeadX";
        public string address59 = "/P2HeadY";
        public string address60 = "/P2HeadZ";
        public string address61 = "/P2ElbowLeftX";
        public string address62 = "/P2ElbowLeftY";
        public string address63 = "/P2ElbowLeftZ";
        public string address64 = "/P2ElbowRightX";
        public string address65 = "/P2ElbowRightY";
        public string address66 = "/P2ElbowRightZ";
        public string address67 = "/P2HandLeftX";
        public string address68 = "/P2HandLeftY";
        public string address69 = "/P2HandLeftZ";
        public string address70 = "/P2HandRightX";
        public string address71 = "/P2HandRightY";
        public string address72 = "/P2HandRightZ";
        public string address73 = "/P2HipX";
        public string address74 = "/P2HipY";
        public string address75 = "/P2HipZ";
        public string address76 = "/P2KneeLeftX";
        public string address77 = "/P2KneeLeftY";
        public string address78 = "/P2KneeLeftZ";
        public string address79 = "/P2KneeRightX";
        public string address80 = "/P2KneeRightY";
        public string address81 = "/P2KneeRightZ";
        public string address82 = "/P2AnkleLeftX";
        public string address83 = "/P2AnkleLeftY";
        public string address84 = "/P2AnkleLeftZ";
        public string address85 = "/P2AnkleRightX";
        public string address86 = "/P2AnkleRightY";
        public string address87 = "/P2AnkleRightZ";

        public string address88 = "/P2HeadS";
        public string address89 = "/P2ElbowLeftS";
        public string address90 = "/P2ElbowRightS";
        public string address91 = "/P2HandLeftS";
        public string address92 = "/P2HandRightS";
        public string address93 = "/P2HipS";
        public string address94 = "/P2KneeLeftS";
        public string address95 = "/P2KneeRightS";
        public string address96 = "/P2AnkleLeftS";
        public string address97 = "/P2AnkleRightS";

        public string address98 = "/P3HeadX";
        public string address99 = "/P3HeadY";
        public string address100 = "/P3HeadZ";
        public string address101 = "/P3ElbowLeftX";
        public string address102 = "/P3ElbowLeftY";
        public string address103 = "/P3ElbowLeftZ";
        public string address104 = "/P3ElbowRightX";
        public string address105 = "/P3ElbowRightY";
        public string address106 = "/P3ElbowRightZ";
        public string address107 = "/P3HandLeftX";
        public string address108 = "/P3HandLeftY";
        public string address109 = "/P3HandLeftZ";
        public string address110 = "/P3HandRightX";
        public string address111 = "/P3HandRightY";
        public string address112 = "/P3HandRightZ";
        public string address113 = "/P3HipX";
        public string address114 = "/P3HipY";
        public string address115 = "/P3HipZ";
        public string address116 = "/P3KneeLeftX";
        public string address117 = "/P3KneeLeftY";
        public string address118 = "/P3KneeLeftZ";
        public string address119 = "/P3KneeRightX";
        public string address120 = "/P3KneeRightY";
        public string address121 = "/P3KneeRightZ";
        public string address122 = "/P3AnkleLeftX";
        public string address123 = "/P3AnkleLeftY";
        public string address124 = "/P3AnkleLeftZ";
        public string address125 = "/P3AnkleRightX";
        public string address126 = "/P3AnkleRightY";
        public string address127 = "/P3AnkleRightZ";

        public string address128 = "/P3HeadS";
        public string address129 = "/P3ElbowLeftS";
        public string address130 = "/P3ElbowRightS";
        public string address131 = "/P3HandLeftS";
        public string address132 = "/P3HandRightS";
        public string address133 = "/P3HipS";
        public string address134 = "/P3KneeLeftS";
        public string address135 = "/P3KneeRightS";
        public string address136 = "/P3AnkleLeftS";
        public string address137 = "/P3AnkleRightS";

        public string address138 = "/P1Check";
        public string address139 = "/P2Check";
        public string address140 = "/P3Check";

        private string LocalIPTarget;
        public PoseEstimator1 script2;

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
            //fac2 = 10.0f / script.imageDims.y;
        }


        void Update()
        {

            _oscOut.Send(address1, script2.pos1[0].x * 10);
            _oscOut.Send(address2, script2.pos1[0].y * 10);
            _oscOut.Send(address3, script2.score1[0]);
            _oscOut.Send(address10, script2.pos1[3].x * 10);
            _oscOut.Send(address11, script2.pos1[3].y * 10);
            _oscOut.Send(address12, script2.score1[3]);
            _oscOut.Send(address13, script2.pos1[4].x * 10);
            _oscOut.Send(address14, script2.pos1[4].y * 10);
            _oscOut.Send(address15, script2.score1[4]);
            _oscOut.Send(address16, script2.pos1[5].x * 10);
            _oscOut.Send(address17, script2.pos1[5].y * 10);
            _oscOut.Send(address18, script2.score1[5]);
            _oscOut.Send(address19, script2.pos1[6].x * 10);
            _oscOut.Send(address20, script2.pos1[6].y * 10);
            _oscOut.Send(address21, script2.score1[6]);
            _oscOut.Send(address22, script2.pos1[7].x * 10);
            _oscOut.Send(address23, script2.pos1[7].y * 10);
            _oscOut.Send(address24, script2.score1[7]);
            _oscOut.Send(address25, script2.pos1[8].x * 10);
            _oscOut.Send(address26, script2.pos1[8].y * 10);
            _oscOut.Send(address27, script2.score1[8]);
            _oscOut.Send(address28, script2.pos1[9].x * 10);
            _oscOut.Send(address29, script2.pos1[9].y * 10);
            _oscOut.Send(address30, script2.score1[9]);
            _oscOut.Send(address31, script2.pos1[10].x * 10);
            _oscOut.Send(address32, script2.pos1[10].y * 10);
            _oscOut.Send(address33, script2.score1[10]);
            _oscOut.Send(address34, script2.pos1[11].x * 10);
            _oscOut.Send(address35, script2.pos1[11].y * 10);
            _oscOut.Send(address36, script2.score1[11]);
            _oscOut.Send(address38, script2.pn0.x);
            _oscOut.Send(address39, script2.pn0.y);
            _oscOut.Send(address40, script2.pn1.x);
            _oscOut.Send(address41, script2.pn1.y);
            _oscOut.Send(address42, script2.pn2.x);
            _oscOut.Send(address43, script2.pn2.y);
            _oscOut.Send(address44, script2.pn3.x);
            _oscOut.Send(address45, script2.pn3.y);
            _oscOut.Send(address46, script2.pn4.x);
            _oscOut.Send(address47, script2.pn4.y);

            _oscOut.Send(address48, script2.pos1[0].z * 10);
            _oscOut.Send(address49, script2.pos1[3].z * 10);
            _oscOut.Send(address50, script2.pos1[4].z * 10);
            _oscOut.Send(address51, script2.pos1[5].z * 10);
            _oscOut.Send(address52, script2.pos1[6].z * 10);
            _oscOut.Send(address53, script2.pos1[7].z * 10);
            _oscOut.Send(address54, script2.pos1[8].z * 10);
            _oscOut.Send(address55, script2.pos1[9].z * 10);
            _oscOut.Send(address56, script2.pos1[10].z * 10);
            _oscOut.Send(address57, script2.pos1[11].z * 10);

            _oscOut.Send(address58, script2.pos2[0].x * 10);
            _oscOut.Send(address59, script2.pos2[0].y * 10);
            _oscOut.Send(address60, script2.pos2[0].z * 10);
            _oscOut.Send(address61, script2.pos2[3].x * 10);
            _oscOut.Send(address62, script2.pos2[3].y * 10);
            _oscOut.Send(address63, script2.pos2[3].z * 10);
            _oscOut.Send(address64, script2.pos2[4].x * 10);
            _oscOut.Send(address65, script2.pos2[4].y * 10);
            _oscOut.Send(address66, script2.pos2[4].z * 10);
            _oscOut.Send(address67, script2.pos2[5].x * 10);
            _oscOut.Send(address68, script2.pos2[5].y * 10);
            _oscOut.Send(address69, script2.pos2[5].z * 10);
            _oscOut.Send(address70, script2.pos2[6].x * 10);
            _oscOut.Send(address71, script2.pos2[6].y * 10);
            _oscOut.Send(address72, script2.pos2[6].z * 10);
            _oscOut.Send(address73, script2.pos2[7].x * 10);
            _oscOut.Send(address74, script2.pos2[7].y * 10);
            _oscOut.Send(address75, script2.pos2[7].z * 10);
            _oscOut.Send(address76, script2.pos2[8].x * 10);
            _oscOut.Send(address77, script2.pos2[8].y * 10);
            _oscOut.Send(address78, script2.pos2[8].z * 10);
            _oscOut.Send(address79, script2.pos2[9].x * 10);
            _oscOut.Send(address80, script2.pos2[9].y * 10);
            _oscOut.Send(address81, script2.pos2[9].z * 10);
            _oscOut.Send(address82, script2.pos2[10].x * 10);
            _oscOut.Send(address83, script2.pos2[10].y * 10);
            _oscOut.Send(address84, script2.pos2[10].z * 10);
            _oscOut.Send(address85, script2.pos2[11].x * 10);
            _oscOut.Send(address86, script2.pos2[11].y * 10);
            _oscOut.Send(address87, script2.pos2[11].z * 10);

            _oscOut.Send(address88, script2.score2[0]);
            _oscOut.Send(address89, script2.score2[3]);
            _oscOut.Send(address90, script2.score2[4]);
            _oscOut.Send(address91, script2.score2[5]);
            _oscOut.Send(address92, script2.score2[6]);
            _oscOut.Send(address93, script2.score2[7]);
            _oscOut.Send(address94, script2.score2[8]);
            _oscOut.Send(address95, script2.score2[9]);
            _oscOut.Send(address96, script2.score2[10]);
            _oscOut.Send(address97, script2.score2[11]);

            _oscOut.Send(address98, script2.pos3[0].x * 10);
            _oscOut.Send(address99, script2.pos3[0].y * 10);
            _oscOut.Send(address100, script2.pos3[0].z * 10);
            _oscOut.Send(address101, script2.pos3[3].x * 10);
            _oscOut.Send(address102, script2.pos3[3].y * 10);
            _oscOut.Send(address103, script2.pos3[3].z * 10);
            _oscOut.Send(address104, script2.pos3[4].x * 10);
            _oscOut.Send(address105, script2.pos3[4].y * 10);
            _oscOut.Send(address106, script2.pos3[4].z * 10);
            _oscOut.Send(address107, script2.pos3[5].x * 10);
            _oscOut.Send(address108, script2.pos3[5].y * 10);
            _oscOut.Send(address109, script2.pos3[5].z * 10);
            _oscOut.Send(address110, script2.pos3[6].x * 10);
            _oscOut.Send(address111, script2.pos3[6].y * 10);
            _oscOut.Send(address112, script2.pos3[6].z * 10);
            _oscOut.Send(address113, script2.pos3[7].x * 10);
            _oscOut.Send(address114, script2.pos3[7].y * 10);
            _oscOut.Send(address115, script2.pos3[7].z * 10);
            _oscOut.Send(address116, script2.pos3[8].x * 10);
            _oscOut.Send(address117, script2.pos3[8].y * 10);
            _oscOut.Send(address118, script2.pos3[8].z * 10);
            _oscOut.Send(address119, script2.pos3[9].x * 10);
            _oscOut.Send(address120, script2.pos3[9].y * 10);
            _oscOut.Send(address121, script2.pos3[9].z * 10);
            _oscOut.Send(address122, script2.pos3[10].x * 10);
            _oscOut.Send(address123, script2.pos3[10].y * 10);
            _oscOut.Send(address124, script2.pos3[10].z * 10);
            _oscOut.Send(address125, script2.pos3[11].x * 10);
            _oscOut.Send(address126, script2.pos3[11].y * 10);
            _oscOut.Send(address127, script2.pos3[11].z * 10);

            _oscOut.Send(address128, script2.score3[0]);
            _oscOut.Send(address129, script2.score3[3]);
            _oscOut.Send(address130, script2.score3[4]);
            _oscOut.Send(address131, script2.score3[5]);
            _oscOut.Send(address132, script2.score3[6]);
            _oscOut.Send(address133, script2.score3[7]);
            _oscOut.Send(address134, script2.score3[8]);
            _oscOut.Send(address135, script2.score3[9]);
            _oscOut.Send(address136, script2.score3[10]);
            _oscOut.Send(address137, script2.score3[11]);

            _oscOut.Send(address138, script2.person1);
            _oscOut.Send(address139, script2.person2);
            _oscOut.Send(address140, script2.person3);
        }
    }
}