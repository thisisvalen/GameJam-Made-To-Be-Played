using System.Collections.Generic;
using UnityEngine;

public class ProbadoraLogica : MonoBehaviour
{
    private void Start()
    {
        // Esperamos un cuadro para asegurarnos de que el GameplayLoop ya se inicializó
        Invoke("EjecutarPruebaDePartida", 0.5f);
    }

    private void EjecutarPruebaDePartida()
    {
        Debug.Log("<color=cyan><b>=== INICIANDO PRUEBA DE LOGA ===</b></color>");

        // 1. PROBAR EL MENU DE LA RONDA 1
        List<PlatilloSO> menuActual = GameplayLoop.Instancia.ObtenerPlatillosDelMenuActual();
        Debug.Log($"<b>[Menú Ronda 1]:</b> Debería tener 2 platillos desbloqueados. Total real: {menuActual.Count}");

        foreach (var platillo in menuActual)
        {
            List<IngredienteSO> buenos;
            List<IngredienteSO> malos;
            GameplayLoop.Instancia.ObtenerReglasDelPlatillo(platillo, out buenos, out malos);

            string textoMalos = "";
            foreach (var m in malos) textoMalos += m.nombreMostrado + ", ";
            Debug.Log($"Platillo: {platillo.nombrePlatillo} | Ingredientes Malos del día: <color=red>{textoMalos}</color>");
        }

        // 2. SIMULAR QUE EL SPAWNER PIDE UN PLATO
        PlatilloSO platilloServido;
        List<IngredienteSO> ingredientesEnMesa;
        GameplayLoop.Instancia.GenerarPlatoFisicoAleatorio(out platilloServido, out ingredientesEnMesa);

        string textoMesa = "";
        foreach (var ing in ingredientesEnMesa) textoMesa += ing.nombreMostrado + ", ";
        Debug.Log($"<b>[Mesa]:</b> Se sirvió: <color=yellow>{platilloServido.nombrePlatillo}</color> con los ingredientes: {textoMesa}");

        // 3. SIMULAR LA EVALUACIÓN DE LA MATRIZ DE JUICIO
        // Vamos a simular que el jugador dice que el plato está LIMPIO (Falso) y no selecciona evidencias
        List<IngredienteSO> evidenciaJugador = new List<IngredienteSO>();
        bool jugadorDijoMalo = false;

        ResultadoJuicio veredicto = GameplayLoop.Instancia.EvaluarRonda(platilloServido, jugadorDijoMalo, evidenciaJugador, ingredientesEnMesa);
        Debug.Log($"<b>[Resultado del Juicio]:</b> El jugador dictaminó Seguro=True. El sistema resolvió: <color=orange>{veredicto}</color>");

        // 4. PROBAR EL DESBLOQUEO DE LA RONDA 2
        Debug.Log("<color=cyan><b>=== AVANZANDO A LA RONDA 2 ===</b></color>");
        GameplayLoop.Instancia.AvanzarSiguienteRonda();

        menuActual = GameplayLoop.Instancia.ObtenerPlatillosDelMenuActual();
        Debug.Log($"<b>[Menú Ronda 2]:</b> Debería tener 3 platillos desbloqueados (Se sumó la Pizza). Total real: {menuActual.Count}");
    }
}