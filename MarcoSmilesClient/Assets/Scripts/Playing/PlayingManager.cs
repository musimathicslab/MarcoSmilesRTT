using System;
using System.Collections;
using OVRSimpleJSON;
using UnityEngine;
using Utilities;

public class PlayingManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SynthManager synthManager;
    [SerializeField] private HandsManager handsManager;

    [SerializeField] private PlayingUIManager playingUIManager;
    [SerializeField] private OttavaManager ottavaManager;
    [SerializeField] private RibattutaManager ribattutaManager;

    [SerializeField] private HandReader handReaderAC;
    [SerializeField] private ServerGateway serverGateway;

    [SerializeField] private float offset = -0.2f;

    
    [SerializeField] private int mainOctave = 4;

    private MarcoNote _currentNote;
    private int _currentOctave;
    private bool _isRibattuta;
    
    private Coroutine _ottavaCoroutine;

    
    private Coroutine _playingCoroutine;
    private Coroutine _ribattutaCoroutine;

    // Start is called before the first frame update
    private void Start()
    {
        var note = new MarcoNote(MarcoNote.NoteEnum.Do);
        _currentOctave = mainOctave;
        _currentNote = note;
        _isRibattuta = false;
        
        playingUIManager.SetCheckInterval(ribattutaManager.GetCheckInterval);
        playingUIManager.SetThresholdDistance(ribattutaManager.GetThresholdDistance);
        
        playingUIManager.SetCenterOffset(ottavaManager.GetCenterOffset);
        playingUIManager.SetIntervalHeight(ottavaManager.GetIntervalHeight);
        playingUIManager.SetIntervalCount(ottavaManager.GetIntervalCount);
        playingUIManager.SetCheckTick(ottavaManager.GetCheckTick);
        
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnEnable()
    {
        RibattutaManager.OnRibattuta += PlayRibattuta;
        OttavaManager.OnOttavaChanged += ChangeOttava;
    }

    private void OnDisable()
    {
        RibattutaManager.OnRibattuta -= PlayRibattuta;
        OttavaManager.OnOttavaChanged -= ChangeOttava;
    }

    public void StartPlaying()
    {
        playingUIManager.SetPlayingNote(_currentNote);
        playingUIManager.SetOttava(_currentOctave);

        _playingCoroutine = StartCoroutine(PlayCoroutine());
        _ribattutaCoroutine = StartCoroutine(ribattutaManager.StartRibattuta());
        _ottavaCoroutine = StartCoroutine(ottavaManager.StartCheckOttava());
    }
    

    private IEnumerator PlayCoroutine()
    {
        while (true)
        {
            if (!handsManager.AreHandsTracked())
            {
                Debug.Log("[PlayingManager] Hand tracking failed");
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            var (rightHeight, leftHeight) = handsManager.HandsHeightFromCenterEye();
            
            var maxHeight = Mathf.Max(rightHeight, leftHeight);
            //Debug.Log($"[PlayingManager] Hand height is {rightHeight} and {leftHeight} max {maxHeight}");
            if (maxHeight < offset)
            {
                yield return null;
                continue;
            }

            var requestWrapper = new RequestWrapper(_isRibattuta);
            _isRibattuta = false;
            for (var i = 0; i < 32; i++)
            {
                var leftHandWrapper = handReaderAC.ReadHand(HandSide.Left);
                var rightHandWrapper = handReaderAC.ReadHand(HandSide.Right);
                requestWrapper.LeftHandWrappers.Add(leftHandWrapper);
                requestWrapper.RightHandWrappers.Add(rightHandWrapper);
            }

            var responseReceived = false;
            serverGateway.SendHandDataPlayMode(requestWrapper, response =>
            {
                responseReceived = true;
                if (string.IsNullOrEmpty(response))
                {
                    Debug.LogWarning("Server request failed!");
                    return;
                }

                var notePredicted = ParseResponse(response);

                if (notePredicted != null && !Equals(_currentNote, notePredicted))
                {
                    _currentNote = notePredicted;
                    Debug.Log($"[PlayingManager] Note Predicted: {_currentNote}");

                    synthManager.PlayNote(_currentNote, _currentOctave);

                    playingUIManager.SetPlayingNote(_currentNote);
                }
            });
            yield return new WaitUntil(() => responseReceived);
        }
    }

    private static MarcoNote ParseResponse(string response)
    {
        try
        {
            var messageAsInt = int.Parse(JSON.Parse(response)["message"]);
            return new MarcoNote((MarcoNote.NoteEnum)messageAsInt);
        }
        catch (FormatException e)
        {
            Debug.Log("Error parsing response: " + e.Message);
            return null;
        }
    }

    private void PlayRibattuta()
    {
        _isRibattuta = true;
        synthManager.PlayNote(_currentNote, _currentOctave);
    }

    private void ChangeOttava(int offsetOctave)
    {
        _currentOctave = mainOctave + offsetOctave;
        Debug.unityLogger.Log($"[PlayingManager] Changing Ottava to {offsetOctave} Octave{_currentOctave}");
        playingUIManager.SetOttava(_currentOctave);
    }

    public void StopPlaying()
    {
        StopCoroutine(_playingCoroutine);
        StopCoroutine(_ribattutaCoroutine);
        StopCoroutine(_ottavaCoroutine);
    }

    public void GoBack()
    {
        StopPlaying();
        synthManager.StopAllNotes();
        gameManager.GoMainMenu();
    }

    public void SetCheckInterval(float value)
    {
        playingUIManager.SetCheckInterval(value);
        ribattutaManager.SetCheckInterval(value);
    }

    public void SetThresholdDistance(float value)
    {
        playingUIManager.SetThresholdDistance(value);
        ribattutaManager.SetThresholdDistance(value);
    }

    public void SetCenterOffeset(float value)
    {
        playingUIManager.SetCenterOffset(value);
        ottavaManager.SetCenterOffset(value);
    }

    public void SetIntervalHeight(float value)
    {
        playingUIManager.SetIntervalHeight(value);
        ottavaManager.SetIntervalHeight(value);
    }

    public void SetIntervalCount(int value)
    {
        playingUIManager.SetIntervalCount(value);
        ottavaManager.SetIntervalCount(value);
    }

    public void SetCheckTick(float value)
    {
        playingUIManager.SetCheckTick(value);
        ottavaManager.SetCheckTick(value);
    }
}