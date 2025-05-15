using UnityEngine;
using UnityEngine.UI;

public class Play : MonoBehaviour
{
    [SerializeField] private PlayingManager playingManager;

    [SerializeField] private GameObject pauseIcon;
    [SerializeField] private GameObject playIcon;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnClick(Toggle keyToggle)
    {
        if (keyToggle.isOn)
        {
            playingManager.StopPlaying();
            pauseIcon.SetActive(false);
            playIcon.SetActive(true);
        }
        else
        {
            playingManager.StartPlaying();
            pauseIcon.SetActive(true);
            playIcon.SetActive(false);
        }
    }
}