using UnityEngine;
using System;

namespace neoludicGames.uDmx
{
    [ExecuteAlways]
    public class DmxLightSourceTemporal : MonoBehaviour
    {
        [Header("DMX Settings")]
        [Range(1, 512)] [SerializeField] private int[] dmxAddresses;

        [SerializeField] public DmxLightProfile lightProfile;
        [SerializeField] [Range(Byte.MinValue, Byte.MaxValue)] private int[] lightStrengths;

        public int LightAddress => dmxAddresses.Length > 0 ? dmxAddresses[0] : 0;

        /* public void SetLightAddress(int[] values)
         {
             dmxAddresses = values;
             // Ensure that the lightStrengths array has the same length as dmxAddresses
           //  Array.Resize(ref lightStrengths, dmxAddresses.Length);
         }   */
        void Start()
        {
            for (int i = 0; i < dmxAddresses.Length; i++)
            {
                dmxAddresses[i] = i + 1;
            }
        }
        public void SetStrength(int index, float newStrength)
        {
            if (index >= 0 && index < lightStrengths.Length)
            {
                lightStrengths[index] = FloatToByte(newStrength);
            }
        }

        public string LightModelName => lightProfile ? lightProfile.modelName : "";
        public int LightChannels => lightProfile ? lightProfile.channelCount : -1;

        [ExecuteAlways]
        private void Update()
        {
            if (lightProfile) ApplyLighting();
        }

        [ExecuteAlways]
        private void OnDisable()
        {
            if (dmxAddresses != null)
            {
                for (int i = 0; i < dmxAddresses.Length; i++)
                {
                    if (lightProfile)
                        DmxController.SetLightData(dmxAddresses[i], lightProfile.ToBytes2(0));
                }
            }
        }

        private void ApplyLighting()
        {
            if (dmxAddresses != null && lightStrengths != null)
            {
                for (int i = 0; i < Mathf.Min(dmxAddresses.Length, lightStrengths.Length); i++)
                {
                    if (lightProfile)
                        DmxController.SetLightData(dmxAddresses[i], lightProfile.ToBytes2((byte)lightStrengths[i]));
                }
            }
        }

        private byte FloatToByte(float value) => (byte)Mathf.RoundToInt(value * Byte.MaxValue);
        private float ByteToFloat(byte value) => (float)value / (float)byte.MaxValue;
    }
}
