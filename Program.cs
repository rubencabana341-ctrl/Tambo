using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace SistemaPOS_Tambo
{
    // --- Estructura de Datos ---
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
        // Base de datos simulada de productos
        private static Dictionary<string, Producto> bdProductos = new Dictionary<string, Producto>()
        {
            { "SKU001", new Producto("SKU001", "Agua San Mateo 600ml", 2.50, 50, 10, new DateTime(2027, 12, 31)) },
            { "SKU002", new Producto("SKU002", "Galleta Field Soda", 1.20, 2, 5, new DateTime(2026, 1, 1)) } // Vencido para prueba
        };

        static void Main(string[] args)
        {
            Procesar_Operacion_Tienda();
        }

        public static Producto Obtener_Producto_BD(string sku)
        {
            if (bdProductos.ContainsKey(sku))
            {
                return bdProductos[sku];
            }
            return new Producto("", "", 0.0, 0, 0, DateTime.MinValue);
        }

        public static bool Es_Fecha_Vencida(DateTime fechaVenc)
        {
            if (fechaVenc == DateTime.MinValue) return false;
            return fechaVenc < DateTime.Now;
        }

        // --- Módulo: Validación de Reglas de Negocio e Inventario ---
        public static bool Modulo_Validacion_Stock(string sku, int cant)
        {
            Producto prod = Obtener_Producto_BD(sku);

            if (prod.CodigoSKU == "")
            {
                Console.WriteLine("ALERTA: Código no registrado.");
                return false;
            }

            if (prod.StockActual < cant)
            {
                Console.WriteLine("ALERTA: Stock insuficiente.");
                return false;
            }

            if (Es_Fecha_Vencida(prod.FechaVencimiento))
            {
                Console.WriteLine("ALERTA: Producto vencido no apto para venta.");
                return false;
            }

            return true;
        }

        public static void Modulo_Actualizacion_ERP(string sku, int cant, string operacion)
        {
            if (bdProductos.ContainsKey(sku))
            {
                if (operacion == "VENTA")
                {
                    bdProductos[sku].StockActual -= cant;
                    Console.WriteLine($"[ERP] Stock actualizado para {sku}. Stock restante: {bdProductos[sku].StockActual}");
                }
            }
        }

        public static void Modulo_Emision_Comprobante(double total)
        {
            Console.WriteLine("\n================================");
            Console.WriteLine("      TICKET DE VENTA - TAMBO      ");
            Console.WriteLine($"      Total a Pagar: S/ {total:F2}");
            Console.WriteLine("================================");
            Console.WriteLine("¡Gracias por su compra en Tambo!\n");
        }

        // --- Proceso Principal de Registro en Caja ---
        public static void Procesar_Operacion_Tienda()
        {
            string sku;
            int cant = 0;
            double total = 0.0;
            bool activo = true;

            Console.WriteLine("=== SISTEMA POS / REGISTRO DE VENTAS TAMBO ===\n");

            while (activo)
            {
                Console.Write("Ingrese Código SKU del producto: ");
                sku = Console.ReadLine()?.Trim() ?? string.Empty;

                Console.Write("Ingrese Cantidad: ");
                if (!int.TryParse(Console.ReadLine(), out cant))
                {
                    Console.WriteLine("Error: Ingrese un número entero válido.");
                    continue;
                }

                if (Modulo_Validacion_Stock(sku, cant))
                {
                    total += Obtener_Producto_BD(sku).Precio * cant;
                    Modulo_Actualizacion_ERP(sku, cant, "VENTA");
                    Console.WriteLine("Producto agregado al carrito.");
                }

                Console.Write("¿Agregar otro? (SI/NO): ");
                string r = Console.ReadLine()?.Trim() ?? string.Empty;
                if (r.ToUpper() == "NO")
                {
                    activo = false;
                }
            }

            Modulo_Emision_Comprobante(total);
        }
    }
}
// Avance 1: Revisión y pruebas de funciones