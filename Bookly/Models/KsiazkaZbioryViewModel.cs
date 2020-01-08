using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class KsiazkaZbioryViewModel
    {
        public int Id_KsiazkaZbiory { get; set; }
        public string Tytul { get; set; }
        public int Naklad { get; set; }
        public int Wypozyczone { get; set; }
        public string ISBN { get; set; }
    }
}