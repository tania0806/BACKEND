using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using reportesApi.DataContext;
using reportesApi.Models;
using System.Collections.Generic;
using reportesApi.Models.Compras;
using OfficeOpenXml;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
namespace reportesApi.Services
{
    public class RecetasService
    {
        private  string connection;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private ArrayList parametros = new ArrayList();


        public RecetasService(IMarcatelDatabaseSetting settings, IWebHostEnvironment webHostEnvironment)
        {
             connection = settings.ConnectionString;

             _webHostEnvironment = webHostEnvironment;
             
        } 
            
            public List<GetRecetasModel> GetRecetaId(int Id)
{
    ConexionDataAccess dac = new ConexionDataAccess(connection);
    ArrayList parametros = new ArrayList();
    parametros.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = Id });

    List<GetRecetasModel> lista = new List<GetRecetasModel>();
    try
    {
        // Llamada al procedimiento almacenado con el parámetro Id
        DataSet ds = dac.Fill("sp_get_recetas", parametros);

        // Comprobar si hay datos en la respuesta
        if (ds.Tables[0].Rows.Count > 0)
        {
            // Mapear los resultados a la lista de GetRecetasModel
            lista = ds.Tables[0].AsEnumerable()
                .Select(dataRow => new GetRecetasModel
                {
                    Id = int.Parse(dataRow["Id"].ToString()),
                    Nombre = dataRow["Nombre"].ToString(),
                    Estatus = int.Parse(dataRow["Estatus"].ToString()),
                    FechaCreacion = dataRow["FechaCreacion"].ToString(),
                    UsuarioRegistra = dataRow["UsuarioRegistra"].ToString(),
                }).ToList();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        throw ex;
    }

    return lista;
}


        public List<GetRecetasModel> GetRecetas(DateTime? fechaInicio = null, DateTime? fechafinal = null)
{
    ConexionDataAccess dac = new ConexionDataAccess(connection);
    var parametros = new ArrayList(); // Inicializar correctamente antes de usar.

    // Agregar parámetros de fecha.
    if (fechaInicio.HasValue)
        parametros.Add(new SqlParameter("@FechaInicio", fechaInicio.Value));
    else
        parametros.Add(new SqlParameter("@FechaInicio", DBNull.Value));

    if (fechafinal.HasValue)
        parametros.Add(new SqlParameter("@FechaFinal", fechafinal.Value));
    else
        parametros.Add(new SqlParameter("@FechaFinal", DBNull.Value));

    var lista = new List<GetRecetasModel>();
    try
    {
        // Llamada al procedimiento almacenado.
        DataSet ds = dac.Fill("sp_get_recetasfecha", parametros);

        // Verificar si hay datos y mapearlos al modelo.
        if (ds.Tables[0].Rows.Count > 0)
        {
            lista = ds.Tables[0].AsEnumerable()
                .Select(dataRow => new GetRecetasModel
                {
                    Id = int.Parse(dataRow["Id"].ToString()),
                    Nombre = dataRow["Nombre"].ToString(),
                    Estatus = int.Parse(dataRow["Estatus"].ToString()),
                    FechaCreacion = dataRow["FechaCreacion"].ToString(),
                    UsuarioRegistra = dataRow["UsuarioRegistra"].ToString(),
                })
                .ToList();
        }
    }
    catch (Exception ex)
    {
        // Agregar información adicional al mensaje de error para diagnóstico.
        Console.WriteLine($"Error al ejecutar 'sp_get_recetasfecha': {ex.Message}");
        throw;
    }

    return lista;
}


        public string InsertReceta(InsertRecetasModel receta)
        {
            ConexionDataAccess dac = new ConexionDataAccess(connection);
            parametros = new ArrayList();
            int IdReceta;
            parametros.Add(new SqlParameter { ParameterName = "@Nombre", SqlDbType = System.Data.SqlDbType.VarChar, Value = receta.Nombre });
            parametros.Add(new SqlParameter { ParameterName = "@Estatus", SqlDbType = System.Data.SqlDbType.Int, Value = receta.Estatus});
            parametros.Add(new SqlParameter { ParameterName = "@UsuarioRegistra", SqlDbType = System.Data.SqlDbType.Int, Value = receta.UsuarioRegistra });


            try 
            {
                DataSet ds = dac.Fill("sp_insert_recetas", parametros);
                IdReceta = ds.Tables[0].AsEnumerable().Select(dataRow=>int.Parse(dataRow["IdReceta"].ToString())).ToList()[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            return IdReceta.ToString();
        }

        public string UpdateRecetas(UpdateRecetasModel recetas)
        {
            ConexionDataAccess dac = new ConexionDataAccess(connection);
            parametros = new ArrayList();
            string mensaje;


            parametros.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = System.Data.SqlDbType.Int, Value = recetas.Id });
            parametros.Add(new SqlParameter { ParameterName = "@Nombre", SqlDbType = System.Data.SqlDbType.VarChar, Value = recetas.Nombre });
            parametros.Add(new SqlParameter { ParameterName = "@Estatus", SqlDbType = System.Data.SqlDbType.Int, Value = recetas.Estatus});
            parametros.Add(new SqlParameter { ParameterName = "@UsuarioRegistra", SqlDbType = System.Data.SqlDbType.Int, Value = recetas.UsuarioRegistra});

            try
            {
                DataSet ds = dac.Fill("sp_update_recetas", parametros);
                mensaje = ds.Tables[0].AsEnumerable().Select(dataRow => dataRow["mensaje"].ToString()).ToList()[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return mensaje;
        }

      public void DeleteRecetas(int id)
        {
            ConexionDataAccess dac = new ConexionDataAccess(connection);
            parametros = new ArrayList();
            parametros.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = id });


            try
            {
                dac.ExecuteNonQuery("sp_delete_recetas", parametros);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal object GetRecetas(int id)
        {
            throw new NotImplementedException();
        }
    }
}