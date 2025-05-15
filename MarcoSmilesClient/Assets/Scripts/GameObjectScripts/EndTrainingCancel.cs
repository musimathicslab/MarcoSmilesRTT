using UnityEngine;

public class EndTrainingCancel : MonoBehaviour
{
    [SerializeField] private TrainingManager trainingManager;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnClick()
    {
        trainingManager.EndTrainingCancel();
    }
}