using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Inmobiliaria.Models
{
    public class RepositorioAuditoria
    {
        readonly string ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";

        public void RegistrarAuditoria(string accion, string usuario, string? detalle = null)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = "INSERT INTO auditorias (Accion, Usuario, Detalle) VALUES (@Accion, @Usuario, @Detalle)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Accion", accion);
                    command.Parameters.AddWithValue("@Usuario", usuario);
                    command.Parameters.AddWithValue("@Detalle", detalle);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public IList<Auditoria> ObtenerAuditorias()
        {
            var auditorias = new List<Auditoria>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = "SELECT * FROM auditorias ORDER BY Fecha DESC";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            auditorias.Add(new Auditoria
                            {
                                Id = reader.GetInt32("Id"),
                                Accion = reader.GetString("Accion"),
                                Usuario = reader.GetString("Usuario"),
                                Fecha = reader.GetDateTime("Fecha"),
                                Detalle = reader.IsDBNull(reader.GetOrdinal("Detalle")) ? null : reader.GetString("Detalle")
                            });
                        }
                    }
                }
            }
            return auditorias;
        }
    }
}
