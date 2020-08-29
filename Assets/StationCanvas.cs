
using UnityEngine;
using UnityEngine.UI;

public class StationCanvas : MonoBehaviour
{
    public Sprite[] Sprites;
    public Sprite UnPressed => Sprites[0];
    public Sprite Pressed => Sprites[1];

    Image image;

    void Start()
    {
        image = GetComponentInChildren<Image>();
        image.sprite = UnPressed;
    }

    public void KeyPressed()
    {
        image.sprite = Pressed;
    }

    public void KeyUnpressed()
    {
        image.sprite = UnPressed;
    }
}
