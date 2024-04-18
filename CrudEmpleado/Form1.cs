using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CrudEmpleado
{
    public partial class Form1 : Form
    {
        void CargarEmpleados()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cnx))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("paListaEmpleados", cn);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        dataGridView1.Rows.Add(dr.GetValue(0),
                        dr.GetValue(1),
                        dr.GetValue(2),
                        dr.GetValue(3),
                        dr.GetValue(4),
                        dr.GetValue(5),
                        dr.GetValue(6),
                        dr.GetValue(7)
                        );
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        void CargarCargos()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cnx))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT idCargo, NomCargo FROM CARGO", cn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "cargos");
                 
                    cbxCriterio.DataSource = ds.Tables["cargos"];
                    cbxCriterio.DisplayMember = "NomCargo";
                    cbxCriterio.ValueMember = "idCargo";
                    cbxCriterio.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los cargos: " + ex.Message);
            }
        }

        void CargarEmpleadosPorCargo()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cnx))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("paListarCargos", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cargo", cbxCriterio.SelectedValue);
                    SqlDataReader dr = cmd.ExecuteReader();
                    dataGridView1.Rows.Clear();
                    while (dr.Read())
                    {
                        dataGridView1.Rows.Add(dr.GetValue(0), dr.GetValue(1), dr.GetValue(2), dr.GetValue(3), dr.GetValue(4), dr.GetValue(5));
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los empleados por cargos: " + ex.Message);
            }
        }

        public void InsertarEmpleado(string apeEmpleado, string nomEmpleado, string dirEmpleado, string nomDistrito, string fonoEmpleado, string nomCargo, char estado)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cnx))
                {
                    cn.Open();
                    SqlCommand command = new SqlCommand("InsertarEmpleado", cn);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ApeEmpleado", apeEmpleado);
                    command.Parameters.AddWithValue("@NomEmpleado", nomEmpleado);
                    command.Parameters.AddWithValue("@DirEmpleado", dirEmpleado);
                    command.Parameters.AddWithValue("@NomDistrito", nomDistrito);
                    command.Parameters.AddWithValue("@FonoEmpleado", fonoEmpleado);
                    command.Parameters.AddWithValue("@NomCargo", nomCargo);
                    command.Parameters.AddWithValue("@Estado", estado);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar empleado: " + ex.Message);
            }
        }

        private void EliminarPedidosDeEmpleado(int idEmpleado)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cnx))
                {
                    cn.Open();
                    string query = "DELETE FROM PEDIDO WHERE IdEmpleado = @IdEmpleado";
                    using (SqlCommand command = new SqlCommand(query, cn))
                    {
                        command.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar los pedidos del empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarEmpleado(int idEmpleado)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cnx))
                {
                    cn.Open();
                    string query = "DELETE FROM EMPLEADO WHERE IdEmpleado = @IdEmpleado";
                    using (SqlCommand command = new SqlCommand(query, cn))
                    {
                        command.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarDatosEmpleadoSeleccionado(int idEmpleado)
        {
            using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cnx))
            {
                string query = "EmpleadosPorID";
                SqlCommand command = new SqlCommand(query, cn);
                command.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                try
                {
                    cn.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        // Asignar los valores a los controles
                        txtApellidos.Text = reader["ApeEmpleado"].ToString();
                        txtNombres.Text = reader["NomEmpleado"].ToString();
                        txtDireccion.Text = reader["DirEmpleado"].ToString();
                        txtTelefono.Text = reader["fonoEmpleado"].ToString();
                        // Asignar el valor seleccionado al ComboBox de Distrito
                        cbxDistrito.SelectedValue = Convert.ToInt32(reader["idDistrito"]);

                        // Asignar el valor seleccionado al ComboBox de Cargo
                        cbxCargo.SelectedValue = Convert.ToInt32(reader["idCargo"]);

                        // Asignar el valor al CheckBox de Estado
                        chkActivo.Checked = reader["Estado"].ToString() == "A";


                    }
                    reader.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error al obtener los datos del empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idEmpleado = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["IdEmpleado"].Value);
                MostrarDatosEmpleadoSeleccionado(idEmpleado);
            }
        }

        private void ActualizarEmpleado(int idEmpleado)
        {
            // Obtener los nuevos valores de los controles
            string apellido = txtApellidos.Text;
            string nombre = txtNombres.Text;
            string direccion = txtDireccion.Text;
            string telefono = txtTelefono.Text;
            string distritoSeleccionado = cbxDistrito.SelectedItem.ToString();
            int idDistrito;
            switch (distritoSeleccionado)
            {
                case "Trujillo":
                    idDistrito = 1;
                    break;
                case "Victor Larco":
                    idDistrito = 2;
                    break;
                case "La Esperanza":
                    idDistrito = 3;
                    break;
                case "El Porvenir":
                    idDistrito = 4;
                    break;
                default:
                    idDistrito = -1;
                    break;
            }

            string cargoSeleccionado = cbxCargo.SelectedItem.ToString();

            int idCargo;
            switch (cargoSeleccionado)
            {
                case "Vendedor":
                    idCargo = 1;
                    break;
                case "Supervisor":
                    idCargo = 2;
                    break;
                default:
                    idCargo = -1; 
                    break;
            }
            
            string estado = chkActivo.Checked ? "A" : "I";

            MessageBox.Show(idDistrito.ToString());
            MessageBox.Show(idCargo.ToString());

            // Actualizar los datos del empleado en la base de datos
            using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cnx))
            {
                string query = "UPDATE EMPLEADO SET ApeEmpleado = @Apellido, NomEmpleado = @Nombre, DirEmpleado = @Direccion, idDistrito = @IdDistrito, fonoEmpleado = @Telefono, idCargo = @IdCargo, Estado = @Estado WHERE IdEmpleado = @IdEmpleado";
                SqlCommand command = new SqlCommand(query, cn);
                command.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                command.Parameters.AddWithValue("@Apellido", apellido);
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Direccion", direccion);
                command.Parameters.AddWithValue("@Telefono", telefono);
                command.Parameters.AddWithValue("@IdDistrito", idDistrito);
                command.Parameters.AddWithValue("@IdCargo", idCargo);
                command.Parameters.AddWithValue("@Estado", estado);

                try
                {
                    cn.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Datos del empleado actualizados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar los datos del empleado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error al actualizar los datos del empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verifica si el clic se realizó en una fila válida
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                // Obtener los valores de la fila seleccionada y mostrarlos en los controles
                txtApellidos.Text = fila.Cells["Apellidos"].Value.ToString();
                txtNombres.Text = fila.Cells["Nombres"].Value.ToString();
                txtDireccion.Text = fila.Cells["Direccion"].Value.ToString();
                txtTelefono.Text = fila.Cells["Telefono"].Value.ToString();
                chkActivo.Checked = fila.Cells["Estado"].Value.ToString() == "A";

                string nombreDistrito = fila.Cells["Distrito"].Value.ToString();
                string nombreCargo = fila.Cells["Cargo"].Value.ToString();

                // Buscar el índice del nombre del distrito en el ComboBox de distritos
                int indexDistrito = cbxDistrito.FindStringExact(nombreDistrito);
                if (indexDistrito != -1)
                {
                    // Seleccionar el distrito en el ComboBox
                    cbxDistrito.SelectedIndex = indexDistrito;
                }

                // Buscar el índice del nombre del cargo en el ComboBox de cargos
                int indexCargo = cbxCargo.FindStringExact(nombreCargo);
                if (indexCargo != -1)
                {
                    // Seleccionar el cargo en el ComboBox
                    cbxCargo.SelectedIndex = indexCargo;
                }

            }
        }


        public Form1()
        {
            InitializeComponent();     
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.CellClick += dataGridView1_CellClick;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargarCargos();
            CargarEmpleados();
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            CargarEmpleados();
        }

        private void txtDireccion_TextChanged(object sender, EventArgs e)
        {

        }

        private void chkActivo_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbxCargo_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarEmpleadosPorCargo();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // Obtener el valor seleccionado del ComboBox de Distrito
            string idDistrito = cbxDistrito.Text;

            // Obtener el valor seleccionado del ComboBox de Cargo
            string idCargo = cbxCargo.Text;


            // Obtener el estado del empleado dependiendo del CheckBox
            char estado = chkActivo.Checked ? 'A' : 'I';

           InsertarEmpleado(txtApellidos.Text, txtNombres.Text, txtDireccion.Text, idDistrito, txtTelefono.Text, idCargo, estado);

            // Luego de insertar el empleado, puedes mostrar un mensaje de éxito o realizar otras acciones necesarias
            MessageBox.Show("Empleado agregado correctamente.");
        }

        private int ObtenerIdEmpleadoSeleccionado()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtener el valor de la celda "IdEmpleado" de la fila seleccionada
                return Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["IdEmpleado"].Value);
            }
            else
            {
                return -1; // Si no hay ninguna fila seleccionada, devuelve -1
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int idEmpleado = ObtenerIdEmpleadoSeleccionado();
            if (idEmpleado != -1)
            {
                if (MessageBox.Show("¿Desea realizar eliminar empleado ?", "Empleado", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // Eliminar los pedidos asociados al empleado
                    EliminarPedidosDeEmpleado(idEmpleado);

                    // Luego, eliminar al empleado
                    EliminarEmpleado(idEmpleado);
                    MessageBox.Show("Empleado eliminado");
                    dataGridView1.Rows.Clear();
                    CargarEmpleados();
                }
                else

                {

                    MessageBox.Show("Empleado NO eliminado");

                }
            }
            else
            {
                MessageBox.Show("Por favor selecciona un empleado para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

       private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Obtener el ID del empleado seleccionado al hacer doble clic
            int idEmpleado = ObtenerIdEmpleadoSeleccionado();

            // Verificar si se ha seleccionado un empleado válido
            if (idEmpleado != -1)
            {
                // Eliminar el empleado automáticamente
                EliminarEmpleado(idEmpleado);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            int idEmpleado = ObtenerIdEmpleadoSeleccionado();

            if (idEmpleado != -1)
            {
                ActualizarEmpleado(idEmpleado);

                dataGridView1.Rows.Clear();
                CargarEmpleados();
            }
            else
            {
                MessageBox.Show("Por favor selecciona un empleado para actualizar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtApellidos.Text = "";
            txtNombres.Text = "";
            txtDireccion.Text = "";
            txtTelefono.Text = "";
            cbxDistrito.SelectedIndex = -1;
            cbxCargo.SelectedIndex = -1;
            chkActivo.Checked = false;
        }

        private void txtBuscarApellido_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtBuscarApellido.Text.Trim();

            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cnx))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("BuscarEmpleadosPorApellido", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ApellidoBusqueda", txtBuscarApellido.Text); // Suponiendo que txtApellidoBusqueda es un TextBox donde el usuario ingresa el apellido a buscar
                    SqlDataReader dr = cmd.ExecuteReader();

                    // Limpiar las filas existentes en el DataGridView
                    dataGridView1.Rows.Clear();

                    // Recorrer los resultados del SqlDataReader y agregar las filas al DataGridView
                    while (dr.Read())
                    {
                        dataGridView1.Rows.Add(
                            dr.GetValue(0),
                            dr.GetValue(1),
                            dr.GetValue(2),
                            dr.GetValue(3),
                            dr.GetValue(4),
                            dr.GetValue(5),
                            dr.GetValue(6)
                        );
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar empleados por apellido: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
    }
}
