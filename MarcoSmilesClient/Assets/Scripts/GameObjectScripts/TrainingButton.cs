using UnityEngine;

namespace GameObjectScripts
{
    public class TrainingButton : MonoBehaviour
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
            gameManager.GoTrainingMenu();
        }
    }
}