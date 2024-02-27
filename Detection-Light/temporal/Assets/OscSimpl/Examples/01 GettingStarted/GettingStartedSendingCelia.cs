using UnityEngine;

namespace OscSimpl.Examples
{
	public class GettingStartedSendingCelia : MonoBehaviour
	{
        [SerializeField] OscOut _oscOut;
        OscMessage _message2; // Cached message.

        private int Nbr_portOut;
        //"nose", "leftShoulder", "rightShoulder", "leftElbow", "rightElbow", "leftWrist", "rightWrist",  "Hip", "leftKnee", "rightKnee", "leftAnkle", "rightAnkle"

        public string address0 = "/P0X";
        public string address1 = "/P0Y";
        public string address2 = "/P1X";
        public string address3 = "/P1Y";
        public string address4 = "/P2X";
        public string address5 = "/P2Y";
        public string address6 = "/P3X";
        public string address7 = "/P3Y";
        public string address8 = "/P4X";
        public string address9 = "/P4Y";
        public string address10 = "/NbrGhost";
        public string address11 = "/PS0X";
        public string address12 = "/PS0Y";
        public string address13 = "/PS1X";
        public string address14 = "/PS1Y";
        public string address15 = "/PS2X";
        public string address16 = "/PS2Y";
        public string address17 = "/PS3X";
        public string address18 = "/PS3Y";
        public string address19 = "/PS4X";
        public string address20 = "/PS4Y";
        public string address21 = "/PS5X";
        public string address22 = "/PS5Y";
        public string address23 = "/PS6X";
        public string address24 = "/PS6Y";
        public string address25 = "/PS7X";
        public string address26 = "/PS7Y";
        public string address27 = "/PS8X";
        public string address28 = "/PS8Y";
        public string address29 = "/PS9X";
        public string address30 = "/PS9Y";
        public string address31 = "/PS10X";
        public string address32 = "/PS10Y";
        public string address33 = "/PS11X";
        public string address34 = "/PS11Y";
        public string address35 = "/PS12X";
        public string address36 = "/PS12Y";
        public string adresse37 = "/ScoreGhost";


        private string LocalIPTarget;
        public PoseEstimator1 script2;
        public TextureComparator resnet;
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

         
            _oscOut.Send(address0, script2.pn0.x);
            _oscOut.Send(address1, 1f-script2.pn0.y);
            _oscOut.Send(address2, script2.pn1.x);
            _oscOut.Send(address3, 1f - script2.pn1.y);
            _oscOut.Send(address4, script2.pn2.x);
            _oscOut.Send(address5, 1f - script2.pn2.y);
            _oscOut.Send(address6, script2.pn3.x);
            _oscOut.Send(address7, 1f - script2.pn3.y);
            _oscOut.Send(address8, script2.pn4.x);
            _oscOut.Send(address9, 1f - script2.pn4.y);
            _oscOut.Send(address10, resnet.result);
            _oscOut.Send(address11, script2.pns[0].x);
            _oscOut.Send(address12, 1f - script2.pns[0].y);
            _oscOut.Send(address13, script2.pns[1].x);
            _oscOut.Send(address14, 1f - script2.pns[1].y);
            _oscOut.Send(address15, script2.pns[2].x);
            _oscOut.Send(address16, 1f - script2.pns[2].y);
            _oscOut.Send(address17, script2.pns[3].x);
            _oscOut.Send(address18, 1f - script2.pns[3].y);
            _oscOut.Send(address19, script2.pns[4].x);
            _oscOut.Send(address20, 1f - script2.pns[4].y);
            _oscOut.Send(address21, script2.pns[5].x);
            _oscOut.Send(address22, 1f - script2.pns[5].y);
            _oscOut.Send(address23, script2.pns[6].x);
            _oscOut.Send(address24, 1f - script2.pns[6].y);
            _oscOut.Send(address25, script2.pns[7].x);
            _oscOut.Send(address26, 1f - script2.pns[7].y);
            _oscOut.Send(address27, script2.pns[8].x);
            _oscOut.Send(address28, 1f - script2.pns[8].y);
            _oscOut.Send(address29, script2.pns[9].x);
            _oscOut.Send(address30, 1f - script2.pns[9].y);
            _oscOut.Send(address31, script2.pns[10].x);
            _oscOut.Send(address32, 1f - script2.pns[10].y);
            _oscOut.Send(address33, script2.pns[11].x);
            _oscOut.Send(address34, 1f - script2.pns[11].y);
            _oscOut.Send(address35, script2.pns[12].x);
            _oscOut.Send(address36, 1f - script2.pns[12].y);
            _oscOut.Send(adresse37, resnet.score);

        }
    }
}