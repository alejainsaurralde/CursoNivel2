using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Service;



namespace presentacion
{
    public partial class frmArticulos : Form
    {
        private List<Articulo> listaArticulo;
        public frmArticulos()
        {
            InitializeComponent();
        }

        private void frmArticulos_Load(object sender, EventArgs e)
        {
            cargar();
            cboFiltroCampo.Items.Add("Precio");
            cboFiltroCampo.Items.Add("Nombre");
            cboFiltroCampo.Items.Add("Descripcion");

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaArticulo alta = new frmAltaArticulo();
            alta.ShowDialog();
            cargar();
        }
        private void btnModificar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
            frmAltaArticulo modificar = new frmAltaArticulo(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvArticulos.CurrentRow != null)
            {
               Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
               cargarImagen(seleccionado.ImagenUrl);  

            }
            
            
        }
        private void cargar()
        {
                ArticuloService service = new ArticuloService();
                try
                {
                    listaArticulo = service.Listar();
                    dgvArticulos.DataSource = listaArticulo;
                    ocultarColumnas();
                    cargarImagen(listaArticulo[0].ImagenUrl);

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.ToString());
                }
        }
        private void ocultarColumnas()
        {
            dgvArticulos.Columns["ImagenUrl"].Visible = false;
            dgvArticulos.Columns["Id"].Visible = false;
        }
        private void cargarImagen(string imagen)
        {
            try
            {
                pbxArticulo.Load(imagen);

            }
            catch (Exception Ex)
            {

                pbxArticulo.Load("https://tse4.mm.bing.net/th?id=OIP.F00dCf4bXxX0J-qEEf4qIQHaD6&pid=Api&P=0&h=180");
            }
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();
        }
        private void btnEliminarLogico_Click(object sender, EventArgs e)
        {
            eliminar(true);

        }
        private void eliminar(bool logico = false)
        {
            ArticuloService service = new ArticuloService();
            Articulo seleccionado;
            try
            {
                DialogResult respuesta = MessageBox.Show("¿Esta seguro de eliminarlo?", "Eliminando...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

                    if (logico)
                        service.eliminar(seleccionado.Id);

                    else
                    service.eliminar(seleccionado.Id);
                    cargar();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            if (cboFiltroCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el campo para filtrar.");
                return true;
            }

            if (cboFiltroCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el criterio para filtrar.");
                return true;
            }

            if (cboFiltroCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar el filtro para numericos...");
                    return true;
                }
                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Solo numeros para filtrar por un campo numerico...");
                    return true;
                }

            }

            return false;
        }
        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            ArticuloService service = new ArticuloService();
            try
            {
                if (validarFiltro())
                    return;
                string campo = cboFiltroCampo.SelectedItem.ToString();
                string criterio = cboFiltroCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvArticulos.DataSource = service.filtrar(campo, criterio, filtro);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }


        private void txtFiltroRapido_KeyPress(object sender, KeyPressEventArgs e)
        { 
        }
        private void txtFiltroRapido_TextChanged(object sender, EventArgs e)
        {

                List<Articulo> listaFiltrada;
                string filtro = txtFiltroRapido.Text;

                if (filtro.Length >= 3)
                {
                    listaFiltrada = listaArticulo.FindAll(x => x.Marca.ToUpper().Contains(filtro.ToUpper()) || x.Categoria.ToUpper().Contains(filtro.ToUpper()));
                }
                else
                {
                    listaFiltrada = listaArticulo;
                }

                dgvArticulos.DataSource = null;
                dgvArticulos.DataSource = listaFiltrada;
                ocultarColumnas();


          


        }

        private void cboFiltroCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboFiltroCampo.SelectedItem.ToString(); 
            if(opcion == "Precio")
            {
                cboFiltroCriterio.Items.Clear();
                cboFiltroCriterio.Items.Add("Mayor a");
                cboFiltroCriterio.Items.Add("Menor a");
                cboFiltroCriterio.Items.Add("Igual a");
            }
            else
            {
                cboFiltroCriterio.Items.Clear();
                cboFiltroCriterio.Items.Add("Comienza con");
                cboFiltroCriterio.Items.Add("Termina con");
                cboFiltroCriterio.Items.Add("Contiene");
            }
        }

       
    }
}
