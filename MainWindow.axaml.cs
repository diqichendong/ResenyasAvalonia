using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace ResenyasAvalonia;

public partial class MainWindow : Window
{
    private const string NOMBRE_FICHERO = "databank.data";
    private const string TEXTO_TOTAL = "Número total de reseñas: ";
    private List<Resenya> lista;
    private int actual;
    private string ultimoImagenPath;
    private IImage defaultImage;
    
    public MainWindow()
    {
        InitializeComponent();
        
        // Estado inicial
        defaultImage = imgPoster.Source;    // Guardar referencia poster por defecto
        lista = new List<Resenya>();
        cbTipo.SelectedIndex = 0;
        btnSubirImagen.IsVisible = false;
        lblTotal.Text = TEXTO_TOTAL + lista.Count;
        actual = 0;
        ultimoImagenPath = "";
        
        // Cargar registros
        cargarRegistros();

        // Actualizar vista
        actualizarVista();
    }
    
    // Carga los registros del fichero
    private void cargarRegistros()
    {
        BinaryReader br;

        try
        {
            br = new BinaryReader(File.OpenRead(NOMBRE_FICHERO));
            while (br.PeekChar() != -1)
            {
                this.lista.Add(Resenya.cargarResenya(br));
            }

            br.Close();
        }
        catch (Exception ex) { }
    }
    
    // Guardar registros en fichero
    private void BtnGuardar_OnClick(object? sender, RoutedEventArgs e)
    {
        File.Delete(NOMBRE_FICHERO);
        BinaryWriter bw;

        try
        {

            bw = new BinaryWriter(File.OpenWrite(NOMBRE_FICHERO));

            foreach (Resenya r in lista)
            {
                r.guardarResenya(bw);
            }
            bw.Close();
            
            mostrarMensaje("Datos guardados.");
        }
        catch (Exception ex) { }
    }
    
    // Actualiza la información de la vista
    private void actualizarVista()
    {
        activarCampos(false);

        if (lista.Count == 0)
        {
            activarBotones(false);
            btnCrear.IsEnabled = true;
            lblTotal.Text = TEXTO_TOTAL + lista.Count;
            cbTipo.SelectedIndex = 0;
            txtTitulo.Text = "";
            txtAnyo.Text = "";
            txtGenero.Text = "";
            numNota.Value = 5;
            txtContenido.Text = "";
            chkRecomendado.IsChecked = false;
            imgPoster.Source = defaultImage;
        }
        else
        {
            activarBotones(true);
            actualizarBotonesNav();

            Resenya r = lista[actual];
            imgPoster.Source = arrayABitmap(r.Imagen);
            switch (r.Tipo)
            {
                case 'P':
                    cbTipo.SelectedIndex = 0;
                    break;
                case 'S':
                    cbTipo.SelectedIndex = 1;
                    break;
                case 'D':
                    cbTipo.SelectedIndex = 2;
                    break;
            }
            txtTitulo.Text = r.Titulo;
            txtAnyo.Text = r.Anyo.ToString();
            txtGenero.Text = r.Generos;
            numNota.Text = r.Nota.ToString();
            txtContenido.Text = r.Contenido;
            chkRecomendado.IsChecked = r.Recomendado;
            lblTotal.Text = TEXTO_TOTAL + lista.Count;
        }
    }
    
    // Activa o desactiva los campos de la vista
    private void activarCampos(bool b)
    {
        cbTipo.IsEnabled = b;
        txtTitulo.IsReadOnly = !b;
        txtAnyo.IsReadOnly = !b;
        txtGenero.IsReadOnly = !b;
        numNota.IsReadOnly = !b;
        txtContenido.IsReadOnly = !b;
        chkRecomendado.IsEnabled = b;
    }
    
    // Activa o desactiva los botones de la vista
    private void activarBotones(bool b)
    {
        btnAnterior.IsEnabled = b;
        btnCrear.IsEnabled = b;
        btnBorrar.IsEnabled = b;
        btnSiguiente.IsEnabled = b;
        btnGuardar.IsEnabled = b;
    }
    
    // Actualiza los botones de navegación
    private void actualizarBotonesNav()
    {
        btnAnterior.IsEnabled = true;
        btnSiguiente.IsEnabled = true;

        if (actual == 0)
        {
            btnAnterior.IsEnabled = false;
        }

        if (actual == lista.Count - 1)
        {
            btnSiguiente.IsEnabled = false;
        }
    }
    
