using System;
using System.Collections.Generic;
using System.Reflection;

namespace Commit2
{ }
//Módulo de Cupones de Descuento//
namespace SistemaPOS_Tambo
{
    public class Producto
    {
        public string CodigoSKU { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public DateTime FechaVencimiento { get; set; } // Se mantiene el campo de fecha

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
            { "SKU002", new Producto("SKU002", "Galleta Field Soda", 1.20, 40, 5, new DateTime(2027, 1, 1)) },
            { "SKU003", new Producto("SKU003", "Turron Doña Pepa", 9.50, 30, 5, new DateTime(2027, 2, 4)) },
            { "SKU004", new Producto("SKU004", "Keke Bimbo", 4.00, 20, 5, new DateTime(2027, 5, 14)) },
            { "SKU005", new Producto("SKU005", "Cereal Angel 450gr", 3.20, 40, 5, new DateTime(2027, 6, 1)) }
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
        public static double Modulo_Aplicar_Descuento(double subtotal, string cupon)
        {
            if (cupon.ToUpper() == "TAMBO10")
            {
                double descuento = subtotal * 0.10; // 10% de descuento
                Console.WriteLine($"\n[CUPÓN APLICADO] ¡Felicidades! Se aplicó un 10% de descuento (-S/ {descuento:F2})");
                return subtotal - descuento;
            }
            else if (!string.IsNullOrEmpty(cupon))
            {
                Console.WriteLine("\n[ALERTA] El cupón ingresado no es válido.");
            }
            return subtotal;
        }

        public static void Procesar_Operacion_Tienda()
        {
            double subtotal = 0.0;
            bool activo = true;

            Console.WriteLine("=== SISTEMA POS TAMBO - V1.1 (Con Módulo de Descuentos) ===\n");

            while (activo)
            {
                Console.Write("Ingrese Código SKU del producto: ");
                string sku = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Ingrese Cantidad: ");
                if (!int.TryParse(Console.ReadLine(), out int cant) || cant <= 0)
                {
                    Console.WriteLine("Error: Ingrese una cantidad válida.\n");
                    continue;
                }

                if (Modulo_Validacion_Stock(sku, cant))
                {
                    subtotal += Obtener_Producto_BD(sku).Precio * cant;
                    Modulo_Actualizacion_ERP(sku, cant);
                    Console.WriteLine("Producto agregado al carrito.\n");
                }

                Console.Write("¿Agregar otro producto? (SI/NO): ");
                if (Console.ReadLine()?.Trim().ToUpper() == "NO") activo = false;
            }

            if (subtotal > 0)
            {
                Console.WriteLine($"\nSubtotal de la compra: S/ {subtotal:F2}");
                Console.Write("¿Tiene un cupón de descuento? (Ejemplo: TAMBO10 / Presione Enter para omitir): ");
                string cupon = Console.ReadLine()?.Trim() ?? "";

                double totalFinal = Modulo_Aplicar_Descuento(subtotal, cupon);
                Console.WriteLine($"\n================================");
                Console.WriteLine($"   TOTAL FINAL A PAGAR: S/ {totalFinal:F2}");
                Console.WriteLine($"================================");
            }
        }
    }
}