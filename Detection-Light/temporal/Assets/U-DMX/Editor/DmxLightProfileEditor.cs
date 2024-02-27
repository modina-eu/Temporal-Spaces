using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace neoludicGames.uDmx.Editor
{
    [CustomEditor(typeof(DmxLightProfile))]
    public class DmxLightProfileEditor : UnityEditor.Editor
    {
        SerializedProperty channelCount;
        private SerializedProperty[] baseChannels, remapableChannels, channelNames;

        void OnEnable()
        {
            channelCount = serializedObject.FindProperty("channelCount");
            string[] channelStrings = new string[] {"master", "red", "green", "blue", "strobe","x","y","z"};
            baseChannels = new SerializedProperty[channelStrings.Length];

            for (int i = 0; i < channelStrings.Length; i++)
                baseChannels[i] = serializedObject.FindProperty(channelStrings[i] + "Channel");
        }

        private SerializedProperty GetProperty(string prefix) => serializedObject.FindProperty(prefix + "Channel");

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("modelName"));

            EditorGUILayout.PropertyField(channelCount);
            channelCount.intValue = Mathf.Clamp(channelCount.intValue, 0, 32);

            foreach (var channel in baseChannels)
            {
                EditorGUILayout.IntSlider(channel, 0, channelCount.intValue,
                    channel.intValue == 0 ? channel.name + " - off" : channel.name);
            }
            
            EditorGUILayout.HelpBox("Set any of the channels to zero in order to turn it off. Do this for all channels you don't want to control or can't control with this light fixture model.", MessageType.Info);

            if (GUILayout.Button("Reset All"))
            {
                foreach (var channel in baseChannels)
                {
                    channel.intValue = 0;
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("notes"));
            if (baseChannels[0].intValue <= 0)
                EditorGUILayout.HelpBox("Light strength will be baked into the color channels", MessageType.Info);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}