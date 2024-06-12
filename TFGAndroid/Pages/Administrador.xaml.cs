using MongoDB.Bson;
using System.Text.RegularExpressions;
using TFGAndroid.Database;
using TFGAndroid.Models;


namespace TFGAndroid.Pages;

public partial class Administrador : ContentPage
{
    private BBDD _bbdd;
    private List<string> usuarios = new List<string>();
    private Usuario usuario = new Usuario();
    private Usuario _usuario;
    public Administrador(Usuario usuario)
	{
		InitializeComponent();

        _usuario = usuario;
        menulateral.setUsuario(_usuario);
        _bbdd = new BBDD();
        CargarUsuarios();

        CargarRegistros();
    }

    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("administrador");
        }
        else
        {
            menulateral.Show("administrador");
        }
    }

    private void RadioButtonLabelTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.GestureRecognizers[0] is TapGestureRecognizer tapGestureRecognizer)
        {
            if (tapGestureRecognizer.CommandParameter is RadioButton radioButton)
            {
                radioButton.IsChecked = true;
            }
        }
    }

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

    private void CargarRegistros()
    {
        // Obtener los registros desde la base de datos
        List<string> registros = _bbdd.ObtenerTodosRegistros();

        // Asignar los registros como origen de datos de la segunda ListView
        UsuariosListView1.ItemsSource = registros;
    }

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
        // Verificar que el email tenga una estructura válida
        if (!EsEmailValido(email))
        {
            DisplayAlert("Error", "El correo electrónico no es válido.", "Aceptar");
            return;
        }
        // Verificar que al menos un RadioButton esté seleccionado
        if (!InvitadoRadioButton.IsChecked && !JardineroRadioButton.IsChecked && !AdministradorRadioButton.IsChecked)
        {
            DisplayAlert("Error", "Seleccione un tipo de usuario.", "Aceptar");
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
        // Get the user's name to be deleted
        string nombreUsuario = EntryNombre.Text;

        // Validate that a name is provided
        if (string.IsNullOrEmpty(nombreUsuario))
        {
            DisplayAlert("Error", "Debes introducir el nombre del usuario a eliminar.", "Aceptar");
            return;
        }
        if (nombreUsuario == "a")
        {
            DisplayAlert("Error", "No puedes eliminar el usuario administrador", "Aceptar");
            return;
        }

        // Attempt to delete the user from the database
        try
        {
            _bbdd.EliminarUsuario(nombreUsuario);

            // Display a success message
            DisplayAlert("Éxito", $"Usuario '{nombreUsuario}' eliminado correctamente.", "Aceptar");

            // Reload the user list in the ListView
            CargarUsuarios();

            // Clear the input field
            EntryNombre.Text = "";
        }
        catch (Exception ex)
        {
            // Display an error message if deletion fails
            DisplayAlert("Error", $"Ocurrió un error al eliminar el usuario: {ex.Message}", "Aceptar");
        }
    }

    private void BtnModificarUsuario_Clicked(object sender, EventArgs e)
    {
        // Get updated user information from UI elements
        string nombre = EntryNombre.Text;
        string nuevaContraseña = EntryContraseña.Text;
        string email = EntryEmail.Text;
        string tipoUsuario = "";

        // Validate user input (excluding password)
        if (nombre == "a")
        {
            DisplayAlert("Error", "No puedes modificar el usuario administrador", "Aceptar");
            return;
        }
        // Validate user input (excluding password)
        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email))
        {
            DisplayAlert("Error", "Los campos nombre y email son obligatorios.", "Aceptar");
            return;
        }
        // Verificar que el email tenga una estructura válida
        if (!EsEmailValido(email))
        {
            DisplayAlert("Error", "El correo electrónico no es válido.", "Aceptar");
            return;
        }

        // Determine the selected user type
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

        // Obtener la contraseña cifrada actual del usuario seleccionado
        var usuarioActual = _bbdd.ObtenerUsuarioPorNombre(usuario.Nombre);
        if (usuarioActual == null)
        {
            DisplayAlert("Error", "Usuario no encontrado.", "Aceptar");
            return;
        }
        string contraseñaActualCifrada = usuarioActual.Pass; // Asumiendo que tu clase Usuario tiene una propiedad Pass

        // Usar la nueva contraseña si se ha proporcionado, de lo contrario, usar la contraseña cifrada actual
        string contraseñaAUsar = !string.IsNullOrEmpty(nuevaContraseña) ? BCrypt.Net.BCrypt.HashPassword(nuevaContraseña) : contraseñaActualCifrada;

        // Construct BsonDocument with updated user information
        var updatedUsuario = new BsonDocument
    {
        { "name", nombre },
        { "mail", email },
        { "pass", contraseñaAUsar },
        { "type", tipoUsuario }
    };

        try
        {
            // Update user in MongoDB
            _bbdd.ActualizarUsuario(usuario.Nombre, updatedUsuario);

            // Display success message and reload user list
            DisplayAlert("Éxito", "Usuario modificado correctamente.", "Aceptar");
            CargarUsuarios();

            // Optionally clear the modification form
            BtnVaciarFormulario_Clicked(vaciar, e);
        }
        catch (Exception ex)
        {
            // Display error message if an issue occurs during update
            DisplayAlert("Error", $"Ocurrió un error al modificar el usuario: {ex.Message}", "Aceptar");
        }
    }





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

    private bool EsEmailValido(string email)
    {
        // Expresión regular para validar el email
        string patronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, patronEmail);
    }

}