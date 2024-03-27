namespace Inmobiliaria.Models;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Microsoft.AspNetCore.Mvc;

public class RepositorioInmueble
{

    public RepositorioInmueble()
    {

    }
    readonly String ConnectionString = "Server=localhost;Database=Inmobiliaria;User=root;Password=";


    public IList<Inmueble> GetInmuebles()
    {
        var inmuebles = new List<Inmueble>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $"SELECT {nameof(Inmueble.IdInmueble)}, {nameof(Inmueble.Direccion)}, {nameof(Inmueble.Uso)}, {nameof(Inmueble.Tipo)}, {nameof(Inmueble.Ambientes)}, {nameof(Inmueble.Superficie)}, {nameof(Inmueble.Valor)}, i.{nameof(Inmueble.IdPropietario)}, p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)} FROM inmuebles i INNER JOIN propietarios p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Uso = reader.GetString(nameof(Inmueble.Uso)),
                            Tipo = reader.GetString(nameof(Inmueble.Tipo)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Superficie = reader.GetInt32(nameof(Inmueble.Superficie)),
                            Valor = reader.GetDecimal(nameof(Inmueble.Valor)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido))
                            }
                        });

                    }

                    connection.Close();
                }
            }
        }
        return inmuebles;
    }

}