﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Cancelar_Reserva
{
    public partial class CancelarReserva : Form
    {
        MenuPrincipal menu;
        public CancelarReserva(MenuPrincipal parent)
        {
            menu = parent;
            InitializeComponent();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            string codigo = TxtCodigo.Text;
            DialogResult confirma = MessageBox.Show("Está seguro de cancelar la reserva ID:" + codigo + "?","Cancelar Reserva",MessageBoxButtons.OKCancel);
                if (confirma == DialogResult.OK)
            {
                try
                {
                    BD bd = new BD();
                    bd.obtenerConexion();
                    validarReserva(bd);
                    validarContraseña(bd);
                    cancelar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo realizar la baja. " + ex.Message);
                    this.DialogResult = DialogResult.Retry;
                }
            }

        }

        private void cancelar()
        {

            MessageBox.Show("La reserva se ha cancelado con éxito.");
            this.Close();
        }

        private void validarContraseña(BD bd)
        {
            if (menu.usuarioActual != "guest")
            {
                if (TxtPass.Text == "")
                {
                    throw new Exception("Debe completar una contraseña. ");
                }
                string query = "SELECT Contraseña FROM FUGAZZETA.Usuarios WHERE Username = '" + menu.usuarioActual + "'";
                SqlDataReader dr = bd.lee(query);
                while (dr.Read())
                {
                    if (dr[0].ToString() != TxtPass.Text)
                    {
                        throw new Exception("La contraseña es incorrecta.");
                    }
                }
                dr.Close();
            }
        }

        private void validarReserva(BD bd)
        {
            string query = "SELECT * FROM FUGAZZETA.Reservas WHERE Id_Reserva = " + TxtCodigo.Text;
            SqlDataReader dr = bd.lee(query);
            if (!dr.HasRows)
            {
                throw new Exception("La reserva que intenta cancelar no existe.");
            }
            dr.Close();
        }

        private void CancelarReserva_Load(object sender, EventArgs e)
        {
            if (menu.usuarioActual == "guest")
            {
                groupBox1.Enabled = false;
            }
        }

    }
}
