using UnityEngine;


[CreateAssetMenu(fileName = "NuevoIngrediente", menuName = "Juego/Ingrediente")]
public class IngredienteSO : ScriptableObject
{
    [Header("Datos de Identificación")]
    public string idIngrediente;
    public string nombreMostrado;

    [Header("Aspecto Visual")]
    public Sprite icono;
}
