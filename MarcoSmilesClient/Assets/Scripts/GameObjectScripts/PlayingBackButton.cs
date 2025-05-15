using UnityEngine;

public class PlayingBackButton : MonoBehaviour
{
    [SerializeField] private PlayingManager playingManager;

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
        playingManager.GoBack();
    }
}