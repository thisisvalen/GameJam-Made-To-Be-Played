using System.Collections.Generic;
using UnityEngine;

// 1. Definimos la ruta en el menú de Unity para crear el asset fácilmente
[CreateAssetMenu(fileName = "NuevoPlatillo", menuName = "Juego/Platillo")]
public class PlatilloSO : ScriptableObject // 2. Cambia MonoBehaviour por ScriptableObject
{
    [Header("Información del Platillo")]
    [Tooltip("El nombre del platillo, por ejemplo: Ramen")]
    public string nombrePlatillo;

    [Header("Pool del Nivel")]
    [Tooltip("Arrastra aquí exactamente los 5 ingredientes disponibles en este nivel")]

    // Usamos una lista que almacena datos de tipo 'IngredienteSO'
    public List<IngredienteSO> poolDeIngredientes = new List<IngredienteSO>(5);
}