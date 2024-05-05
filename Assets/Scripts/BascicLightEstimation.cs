using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class BasicLightEstimation : MonoBehaviour
{
    [SerializeField] private ARCameraManager arCameraManager;

    //show 3 values from camera on screen: brightness, color temperature, color correction
    [SerializeField] private TMP_Text brightnessVal;
    [SerializeField] private TMP_Text tempVal;
    [SerializeField] private TMP_Text colorCorrectionVal;


    [SerializeField] private Light currentLight;

    //get light component
    private void Awake()
    {
        currentLight = GetComponent<Light>();
    }

    private void OnEnable()
    {
        arCameraManager.frameReceived += FrameUpdated;
    }

    private void OnDisable()
    {
        arCameraManager.frameReceived -= FrameUpdated;
    }

    private void FrameUpdated(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            currentLight.intensity = args.lightEstimation.averageBrightness.Value;
            //show on screen to test
            brightnessVal.text = $"Brightness: {currentLight.intensity}";
        } else
        {
            brightnessVal.text = $"Brightness: {"null"}";
        }

        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            currentLight.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            //testing
            tempVal.text = $"Color Temperature: {currentLight.colorTemperature}";
        }else
        {
            tempVal.text = $"Color Temperature: {"null"}";
        }

        if (args.lightEstimation.colorCorrection.HasValue)
        {
            currentLight.color = args.lightEstimation.colorCorrection.Value;
            //testing
            colorCorrectionVal.text = $"Color: {currentLight.color}";
        }else
        {
            colorCorrectionVal.text = $"Color: {"null"}";
        }
    }

}
