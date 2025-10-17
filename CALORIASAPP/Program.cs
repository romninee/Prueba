using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

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
      static string connectionString = "Server=CHAMBEADORA;Database=CALORIAS_DB;Trusted_Connection=True;TrustServerCertificate=True;";



        static void Main(string[] args)
        {
            bool repetir = true;

            while (repetir)
            {
                Console.Clear();
                Console.WriteLine("=== BIENVENIDO AL OPTIMIZADOR DE ALIMENTOS ===\n");

                int ckalMin = 15;
                int pesoMax = 10;
                int maxAlimentos = 15;

                Console.WriteLine("Nota: El peso máximo permitido es de 10 kg y las calorías mínimas requeridas son de 15 para poder escalar el risco.\n");
                Console.WriteLine("Agregue sus alimentos y el programa encontrará el balance más óptimo para usted.\n");
                Console.WriteLine($"La cantidad máxima de alimentos que puede ingresar es: {maxAlimentos}\n");

                List<producto> alimentos = new List<producto>();
                Console.Write("¿Cuántos alimentos desea ingresar?: ");
                int cantidad;

                while (!int.TryParse(Console.ReadLine(), out cantidad) || cantidad <= 0 || cantidad > maxAlimentos)
                {
                    Console.Write($"No se permiten menos de 10 alimentos para que cumpla la necesidad calórica, y no se permiten más de 15 (máximo {maxAlimentos}). Ingrese nuevamente: ");
                }

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

                    var alimento = new producto { Nombre = nombre, Peso = peso, ckal = ckal };

                    alimentos.Add(alimento);

                    GuardarEnBaseDeDatos(alimento);

                    Console.WriteLine("Alimento guardado correctamente en la base de datos.");
                }

                Console.WriteLine("\n--- Alimentos Ingresados ---");
                foreach (var a in alimentos)
                {
                    Console.WriteLine($"Nombre: {a.Nombre}, Peso: {a.Peso} kg, Calorías: {a.ckal}");
                }

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
                    Console.WriteLine("\nDigite de nuevo la información con otros alimentos.");
                }

                Console.WriteLine("\n¿Desea reiniciar el programa? (S/N)");
                string respuesta = Console.ReadLine()?.ToUpper();

                if (respuesta != "S")
                    repetir = false;
            }

            Console.WriteLine("\nGracias por usar el programa. ¡Hasta luego!");
        }

        static void GuardarEnBaseDeDatos(producto p)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Alimentos (Nombre, Peso, Calorias) VALUES (@Nombre, @Peso, @Calorias)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                    cmd.Parameters.AddWithValue("@Peso", p.Peso);
                    cmd.Parameters.AddWithValue("@Calorias", p.ckal);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        static List<producto> combinacionOptima(List<producto> alimentos, int pesoMax, int ckalMin)
        {
            List<producto> mejorCombinacion = null;
            int mejorPeso = int.MaxValue;
            int n = alimentos.Count;

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
