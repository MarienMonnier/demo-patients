using System;

namespace DemoPatients.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public Civilite Civilite { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public DateTime DateNaissance { get; set; }
        public bool Present { get; set; }
    }
}
