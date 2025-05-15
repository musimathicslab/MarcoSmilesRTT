using System.Globalization;
using TMPro;
using UnityEngine;
using Utilities;

public class PlayingUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playingNote;
    [SerializeField] private TextMeshProUGUI ottava;
    
    [SerializeField] private TextMeshProUGUI checkIntervalValue;
    [SerializeField] private TextMeshProUGUI thresholdDistanceValue;
    
    [SerializeField] private TextMeshProUGUI centerOffsetValue;
    [SerializeField] private TextMeshProUGUI intervalHeightValue;
    [SerializeField] private TextMeshProUGUI intervalCountValue;
    [SerializeField] private TextMeshProUGUI checkTickValue;
    
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SetPlayingNote(MarcoNote note)
    {
        playingNote.text = note.Name;
    }

    public void SetOttava(float value)
    {
        ottava.text = value.ToString(CultureInfo.InvariantCulture);
    }

    

    public void SetCheckInterval(float value)
    {
        checkIntervalValue.text = value.ToString(CultureInfo.InvariantCulture);
    }

    public void SetThresholdDistance(float value)
    {
        thresholdDistanceValue.text = value.ToString(CultureInfo.InvariantCulture);
    }
    
    public void SetCenterOffset(float value)
    {
        centerOffsetValue.text = value.ToString(CultureInfo.InvariantCulture);
    }

    public void SetIntervalHeight(float value)
    {
        intervalHeightValue.text = value.ToString(CultureInfo.InvariantCulture);
    }

    public void SetIntervalCount(int value)
    {
        intervalCountValue.text = value.ToString();
    }

    public void SetCheckTick(float value)
    {
        checkTickValue.text = value.ToString(CultureInfo.InvariantCulture);
    }
}