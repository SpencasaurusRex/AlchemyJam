
using UnityEngine;
using UnityEngine.UI;

public class StationCanvas : MonoBehaviour
{
    public Sprite[] KeySprites;
    public Sprite UnPressed => KeySprites[0];
    public Sprite Pressed => KeySprites[1];

    public Image KeyImage;
    public Image ProgressImage;
    public Image ValidRecipeImage;

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
