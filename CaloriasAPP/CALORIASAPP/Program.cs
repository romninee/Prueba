using System;
using System.Collections.Generic;
using System.Linq;

namespace CALORIASAPP
{
    class producto
    {
        public string Nombre { get; set; }
        public int Peso { get; set; }
        public int ckal { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool repetir = true;

            while (repetir)
            {
                Console.Clear();
                //Se dá la bienvenida al usuario y se notifica sobre las restricciones de peso y calorías
                Console.WriteLine("=== BIENVENIDO AL OPTIMIZADOR DE ALIMENTOS ===\n");

                //Aquí se crean las restricciones de peso y calorías
                int ckalMin = 15;
                int pesoMax = 10;
                int maxAlimentos = 5; // límite de cantidad máxima de alimentos para que no sobrepase el peso máximo

                //Se informa al usuario sobre las restricciones
                Console.WriteLine("Nota: El peso máximo permitido es de 10 kg y las calorías mínimas requeridas son de 15 para poder escalar el risco.\n");
            
                Console.WriteLine($"La cantidad máxima de alimentos que puede ingresar es: {maxAlimentos}\n");

                //Ingreso de valores por usuario
                List<producto> alimentos = new List<producto>();
                Console.Write("¿Cuántos alimentos desea ingresar?: ");
                int cantidad;

                // Validación de cantidad ingresada y límite máximo
                while (!int.TryParse(Console.ReadLine(), out cantidad) || cantidad <= 0 || cantidad > maxAlimentos)
                {
                    Console.Write($"No se permiten mas de 5 alimentos debido a que sobrepasará el peso (máximo {maxAlimentos}) Ingrese nuevamente cuantos alimentos: ");
                }

                // Registro de cada alimento
                for (int i = 0; i < cantidad; i++)
                {
                    Console.WriteLine($"\n--- Alimento #{i + 1} ---");

                    Console.Write("Nombre: ");
                    string nombre = Console.ReadLine();

                    Console.Write("Peso (en kg): ");
                    int peso;
                    while (!int.TryParse(Console.ReadLine(), out peso) || peso < 0)
                    {
                        Console.Write("Por favor ingrese un número válido para el peso: ");
                    }

                    Console.Write("Calorías: ");
                    int ckal;
                    while (!int.TryParse(Console.ReadLine(), out ckal) || ckal < 0)
                    {
                        Console.Write("Por favor ingrese un número válido para las calorías: ");
                    }

                    alimentos.Add(new producto { Nombre = nombre, Peso = peso, ckal = ckal });
                }

                //Se muestra lo ingresado
                Console.WriteLine("\n--- Alimentos Ingresados ---");
                foreach (var a in alimentos)
                {
                    Console.WriteLine($"Nombre: {a.Nombre}, Peso: {a.Peso} kg, Calorías: {a.ckal}");
                }

                //Aquí se realiza el cálculo de las combinaciones posibles
                var mejores = combinacionOptima(alimentos, pesoMax, ckalMin);

                if (mejores != null && mejores.Count > 0)
                {
                    Console.WriteLine("\n--- Mejor combinación encontrada ---");
                    foreach (var a in mejores)
                    {
                        Console.WriteLine($"{a.Nombre} (Peso: {a.Peso} kg, Calorías: {a.ckal})");
                    }

                    Console.WriteLine($"\nPeso total: {mejores.Sum(a => a.Peso)} kg");
                    Console.WriteLine($"Calorías totales: {mejores.Sum(a => a.ckal)}");
                }
                else
                {
                    Console.WriteLine("\nNo se encontró una combinación que cumpla con las restricciones.");
                }

                //Se pregunta al usuario si desea repetir el proceso
                Console.WriteLine("\n¿Desea reiniciar el programa? (S/N)");
                string respuesta = Console.ReadLine()?.ToUpper();

                if (respuesta != "S")
                    repetir = false;
            }

            Console.WriteLine("\nGracias por usar el programa. ¡Hasta luego!");
        }

        // --- Función que busca la mejor combinación ---
        static List<producto> combinacionOptima(List<producto> alimentos, int pesoMax, int ckalMin)
        {
            List<producto> mejorCombinacion = null;
            int mejorPeso = int.MaxValue;
            int n = alimentos.Count;

            // Se buscan todas las combinaciones posibles
            for (int i = 1; i < (1 << n); i++)
            {
                var seleccion = new List<producto>();

                for (int j = 0; j < n; j++)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        seleccion.Add(alimentos[j]);
                    }
                }

                int pesoTotal = seleccion.Sum(a => a.Peso);
                int ckalTotal = seleccion.Sum(a => a.ckal);

                // Se verifica si la combinación cumple con las restricciones
                if (ckalTotal >= ckalMin && pesoTotal <= pesoMax)
                {
                    if (pesoTotal < mejorPeso)
                    {
                        mejorPeso = pesoTotal;
                        mejorCombinacion = seleccion;
                    }
                }
            }

            return mejorCombinacion ?? new List<producto>();
        }
    }
}
