using System;
using System.Collections.Generic;
using UnityEngine;
namespace neoludicGames.uDmx
{
    [CreateAssetMenu(menuName = "U-DMX/DMX Light Profile")]
    public class DmxLightProfile : ScriptableObject
    {
        [Header("Device Settings")] public string modelName;
        [Header("Channel Settings")] [Tooltip("The channel count of the DMX light.")]
        public int channelCount = 8;

        [Tooltip("The channel relative to the DMX Light's address. Please refer to the DMX fixture's manual to configure these appropriately for every device. Set the channel to 0 to exclude it.")] [SerializeField]
        int masterChannel = 0, redChannel = 1, greenChannel = 2, blueChannel = 3,
            strobeChannel = 0, xChannel = 0, yChannel = 0, zChannel = 0;
        
#if UNITY_EDITOR
        [Tooltip("Feel free to take editor-only notes here.")]
        [SerializeField, TextArea(5, 10)] string notes;
#endif

        
        private const int OFF_CHANNEL = 0;
        private const int CHANNEL_OFFSET = 1;
        
        private bool mixStrengthWithColors => masterChannel == -1;

/// <summary>
/// Takes the model independent light information and converts it to the appropriate channels and values ready to be sent off to the DMX interface.
/// </summary>
/// <param name="color"></param>
/// <param name="strength"></param>
/// <param name="strobe"></param>
/// <param name="x"></param>
/// <param name="y"></param>
/// <param name="z"></param>
/// <returns>Byte values for every channel of this light fixture</returns>
        public byte[] ToBytes(Color color, byte strength, byte strobe, byte x, byte y, byte z)
        {
            Color resultColor = GetAdjustedColor(color, strength);
            byte[] bytes = new byte[channelCount];
            if (masterChannel > OFF_CHANNEL) bytes[masterChannel -CHANNEL_OFFSET] = strength;
            if (redChannel > OFF_CHANNEL) bytes[redChannel-CHANNEL_OFFSET] = (byte)Mathf.RoundToInt(resultColor.r * byte.MaxValue);
            if (greenChannel > OFF_CHANNEL) bytes[greenChannel-CHANNEL_OFFSET] = (byte)Mathf.RoundToInt(resultColor.g * byte.MaxValue);
            if (blueChannel > OFF_CHANNEL) bytes[blueChannel-CHANNEL_OFFSET] = (byte)Mathf.RoundToInt(resultColor.b * byte.MaxValue);
            ApplyChannel(bytes,strobeChannel,strobe);
            ApplyChannel(bytes,xChannel,x);
            ApplyChannel(bytes,yChannel,y);
            ApplyChannel(bytes,zChannel,z);
            return bytes;
        }
        public byte[] ToBytes2( byte strength)
        {
            //Color resultColor = GetAdjustedColor(color, strength);
            byte[] bytes = new byte[channelCount];
            if (masterChannel > OFF_CHANNEL) bytes[masterChannel - CHANNEL_OFFSET] = strength;
           // if (redChannel > OFF_CHANNEL) bytes[redChannel - CHANNEL_OFFSET] = (byte)Mathf.RoundToInt(resultColor.r * byte.MaxValue);
            //if (greenChannel > OFF_CHANNEL) bytes[greenChannel - CHANNEL_OFFSET] = (byte)Mathf.RoundToInt(resultColor.g * byte.MaxValue);
           // if (blueChannel > OFF_CHANNEL) bytes[blueChannel - CHANNEL_OFFSET] = (byte)Mathf.RoundToInt(resultColor.b * byte.MaxValue);
           // ApplyChannel(bytes, strobeChannel, strobe);
           // ApplyChannel(bytes, xChannel, x);
           // ApplyChannel(bytes, yChannel, y);
           // ApplyChannel(bytes, zChannel, z);
            return bytes;
        }
        private Color GetAdjustedColor(Color input, byte strength = 255)
        {
            if (mixStrengthWithColors)
            {
                float a = ((float) strength) / 255f;
                input *= a;
            }

            return input;
        }

        private void ApplyChannel(byte[] bytes, int channel, byte value)
        {
            if (channel > OFF_CHANNEL) bytes[channel - CHANNEL_OFFSET] = value;
        }
    }
}