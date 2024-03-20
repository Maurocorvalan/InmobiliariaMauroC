namespace Inmobiliaria.Models;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Microsoft.AspNetCore.Mvc;

public class RepositorioPropietarios
{
    public RepositorioPropietarios()
    {

    }
    readonly String ConnectionString = "Server=localhost;Database=Inmobiliaria;User=root;Password=";


    //Muestra todos los propietarios
    public IList<Propietario> GetPropietarios()
    {
        var propietarios = new List<Propietario>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $"SELECT {nameof(Propietario.IdPropietario)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.Dni)}, {nameof(Propietario.Email)}, {nameof(Propietario.Telefono)}  FROM propietarios";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        propietarios.Add(new Propietario
                        {
                            IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Dni = reader.GetString(nameof(Propietario.Dni)),
                            Email = reader.GetString(nameof(Propietario.Email)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono))
                        });

                    }
                }
            }
        }
        return propietarios;
    }

    public Propietario? GetPropietario(int idPropietario)
    {
        Propietario? propietario = null;

        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT {nameof(Propietario.IdPropietario)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.Dni)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)}, {nameof(Propietario.Clave)}  FROM propietarios WHERE {nameof(Propietario.IdPropietario)} = @{nameof(Propietario.IdPropietario)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Propietario.IdPropietario)}", idPropietario);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        propietario = new Propietario
                        {
                            IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Dni = reader.GetString(nameof(Propietario.Dni)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Email = reader.GetString(nameof(Propietario.Email)),
                            Clave = reader.GetString(nameof(Propietario.Clave))
                        };
                    }
                }

            }
        }
        return propietario;
    }

    public int CrearPropietario(Propietario propietario)
    {
        int Id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"INSERT INTO propietarios ({nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.Dni)}, {nameof(Propietario.Email)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Clave)}) 
                     VALUES (@{nameof(Propietario.Nombre)}, @{nameof(Propietario.Apellido)}, @{nameof(Propietario.Dni)}, @{nameof(Propietario.Email)}, @{nameof(Propietario.Telefono)}, @{nameof(Propietario.Clave)});
                     SELECT LAST_INSERT_ID()";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Propietario.Nombre)}", propietario.Nombre);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Apellido)}", propietario.Apellido);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Dni)}", propietario.Dni);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Email)}", propietario.Email);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Telefono)}", propietario.Telefono);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Clave)}", propietario.Clave);

                connection.Open();
                Id = Convert.ToInt32(command.ExecuteScalar());
                propietario.IdPropietario = Id;
                connection.Close();
            }
        }
        return Id;
    }
    public int ModificarPropietario(Propietario propietario)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE propietarios SET {nameof(Propietario.IdPropietario)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.Dni)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)}, {nameof(Propietario.Clave)}  FROM propietarios WHERE {nameof
            (Propietario.IdPropietario)} = @{nameof(Propietario.IdPropietario)}";

            using (var command = new MySqlCommand(sql, connection))
            {

                command.Parameters.AddWithValue($"@{nameof(Propietario.Nombre)}", propietario.Nombre);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Apellido)}", propietario.Apellido);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Dni)}", propietario.Dni);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Email)}", propietario.Email);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Telefono)}", propietario.Telefono);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Clave)}", propietario.Clave);


                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;
    }



    public int EliminarPropietario(int id)
    {
        using (var connection = new MySqlConnection(ConnectionString)){
            var sql = @$"DELETE FROM propietarios WHERE {nameof(Propietario.IdPropietario)} = @{nameof(Propietario.IdPropietario)}";
            using (var command = new MySqlCommand(sql, connection)){
                
                command.Parameters.AddWithValue($"@{nameof(Propietario.IdPropietario)}", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;
    }
}