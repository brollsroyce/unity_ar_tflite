using UnityEngine;
using UnityEngine.UI;

public class ModeHandler : MonoBehaviour
{
    [SerializeField] ScreenToWorldAnchors screenToWorldAnchors;
    [SerializeField] SsdSpecific ssdSpecific;
    [SerializeField] TMPro.TMP_Dropdown modeDropdown;
    [SerializeField] Image crosshairImage;
    [SerializeField] Button manualSpawnButton;
    [SerializeField] Button resetButton;

    public static bool automaticMode = true;
    public static bool manualMode = false;

    int currentMode = 0;

    void Awake()
    {
        modeDropdown.onValueChanged.AddListener(OnModeChanged);
        manualSpawnButton.onClick.AddListener(() => screenToWorldAnchors.ManualAnchorAndDistances(
                                                    crosshairImage.rectTransform.position));
        resetButton.onClick.AddListener(() => OnModeChanged(currentMode));
        OnModeChanged(0);
    }

    void OnModeChanged(int modeIndex)
    {
        currentMode = modeIndex;
        automaticMode = modeIndex == 0; // 0: Automatic, 1: Manual
        manualMode = modeIndex == 1;
        
        crosshairImage.gameObject.SetActive(manualMode);
        manualSpawnButton.gameObject.SetActive(manualMode);

        screenToWorldAnchors.ResetAnchorsAndDistances();        

        ssdSpecific.createDetectedAnchorButton.gameObject.SetActive(automaticMode && !ssdSpecific.automaticSpawning);
    }
}