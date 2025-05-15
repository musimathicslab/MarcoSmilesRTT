using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject trainingMenuCanvas;
    [SerializeField] private GameObject playingMenuCanvas;


    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void DeactivateAllCanvases()
    {
        mainMenuCanvas.SetActive(false);
        trainingMenuCanvas.SetActive(false);
        playingMenuCanvas.SetActive(false);
    }

    public void ShowMainMenu()
    {
        DeactivateAllCanvases();
        mainMenuCanvas.SetActive(true);
    }

    public void ShowTrainingMenu()
    {
        DeactivateAllCanvases();
        trainingMenuCanvas.SetActive(true);
    }

    public void ShowPlayingMenu()
    {
        DeactivateAllCanvases();
        playingMenuCanvas.SetActive(true);
    }
}