using UnityEngine;
using System;

namespace neoludicGames.uDmx
{
    [ExecuteAlways]
    public class DmxLightSource : MonoBehaviour
    {
        [Header("DMX Settings")] 
        [Range(1, 512)] [SerializeField] private int dmxAddress = 1;
        [SerializeField] public DmxLightProfile lightProfile;
        
        [Header("Light Settings")] 
        [SerializeField] [Range(Byte.MinValue, Byte.MaxValue)] private int lightStrength = Byte.MaxValue;
        [SerializeField] private Color lightColor = Color.white;
        [SerializeField] [Range(Byte.MinValue, Byte.MaxValue)] private int strobe, x, y, z;

        [Header("Preview Settings")] 
        [SerializeField] private Light previewLight;
        [SerializeField] private SpriteRenderer previewSprite;

        public int LightAddress => dmxAddress;
        public void SetLightAddress(int value) => dmxAddress = value;

        public void SetColor(Color newColor) => lightColor = newColor;
        public void SetStrength(float newStrength) => lightStrength = FloatToByte(newStrength);

        public void SetStrobe(float value) => strobe = FloatToByte(value);
        public void SetX(float value) => x = FloatToByte(value);
        public void SetY(float value) => y = FloatToByte(value);
        public void SetZ(float value) => z = FloatToByte(value);

        public string LightModelName => lightProfile ? lightProfile.modelName : "";
        public int LightChannels => lightProfile ? lightProfile.channelCount : -1;
        
        [ExecuteAlways]
        private void Update()
        {
            if (lightProfile) ApplyLighting();
            if (previewLight) PreviewDmxLight();
            if (previewSprite) PreviewDmxSprite();
        }

        [ExecuteAlways]
        private void OnDisable()
        {
            if (lightProfile) DmxController.SetLightData(dmxAddress,lightProfile.ToBytes(Color.black, 0,0,0,0,0));
        }

        private void ApplyLighting() =>
            DmxController.SetLightData(dmxAddress,
                lightProfile.ToBytes(lightColor, (byte) lightStrength, (byte) strobe, (byte) x, (byte) y, (byte) z));

        private void PreviewDmxLight()
        {
            previewLight.color = lightColor;
            previewLight.intensity = ByteToFloat((byte) lightStrength);
        }
        private void PreviewDmxSprite()
        {
            Color c = lightColor;
            c.a = ByteToFloat((byte) lightStrength);
            previewSprite.color = c;
        }
        
        private byte FloatToByte(float value) => (byte) Mathf.RoundToInt(value * Byte.MaxValue);
        private float ByteToFloat(byte value) => (float) value / (float) byte.MaxValue;
    }
}