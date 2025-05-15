using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private PlayingManager playingManager;
    
    [SerializeField] private Slider checkIntervalSlider;
    [SerializeField] private Slider thresholdDistanceSlider;
    
    [SerializeField] private Slider centerOffsetSlider;
    [SerializeField] private Slider intervalHeightSlider;
    [SerializeField] private Slider intervalCountSlider;
    [SerializeField] private Slider checkTickSlider;

    // Start is called before the first frame update
    private void Start()
    {
        if (checkIntervalSlider != null) checkIntervalSlider.onValueChanged.AddListener(OnChangeCheckInterval);

        if (thresholdDistanceSlider != null)
            thresholdDistanceSlider.onValueChanged.AddListener(OnChangeThresholdDistance);

        if(centerOffsetSlider !=null) centerOffsetSlider.onValueChanged.AddListener(OnChangeCenterOffset);
        
        if (intervalHeightSlider != null) intervalHeightSlider.onValueChanged.AddListener(OnChangeIntervalHeight);

        if (intervalCountSlider != null) intervalCountSlider.onValueChanged.AddListener(OnChangeIntervalCount);

        if (checkTickSlider != null) checkTickSlider.onValueChanged.AddListener(OnChangeCheckTick);
        
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnChangeCheckInterval(float value)
    {
        playingManager.SetCheckInterval(value);
    }

    public void OnChangeThresholdDistance(float value)
    {
        playingManager.SetThresholdDistance(value);
    }
    
    
    public void OnChangeCenterOffset(float value)
    {
        playingManager.SetCenterOffeset(value);
    }

    public void OnChangeIntervalHeight(float value)
    {
        playingManager.SetIntervalHeight(value);
    }

    public void OnChangeIntervalCount(float value)
    {
        playingManager.SetIntervalCount((int)value);
    }

    public void OnChangeCheckTick(float value)
    {
        playingManager.SetCheckTick(value);
    }
}