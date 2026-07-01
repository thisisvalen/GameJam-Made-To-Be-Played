using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Definimos los posibles resultados para que la UI sepa qué pantalla mostrar
public enum ResultadoJuicio
{
    PlatoLimpioCorrecto,
    CulpablesIdentificados,
    FalsaAlarma,
    DejoPasarVeneno,
    ErrorDeEvidencia
}

public class GameplayLoop : MonoBehaviour
{
    public static GameplayLoop Instancia { get; private set; }

    [Header("Base de Datos Maestra")]
    [Tooltip("Arrastra TODOS los PlatilloSO del juego en el orden que quieres que aparezcan")]
    public List<PlatilloSO> ordenDeDesbloqueoTotal;

    // Variables de estado interno (Privadas para protegerlas)
    private List<PlatilloSO> platillosActivosEstaRonda = new List<PlatilloSO>();
    private Dictionary<PlatilloSO, List<IngredienteSO>> reglasDeLaPartida = new Dictionary<PlatilloSO, List<IngredienteSO>>();
    private int indiceRondaActual = 0;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Al abrir el juego, se definen las reglas y se prepara la primera ronda
        GenerarReglasUnicasDeLaPartida();
        AvanzarSiguienteRonda();
    }

    // ==========================================
    // LÓGICA INTERNA (Solo tuya)
    // ==========================================

    private void GenerarReglasUnicasDeLaPartida()
    {
        reglasDeLaPartida.Clear();

        foreach (PlatilloSO platillo in ordenDeDesbloqueoTotal)
        {
            List<IngredienteSO> poolTemporal = new List<IngredienteSO>(platillo.poolDeIngredientes);
            List<IngredienteSO> malos = new List<IngredienteSO>();

            // Seleccionamos 2 ingredientes al azar para que sean los venenosos en esta partida
            for (int i = 0; i < 2; i++)
            {
                int rnd = Random.Range(0, poolTemporal.Count);
                malos.Add(poolTemporal[rnd]);
                poolTemporal.RemoveAt(rnd);
            }

            reglasDeLaPartida.Add(platillo, malos);
        }
    }

    // ==========================================
    // API PARA EL PROGRAMADOR 1 (Flujo y UI)
    // ==========================================

    // Llama a esto cuando termine una ronda para actualizar la dificultad y el menú
    public void AvanzarSiguienteRonda()
    {
        if (indiceRondaActual == 0)
        {
            // Ronda 1: Empieza con los 2 primeros platillos
            platillosActivosEstaRonda.Add(ordenDeDesbloqueoTotal[0]);
            platillosActivosEstaRonda.Add(ordenDeDesbloqueoTotal[1]);
        }
        else if (indiceRondaActual + 1 < ordenDeDesbloqueoTotal.Count)
        {
            // Rondas siguientes: Agrega el siguiente platillo de la lista
            platillosActivosEstaRonda.Add(ordenDeDesbloqueoTotal[indiceRondaActual + 1]);
        }

        indiceRondaActual++;
    }

    // Devuelve la lista de platillos que la UI debe dibujar en el menú de reglas
    public List<PlatilloSO> ObtenerPlatillosDelMenuActual()
    {
        return platillosActivosEstaRonda;
    }

    // Devuelve exactamente cuáles son los 3 buenos y 2 malos de un platillo para dibujarlos en la UI
    public void ObtenerReglasDelPlatillo(PlatilloSO platillo, out List<IngredienteSO> buenos, out List<IngredienteSO> malos)
    {
        malos = reglasDeLaPartida[platillo];
        buenos = new List<IngredienteSO>();

        foreach (IngredienteSO ing in platillo.poolDeIngredientes)
        {
            if (!malos.Contains(ing))
            {
                buenos.Add(ing);
            }
        }
    }

    // El cerebro evaluador. El UI lo llama cuando el jugador confirma su decisión.
    public ResultadoJuicio EvaluarRonda(PlatilloSO platilloBase, bool jugadorDijoMalo, List<IngredienteSO> seleccionUI, List<IngredienteSO> platoEnMesa)
    {
        List<IngredienteSO> malosRealesDelPlatillo = reglasDeLaPartida[platilloBase];

        List<IngredienteSO> malosRealesEnPlato = platoEnMesa
            .Where(ing => malosRealesDelPlatillo.Contains(ing))
            .ToList();

        bool platoRealmenteMalo = malosRealesEnPlato.Count > 0;

        // Evaluamos errores de Falso Positivo/Negativo
        if (!jugadorDijoMalo && !platoRealmenteMalo) return ResultadoJuicio.PlatoLimpioCorrecto;
        if (!jugadorDijoMalo && platoRealmenteMalo) return ResultadoJuicio.DejoPasarVeneno;
        if (jugadorDijoMalo && !platoRealmenteMalo) return ResultadoJuicio.FalsaAlarma;

        // Si era malo y dijo malo, evaluamos si la evidencia seleccionada es correcta
        bool evidenciaCorrecta = seleccionUI.Count == malosRealesEnPlato.Count &&
                                 seleccionUI.All(malosRealesEnPlato.Contains);

        return evidenciaCorrecta ? ResultadoJuicio.CulpablesIdentificados : ResultadoJuicio.ErrorDeEvidencia;
    }

    // ==========================================
    // API PARA EL PROGRAMADOR 2 (Spawner de Mesa)
    // ==========================================

    // El Spawner llama a esto y recibe: 1) El platillo base que se eligió, 2) Los 3 ingredientes físicos
    public void GenerarPlatoFisicoAleatorio(out PlatilloSO platilloElegido, out List<IngredienteSO> ingredientesFisicos)
    {
        // 1. Elegimos un platillo al azar de los que están permitidos en esta ronda
        platilloElegido = platillosActivosEstaRonda[Random.Range(0, platillosActivosEstaRonda.Count)];

        // 2. Sacamos 3 ingredientes al azar de los 5 posibles
        List<IngredienteSO> poolTemporal = new List<IngredienteSO>(platilloElegido.poolDeIngredientes);
        ingredientesFisicos = new List<IngredienteSO>();

        for (int i = 0; i < 3; i++)
        {
            int rnd = Random.Range(0, poolTemporal.Count);
            ingredientesFisicos.Add(poolTemporal[rnd]);
            poolTemporal.RemoveAt(rnd);
        }
    }
}