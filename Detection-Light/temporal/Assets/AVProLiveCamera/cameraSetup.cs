
using UnityEngine;
using RenderHeads.Media.AVProLiveCamera;

public class CameraSetup : MonoBehaviour
{
    [Range(0f,1f)]
    public float Gain = 0;
    void Start()
   // void Update()
    {
        
       
        AVProLiveCameraDevice LiveCamera = AVProLiveCameraManager.Instance.GetDevice(0);
        for (int j = 0; j < LiveCamera.NumSettings; j++)
        {
            AVProLiveCameraSettingBase settingBase = LiveCamera.GetVideoSettingByIndex(j);
           
            settingBase.IsAutomatic = false;
            settingBase.SetDefault();
          

        }
        AVProLiveCameraDevice LiveCamera2 = AVProLiveCameraManager.Instance.GetDevice(1);

        for (int j = 0; j < LiveCamera2.NumSettings; j++)
        {
            AVProLiveCameraSettingBase settingBase = LiveCamera2.GetVideoSettingByIndex(j);
            
            settingBase.IsAutomatic = false;
            settingBase.SetDefault();
        }
        AVProLiveCameraDevice LiveCamera3 = AVProLiveCameraManager.Instance.GetDevice(2);

        for (int j = 0; j < LiveCamera3.NumSettings; j++)
        {
            AVProLiveCameraSettingBase settingBase = LiveCamera3.GetVideoSettingByIndex(j);
           
            settingBase.IsAutomatic = false;
            settingBase.SetDefault();
        }
    }
     void Update()
    {
        AVProLiveCameraDevice LiveCamera = AVProLiveCameraManager.Instance.GetDevice(0);
        AVProLiveCameraSettingBase gainSetting = LiveCamera.GetVideoSettingByIndex(6);
        AVProLiveCameraSettingFloat settingFloat = (AVProLiveCameraSettingFloat)gainSetting;
        settingFloat.CurrentValue = 70 * Gain;
        AVProLiveCameraDevice LiveCamera2 = AVProLiveCameraManager.Instance.GetDevice(1);
        AVProLiveCameraSettingBase gainSetting2 = LiveCamera2.GetVideoSettingByIndex(6);
        AVProLiveCameraSettingFloat settingFloat2 = (AVProLiveCameraSettingFloat)gainSetting2;
        settingFloat2.CurrentValue = 70 * Gain;
        AVProLiveCameraDevice LiveCamera3 = AVProLiveCameraManager.Instance.GetDevice(2);
        AVProLiveCameraSettingBase gainSetting3 = LiveCamera3.GetVideoSettingByIndex(6);
        AVProLiveCameraSettingFloat settingFloat3 = (AVProLiveCameraSettingFloat)gainSetting3;
        settingFloat3.CurrentValue = 70 * Gain;
        /*if(Time.time>0.1f)
        {
            LiveCamera.UpdateSettings = false;
            LiveCamera2.UpdateSettings = false;
            LiveCamera3.UpdateSettings = false;
        } */

    }
}