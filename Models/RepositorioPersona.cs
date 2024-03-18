 namespace Inmobiliaria.Models;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

public class RepositorioPersona
{
    readonly String  ConnectionString = "Server=localhost;Database=Inmobiliaria;User=root;Password=";
    
    public RepositorioPersona(){

    }
    
    public IList<Persona> GetPersonas()
    {
        var personas = new List<Persona>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql =$"SELECT {nameof(Persona.Id)}, {nameof(Persona.Nombre)}, {nameof(Persona.Apellido)}, {nameof(Persona.Dni)}, {nameof(Persona.Email)}  FROM personas";
            using (var command = new MySqlCommand(sql, connection)){
                connection.Open();
                using(var reader = command.ExecuteReader()){
                    while (reader.Read()){
                        personas.Add(new Persona{
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
}
