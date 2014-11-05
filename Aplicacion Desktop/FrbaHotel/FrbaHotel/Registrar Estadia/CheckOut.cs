﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Registrar_Estadia
{
    public partial class CheckOut : Buscador, ITraeBusqueda
    {
        MenuPrincipal parent;
        int idReserva;
        int idRegimen;
        int cantDiasEstadia;
        int cantDiasReserva;
        TableCarrito carrito;

        public CheckOut(MenuPrincipal mp)
        {
            parent = mp;
            InitializeComponent();
            crearBuscador(this, "Id_Consumible as ID, Descripcion, Precio", "Consumibles");
            setearGrid(GridConsumibles);
            setearGrid(GridHabitacion);
            carrito = new TableCarrito();
        }

        private void CheckOut_Load(object sender, EventArgs e)
        {
            cargarGrilla(GridConsumibles, todos);
            GridCarrito.DataSource = carrito.tabla;
        }

        private void VerCO_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
            GroupTotal.Visible = true;
            LabelAllinc.Visible = false;
                if (TxtReserva.Text == "")
            {
                MessageBox.Show("No se ingresó ninguna reserva.");
            }
            else
            {
                idReserva = Int32.Parse(TxtReserva.Text);
                BD bd = new BD();
                bd.obtenerConexion();
                try
                {
                   SqlDataReader dr = bd.lee("EXEC FUGAZZETA.ValidarEstadia " + idReserva + ", '" + new DatePrograma(Program.hoy()).ToString() + "'");
                    while (dr.Read())
                    {
                        idRegimen = Int32.Parse(dr[1].ToString());
                        cantDiasReserva = Convert.ToInt16((Convert.ToDateTime(dr[3].ToString()) - Convert.ToDateTime(dr[2].ToString())).TotalDays);
                        cantDiasEstadia = Convert.ToInt16((Program.hoy() - Convert.ToDateTime(dr[2].ToString())).TotalDays);
                    }
                    dr.Close();
                    GroupConsumibles.Enabled = true;
                    GroupHabitacion.Enabled = true;
                    TxtDiasReserva.Text = cantDiasReserva.ToString();
                    TxtDiasEstadia.Text = cantDiasEstadia.ToString();
                    TxtDiasNoEstadia.Text = (cantDiasReserva - cantDiasEstadia).ToString();
                    TxtDiasReserva.Visible = true;
                    TxtDiasEstadia.Visible = true;
                    TxtDiasNoEstadia.Visible = true;
                    if (idRegimen == 2) LabelAllinc.Visible = true;
                    cargarHabitaciones();
                    mostrarTotalHabitacion();
                    mostrarTotalTotal();
                }
                catch (SqlException ex)
                {
                    foreach (SqlError sqlError in ex.Errors)
                        MessageBox.Show("Error: " + sqlError.Message);
                }
            }
        }


        #region ITraeBusqueda Members

        public void agregar(string id, string descripcion)
        {
            TxtConsumible.Tag = new Consumible(id, descripcion);
            TxtConsumible.Text = TxtConsumible.Tag.ToString();
        }

        #endregion

        private void Cantidad_ValueChanged(object sender, EventArgs e)
        {
            double precio = (TxtConsumible.Tag as Consumible).precio * Convert.ToInt32(Cantidad.Value);
            retPrecio(precio);
        }

        private void retPrecio(double precio)
        {
            if (idRegimen == 2) LblPrecio.Text = "$ 0.-";
            else LblPrecio.Text = "$ " + precio + ".-";
        }

        private void AgregarCarrito_Click(object sender, EventArgs e)
        {
            carrito.add((TxtConsumible.Tag as Consumible),Convert.ToInt32(Cantidad.Value),idRegimen);
            Cantidad.Value = 1;
            LblPrecio.Text = "$";
            TxtConsumible.Text = "";
            TxtConsumible.Tag = null;
            mostrarTotalConsumible();
            mostrarTotalTotal();
            AgregarCarrito.Enabled = false;
            Cantidad.Enabled = false;

        }

        private void mostrarTotalConsumible()
        {
            LblSubConsumibles.Text = carrito.total().ToString();
        }

        private void mostrarConsumible()
        {
            agregar(celdaElegida(GridConsumibles, 0), celdaElegida(GridConsumibles, 1));
            retPrecio((TxtConsumible.Tag as Consumible).precio * Convert.ToInt32(Cantidad.Value));
            AgregarCarrito.Enabled = true;
            Cantidad.Enabled = true;
        }

        #region GridConsumibles
        private void GridConsumibles_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            mostrarConsumible();
        }

        private void GridConsumibles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            mostrarConsumible();
        }

        private void GridCarrito_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 & e.RowIndex >= 0)
            {
                carrito.remove(e.RowIndex);
                mostrarTotalConsumible();
                mostrarTotalTotal();
            }
        }
        #endregion

        private void cargarHabitaciones()
        {
            BD bd2 = new BD();
            bd2.obtenerConexion();
            string query = "EXEC FUGAZZETA.VerHabitacionesDeEstadia " + idReserva + ", '" + new DatePrograma(Program.hoy()).ToString() + "'";
            GridHabitacion.DataSource = bd2.ejecutar(query);
            bd2.cerrar();
            
        }

        private void TxtReserva_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                VerCO_Click(sender, e);
            }
        }

        private void mostrarTotalHabitacion()
        {
            double totalHabitacion = 0;
            for (int i = 0; i < GridHabitacion.Rows.Count; i++)
            {
                double tot = Convert.ToDouble(GridHabitacion.Rows[i].Cells[4].Value.ToString());
                totalHabitacion = tot + totalHabitacion;
            }
            LblSubEstadia.Text = totalHabitacion.ToString();
        }

        private void mostrarTotalTotal()
        {
            LblTotal.Text = (Int32.Parse(LblSubConsumibles.Text) + Int32.Parse(LblSubEstadia.Text)).ToString();
        }

    }
}
