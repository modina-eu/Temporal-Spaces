using System;
using UnityEditor;
using UnityEngine;

namespace neoludicGames.uDmx.Editor
{
    [CustomEditor(typeof(DmxLightSource))]
    public class DmxLightSourceEditor : UnityEditor.Editor
    {
        private DmxLightSource _lightSource;
        private DmxLightProfile _lightProfile;

        private void OnEnable()
        {
            _lightSource = target as DmxLightSource;
        }

        public override void OnInspectorGUI()
        {
            _lightProfile = _lightSource.lightProfile;
            serializedObject.Update();
            if (_lightSource) EditorGUILayout.HelpBox(GetInfoText(_lightSource), MessageType.None);
            DrawDefaultInspector();
            
            if (!DmxController.isDmxSenderFunctional)
                EditorGUILayout.HelpBox("Your USB-DMX sender seems to not be operational", MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
        }

        private string GetInfoText(DmxLightSource lightSource)
        {
            string infoText = "DMX Light Source Info:" + Environment.NewLine;
            if (lightSource.LightModelName.Length > 1) infoText += lightSource.LightModelName + Environment.NewLine;
            infoText += string.Format("DMX Address: {0}", lightSource.LightAddress) + Environment.NewLine;
            if (lightSource.LightChannels > 0) infoText += string.Format("Channels: {0}", lightSource.LightChannels);
            return infoText;
        }
    }
}