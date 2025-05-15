using UnityEngine;
using Utilities;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TrainingManager trainingManager;
    [SerializeField] private PlayingManager playingManager;
    [SerializeField] private ServerGateway serverGateway;
    

    // Start is called before the first frame update
    private void Start()
    {
        uiManager.ShowMainMenu();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void GoTrainingMenu()
    {
        uiManager.ShowTrainingMenu();
        //trainingManager.StartTraining();
        trainingManager.RttuStartTraining();
    }

    public void GoMainMenu()
    {
        uiManager.ShowMainMenu();
    }

    public void GoPlayingMenu()
    {
        uiManager.ShowPlayingMenu();
        playingManager.StartPlaying();
        serverGateway.SendStartNote(new MarcoNote(MarcoNote.NoteEnum.Do).ToString(), _ =>{});
    }
    
    public void SetServerGateway(string serverUrl)
    {
        serverGateway.SetServerUrl(serverUrl);
    }
}