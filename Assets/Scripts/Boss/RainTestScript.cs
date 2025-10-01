using UnityEngine;
using UnityEngine.InputSystem;

public class RainTestScript : MonoBehaviour
{
    public GameObject rain1,rain2,rain3,rain4,rain5,rain6,rain7,rain8,rain9,rain10;
    public GameObject[] rain;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Instantiate(rain1,transform.position,Quaternion.identity);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Instantiate(rain2,transform.position,Quaternion.identity);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Instantiate(rain3,transform.position,Quaternion.identity);
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Instantiate(rain4,transform.position,Quaternion.identity);
        }

        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            Instantiate(rain5,transform.position,Quaternion.identity);
        }

        if (Keyboard.current.digit6Key.wasPressedThisFrame)
        {
            Instantiate(rain6,transform.position,Quaternion.identity);
        }

        if (Keyboard.current.digit7Key.wasPressedThisFrame)
        {
            Instantiate(rain7,transform.position,Quaternion.identity);
        }

        if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            Instantiate(rain8,transform.position,Quaternion.identity);
        }

        if (Keyboard.current.digit9Key.wasPressedThisFrame)
        {
            Instantiate(rain9,transform.position,Quaternion.identity);
        }

        if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            Instantiate(rain10,transform.position,Quaternion.identity);
        }

    }

    public void SpawnRain()
    {
        int randomIndex = Random.Range(0, rain.Length);
        GameObject randomPrefab = rain[randomIndex];
            
        Instantiate(randomPrefab, transform.position, transform.rotation);
    }
}
