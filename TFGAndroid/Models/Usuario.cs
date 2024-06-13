using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFGAndroid.Models
{
    // Clase que representa un usuario en el sistema
    public class Usuario
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Mail { get; set; }
        public string Pass { get; set; }
        public string Type { get; set; }

        public Usuario(string nombre, string mail, string pass, string type)
        {
            Nombre = nombre;
            Mail = mail;
            Pass = pass;
            Type = type;
        }
        public Usuario() { }
    }
}
