namespace Inmobiliaria.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using MySql.Data.MySqlClient;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    public class RepositorioPago
    {
        public RepositorioPago() { }

        readonly String ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";

        public IList<Pago> GetPagos()
        {
            var pagos = new List<Pago>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"SELECT 
                        p.{nameof(Pago.IdPago)}, 
                        p.{nameof(Pago.FechaPago)}, 
                        p.{nameof(Pago.Monto)}, 
                        p.{nameof(Pago.Detalle)}, 
                        p.{nameof(Pago.Estado)}, 
                        p.{nameof(Pago.IdContrato)},
                        P.{nameof(Pago.EsMulta)}
                    FROM pagos p 
                    INNER JOIN contratos c ON p.{nameof(Pago.IdContrato)} = c.{nameof(Contrato.IdContrato)}";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pagos.Add(new Pago()
                            {
                                IdPago = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.IdPago))) ? 0 : reader.GetInt32(nameof(Pago.IdPago)),
                                FechaPago = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.FechaPago))) ? DateTime.MinValue : reader.GetDateTime(nameof(Pago.FechaPago)),
                                Monto = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Monto))) ? 0 : reader.GetDecimal(nameof(Pago.Monto)),
                                Detalle = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Detalle))) ? string.Empty : reader.GetString(nameof(Pago.Detalle)),
                                Estado = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Estado))) ? false : reader.GetBoolean(nameof(Pago.Estado)),
                                IdContrato = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.IdContrato))) ? 0 : reader.GetInt32(nameof(Pago.IdContrato)),
                                EsMulta = reader.GetBoolean(nameof(Pago.EsMulta))
                            });
                        }
                    }
                }
            }
            return pagos;
        }



        public Pago? GetPago(int id)
        {
            Pago? pago = null;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"SELECT 
        p.{nameof(Pago.IdPago)}, 
        p.{nameof(Pago.FechaPago)}, 
        p.{nameof(Pago.Monto)}, 
        p.{nameof(Pago.Detalle)}, 
        p.{nameof(Pago.Estado)}, 
        p.{nameof(Pago.EsMulta)},
        p.{nameof(Pago.IdContrato)} 
        FROM 
            pagos p 
        INNER JOIN 
            contratos c ON p.{nameof(Pago.IdContrato)} = c.{nameof(Contrato.IdContrato)}
        WHERE 
            p.{nameof(Pago.IdPago)} = @IdPago";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdPago", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = new Pago()
                            {
                                IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                                Monto = reader.GetDecimal(nameof(Pago.Monto)),
                                FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
                                Detalle = reader.GetString(nameof(Pago.Detalle)),
                                IdContrato = reader.GetInt32(nameof(Pago.IdContrato)),
                                Estado = reader.GetBoolean(nameof(Pago.Estado)),
                                EsMulta = reader.GetBoolean(nameof(Pago.EsMulta)),
                                Contrato = new Contrato
                                {
                                    IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                                    Estado = reader.GetBoolean(nameof(Contrato.Estado)),
                                }
                            };
                        }
                    }
                }
            }
            return pago;
        }


        public int ModificarPago(Pago pago)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"UPDATE pagos 
                            SET 
                             {nameof(Pago.FechaPago)} = @{nameof(Pago.FechaPago)},
                             {nameof(Pago.Monto)} = @{nameof(Pago.Monto)},
                             {nameof(Pago.Detalle)} = @{nameof(Pago.Detalle)},
                             {nameof(Pago.Estado)} = @{nameof(Pago.Estado)},
                             {nameof(Pago.EsMulta)} = @{nameof(Pago.EsMulta)},
                             {nameof(Pago.IdContrato)} = @{nameof(Pago.IdContrato)}

                            WHERE 
                            {nameof(Pago.IdPago)} = @{nameof(Pago.IdPago)};";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Pago.IdPago)}", pago.IdPago);
                    command.Parameters.AddWithValue($"@{nameof(Pago.FechaPago)}", pago.FechaPago);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Monto)}", pago.Monto);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Detalle)}", pago.Detalle);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Estado)}", pago.Estado ? 1 : 0);
                    command.Parameters.AddWithValue($"@{nameof(Pago.EsMulta)}", pago.EsMulta);
                    command.Parameters.AddWithValue($"@{nameof(Pago.IdContrato)}", pago.IdContrato);
                    connection.Open();
                    var result = command.ExecuteNonQuery();
                    connection.Close();

                    return result;
                }
            }
        }

        public int CrearPago(Pago pago)
        {
            int Id = 0;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"INSERT INTO pagos ({nameof(Pago.FechaPago)}, {nameof(Pago.Monto)}, {nameof(Pago.Detalle)}, {nameof(Pago.Estado)}, {nameof(Pago.IdContrato)}) 
                             VALUES (@{nameof(Pago.FechaPago)}, @{nameof(Pago.Monto)}, @{nameof(Pago.Detalle)}, @{nameof(Pago.Estado)}, @{nameof(Pago.IdContrato)});
                             SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Pago.FechaPago)}", pago.FechaPago);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Monto)}", pago.Monto);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Detalle)}", pago.Detalle);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Estado)}", pago.Estado);
                    command.Parameters.AddWithValue($"@{nameof(Pago.IdContrato)}", pago.IdContrato);
                    connection.Open();
                    Id = Convert.ToInt32(command.ExecuteScalar());
                    pago.IdPago = Id;
                    connection.Close();
                }
            }
            return Id;
        }

        [Authorize(Policy = "Administrador")]

        public int EliminarPago(int id, bool estado)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"UPDATE pagos SET {nameof(Pago.Estado)} = @{nameof(Pago.Estado)} WHERE {nameof(Pago.IdPago)} = @{nameof(Pago.IdPago)};";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Pago.IdPago)}", id);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Estado)}", estado);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return 0;
        }

        public IList<Pago> GetPagosPorContrato(int idContrato)
        {
            var pagos = new List<Pago>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"
                            SELECT 
                                {nameof(Pago.IdPago)},
                                {nameof(Pago.FechaPago)},
                                {nameof(Pago.Monto)},
                                {nameof(Pago.Detalle)},
                                {nameof(Pago.Estado)},
                                {nameof(Pago.IdContrato)}
                            FROM pagos
                            WHERE {nameof(Pago.IdContrato)} = @IdContrato"; ;

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", idContrato);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pagos.Add(new Pago
                            {
                                IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                                FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
                                Monto = reader.GetDecimal(nameof(Pago.Monto)),
                                Detalle = reader.GetString(nameof(Pago.Detalle)),
                                Estado = reader.GetBoolean(nameof(Pago.Estado)),
                                IdContrato = reader.GetInt32(nameof(Pago.IdContrato))
                            });
                        }
                    }
                }
            }
            return pagos;
        }


        public IList<Pago> GetPagosPendientes(int idContrato)
        {
            var pagosPendientes = new List<Pago>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"
            SELECT * 
            FROM pagos 
            WHERE IdContrato = @IdContrato AND Estado = 0";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", idContrato);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pagosPendientes.Add(new Pago
                            {
                                IdPago = reader.GetInt32("IdPago"),
                                IdContrato = reader.GetInt32("IdContrato"),
                                FechaPago = reader.GetDateTime("FechaPago"),
                                Estado = reader.GetBoolean("Estado"),
                                Monto = reader.GetDecimal("Monto")
                            });
                        }
                    }
                }
            }
            return pagosPendientes;
        }


        public void RegistrarMulta(int idContrato, decimal montoMulta)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"
            INSERT INTO pagos (IdContrato, FechaPago, Estado, Monto, Detalle) 
            VALUES (@IdContrato, @FechaPago, 0, @MontoMulta, @Detalle, 1)";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", idContrato);
                    command.Parameters.AddWithValue("@FechaPago", DateTime.Now);
                    command.Parameters.AddWithValue("@MontoMulta", montoMulta);
                    command.Parameters.AddWithValue("@Detalle", "Multa por finalización anticipada de contrato");

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


        public int CalcularPagosAdeudados(int idContrato, DateTime fechaInicio, DateTime fechaTerminacion)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                int mesesTranscurridos = ((fechaTerminacion.Year - fechaInicio.Year) * 12) + fechaTerminacion.Month - fechaInicio.Month;

                string pagosRealizadosQuery = @"
            SELECT COUNT(*) 
            FROM pagos 
            WHERE IdContrato = @IdContrato AND Estado = 1";

                using (var command = new MySqlCommand(pagosRealizadosQuery, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", idContrato);
                    int pagosRealizados = Convert.ToInt32(command.ExecuteScalar());

                    int mesesAdeudados = mesesTranscurridos - pagosRealizados;
                    return mesesAdeudados > 0 ? mesesAdeudados : 0;
                }
            }
        }

    }


}
