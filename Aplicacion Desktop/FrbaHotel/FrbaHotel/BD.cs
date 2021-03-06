﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System.Configuration;

namespace FrbaHotel
{
    public class BD
    {    
        static SqlConnection Conexion = new SqlConnection();
        public SqlDataAdapter dataAdapter;

        public SqlConnection obtenerConexion()
            {
                if (Conexion.State == ConnectionState.Closed)
                {
                    Conexion.ConnectionString = this.connectionString();
                    Conexion.Open();
                }
                return Conexion;
            }

        public String connectionString()
            {
                return "Server=localhost\\SQLSERVER2008;Database=GD2C2014;User Id=gd;Password=gd2014;MultipleActiveResultSets=True";
            }

        public DataTable ejecutar(string selectCommand)
        {
            DataTable tabla = new DataTable();
            try
            {
                dataAdapter = new SqlDataAdapter(selectCommand, this.connectionString());
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                tabla.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(tabla);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("No se pudo conectar a la base de datos. Error " + ex.Number.ToString() + ": " + ex.Message);
            }
            return tabla;
        }

        public void insertar(string tablaDestino, string campos, string valores)
        {
            string comando = "INSERT INTO FUGAZZETA." + tablaDestino + " (" + campos + ") values (" + valores + ")";
            this.ejecutar(comando);
        }

        public void eliminar(string tablaDestino, string condiciones)
        {
            string comando = "DELETE FROM FUGAZZETA." + tablaDestino + " WHERE " + condiciones;
            this.ejecutar(comando);
        }

        public void insertar(string tablaDestino, string valores)
        {
            string comando = "INSERT INTO FUGAZZETA." + tablaDestino + " values (" + valores + ")";
            this.ejecutar(comando);
        }

        public void cerrar()
        {
            Conexion.Close();
        }

        #region rellenarListasCombos
        public void rellenarDesde(string campo, string tablaOrigen, ListBox lista)
        {
            try
            {
                string comando = "SELECT " + campo + "FROM FUGAZZETA." + tablaOrigen;
                SqlDataReader respuesta = lee(comando);

                while (respuesta.Read())
                {
                    lista.Items.Add(respuesta[campo].ToString());
                }
                respuesta.Close();
            } catch (Exception ex)
            {
                MessageBox.Show("Error al completar: " + ex.Message);
            }
        }

        public void rellenarDesde(string campo, string tablaOrigen, ComboBox cb)
        {
            try
            {
                string comando = "SELECT " + campo + " FROM FUGAZZETA." + tablaOrigen;
                SqlDataReader respuesta = lee(comando);

                while (respuesta.Read())
                {
                    cb.Items.Add(respuesta[campo].ToString());
                }
                respuesta.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al completar: " + ex.Message);
            }
        }
        #endregion

        internal SqlConnection getConexion()
        {
            return Conexion;
        }

        public SqlDataReader lee(string query)
        {
            obtenerConexion();
            SqlCommand cmd = new SqlCommand(query, getConexion());
            return cmd.ExecuteReader();
        }
    }
}

