using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PlacementPedestal : MonoBehaviour
{
    public List<IngredientType> GoalTypes;
    public StageController StageController;

    public bool TryPlace(Ingredient ingredient)
    {
        print("Valid type: " + GoalTypes.Contains(ingredient.Type));
        if (!GoalTypes.Contains(ingredient.Type)) return false;
        Destroy(ingredient.gameObject);
        StageController.Progress(ingredient.Type);
        return true;
    }
}
