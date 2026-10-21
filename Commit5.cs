using System;
using System.Collections.Generic;
namespace Commit5
{ }
//Sistema completo con alertas de reabastecimiento por stock mínimo//
namespace SistemaPOS_Tambo
{
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
            { "SKU002", new Producto("SKU002", "Galleta Field Soda", 1.20, 6, 5, new DateTime(2027, 1, 1)) },
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

        // --- NUEVA FUNCIONALIDAD COMMIT 5: Control y Alerta de Stock Mínimo ---
        public static void Modulo_Actualizacion_ERP(string sku, int cant)
        {
            if (bdProductos.ContainsKey(sku))
            {
                bdProductos[sku].StockActual -= cant;
                Console.WriteLine($"[ERP] Stock actualizado para {sku}. Restante: {bdProductos[sku].StockActual}");

                if (bdProductos[sku].StockActual <= bdProductos[sku].StockMinimo)
                {
                    Console.WriteLine($"[ALERTA REABASTECIMIENTO] El producto '{bdProductos[sku].Nombre}' alcanzó el stock mínimo ({bdProductos[sku].StockActual} unidades restantes).");
                }
            }
        }

        public static double Modulo_Aplicar_Descuento(double subtotal, string cupon)
        {
            if (cupon.ToUpper() == "TAMBO10")
            {
                double descuento = subtotal * 0.10;
                Console.WriteLine($"\n[CUPÓN APLICADO] Descuento del 10% (-S/ {descuento:F2})");
                return subtotal - descuento;
            }
            return subtotal;
        }

        public static bool Modulo_Procesar_Pago(double totalPagar)
        {
            Console.WriteLine("\n================================");
            Console.WriteLine("      MÓDULO DE PAGO - TAMBO    ");
            Console.WriteLine($"      Total a Pagar: S/ {totalPagar:F2}");
            Console.WriteLine("================================");
            Console.WriteLine("1. Yape");
            Console.WriteLine("2. Tarjeta de Crédito / Débito");
            Console.Write("Ingrese opción (1 o 2): ");

            string opcion = Console.ReadLine()?.Trim() ?? "";

            if (opcion == "1")
            {
                Console.Write("Ingrese celular Yape (9 dígitos): ");
                return (Console.ReadLine()?.Trim() ?? "").Length == 9;
            }
            else if (opcion == "2")
            {
                Console.Write("Ingrese número de tarjeta (16 dígitos): ");
                return (Console.ReadLine()?.Trim() ?? "").Length >= 16;
            }
            return false;
        }

        public static void Modulo_Emision_Comprobante(double total)
        {
            Console.WriteLine("\n================================");
            Console.WriteLine("      TICKET DE VENTA - TAMBO      ");
            Console.WriteLine($"      Fecha: {DateTime.Now}");
            Console.WriteLine($"      Total Pagado: S/ {total:F2}");
            Console.WriteLine("================================");
            Console.WriteLine("¡Gracias por su compra en Tambo!\n");
        }

        public static void Procesar_Operacion_Tienda()
        {
            double subtotal = 0.0;
            bool activo = true;

            Console.WriteLine("=== SISTEMA POS TAMBO - V1.4 (SISTEMA COMPLETO) ===\n");

            while (activo)
            {
                Console.Write("Ingrese Código SKU del producto: ");
                string sku = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Ingrese Cantidad: ");
                if (!int.TryParse(Console.ReadLine(), out int cant) || cant <= 0) continue;

                if (Modulo_Validacion_Stock(sku, cant))
                {
                    subtotal += Obtener_Producto_BD(sku).Precio * cant;
                    Modulo_Actualizacion_ERP(sku, cant);
                }

                Console.Write("¿Agregar otro producto? (SI/NO): ");
                if (Console.ReadLine()?.Trim().ToUpper() == "NO") activo = false;
            }

            if (subtotal > 0)
            {
                Console.Write("\n¿Tiene cupón de descuento? (Ej: TAMBO10 / Enter para omitir): ");
                string cupon = Console.ReadLine()?.Trim() ?? "";
                double totalFinal = Modulo_Aplicar_Descuento(subtotal, cupon);

                if (Modulo_Procesar_Pago(totalFinal))
                {
                    Modulo_Emision_Comprobante(totalFinal);
                }
                else
                {
                    Console.WriteLine("\n[ERROR] Venta cancelada debido a fallo en el pago.");
                }
            }
        }
    }
}