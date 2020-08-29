using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public IngredientType Type;
    public SpriteRenderer SpriteRenderer;
    public Collider2D Collider2D;
    public Color[] IngredientColors;

    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Collider2D = GetComponent<Collider2D>();
        SpriteRenderer.color = IngredientColors[(int) Type];
    }
}

public enum IngredientType
{
    Red, Green, Blue, Yellow, Orange, YellowGreen, Purple, Cyan
}