// - ElectricTorchOnOff - Script by Marcelli Michele

// This script is attached in primary model (default) of the Electric Torch.
// You can On/Off the light and choose any letter on the keyboard to control it
// Use the "battery" or no and the duration time
// Change the intensity of the light

using UnityEngine;

public class ElectricTorchOnOff : MonoBehaviour
{
    EmissionMaterialGlassTorchFadeOut _emissionMaterialFade;
    BatteryPowerPickup _batteryPower;
    //

    public enum LightChoose
    {
        noBattery,
        withBattery
    }

    public LightChoose modoLightChoose;
    [Space]
    [Space]
    public string onOffLightKey = "F";
    private KeyCode _kCode;
    [Space]
    [Space]
    public bool _PowerPickUp = false;
    [Space]
    public float intensityLight = 2.5F;
    private bool _flashLightOn = false;
    [SerializeField] float _lightTime = 0.05f;

    private void Awake()
    {
        // Trova il componente BatteryPowerPickup nella scena
        _batteryPower = FindObjectOfType<BatteryPowerPickup>();
        if (_batteryPower == null)
        {
            Debug.LogError("Nessun oggetto 'BatteryPowerPickup' trovato nella scena.");
        }
    }

    void Start()
    {
        // Trova l'oggetto 'Torch/default' e il suo script EmissionMaterialGlassTorchFadeOut
        GameObject _scriptControllerEmissionFade = GameObject.Find("Torch/default");

        if (_scriptControllerEmissionFade != null)
        {
            _emissionMaterialFade = _scriptControllerEmissionFade.GetComponent<EmissionMaterialGlassTorchFadeOut>();
            if (_emissionMaterialFade == null)
            {
                Debug.LogError("EmissionMaterialGlassTorchFadeOut non trovato su 'Torch/default'.");
            }
        }
        else
        {
            Debug.LogError("Oggetto 'Torch/default' non trovato nella scena.");
        }

        // Parsing della stringa in KeyCode
        if (!System.Enum.TryParse(onOffLightKey, out _kCode))
        {
            Debug.LogError("Errore nel parsing della chiave per l'accensione/spegnimento. Impostata la chiave di default 'F'.");
            _kCode = KeyCode.F; // Imposta un valore di fallback
        }
    }

    void Update()
    {
        // Gestione dell'input della tastiera
        InputKey();

        switch (modoLightChoose)
        {
            case LightChoose.noBattery:
                NoBatteryLight();
                break;
            case LightChoose.withBattery:
                WithBatteryLight();
                break;
        }
    }

    void InputKey()
    {
        if (Input.GetKeyDown(_kCode) && _flashLightOn == true)
        {
            _flashLightOn = false;
        }
        else if (Input.GetKeyDown(_kCode) && _flashLightOn == false)
        {
            _flashLightOn = true;
        }
    }

    void NoBatteryLight()
    {
        if (_emissionMaterialFade != null)
        {
            Light lightComponent = GetComponent<Light>();
            if (lightComponent != null)
            {
                if (_flashLightOn)
                {
                    lightComponent.intensity = intensityLight;
                    _emissionMaterialFade.OnEmission();
                }
                else
                {
                    lightComponent.intensity = 0.0f;
                    _emissionMaterialFade.OffEmission();
                }
            }
            else
            {
                Debug.LogError("Componente 'Light' non trovato sull'oggetto.");
            }
        }
        else
        {
            Debug.LogError("EmissionMaterialGlassTorchFadeOut non è stato trovato.");
        }
    }

    void WithBatteryLight()
    {
        if (_emissionMaterialFade != null)
        {
            Light lightComponent = GetComponent<Light>();
            if (lightComponent != null)
            {
                if (_flashLightOn)
                {
                    lightComponent.intensity = intensityLight;
                    intensityLight -= Time.deltaTime * _lightTime;
                    _emissionMaterialFade.TimeEmission(_lightTime);

                    if (intensityLight < 0)
                    {
                        intensityLight = 0;
                    }

                    // Se la batteria è stata presa, ripristina l'intensità della luce
                    if (_PowerPickUp)
                    {
                        intensityLight = _batteryPower.PowerIntensityLight;
                    }
                }
                else
                {
                    lightComponent.intensity = 0.0f;
                    _emissionMaterialFade.OffEmission();

                    if (_PowerPickUp)
                    {
                        intensityLight = _batteryPower.PowerIntensityLight;
                    }
                }
            }
            else
            {
                Debug.LogError("Componente 'Light' non trovato sull'oggetto.");
            }
        }
        else
        {
            Debug.LogError("EmissionMaterialGlassTorchFadeOut non è stato trovato.");
        }
    }
}
