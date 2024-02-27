using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace neoludicGames.uDmx
{
    [ExecuteAlways]
    public class DmxController : MonoBehaviour
    {
        /// <summary>
        /// Returns the state of the DMX sender. Note that it returns false, both if there is no DMX Controller in the scene, and if there is a problem with the networked connection.
        /// </summary>
        public static bool isDmxSenderFunctional => _instance ? _instance.dmxSenderIsFunctional : false;
        
        /// <summary>
        /// Event being invoked if the connection to the dmx interface, or something with the dmx interface itself changes.
        /// </summary>
        public static UnityEvent<bool> dmxStateChanged;
        
        [Tooltip("This is the URI address of the of the USB-DMX Server. It can be on this computer, as in the case of localhost, it can reference a device on your local network, or even a remote device.")]
        [SerializeField] private string uri = "http://localhost:14444";
        [SerializeField] private bool updateLightsDuringEditMode = true;
        [SerializeField] private UnityEvent dmxFunctioning, dmxNotFunctioning;
        
        private static byte[] _lightData = new Byte[513];
        private static bool _readyToSend = true;
        private static bool _dirty = true;
        private static DmxController _instance;
        private YieldInstruction _sendingDelay = null;
        private bool dmxSenderIsFunctional = false;
        
        public void SetLightServerURL(string url) => uri = url;
        public void SetLightServerURL(string url, int port) => uri = string.Format("{0}:{1}", url, port);
        
        /// <summary>
        /// DMX style start value (1-512) and a variable list of attributes for the subsequent channels.
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="values"></param>
        public static void SetLightData(int startValue, params byte[] values)
        {
            startValue--;
            if (_lightData == null || _lightData.Length < 513) _lightData = new byte[513];
            for (int i = startValue; i < startValue + values.Length && i < _lightData.Length; i++)
                _lightData[i] = values[i - startValue];
            _dirty = true;
            if (!Application.isPlaying) EditorLightDataPreview();
        }

        /// <summary>
        /// Responsible for synchronizing the lights during Edit mode.
        /// </summary>
        private static void EditorLightDataPreview()
        {
            if (!_instance) _instance = GameObject.FindObjectOfType<DmxController>();
            if (!_instance) Debug.LogWarning("A DMX controller has to be in the scene");
            else if (_instance.updateLightsDuringEditMode && _dirty && _readyToSend)
                _instance.StartCoroutine(_instance.SendValues());
        }
        
        [ExecuteAlways]
        void Awake()
        {
            _instance = this;
            _lightData = new byte[513];
            dmxNotFunctioning?.Invoke();
            StartCoroutine(SendValues());
        }

        private void Update()
        {
            if (Application.isPlaying && _dirty && _readyToSend) StartCoroutine(SendValues());
        }

        private void SetDMXState(bool state, string message = "")
        {
            if (state != dmxSenderIsFunctional)
            {
                dmxStateChanged?.Invoke(state);
                if (state) dmxFunctioning?.Invoke();
                else dmxNotFunctioning?.Invoke();
            }
            dmxSenderIsFunctional = state;
        }

        IEnumerator SendValues()
        {
            _readyToSend = false;
            _dirty = false;
            yield return StartCoroutine(SendValuesUnityWebRequest());
            _readyToSend = true;
        }
        
        private IEnumerator SendValuesUnityWebRequest()
        {
            using (UnityWebRequest www = UnityWebRequest.Put(uri, _lightData))
            {
                yield return www.SendWebRequest();
                yield return _sendingDelay;
                
                #if UNITY_2021_1_OR_NEWER
                if (www.result != UnityWebRequest.Result.Success) SetDMXState(false, "Network Error");
                else if (www.downloadHandler.data == null || www.downloadHandler.data.Length == 0 ||
                         www.downloadHandler.data[0] == byte.MinValue) SetDMXState(false, "USB Error");
                else SetDMXState(true);
                #else
                if (www.result == UnityWebRequest.Result.ConnectionError) SetDMXState(false, "Network Error");
                else if (www.downloadHandler.data == null || www.downloadHandler.data.Length == 0 ||
                         www.downloadHandler.data[0] == byte.MinValue) SetDMXState(false, "USB Error");
                else SetDMXState(true);
                #endif
            }
        }
    }
}