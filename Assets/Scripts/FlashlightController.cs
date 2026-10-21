using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Light))]
public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight")]
    public bool startsOn = false;

    private Light flashlight;

    private void Awake()
    {
        flashlight = GetComponent<Light>();

        flashlight.type = LightType.Spot;

        flashlight.enabled = startsOn;
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }
}