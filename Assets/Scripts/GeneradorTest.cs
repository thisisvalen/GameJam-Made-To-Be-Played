using System.Collections.Generic;
using UnityEngine;

public class GeneradorTest : MonoBehaviour
{
    [Header("Referencias")]
    public SpawnPositionGenerator generador;

    [Header("Configuración de Prueba")]
    public Vector3 tamanoPlato = new Vector3(2.5f, 0f, 2.5f);
    public float distanciaClusters = 0.8f;      // Distancia para elementos de área (ej. crutones)
    public float distanciaDecoracion = 1.0f;    // Distancia para elementos focales (ej. cerezas)

    [Header("Resultados")]
    public List<Vector3> clusters = new List<Vector3>();
    public List<Vector3> decoraciones = new List<Vector3>();

    [ContextMenu("Ejecutar Prueba de Capas")]
    public void ProbarMatematica()
    {
        clusters.Clear();
        decoraciones.Clear();
        Bounds limitesPlato = new Bounds(transform.position, tamanoPlato);

        // 1. Spawneamos Clusters (Capa 1)
        for (int i = 0; i < 10; i++)
        {
            Vector3 punto = generador.GeneratePosition(limitesPlato, clusters, distanciaClusters);
            if (punto != Vector3.zero) clusters.Add(punto);
        }

        // 2. Spawneamos Decoraciones (Capa 2)
        // Nota: Pasamos la lista de 'clusters' como obstáculos para que no se encimen
        for (int i = 0; i < 5; i++)
        {
            // Creamos una lista temporal que combine ambos para validar colisiones
            List<Vector3> obstaculos = new List<Vector3>(clusters);
            obstaculos.AddRange(decoraciones);

            Vector3 punto = generador.GeneratePosition(limitesPlato, obstaculos, distanciaDecoracion);
            if (punto != Vector3.zero) decoraciones.Add(punto);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, tamanoPlato);

        // Dibujar Clusters en Rojo
        Gizmos.color = Color.red;
        foreach (Vector3 p in clusters) Gizmos.DrawWireSphere(p, distanciaClusters / 2f);

        // Dibujar Decoraciones en Amarillo (Así verás si se enciman o no)
        Gizmos.color = Color.yellow;
        foreach (Vector3 p in decoraciones) Gizmos.DrawWireSphere(p, distanciaDecoracion / 2f);
    }
}