using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPedestal : MonoBehaviour
{
    public Ingredient IngredientPrefab;

    Queue<IngredientType> queuedIngredients = new Queue<IngredientType>();

    Ingredient spawnedIngredient;

    public void Spawn(IngredientType type)
    {
        if (spawnedIngredient == null)
        {
            CreateIngredient(type);
        }
        else
        {
            queuedIngredients.Enqueue(type);
        }
    }

    public Ingredient RemoveIngredient()
    {
        var result = spawnedIngredient;
        spawnedIngredient = null;
        if (queuedIngredients.Any())
        {
            CreateIngredient(queuedIngredients.Dequeue());
        }

        return result;
    }

    void CreateIngredient(IngredientType type)
    {
        var ingredient = Instantiate(IngredientPrefab);
        ingredient.transform.position = transform.position;
        ingredient.Type = type;
        ingredient.Setup();
        ingredient.Collider2D.enabled = false;
        spawnedIngredient = ingredient;
    }

}
