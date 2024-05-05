using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

public class HDRLightEstimation : MonoBehaviour
{
    [SerializeField]
    private ARCameraManager arCameraManager;

    Light currentLight;

    public float? brightness { get; private set; }
    public float? colorTemperature { get; private set; }
    public Color? colorCorrection { get; private set; }
    public Vector3? mainLightDirection { get; private set; }
    public Color? mainLightColor { get; private set; }
    public float? mainLightIntensityLumens { get; private set; }
    public SphericalHarmonicsL2? sphericalHarmonics { get; private set; }

    void Awake()
    {
        currentLight = GetComponent<Light>();
    }

    void OnEnable()
    {
        if (arCameraManager != null)
            arCameraManager.frameReceived += FrameChanged;
    }
    void OnDisable()
    {
        if (arCameraManager != null)
            arCameraManager.frameReceived -= FrameChanged;
    }

    //get light info from camera
    void FrameChanged(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            brightness = args.lightEstimation.averageBrightness.Value;
            currentLight.intensity = brightness.Value;
        }
        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            currentLight.colorTemperature = colorTemperature.Value;
        }

        if (args.lightEstimation.colorCorrection.HasValue)
        {
            colorCorrection = args.lightEstimation.colorCorrection.Value;
            currentLight.color = colorCorrection.Value;
        }
        if (args.lightEstimation.mainLightDirection.HasValue)
        {
            mainLightDirection = args.lightEstimation.mainLightDirection;
            currentLight.transform.rotation = Quaternion.LookRotation(mainLightDirection.Value);
        }
        if (args.lightEstimation.mainLightColor.HasValue)
        {
            mainLightColor = args.lightEstimation.mainLightColor;


//android settings
#if PLATFORM_ANDROID
            // ARCore needs to apply energy conservation term (1 / PI) and be placed in gamma
            currentLight.color = mainLightColor.Value / Mathf.PI;
            currentLight.color = currentLight.color.gamma;

            // ARCore returns color in HDR format (can be represented as FP16 and have values above 1.0)
            var camera = arCameraManager.GetComponentInParent<Camera>();
            if (camera == null || !camera.allowHDR)
            {
                Debug.LogWarning($"HDR Rendering is not allowed.  Color values returned could be above the maximum representable value.");
            }
#endif
        }
        if (args.lightEstimation.mainLightIntensityLumens.HasValue)
        {
            mainLightIntensityLumens = args.lightEstimation.mainLightIntensityLumens;
            currentLight.intensity = args.lightEstimation.averageMainLightBrightness.Value;
        }
        if (args.lightEstimation.ambientSphericalHarmonics.HasValue)
        {
            sphericalHarmonics = args.lightEstimation.ambientSphericalHarmonics;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientProbe = sphericalHarmonics.Value;
        }
    }
}