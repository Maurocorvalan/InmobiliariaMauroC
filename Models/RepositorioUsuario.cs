namespace Inmobiliaria.Models;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Microsoft.AspNetCore.Mvc;

public class RepositorioUsuario
{
    public RepositorioUsuario() { }

    readonly String ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";


    public Usuario GetUsuario(int id)
    {
        Usuario? usuario = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT * FROM usuarios WHERE {nameof(Usuario.IdUsuario)} = @{nameof(Usuario.IdUsuario)}, {nameof(Usuario.Nombre)}, {nameof(Usuario.Apellido)}, {nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.Rol)};";
        }

        return usuario;
    }

    public int CrearUsuario(Usuario usuario)
    {
        int Id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"INSERT INTO usuarios ({nameof(Usuario.Nombre)}, {nameof(Usuario.Apellido)}, {nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.AvatarUrl)},{nameof(Usuario.Rol)})
                VALUES (@{nameof(Usuario.Nombre)}, @{nameof(Usuario.Apellido)}, @{nameof(Usuario.Email)}, @{nameof(Usuario.Clave)},@{nameof(Usuario.AvatarUrl)} ,@{nameof(Usuario.Rol)});
                SELECT LAST_INSERT_ID();";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Usuario.Nombre)}", usuario.Nombre);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Apellido)}", usuario.Apellido);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Email)}", usuario.Email);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Clave)}", usuario.Clave);
					if (String.IsNullOrEmpty(usuario.AvatarUrl))
						command.Parameters.AddWithValue($"@{nameof(Usuario.AvatarUrl)}", DBNull.Value);
					else
						command.Parameters.AddWithValue("@AvatarUrl", usuario.AvatarUrl);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Rol)}", usuario.Rol);

                connection.Open();
                Id = Convert.ToInt32(command.ExecuteScalar());
                usuario.IdUsuario = Id;
                connection.Close();
            }
        }
        return Id;
    }
    public int ModificarUsuario(Usuario usuario)
    {
        return 0;
    }

}