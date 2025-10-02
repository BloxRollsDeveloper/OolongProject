using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController4 : MonoBehaviour
{
    public PlayerController target;
    public Image[] hearts;
    public Sprite FullHeart;
    public Sprite EmptyHeart;

    private void Update()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < target.playerHealth)
            {
                
                hearts[i].sprite = FullHeart;
            }
            else
            {
                hearts[i].sprite = EmptyHeart;
            }
        }
    }
}
