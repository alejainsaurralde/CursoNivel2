using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Service;
using System.Configuration;

namespace presentacion
{
    public partial class frmAltaArticulo : Form
    {

        private Articulo articulo = null;
        private OpenFileDialog archivo = null;
        public frmAltaArticulo()
        {
            InitializeComponent();
        }
        public frmAltaArticulo(Articulo articulo)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Modificar Articulo";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            ArticuloService service = new ArticuloService();
            try
            {
                if (articulo == null)
                    articulo = new Articulo();
                
                articulo.Codigo = txtCodigo.Text;
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion = txtDescripcion.Text;
                articulo.Marca = cboMarca.SelectedValue.ToString();
                articulo.Categoria = cboCategoria.SelectedValue.ToString();
                articulo.ImagenUrl = txtImagenUrl.Text; 
                articulo.Precio = decimal.Parse(txtPrecio.Text);



                if (articulo.Id != 0)
                {
                    service.modificar(articulo);
                    MessageBox.Show("Modificado exitosamente");
                }
                else
                {
                    service.agregar(articulo);
                    MessageBox.Show("Agregado exitosamente");
                }

                //GUARDO IMAGEN SI LA LEVANTO LOCALMENTE

                if (archivo != null && !(txtImagenUrl.Text.ToUpper().Contains("HTTP")))
                   File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);

                Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void frmAltaArticulo_Load(object sender, EventArgs e)
        {

            MarcaService marcaService = new MarcaService();
            CategoriaService categoriaService = new CategoriaService();
            try
            {
                cboMarca.DataSource = marcaService.listar();
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Descripcion";

                cboCategoria.DataSource = categoriaService.listar();
                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Descripcion";

                if (articulo != null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;   
                    txtDescripcion.Text = articulo.Descripcion;
                    txtImagenUrl.Text = articulo.ImagenUrl; 
                    txtPrecio.Text = articulo.Precio.ToString();  
                    cargarImagen(articulo.ImagenUrl);
                    cboMarca.SelectedValue = articulo.Id;    
                    cboCategoria.SelectedValue = articulo.Id;    

                
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }


        }

        private void txtImagenUrl_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtImagenUrl.Text);

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

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg; |png|*.png";
            archivo.ShowDialog();
            if (archivo.ShowDialog() == DialogResult.OK)
            {
                txtImagenUrl.Text = archivo.FileName;
                cargarImagen(archivo.FileName);

                //GUARDO LA IMAGEN
               // File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);


            }
        }
    }
}
