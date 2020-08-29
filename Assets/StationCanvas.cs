
using UnityEngine;
using UnityEngine.UI;

public class StationCanvas : MonoBehaviour
{
    public Sprite[] Sprites;
    public Sprite UnPressed => Sprites[0];
    public Sprite Pressed => Sprites[1];

    public Image KeyImage;
    public Image ProgressImage;

    void Start()
    {
        KeyImage.sprite = UnPressed;
    }

    public void ProgressChange(float percent)
    {
        ProgressImage.GetComponent<RectTransform>().sizeDelta = new Vector2(percent, 1);
    }

    public void KeyPressed()
    {
        KeyImage.sprite = Pressed;
    }

    public void KeyUnpressed()
    {
        KeyImage.sprite = UnPressed;
    }
}
