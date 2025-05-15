using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class TrainingUIManager : MonoBehaviour
{
    [SerializeField] private GameObject actualNoteContainer;
    [SerializeField] private TextMeshProUGUI actualNoteValue;

    [SerializeField] private TextMeshProUGUI nextNoteTitle;
    [SerializeField] private GameObject nextNoteContainer;
    [SerializeField] private TextMeshProUGUI nextNoteValue;

    [SerializeField] private GameObject mainLabel;
    [SerializeField] private GameObject warningMessage;

    [SerializeField] private GameObject progressBar;
    [SerializeField] private GameObject progressBarLabel;

    [SerializeField] private GameObject checkMark;
    [SerializeField] private GameObject checkMarkLabel;

    [SerializeField] private GameObject endTrainingDialogBox;
    [SerializeField] private GameObject endTrainingTitle;
    [SerializeField] private GameObject endTrainingBody;
    [SerializeField] private GameObject endTrainingProgressBar;
    [SerializeField] private GameObject endTrainingProgressLabel;
    [SerializeField] private GameObject endTrainingCheckMark;
    [SerializeField] private GameObject endTrainingCheckMarkLabel;
    [SerializeField] private GameObject endTrainingButtons;
    
    
    private Image _endTrainingProgressBarImage;
    private Image _progressBarImage;


    // Start is called before the first frame update
    private void Start()
    {
        _progressBarImage = progressBar.GetComponent<Image>();
        _endTrainingProgressBarImage = endTrainingProgressBar.GetComponent<Image>();
    }

    // Update is called once per frame
    private void Update()
    {
    }


    public void HideNextNote()
    {
        nextNoteTitle.gameObject.SetActive(false);
        nextNoteContainer.SetActive(false);
    }

    public void ShowNextNote()
    {
        nextNoteTitle.gameObject.SetActive(true);
        nextNoteContainer.SetActive(true);
    }

    public void SetActualNoteValue(MarcoNote note)
    {
        actualNoteValue.text = note.Name;
    }

    public void SetNextNoteValue(MarcoNote note)
    {
        nextNoteValue.text = note.Name;
    }

    public void HideAll()
    {
        mainLabel.SetActive(false);
        warningMessage.SetActive(false);
        progressBar.SetActive(false);
        progressBarLabel.SetActive(false);
        checkMark.SetActive(false);
        checkMarkLabel.SetActive(false);
    }

    public void ShowWarningMessage()
    {
        warningMessage.gameObject.SetActive(true);
    }

    public void ShowMainView()
    {
        HideAll();
        HideEndTrainingDialogBox();
        mainLabel.SetActive(true);
    }

    public void ShowProgressBar()
    {
        HideAll();
        progressBar.SetActive(true);
        progressBarLabel.SetActive(true);
    }

    public void ShowCheckMark()
    {
        HideAll();
        checkMark.SetActive(true);
        checkMarkLabel.SetActive(true);
    }

    public IEnumerator FillProgressBarCoroutine(float fillDuration)
    {
        var elapsed = 0f;
        _progressBarImage.fillAmount = 0f;

        while (elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            _progressBarImage.fillAmount = Mathf.Clamp01(elapsed / fillDuration);
            yield return null;
        }

        _progressBarImage.fillAmount = 1f;
    }

    public void EndTrainingHideAll()
    {
        endTrainingProgressBar.SetActive(false);
        endTrainingProgressLabel.SetActive(false);
        endTrainingCheckMark.SetActive(false);
        endTrainingCheckMarkLabel.SetActive(false);
        endTrainingTitle.SetActive(false);
        endTrainingBody.SetActive(false);
        endTrainingButtons.SetActive(false);
    }

    public void ShowEndTrainingDialogBox()
    {
        EndTrainingHideAll();
        endTrainingDialogBox.SetActive(true);
        endTrainingTitle.SetActive(true);
        endTrainingBody.SetActive(true);
        endTrainingButtons.SetActive(true);
    }

    public void HideEndTrainingDialogBox()
    {
        EndTrainingHideAll();
        endTrainingDialogBox.SetActive(false);
    }

    public void ShowEndTrainingProgressBar()
    {
        EndTrainingHideAll();
        endTrainingProgressBar.SetActive(true);
        endTrainingProgressLabel.SetActive(true);
    }

    public void ShowEndTrainingCheckMark()
    {
        EndTrainingHideAll();
        endTrainingCheckMark.SetActive(true);
        endTrainingCheckMarkLabel.SetActive(true);
    }

    public IEnumerator EndTrainingProgressBarCoroutine(float fillDuration)
    {
        _endTrainingProgressBarImage.fillAmount = 0f;

        while (true)
        {
            var elapsed = 0f;
            _endTrainingProgressBarImage.fillOrigin = 2;
            while (elapsed < fillDuration)
            {
                elapsed += Time.deltaTime;
                _endTrainingProgressBarImage.fillAmount = Mathf.Clamp01(elapsed / fillDuration);
                yield return null;
            }

            _endTrainingProgressBarImage.fillAmount = 1f;

            elapsed = 0f;
            _endTrainingProgressBarImage.fillOrigin = 0;
            while (elapsed < fillDuration)
            {
                elapsed += Time.deltaTime;
                _endTrainingProgressBarImage.fillAmount = Mathf.Clamp01(1 - elapsed / fillDuration);
                yield return null;
            }

            _endTrainingProgressBarImage.fillAmount = 0f;

            yield return null;
        }
    }
}