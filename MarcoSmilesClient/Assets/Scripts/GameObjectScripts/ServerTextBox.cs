using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerTextBox : MonoBehaviour
{
    
    [SerializeField] private GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnChangeServerIP(string value)
    {
        Debug.Log("OnChangeServerIP: " + value);
        gameManager.SetServerGateway(value);
    }
}
