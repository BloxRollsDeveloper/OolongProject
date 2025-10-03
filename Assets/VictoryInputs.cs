using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryInputs : MonoBehaviour
{
    
    private InputManager2 _inputManager;


    private void Start()
    {
        _inputManager = GetComponent<InputManager2>();
    }

    void Update()
    {
        if (_inputManager.Jump)
        {
            SceneManager.LoadScene(0);
        }
    }
}
