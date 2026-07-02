using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionGenerator : MonoBehaviour
{
    /*
    Genera un punto Vector3 aleatorio dentro de los límites. 
    Si choca con otra posición, recalcula.
    */

    public Vector3 GeneratePosition(Bounds dishBounds, List<Vector3> existingPositions, float minDistance)
    {
        int maxIterations = 50; // Bucle de seguridad (Criterio de aceptación)
        int currentIteration = 0;

        Vector3 proposedPos = Vector3.zero;
        bool validPositionFound = false;

        while (currentIteration < maxIterations)
        {
            // Generamos posiciones aleatorias en X y Z dentro de los límites del plato.
            // Mantenemos Y constante (usando el centro del plato)
            float randomX = Random.Range(dishBounds.min.x, dishBounds.max.x);
            float randomZ = Random.Range(dishBounds.min.z, dishBounds.max.z);

            proposedPos = new Vector3(randomX, dishBounds.center.y, randomZ);

            // Validamos si la posición propuesta se solapa con algo ya existente
            if (!CheckOverlap(proposedPos, existingPositions, minDistance))
            {
                validPositionFound = true;
                break; // Encontramos un buen lugar, rompemos el bucle
            }

            currentIteration++;
        }

        // Si se alcanzó el límite de 50 iteraciones (el plato está muy lleno)
        if (!validPositionFound)
        {
            Debug.LogWarning("Se alcanzó el límite de 50 iteraciones. El plato está lleno. Devolviendo la última posición calculada.");
        }

        return proposedPos;
    }

    /*
    Compara la posición propuesta contra las que ya fueron aceptadas. 
    Retorna 'true' si la distancia es menor a 'minDistance'.
    */
    private bool CheckOverlap(Vector3 proposedPos, List<Vector3> existingPositions, float minDistance)
    {
        foreach (Vector3 pos in existingPositions)
        {
            // Medimos la distancia entre el punto propuesto y cada ingrediente ya colocado
            if (Vector3.Distance(proposedPos, pos) < minDistance)
            {
                return true; // Hay solapamiento, están muy cerca
            }
        }

        return false; // No hay solapamiento con ninguno, es seguro
    }
}