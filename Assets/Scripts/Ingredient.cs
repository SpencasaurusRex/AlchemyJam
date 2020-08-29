using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public enum IngredientType
    {
        AmanitaCap,
        AmanitaMush
    }

    public IngredientType Type;
    public SpriteRenderer SpriteRenderer;
    public Collider2D Collider2D;

    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Collider2D = GetComponent<Collider2D>();
    }
}
