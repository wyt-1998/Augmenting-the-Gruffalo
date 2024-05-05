using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.Rendering;

public class LightEstimation : MonoBehaviour
{
    [SerializeField] private ARCameraManager arCameraManager;

    //show 3 values from camera on screen: brightness, color temperature, color correction
    [SerializeField] private TMP_Text brightnessVal;
    [SerializeField] private TMP_Text tempVal;
    [SerializeField] private TMP_Text colorCorrectionVal;


    [SerializeField] private Light currentLight; //light

    [SerializeField] Transform m_Arrow;

    public Transform arrow
    {
        get => m_Arrow;
        set => m_Arrow = value;
    }


    public Vector3? mainLightDirection { get; private set; }
    public Color? mainLightColor { get; private set; }
    public float? mainLightIntensityLumens { get; private set; }
    //public SphericalHarmonicsL2? sphericalHarmonics { get; private set; }



    //get light component
    private void Awake()
    {
        currentLight = GetComponent<Light>();
    }

    private void OnEnable()
    {
        arCameraManager.frameReceived += FrameUpdated;

        // Disable the arrow to start; enable it later if we get directional light info
        if (arrow)
        {
            arrow.gameObject.SetActive(false);
        }
        Application.onBeforeRender += OnBeforeRender;
    }

    private void OnDisable()
    {
        Application.onBeforeRender -= OnBeforeRender;
        arCameraManager.frameReceived -= FrameUpdated;
    }

    void OnBeforeRender()
    {
        if (arrow && arCameraManager)
        {
            var cameraTransform = arCameraManager.GetComponent<Camera>().transform;
            arrow.position = cameraTransform.position + cameraTransform.forward * .25f;
        }
    }

    private void FrameUpdated(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            currentLight.intensity = args.lightEstimation.averageBrightness.Value;
            //show on screen to test
            /*brightnessVal.text = $"Brightness: {currentLight.intensity}";
        } else
        {
            brightnessVal.text = $"Brightness: {"null"}";
            */
        }

        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            currentLight.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            //testing
            tempVal.text = $"Color Temperature: {currentLight.colorTemperature}";
        }
        else
        {
            tempVal.text = $"Color Temperature: {"null"}";
        }

        /*
        if (args.lightEstimation.colorCorrection.HasValue)
        {
            currentLight.color = args.lightEstimation.colorCorrection.Value;
            //testing
            colorCorrectionVal.text = $"Color: {currentLight.color}";
        }else
        {
            colorCorrectionVal.text = $"Color: {"null"}";
        }
        */

        if (args.lightEstimation.mainLightDirection.HasValue)
        {
            mainLightDirection = args.lightEstimation.mainLightDirection;
            currentLight.transform.rotation = Quaternion.LookRotation(mainLightDirection.Value);
            if (arrow)
            {
                arrow.gameObject.SetActive(true);
                arrow.rotation = Quaternion.LookRotation(mainLightDirection.Value);
            }

        }
        else if (arrow)
        {
            arrow.gameObject.SetActive(false);
            mainLightDirection = null;
        }

        if (args.lightEstimation.mainLightColor.HasValue)
        {
            mainLightColor = args.lightEstimation.mainLightColor;
            currentLight.color = mainLightColor.Value;
            //color
            colorCorrectionVal.text = $"Color: {currentLight.color}";
        }
        else
        {
            mainLightColor = null;
        }


        if (args.lightEstimation.mainLightIntensityLumens.HasValue)
        {
            mainLightIntensityLumens = args.lightEstimation.mainLightIntensityLumens;
            currentLight.intensity = args.lightEstimation.averageMainLightBrightness.Value;
            //intensity
            brightnessVal.text = $"Brightness: {currentLight.intensity}";
        }
        else
        {
            mainLightIntensityLumens = null;
        }

        /*
        if (args.lightEstimation.ambientSphericalHarmonics.HasValue)
        {
            sphericalHarmonics = args.lightEstimation.ambientSphericalHarmonics;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientProbe = sphericalHarmonics.Value;
        }
        else
        {
            sphericalHarmonics = null;
        }
        */
    }
}
