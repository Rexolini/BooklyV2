using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Etykieta = Bookly.Models.Etykieta;

namespace Bookly.Models
{
    public class KsiazkaViewModel
    {
        public KsiazkaViewModel()
        {
            Pliki = new List<HttpPostedFileBase>();
            PlikiNazwy = new List<string>();
            Pisarze = new List<Pisarz>();
            Etykiety = new List<Etykieta>();
        }

    public int Id_Kategorii { get; set; }
        public int Id_Ksiazka { get; set; }
        [Required]
        [MinLength(3)]
        public string Tytul { get; set; }
        [Required]
        [MaxLength(13)]
        [MinLength(13)]
        public string ISBN { get; set; }
        [Required]
        [Range(0, 2020)]
        public int Rok { get; set; }
        [Required]
        [Range(0, Int64.MaxValue)]
        public int Naklad { get; set; }
        [Required]
        public string Opis { get; set; }
        [Required]
        public HttpPostedFileBase Zawartosc { get; set; }
        [Required]
        public int[] WybraniPisarze { get; set; }
        [Required]
        public int[] WybraneEtykiety { get; set; }

        public List<Pisarz> Pisarze { get; set; }
        public List<Etykieta> Etykiety { get; set; }
        public List<HttpPostedFileBase> Pliki { get; set; }
        public List<string> PlikiNazwy { get; set; }
    }

    public class KsiazkaEdytujViewModel
    {
        public KsiazkaEdytujViewModel()
        {
            KsiazkaViewModel = new KsiazkaViewModel();
            StarePliki = new List<string>();
            StarePlikiTekst = new List<string>();
        }

        public KsiazkaViewModel KsiazkaViewModel { get; set; }
        public List<string> StarePliki { get; set; }
        public List<string> StarePlikiTekst { get; set; }
        public string StaraZawartosc { get; set; }
    }
}