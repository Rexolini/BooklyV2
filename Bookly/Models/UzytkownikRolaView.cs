using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class UzytkownikRolaView
    {
        public string Id_Uzytkownika { get; set; }
        public bool[] SprawdzRole { get; set; }
        public List<string> WszystkieRole { get; set; }
    }
}