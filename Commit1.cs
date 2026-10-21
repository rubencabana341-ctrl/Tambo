using System;
using System.Collections.Generic;

namespace Commit1
{
    //Estructura base y lógica de venta//
    namespace SistemaPOS_Tambo
    { }
        public class Producto
    {
        public string CodigoSKU { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public DateTime FechaVencimiento { get; set; }

        public Producto(string sku, string nombre, double precio, int stockActual, int stockMinimo, DateTime fechaVencimiento)
        {
            CodigoSKU = sku;
            Nombre = nombre;
            Precio = precio;
            StockActual = stockActual;
            StockMinimo = stockMinimo;
            FechaVencimiento = fechaVencimiento;
        }
    }

    class Program
    {
        private static Dictionary<string, Producto> bdProductos = new Dictionary<string, Producto>()
        {
            { "SKU001", new Producto("SKU001", "Agua San Mateo 600ml", 2.50, 50, 10, new DateTime(2027, 12, 31)) },
            { "SKU002", new Producto("SKU002", "Galleta Field Soda", 1.20, 40, 5, new DateTime(2027, 1, 1)) }
        };

        static void Main(string[] args)
        {
            Procesar_Operacion_Tienda();
        }

        public static Producto Obtener_Producto_BD(string sku)
        {
            if (bdProductos.ContainsKey(sku)) return bdProductos[sku];
            return new Producto("", "", 0.0, 0, 0, DateTime.MinValue);
        }

        public static bool Modulo_Validacion_Stock(string sku, int cant)
        {
            Producto prod = Obtener_Producto_BD(sku);
            if (prod.CodigoSKU == "") { Console.WriteLine("ALERTA: Código no registrado."); return false; }
            if (prod.StockActual < cant) { Console.WriteLine("ALERTA: Stock insuficiente."); return false; }
            return true;
        }

        public static void Modulo_Actualizacion_ERP(string sku, int cant)
        {
            if (bdProductos.ContainsKey(sku))
            {
                bdProductos[sku].StockActual -= cant;
                Console.WriteLine($"[ERP] Stock actualizado para {sku}. Restante: {bdProductos[sku].StockActual}");
            }
        }

        public static void Procesar_Operacion_Tienda()
        {
            double total = 0.0;
            bool activo = true;

            Console.WriteLine("=== SISTEMA POS TAMBO - V1.0 ===");

            while (activo)
            {
                Console.Write("Ingrese Código SKU: ");
                string sku = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Ingrese Cantidad: ");
                if (!int.TryParse(Console.ReadLine(), out int cant)) continue;

                if (Modulo_Validacion_Stock(sku, cant))
                {
                    total += Obtener_Producto_BD(sku).Precio * cant;
                    Modulo_Actualizacion_ERP(sku, cant);
                }

                Console.Write("¿Agregar otro? (SI/NO): ");
                if (Console.ReadLine()?.Trim().ToUpper() == "NO") activo = false;
            }

            Console.WriteLine($"\nTotal a pagar: S/ {total:F2}");
        }
    }
}