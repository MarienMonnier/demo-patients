using System;
using System.ComponentModel.DataAnnotations;
using DemoPatients.Models;

namespace DemoPatients.WebApp.Models
{
    public class PatientViewModel
    {
        public PatientViewModel()
            : this(null)
        {
        }

        public PatientViewModel(Patient patient)
        {
            if (patient == null)
            {
                DateNaissance = DateTime.Today;
                return;
            }

            Id = patient.Id;
            Civilite = patient.Civilite;
            Prenom = patient.Prenom;
            Nom = patient.Nom;
            DateNaissance = patient.DateNaissance;
            Present = patient.Present;
        }

        public int Id { get; set; }

        [Display(Name = "Civilité", Prompt = "Sélectionnez une civilité")]
        public Civilite Civilite { get; set; }

        [Display(Name = "Prénom", Prompt = "Entrez un prénom")]
        public string Prenom { get; set; }

        [Display(Name = "Nom", Prompt = "Entrez un nom")]
        public string Nom { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de naisance", Prompt = "Entrez sa date de naissance")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateNaissance { get; set; }

        [Display(Name = "Est-il présent ?")]
        public bool Present { get; set; }

        public int Age
        {
            get
            {
                return DateTime.Today.Year - DateNaissance.Year;
            }
        }

        public string NomComplet
        {
            get
            {
                return string.Format("{0} {1} {2}", Civilite, Prenom, Nom != null ? Nom.ToUpper() : Nom);
            }
        }

        public Patient ToPatient()
        {
            return new Patient
            {
                Prenom = Prenom,
                Nom = Nom,
                DateNaissance = DateNaissance,
                Civilite = Civilite,
                Present = Present
            };
        }
    }
}