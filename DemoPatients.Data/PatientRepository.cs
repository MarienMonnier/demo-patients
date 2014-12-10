using System;
using System.Collections.Generic;
using System.Linq;
using DemoPatients.Models;

namespace DemoPatients.Data
{
    public class PatientRepository : IPatientRepository
    {
        private static readonly List<Patient> Patients;

        static PatientRepository()
        {
            Patients = new List<Patient>
            {
                new Patient { Id = 1, Civilite = Civilite.M, Prenom = "Marien", Nom = "Monnier", DateNaissance = new DateTime(1986, 10, 17), Present = true },
                new Patient { Id = 2, Civilite = Civilite.M, Prenom = "Rémi", Nom = "Lesieur", DateNaissance = new DateTime(1981, 5, 10), Present = true },
                new Patient { Id = 3, Civilite = Civilite.M, Prenom = "Pier-Lionel", Nom = "Sgard", DateNaissance = new DateTime(1986, 4, 5), Present = false },
                new Patient { Id = 4, Civilite = Civilite.M, Prenom = "Jérôme", Nom = "Veneziani", DateNaissance = new DateTime(1986, 1, 13), Present = false },
            };
        }

        public IQueryable<Patient> GetPatients()
        {
            return Patients.AsQueryable();
        }

        public Patient GetPatientById(int id)
        {
            return Patients.SingleOrDefault(p => p.Id == id);
        }

        public Patient AddPatient(Patient patient)
        {
            Patients.Add(patient);
            patient.Id = Patients.Count > 0 ? Patients.Max(p => p.Id) + 1 : 1;
            return patient;
        }

        public void RemovePatient(int id)
        {
            Patient patient = GetPatientById(id);
            if (patient != null)
            {
                Patients.Remove(patient);
            }
        }

        public void UpdatePatient(int id, Patient patient)
        {
            Patient basePatient = GetPatientById(id);
            if (basePatient != null)
            {
                basePatient.Civilite = patient.Civilite;
                basePatient.Nom = patient.Nom;
                basePatient.Prenom = patient.Prenom;
                basePatient.DateNaissance = patient.DateNaissance;
                basePatient.Present = patient.Present;
            }
        }
    }
}
