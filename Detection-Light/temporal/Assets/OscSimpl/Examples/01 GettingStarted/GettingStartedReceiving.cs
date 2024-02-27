/*
	Created by Carl Emil Carlsen.
	Copyright 2016-2018 Sixth Sensor.
	All rights reserved.
	http://sixthsensor.dk
*/

using UnityEngine;

namespace OscSimpl.Examples
{
    public class GettingStartedReceiving : MonoBehaviour
    {
        [SerializeField] OscIn _oscIn;

        public string address1 = "/P1HandLeftSpeed";
        public string address2 = "/P1HandRightSpeed";
        public string address3 = "/P2HandLeftSpeed";
        public string address4 = "/P2HandRightSpeed";
        public string address5 = "/P3HandLeftSpeed";
        public string address6 = "/P3HandRightSpeed";
        public string address7 = "/StartR";
        public string address8 = "/StopR";
        public string address9 = "/Phase2";
        public string address10 = "/NbrR";
        public string address11 = "/P1AnkleLeftSpeed";
        public string address12 = "/P1AnkleRightSpeed";
        public string address13 = "/PAnkleLeftSpeed";
        public string address14 = "/P2AnkleRightSpeed";
        public string address15 = "/P3AnkleLeftSpeed";
        public string address16 = "/P3AnkleRightSpeed";
        public string address17 = "/MaxSpeed";
        float _incomingFloat;
        private int Nbr_portIn;
        public float HandL;
        public float HandR;
        public float HandL2;
        public float HandR2;
        public float HandL3;
        public float HandR3;
        public float startR;
        public float playR;
        public float Phase2;
        public float NbrR;
        public float AnkleL;
        public float AnkleR;
        public float AnkleL2;
        public float AnkleR2;
        public float AnkleL3;
        public float AnkleR3;
        public float MaxSpeed;
        void Start()
        {
            Nbr_portIn = _oscIn.port;
            // Ensure that we have a OscIn component and start receiving on port 7000.
            if (!_oscIn) _oscIn = gameObject.AddComponent<OscIn>();
            _oscIn.Open(Nbr_portIn);
        }

        void OnEnable()
        {
            // You can "map" messages to methods in two ways:
            // 1) For messages with a single argument, route the value using the type specific map methods.
            /////// EVENEMENT MAPPING_oscIn.MapFloat( address1, Event1 );
            // 2) For messages with multiple arguments, route the message using the Map method.
            //_oscIn.Map( address2, OnCusto );
            _oscIn.MapFloat(address1, In_Trigger1);
            _oscIn.MapFloat(address2, In_Trigger2);
            _oscIn.MapFloat(address3, In_Trigger3);
            _oscIn.MapFloat(address4, In_Trigger4);
            _oscIn.MapFloat(address5, In_Trigger5);
            _oscIn.MapFloat(address6, In_Trigger6);
            _oscIn.MapFloat(address7, In_Trigger7);
            _oscIn.MapFloat(address8, In_Trigger8);
            _oscIn.MapFloat(address9, In_Trigger9);
            _oscIn.MapFloat(address10, In_Trigger10);
            _oscIn.MapFloat(address11, In_Trigger11);
            _oscIn.MapFloat(address12, In_Trigger12);
            _oscIn.MapFloat(address13, In_Trigger13);
            _oscIn.MapFloat(address14, In_Trigger14);
            _oscIn.MapFloat(address15, In_Trigger15);
            _oscIn.MapFloat(address16, In_Trigger16);
            _oscIn.MapFloat(address17, In_Trigger17);
        }
        void In_Trigger1(float value)
        {
           HandL = value ;
        }
        void In_Trigger2(float value)
        {
           HandR = value;
        }
        void In_Trigger3(float value)
        {
            HandL2 = value;
        }
        void In_Trigger4(float value)
        {
            HandR2 = value;
        }
        void In_Trigger5(float value)
        {
            HandL3 = value;
        }
        void In_Trigger6(float value)
        {
            HandR3 = value;
        }
        void In_Trigger7(float value)
        {
            startR = value;
        }
        void In_Trigger8(float value)
        {
            playR = value;
        }
        void In_Trigger9(float value)
        {
            Phase2 = value;
        }
        void In_Trigger10(float value)
        {
            NbrR = value;
        }
        void In_Trigger11(float value)
        {
            AnkleL = value;
        }
        void In_Trigger12(float value)
        {
            AnkleR = value;
        }
        void In_Trigger13(float value)
        {
            AnkleL2 = value;
        }
        void In_Trigger14(float value)
        {
            AnkleR2 = value;
        }
        void In_Trigger15(float value)
        {
            AnkleL3 = value;
        }
        void In_Trigger16(float value)
        {
            AnkleR3 = value;
        }
        void In_Trigger17(float value)
        {
            MaxSpeed = value;
        }
        void OnDisable()
        {
            // If you want to stop receiving messages you have to "unmap".
            _oscIn.UnmapFloat(In_Trigger1);
          //  _oscIn.Unmap(OnTest2);
        }
        void Test1(OscMessage incomingMessage)
        {
            if (incomingMessage.TryGet(0, out _incomingFloat))
            {
                Debug.Log("ConditionOk");
            }
            //  Debug.Log("Received f7/f1" + "\n");
        }
    }
}