using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidroponíaTFG.Models
{
    // Clase objeto para almacenar usuarios
    public class Usuario
    {
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