    // Evento botón Crear y Aceptar
    private void BtnCrear_OnClick(object? sender, RoutedEventArgs e)
    {
        if (btnCrear.Content == "Crear")
        {
            activarBotones(false);
            btnCrear.Content = "Aceptar";
            btnBorrar.Content = "Cancelar";
            btnSubirImagen.IsVisible = true;
            activarCampos(true);
            btnBorrar.IsEnabled = true;
            btnCrear.IsEnabled = true;

            imgPoster.Source = defaultImage;
            cbTipo.SelectedIndex = 0;
            txtTitulo.Text = "";
            txtAnyo.Text = "";
            txtGenero.Text = "";
            numNota.Value = 5;
            txtContenido.Text = "";
            chkRecomendado.IsChecked = false;
        }
        else
        {
            bool datosCorrectos = true;
            char tipo = '.';
            int anyo = -1;
            double nota;
            bool recomendado;
            string titulo, generos, contenido, imagenPath;

            titulo = txtTitulo.Text.Trim();
            generos = txtGenero.Text.Trim();
            contenido = txtContenido.Text.Trim();
            imagenPath = ultimoImagenPath;
            
            switch (cbTipo.SelectionBoxItem)
            {
                case "Película":
                    tipo = 'P';
                    break;
                case "Serie":
                    tipo = 'S';
                    break;
                case "Documental":
                    tipo = 'D';
                    break;
            }

            try
            {
                anyo = int.Parse(txtAnyo.Text.Trim());
            }
            catch (Exception ex)
            {
                mostrarMensaje("El año tiene que ser un número.");
                datosCorrectos = false;
            }

            nota = double.Parse(numNota.Text);

            if (titulo == "")
            {
                mostrarMensaje("El título está vacío.");
                datosCorrectos = false;
            }

            if (generos == "")
            {
                mostrarMensaje("El género está vacío.");
                datosCorrectos = false;
            }

            if (contenido == "")
            {
                mostrarMensaje("La reseña está vacía");
                datosCorrectos = false;
            }

            if (imagenPath == "")
            {
                mostrarMensaje("No se ha subido una imagen");
                datosCorrectos = false;
            }

            recomendado = chkRecomendado.IsChecked.HasValue ? chkRecomendado.IsChecked.Value : false;

            if (datosCorrectos)
            {
                lista.Add(
                    new Resenya(
                        tipo,
                        titulo,
                        anyo,
                        generos,
                        nota,
                        contenido,
                        recomendado,
                        bitmapAArray(new Bitmap(imagenPath))
                    )
                );
                this.ultimoImagenPath = "";
                btnCrear.Content = "Crear";
                btnBorrar.Content = "Borrar";
                btnSubirImagen.IsVisible = false;
                activarCampos(false);
                activarBotones(true);
                actualizarVista();
            }
        }
    }
    
    // Evento botón Borrar y Cancelar
    private void BtnBorrar_OnClick(object? sender, RoutedEventArgs e)
    {
        if (btnBorrar.Content == "Cancelar")
        {
            this.ultimoImagenPath = "";
            btnCrear.Content = "Crear";
            btnBorrar.Content = "Borrar";
            btnSubirImagen.IsVisible = false;
            activarCampos(false);
            activarBotones(true);
            actualizarVista();
        }
        else
        {
            lista.Remove(lista[actual]);
            if (lista.Count - 1 < actual)
            {
                if (actual > 0)
                {
                    actual--;
                }
            }

            actualizarVista();
        }
    }
    
    // Evento botón Anterior
    private void BtnAnterior_OnClick(object? sender, RoutedEventArgs e)
    {
        actual--;
        actualizarVista();
    }
    
    // Evento botón Siguiente
    private void BtnSiguiente_OnClick(object? sender, RoutedEventArgs e)
    {
        actual++;
        actualizarVista();
    }
    
    // Evento botón Subir imagen
    private void BtnSubirImagen_OnClick(object? sender, RoutedEventArgs e)
    {
        fileDialog();
    }

    // Método para utilizar el file chooser
    private async void fileDialog()
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Elegir una imagen",
            AllowMultiple = false,
            FileTypeFilter = new []{FilePickerFileTypes.ImageAll}
        });

        if (files.Count >= 1)
        {
            this.ultimoImagenPath = files[0].Path.AbsolutePath;
            imgPoster.Source = new Bitmap(this.ultimoImagenPath);
        }
    }

    // Botón de aceptar el mensaje
    private void BtnAceptarMensaje_OnClick(object? sender, RoutedEventArgs e)
    {
        PanelMensaje.IsVisible = false;
    }

    // Convierte de byte[] a Bitmap
    private Bitmap arrayABitmap(byte[] arr)
    {
        using (MemoryStream ms = new MemoryStream(arr))
        {
            Bitmap img = new Bitmap(ms);
            return img;
        }
    }

    // Convierte de Bitmap a byte[]
    private byte[] bitmapAArray(Bitmap img)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            img.Save(ms);
            return ms.ToArray();
        }
    }
    
    // Mostrar el mensaje
    private void mostrarMensaje(string mensaje)
    {
        MensajeBox.Text = mensaje;
        PanelMensaje.IsVisible = true;
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        BtnGuardar_OnClick(sender, null);
    }
}