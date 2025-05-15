using UnityEngine;

public class PlayingButton : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

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
        gameManager.GoPlayingMenu();
    }
}