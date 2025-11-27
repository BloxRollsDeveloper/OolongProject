using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    public static bool IsPaused;
    public GameObject[] inventory;
    private void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            PlayerPrefs.SetFloat("xAxis", transform.position.x);
            PlayerPrefs.SetInt("isPaused", IsPaused ? 1 : 0);
            PlayerPrefs.SetString("isPaused", IsPaused ? "true" : "false");
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            var x = PlayerPrefs.GetFloat("xAxis");
            transform.position = new Vector2(x, 0);
            var pause = PlayerPrefs.GetInt("isPaused");
            IsPaused = pause == 1;
        }

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.DeleteKey("xAxis");
        }

        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                PlayerPrefs.SetInt("_" + inventory[i].name, i);
            }
        }
    }
}
