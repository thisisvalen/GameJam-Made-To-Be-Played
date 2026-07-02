using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SpawnPositionGenerator generator; 
    [SerializeField] private SpriteRenderer dishSpriteRenderer; // Para cambiar la imagen del plato

    [Header("Configuración")]
    public GameObject ingredientPrefab; // Prefab base con SpriteRenderer y BoxCollider2D
    public Transform dishContainer; // Transform donde vive el plato

    // Lista para llevar el control de lo que hemos creado y poder borrarlo luego
    private List<GameObject> activeIngredients = new List<GameObject>();

    // Cambia la imagen del plato base.   
    public void SpawnDishSurface(Sprite dishSprite)
    {
        if (dishSpriteRenderer != null)
        {
            dishSpriteRenderer.sprite = dishSprite;
        }
    }

    // Itera la lista de ingredientes, solicita coordenadas y los instancia con su tipo.
    public void SpawnIngredientsOnDish(List<IngredientSO> ingredientsToSpawn)
    {
   
        ClearDish();
        //Límite del espacio de spawn
        Bounds dishBounds = new Bounds(dishContainer.position, new Vector3(3f, 0f, 3f));

        List<Vector3> placedPositions = new List<Vector3>();

        foreach (var ingredient in ingredientsToSpawn)
        {
            // Solicitamos una posición segura al generador
            Vector3 spawnPos = generator.GeneratePosition(dishBounds, placedPositions, ingredient.minDistance);

            if (spawnPos != Vector3.zero)
            {
                // Instanciamos el ingrediente
                GameObject go = Instantiate(ingredientPrefab, spawnPos, Quaternion.identity, dishContainer);
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                sr.sprite = ingredient.sprite;

                // Aplicamos el Sorting Order según el tipo de ingrediente
                // Special = 0 (fondo), Individual = 1 (frente)
                sr.sortingOrder = (int)ingredient.type;
                activeIngredients.Add(go);
                placedPositions.Add(spawnPos);
            }
            else
            {
                Debug.LogWarning($"No se pudo colocar el ingrediente: {ingredient.name}");
            }
        }
    }

    // Destruye los objetos creados para limpiar Hierarchy.
   
    public void ClearDish()
    {
        foreach (GameObject obj in activeIngredients)
        {
            Destroy(obj);
        }
        activeIngredients.Clear();
    }
}