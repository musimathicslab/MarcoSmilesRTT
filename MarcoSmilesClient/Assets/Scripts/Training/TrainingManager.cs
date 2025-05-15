using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class TrainingManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private HandReader handReaderAC;
    [SerializeField] private ServerGateway serverGateway;

    [SerializeField] private TrainingUIManager trainingUIManager;
    [SerializeField] private HandStillnessManager handStillnessManager;

    //Quanto tempo l'utente deve rimanere fermo prima di iniziare l'acquisizione
    [SerializeField] private float waitingStillTime = 3f;

    //Quanti secondi dura l'acquisizione
    [SerializeField] private float acquiringTime = 4f;

    //Quante volte vengono acquisite
    [SerializeField] private int acquiringSteps = 16;
    private int _actualNoteIndex;

    private Coroutine _collectDataCoroutine;
    private TrainingState _currentState;
    private Coroutine _endTrainingProgressCoroutine;

    private Coroutine _endTrainingSaveCoroutine;

    private Coroutine _handStillnessCoroutine;


    private readonly List<MarcoNote> _notes = new();
    private Coroutine _progressBarCoroutine;
    private Coroutine _trainingCoroutine;

    //Real time training fields
    private bool _trainEnded;
    
    private bool _serverTrainEnded;

    private void Awake()
    {
        // Attualmente tutte le note vengono addestrate 
        foreach (MarcoNote.NoteEnum note in Enum.GetValues(typeof(MarcoNote.NoteEnum)))
        {
            _notes.Add(new MarcoNote(note));
        }
        // _notes.Add(new MarcoNote(MarcoNote.NoteEnum.Do));
        // _notes.Add(new MarcoNote(MarcoNote.NoteEnum.DoSharp));
        // _notes.Add(new MarcoNote(MarcoNote.NoteEnum.Re));
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    //Se _notes ha meno di due note si rompe
    public void StartTraining()
    {
        _actualNoteIndex = 0;  
        UploadNotes();
        // trainingUIManager.SetActualNoteValue(_notes[0]);
        // trainingUIManager.SetNextNoteValue(_notes[1]);

        trainingUIManager.ShowMainView();
        trainingUIManager.ShowNextNote();    

        _handStillnessCoroutine = StartCoroutine(handStillnessManager.AreHandsStillCourutine());
        _trainingCoroutine = StartCoroutine(TrainCoroutine());
    }

    public void RttuStartTraining()
    {
        _trainEnded = false;
        _serverTrainEnded = true;
        
        trainingUIManager.ShowMainView();
        trainingUIManager.ShowNextNote();  
        
        serverGateway.RTTCreateNewModel(_notes.Count, response =>
        {
            _actualNoteIndex = 0;
            UploadNotes();

            trainingUIManager.ShowMainView();

            _handStillnessCoroutine = StartCoroutine(handStillnessManager.AreHandsStillCourutine());
            _trainingCoroutine = StartCoroutine(RttuTrainCoroutine());
        });
    }

    private bool UploadNotes()
    {
        if (_actualNoteIndex >= _notes.Count) return false;
        Debug.Log($"[TrainingManager] Next note: {_actualNoteIndex}");

        //Controlla se è l'ultima nota
        if (_actualNoteIndex + 1 < _notes.Count)
        {
            trainingUIManager.SetActualNoteValue(_notes[_actualNoteIndex]);
            trainingUIManager.SetNextNoteValue(_notes[_actualNoteIndex + 1]);
            return true;
        }

        trainingUIManager.SetActualNoteValue(_notes[_actualNoteIndex]);
        trainingUIManager.HideNextNote();
        return true;
    }

    //Approssimiamo che ogni ciclo sia passato un secondo [timer += 1f;]
    private IEnumerator TrainCoroutine()
    {
        var timer = 0f;
        _currentState = TrainingState.WaitingStill;

        while (true)
        {
            yield return new WaitForSeconds(1f);

            //Durante il collecting non fare niente
            if (_currentState == TrainingState.CollectingData) continue;

            //Controlla se le mani si sono mosse
            if (!handStillnessManager.AreHandsStill)
            {
                //Ferma l'acquisizione
                if (_progressBarCoroutine != null) StopCoroutine(_progressBarCoroutine);

                if (_collectDataCoroutine != null) StopCoroutine(_collectDataCoroutine);

                timer = 0f;
                _currentState = TrainingState.WaitingStill;

                trainingUIManager.ShowMainView();
                trainingUIManager.ShowWarningMessage();
                continue;
            }

            //Si approssima che sia passato un secondo
            timer += 1f;

            //Controlla se è fermo da almeno waitingStillTime secondi
            //Controlla se l'acquiring è già iniziato
            if (timer < waitingStillTime || _currentState == TrainingState.Acquiring) continue;

            _currentState = TrainingState.Acquiring;

            trainingUIManager.ShowProgressBar();
            _progressBarCoroutine = StartCoroutine(trainingUIManager.FillProgressBarCoroutine(acquiringTime));
            _collectDataCoroutine = StartCoroutine(CollectDataAC(acquiringTime));
        }
    }

    private IEnumerator RttuTrainCoroutine()
    {
        var timer = 0f;
        _currentState = TrainingState.WaitingStill;

        while (true)
        {
            yield return new WaitForSeconds(1f);

            //Durante il collecting non fare niente
            if (_currentState == TrainingState.CollectingData) continue;

            //Controlla se le mani si sono mosse
            if (!handStillnessManager.AreHandsStill)
            {
                //Ferma l'acquisizione
                if (_progressBarCoroutine != null) StopCoroutine(_progressBarCoroutine);

                if (_collectDataCoroutine != null) StopCoroutine(_collectDataCoroutine);

                timer = 0f;
                _currentState = TrainingState.WaitingStill;

                trainingUIManager.ShowMainView();
                trainingUIManager.ShowWarningMessage();
                continue;
            }

            //Si approssima che sia passato un secondo
            timer += 1f;

            //Controlla se è fermo da almeno waitingStillTime secondi
            //Controlla se l'acquiring è già iniziato
            if (timer < waitingStillTime || _currentState == TrainingState.Acquiring) continue;

            _currentState = TrainingState.Acquiring;

            trainingUIManager.ShowProgressBar();
            _progressBarCoroutine = StartCoroutine(trainingUIManager.FillProgressBarCoroutine(acquiringTime));
            _collectDataCoroutine = StartCoroutine(RttuCollectDataAc(acquiringTime));
        }
    }

    private IEnumerator CollectDataAC(float acquiringDuration)
    {
        var requestWrapper = new RequestWrapper((int)_notes[_actualNoteIndex].Value);

        var stepTime = acquiringDuration / acquiringSteps;

        for (var i = 0; i < acquiringSteps; i++)
        {
            var leftHandWrapper = handReaderAC.ReadHand(HandSide.Left);
            var rightHandWrapper = handReaderAC.ReadHand(HandSide.Right);
            requestWrapper.LeftHandWrappers.Add(leftHandWrapper);
            requestWrapper.RightHandWrappers.Add(rightHandWrapper);

            yield return new WaitForSeconds(stepTime);
        }

        _currentState = TrainingState.CollectingData;
        serverGateway.SendHandData(requestWrapper, response => { });
        trainingUIManager.ShowCheckMark();
        yield return new WaitForSeconds(2f);

        _actualNoteIndex += 1;
        if (_actualNoteIndex >= _notes.Count)
        {
            StopTraining();
            ShowEndTrainingDialogBox();
            yield return null;
        }

        UploadNotes();
        trainingUIManager.ShowMainView();
        _currentState = TrainingState.WaitingStill;
        yield return null;
    }

    private IEnumerator RttuCollectDataAc(float acquiringDuration)
    {
        var requestWrapper = new RequestWrapper((int)_notes[_actualNoteIndex].Value);

        var stepTime = acquiringDuration / acquiringSteps;

        for (var i = 0; i < acquiringSteps; i++)
        {
            var leftHandWrapper = handReaderAC.ReadHand(HandSide.Left);
            var rightHandWrapper = handReaderAC.ReadHand(HandSide.Right);
            requestWrapper.LeftHandWrappers.Add(leftHandWrapper);
            requestWrapper.RightHandWrappers.Add(rightHandWrapper);

            yield return new WaitForSeconds(stepTime);
        }

        _actualNoteIndex += 1;
        if (_actualNoteIndex >= _notes.Count)
        {
            _trainEnded = true;
            yield return null;
        }

        _currentState = TrainingState.CollectingData;

        if (_trainEnded)
        {
            Debug.Log($"[TrainingManager] training finito");

            serverGateway.RTTSendHandData(requestWrapper, response =>
            {
                ShowEndTrainingDialogBox();
                trainingUIManager.ShowEndTrainingProgressBar();
                _endTrainingProgressCoroutine = StartCoroutine(trainingUIManager.EndTrainingProgressBarCoroutine(3f));

                if (!_serverTrainEnded) return;

                _serverTrainEnded = false;
                serverGateway.RTTTrain(responseTrain =>
                {
                    _serverTrainEnded = true;
                });
            });
            
            yield return new WaitUntil(() => _serverTrainEnded);

            for (var i = 0; i < 12; i++)
            {
                var isTrainingDone = false;
                serverGateway.RTTTrain(response => { isTrainingDone = true; });
                yield return new WaitUntil(() => isTrainingDone);
            }
            
            serverGateway.RTTEndTraining(response => { });
            trainingUIManager.ShowEndTrainingCheckMark();

            yield return new WaitForSeconds(2f);
            GoBack();
        }
        else
        {
            Debug.Log($"[TrainingManager] primo training");
            serverGateway.RTTSendHandData(requestWrapper, response =>
            {
                if (!_serverTrainEnded) return;
                _serverTrainEnded = false;
                serverGateway.RTTTrain(responseTrain => { _serverTrainEnded = true; });
            });
        }

        trainingUIManager.ShowCheckMark();
        yield return new WaitForSeconds(2f);

        UploadNotes();
        trainingUIManager.ShowMainView();
        _currentState = TrainingState.WaitingStill;
        yield return null;
    }

    private void ShowEndTrainingDialogBox()
    {
        trainingUIManager.ShowEndTrainingDialogBox();
    }

    //Bottone che preve Salva
    public void EndTrainingSave()
    {
        _endTrainingSaveCoroutine = StartCoroutine(EndTrainingSaveCoroutine());
    }

    private IEnumerator EndTrainingSaveCoroutine()
    {
        trainingUIManager.ShowEndTrainingProgressBar();
        _endTrainingProgressCoroutine = StartCoroutine(trainingUIManager.EndTrainingProgressBarCoroutine(3f));
        var modelCreated = false;
        serverGateway.CreateNewModel(_notes.Count, response => { modelCreated = true; });
        yield return new WaitUntil(() => modelCreated);

        for (var i = 0; i < 4; i++)
        {
            var done = false;
            serverGateway.Train(response => { done = true; });
            yield return new WaitUntil(() => done);
        }

        serverGateway.EndTraining(response => { });

        trainingUIManager.ShowEndTrainingCheckMark();

        yield return new WaitForSeconds(2f);

        GoBack();
    }

    //Bottone che cancella
    public void EndTrainingCancel()
    {
        serverGateway.EndTraining(response => { });
        GoBack();
    }


    private void StopTraining()
    {
        if (_handStillnessCoroutine != null) StopCoroutine(_handStillnessCoroutine);
        if (_trainingCoroutine != null) StopCoroutine(_trainingCoroutine);
        if (_collectDataCoroutine != null) StopCoroutine(_collectDataCoroutine);
        if (_progressBarCoroutine != null) StopCoroutine(_progressBarCoroutine);
        if (_endTrainingProgressCoroutine != null) StopCoroutine(_endTrainingProgressCoroutine);
    }

    public void GoBack()
    {
        StopTraining();
        gameManager.GoMainMenu();
    }

    //Stati in cui si può trovare il training
    private enum TrainingState
    {
        WaitingStill,
        Acquiring,
        CollectingData
    }
}