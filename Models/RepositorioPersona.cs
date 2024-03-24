namespace Inmobiliaria.Models;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Microsoft.AspNetCore.Mvc;

public class RepositorioPersona
{
    public RepositorioPersona()
    {
    }
    readonly String ConnectionString = "Server=localhost;Database=Inmobiliaria;User=root;Password=";



    public IList<Persona> GetPersonas()
    {
        var personas = new List<Persona>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $"SELECT {nameof(Persona.Id)}, {nameof(Persona.Nombre)}, {nameof(Persona.Apellido)}, {nameof(Persona.Dni)}, {nameof(Persona.Email)}  FROM personas";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        personas.Add(new Persona
                        {
                            Id = reader.GetInt32(nameof(Persona.Id)),
                            Nombre = reader.GetString(nameof(Persona.Nombre)),
                            Apellido = reader.GetString(nameof(Persona.Apellido)),
                            Dni = reader.GetInt32(nameof(Persona.Dni)),
                            Email = reader.GetString(nameof(Persona.Email)),
                        });

                    }
                }
            }
        }
        return personas;
    }
    public Persona? GetPersona(int id)
    {
        Persona? persona = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT {nameof(Persona.Id)},{nameof(Persona.Nombre)},{nameof(Persona.Apellido)}, {nameof(Persona.Dni)}, {nameof(Persona.Email)},
				{nameof(Persona.Nombre)}
			 FROM personas
			 WHERE {nameof(Persona.Id)} = @{nameof(Persona.Id)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Persona.Id)}", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        persona = new Persona
                        {
                            Id = reader.GetInt32(nameof(Persona.Id)),
                            Nombre = reader.GetString(nameof(Persona.Nombre)),
                            Apellido = reader.GetString(nameof(Persona.Apellido)),
                            Dni = reader.GetInt32(nameof(Persona.Dni)),
                            Email = reader.GetString(nameof(Persona.Email)),

                        };
                    }
                }
            }
        }
        return persona;
    }
    public int AltaPersona(Persona persona)
    {
        int id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"INSERT INTO personas ({nameof(Persona.Dni)}, {nameof(Persona.Nombre)}, {nameof(Persona.Apellido)}, {nameof(Persona.Email)}) VALUES (@{nameof(Persona.Dni)}, @{nameof(Persona.Nombre)}, @{nameof(Persona.Apellido)}, @{nameof(Persona.Email)}); SELECT LAST_INSERT_ID()"; ;
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Persona.Dni)}", persona.Dni);
                command.Parameters.AddWithValue($"@{nameof(Persona.Nombre)}", persona.Nombre);
                command.Parameters.AddWithValue($"@{nameof(Persona.Apellido)}", persona.Apellido);
                command.Parameters.AddWithValue($"@{nameof(Persona.Email)}", persona.Email);

                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
                persona.Id = id;
                connection.Close();

            }

        }
        return id;

    }

    public int ModificarPersona(Persona persona)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE personas 
             SET {nameof(Persona.Dni)} = @{nameof(Persona.Dni)}, 
                 {nameof(Persona.Nombre)} = @{nameof(Persona.Nombre)}, 
                 {nameof(Persona.Apellido)} = @{nameof(Persona.Apellido)}, 
                 {nameof(Persona.Email)} = @{nameof(Persona.Email)} 
             WHERE {nameof(Persona.Id)} = @{nameof(Persona.Id)}";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Persona.Id)}", persona.Id);
                command.Parameters.AddWithValue($"@{nameof(Persona.Nombre)}", persona.Nombre);
                command.Parameters.AddWithValue($"@{nameof(Persona.Apellido)}", persona.Apellido);
                command.Parameters.AddWithValue($"@{nameof(Persona.Dni)}", persona.Dni);
                command.Parameters.AddWithValue($"@{nameof(Persona.Email)}", persona.Email);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }
        }


        return 0;
    }

    public int EliminaPersona(int id)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"DELETE FROM personas
				WHERE {nameof(Persona.Id)} = @{nameof(Persona.Id)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Persona.Id)}", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;
    }

}
