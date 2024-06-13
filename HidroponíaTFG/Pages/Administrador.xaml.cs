using HidroponíaTFG.Database;
using HidroponíaTFG.Models;
using MongoDB.Bson;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HidroponíaTFG.Pages;

public partial class Administrador : ContentPage
{
    private BBDD _bbdd;// Instancia de la clase BBDD para interactuar con la base de datos
    private Usuario usuario = new Usuario();// Instancia de la clase Usuario
    private Usuario _usuario;// Usuario actual

    // Constructor de la página Administrador
    public Administrador(Usuario usuarioParam)
	{
        _usuario = usuarioParam;// Asigna el usuario recibido como parámetro
        InitializeComponent();// Inicializa los componentes de la página
        menulateral.setUsuario(_usuario);// Configura el usuario en el menú lateral
        _bbdd = new BBDD();// Inicializa la instancia de BBDD para operaciones de base de datos
        CargarUsuarios();// Carga la lista de usuarios en la interfaz
        CargarRegistros();// Carga los registros en la interfaz
    }

    // Método para controlar la acción de mostrar/ocultar el menú lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("administrador");// Oculta el menú lateral
        }
        else
        {
            menulateral.Show("administrador");// Muestra el menú lateral
        }

    }

    // Método para manejar el evento de pulsar en una etiqueta de RadioButton
    private void RadioButtonLabelTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.GestureRecognizers[0] is TapGestureRecognizer tapGestureRecognizer)
        {
            if (tapGestureRecognizer.CommandParameter is RadioButton radioButton)
            {
                radioButton.IsChecked = true; // Marca el RadioButton seleccionado
            }
        }
    }

    // Método para cargar la lista de usuarios en la interfaz
    private void CargarUsuarios()
    {
        // Obtener la lista de usuarios desde la base de datos
        List<Usuario> usuarios = _bbdd.GetUsuarios();

        // Crear una nueva lista de nombres de usuarios
        var nombresUsuarios = new List<string>();

        // Agregar los nombres de los usuarios a la lista
        foreach (var usuario in usuarios)
        {
            nombresUsuarios.Add(usuario.Nombre);
        }

        // Asignar la nueva lista como origen de datos
        UsuariosListView.ItemsSource = nombresUsuarios;
    }

    // Método para cargar los registros en la interfaz
    private void CargarRegistros()
    {
        // Obtener los registros desde la base de datos
        List<string> registros = _bbdd.ObtenerTodosRegistros();

        // Asignar los registros como origen de datos de la segunda ListView
        UsuariosListView1.ItemsSource = registros;
    }


    // Método invocado al seleccionar un usuario en el ListView
    private void UsuariosListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item != null && e.Item is string nombreUsuario)
        {
            // Obtener la lista de usuarios desde la base de datos
            List<Usuario> usuarios = _bbdd.GetUsuarios();

            // Buscar el usuario por nombre en la lista completa de usuarios
            Usuario usuarioSeleccionado = usuarios.FirstOrDefault(u => u.Nombre == nombreUsuario);
            usuario = usuarioSeleccionado;

            if (usuarioSeleccionado != null)
            {
                // Actualizar los campos del formulario con los datos del usuario seleccionado
                EntryNombre.Text = usuarioSeleccionado.Nombre;
                EntryContraseña.Placeholder = "Introduce aquí la nueva contraseña";
                EntryEmail.Text = usuarioSeleccionado.Mail;

                // Seleccionar el tipo de usuario en base al usuario seleccionado
                switch (usuarioSeleccionado.Type)
                {
                    case "invit":
                        InvitadoRadioButton.IsChecked = true;
                        break;
                    case "jardi":
                        JardineroRadioButton.IsChecked = true;
                        break;
                    case "admin":
                        AdministradorRadioButton.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // Método para limpiar los campos del formulario
    private void BtnVaciarFormulario_Clicked(object sender, EventArgs e)
    {
        // Vaciar los campos del formulario
        EntryNombre.Text = string.Empty;
        EntryContraseña.Text = string.Empty;
        EntryContraseña.Placeholder = "Introduce aquí la contraseña";
        EntryEmail.Text = string.Empty;
        InvitadoRadioButton.IsChecked = false;
        JardineroRadioButton.IsChecked = false;
        AdministradorRadioButton.IsChecked = false;
    }

    private void BtnInsertarUsuario_Clicked(object sender, EventArgs e)
    {
        // Obtener los datos del formulario
        string nombre = EntryNombre.Text;
        string contraseña = EntryContraseña.Text;
        string email = EntryEmail.Text;
        string tipoUsuario = "";

        // Verificar que ningún dato esté vacío
        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(contraseña) || string.IsNullOrEmpty(email))
        {
            // Mostrar un mensaje de error si algún dato está vacío
            DisplayAlert("Error", "Todos los campos son obligatorios.", "Aceptar");
            return;
        }
        // Verificar que al menos un RadioButton esté seleccionado
        if (!InvitadoRadioButton.IsChecked && !JardineroRadioButton.IsChecked && !AdministradorRadioButton.IsChecked)
        {
            DisplayAlert("Error", "Seleccione un tipo de usuario.", "Aceptar");
            return;
        }
        // Verificar que el email tenga una estructura válida
        if (!EsEmailValido(email))
        {
            DisplayAlert("Error", "El correo electrónico no es válido.", "Aceptar");
            return;
        }

        // Determinar el tipo de usuario seleccionado
        if (InvitadoRadioButton.IsChecked)
        {
            tipoUsuario = "invit";
        }
        else if (JardineroRadioButton.IsChecked)
        {
            tipoUsuario = "jardi";
        }
        else if (AdministradorRadioButton.IsChecked)
        {
            tipoUsuario = "admin";
        }

        // Verificar si ya existe un usuario con el mismo nombre
        if (_bbdd.GetUsuarios().Any(u => u.Nombre == nombre))
        {
            DisplayAlert("Error", "Ya existe un usuario con el mismo nombre.", "Aceptar");
            return;
        }

        // Crear un documento BSON con los datos del nuevo usuario
        var nuevoUsuario = new BsonDocument
    {
        { "name", nombre },
        { "mail", email },
        { "pass", contraseña },
        { "type", tipoUsuario }
    };

        try
        {
            // Insertar el nuevo usuario en la base de datos
            _bbdd.InsertarUsuario(nuevoUsuario);

            // Mostrar un mensaje de éxito
            DisplayAlert("Éxito", "Usuario insertado correctamente.", "Aceptar");

            CargarUsuarios();

            // Limpiar el formulario
            BtnVaciarFormulario_Clicked(vaciar, e);

        }
        catch (Exception ex)
        {
            // Mostrar un mensaje de error si ocurre algún problema durante la inserción
            DisplayAlert("Error", $"Ocurrió un error al insertar el usuario: {ex.Message}", "Aceptar");
        }
    }

    private void BtnEliminarUsuario_Clicked(object sender, EventArgs e)
    {
        // Obtiene el nombre del usuario a eliminar
        string nombreUsuario = EntryNombre.Text;
        // Verifica que no se intente eliminar el usuario administrador
        if (nombreUsuario == "a")
        {
            DisplayAlert("Error", "No puedes eliminar el usuario administrador", "Aceptar");
            return;
        }
        // Valida que se haya proporcionado un nombre
        if (string.IsNullOrEmpty(nombreUsuario))
        {
            DisplayAlert("Error", "Debes introducir el nombre del usuario a eliminar.", "Aceptar");
            return;
        }

        // Intenta eliminar el usuario de la base de datos
        try
        {
            _bbdd.EliminarUsuario(nombreUsuario);

            // Muestra un mensaje de éxito
            DisplayAlert("Éxito", $"Usuario '{nombreUsuario}' eliminado correctamente.", "Aceptar");

            // Recarga la lista de usuarios en el ListView
            CargarUsuarios();

            // Limpia el campo de entrada
            EntryNombre.Text = "";
        }
        catch (Exception ex)
        {
            // Muestra un mensaje de error si falla la eliminación
            DisplayAlert("Error", $"Ocurrió un error al eliminar el usuario: {ex.Message}", "Aceptar");
        }
    }

    // Método para modificar un usuario existente
    private void BtnModificarUsuario_Clicked(object sender, EventArgs e)
    {
        // Obtiene la información actualizada del usuario desde los elementos de la interfaz
        string nombre = EntryNombre.Text;
        string nuevaContraseña = EntryContraseña.Text;
        string email = EntryEmail.Text;
        string tipoUsuario = "";

        // Valida la entrada del usuario (excepto la contraseña)
        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email))
        {
            DisplayAlert("Error", "Los campos nombre y email son obligatorios.", "Aceptar");
            return;
        }
        // Valida que no se intente modificar el usuario administrador

        if (nombre == "a")
        {
            DisplayAlert("Error", "No puedes modificar el usuario administrador", "Aceptar");
            return;
        }
        // Verificar que el email tenga una estructura válida
        if (!EsEmailValido(email))
        {
            DisplayAlert("Error", "El correo electrónico no es válido.", "Aceptar");
            return;
        }

        // Verifica que al menos un RadioButton esté seleccionado
        if (!InvitadoRadioButton.IsChecked && !JardineroRadioButton.IsChecked && !AdministradorRadioButton.IsChecked)
        {
            DisplayAlert("Error", "Seleccione un tipo de usuario.", "Aceptar");
            return;
        }

        // Determina el tipo de usuario seleccionado
        if (InvitadoRadioButton.IsChecked)
        {
            tipoUsuario = "invit";
        }
        else if (JardineroRadioButton.IsChecked)
        {
            tipoUsuario = "jardi";
        }
        else if (AdministradorRadioButton.IsChecked)
        {
            tipoUsuario = "admin";
        }

        // Verificar que al menos un RadioButton esté seleccionado
        if (tipoUsuario == "")
        {
            DisplayAlert("Error", "Seleccione un tipo de usuario.", "Aceptar");
            return;
        }

        // Obtener la contraseña cifrada actual del usuario seleccionado
        var usuarioActual = _bbdd.ObtenerUsuarioPorNombre(usuario.Nombre);
        if (usuarioActual == null)
        {
            DisplayAlert("Error", "Usuario no encontrado.", "Aceptar");
            return;
        }
        string contraseñaActualCifrada = usuarioActual.Pass; // Asumiendo que tu clase Usuario tiene una propiedad Pass

        // Utiliza la nueva contraseña si se proporcionó, de lo contrario, usa la contraseña cifrada actual
        string contraseñaAUsar = !string.IsNullOrEmpty(nuevaContraseña) ? BCrypt.Net.BCrypt.HashPassword(nuevaContraseña) : contraseñaActualCifrada;

        // Construye un documento BSON con la información actualizada del usuario
        var updatedUsuario = new BsonDocument
    {
        { "name", nombre },
        { "mail", email },
        { "pass", contraseñaAUsar },
        { "type", tipoUsuario }
    };

        try
        {
            // Actualiza el usuario en MongoDB
            _bbdd.ActualizarUsuario(usuario.Nombre, updatedUsuario);

            // Muestra un mensaje de éxito y recarga la lista de usuarios
            DisplayAlert("Éxito", "Usuario modificado correctamente.", "Aceptar");
            CargarUsuarios();

            // Opcionalmente, limpia el formulario de modificación
            BtnVaciarFormulario_Clicked(vaciar, e);
        }
        catch (Exception ex)
        {
            // Muestra un mensaje de error si ocurre algún problema durante la actualización
            DisplayAlert("Error", $"Ocurrió un error al modificar el usuario: {ex.Message}", "Aceptar");
        }
    }




    // Método para manejar el cambio de texto en la barra de búsqueda de usuarios
    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {

        List<Usuario> usuarios = _bbdd.GetUsuarios();
        // Get the current search term
        string searchTerm = searchBar.Text.ToLower();

        // Filter the usuarios list based on the search term
        var filteredUsuarios = usuarios.Where(usuario => usuario.Nombre.ToLower().Contains(searchTerm)).ToList();

        // Create a new list of user names from the filtered usuarios
        var filteredNombresUsuarios = new List<string>();
        foreach (var usuario in filteredUsuarios)
        {
            filteredNombresUsuarios.Add(usuario.Nombre);
        }

        // Update the ListView's ItemsSource with the filtered names
        UsuariosListView.ItemsSource = filteredNombresUsuarios;


    }

    // Método para manejar el cambio de texto en la barra de búsqueda de registros
    private void SearchBar1_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Obtener los registros desde la base de datos
        List<string> registros = _bbdd.ObtenerTodosRegistros();

        // Obtener el término de búsqueda actual
        string searchTerm = searchBar1.Text.ToLower();

        // Filtrar los registros basados en el término de búsqueda
        var filteredRegistros = registros.Where(registro => registro.ToLower().Contains(searchTerm)).ToList();

        // Actualizar el ListView's ItemsSource con los registros filtrados
        UsuariosListView1.ItemsSource = filteredRegistros;
    }


    // Método para verificar si un email es válido utilizando una expresión regular
    private bool EsEmailValido(string email)
    {
        // Expresión regular para validar el email
        string patronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, patronEmail);
    }

}