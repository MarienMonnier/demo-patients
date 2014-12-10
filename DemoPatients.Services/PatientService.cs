using System.Collections.Generic;
using System.Linq;
using DemoPatients.Data;
using DemoPatients.Models;

namespace DemoPatients.Services
{
    public class PatientService
    {
        private readonly IPatientRepository _repository;

        public PatientService(IPatientRepository repository)
        {
            _repository = repository;
        }

        public List<Patient> GetPatients()
        {
            return _repository.GetPatients().ToList();
        }

        public Patient GetPatientById(int id)
        {
            return _repository.GetPatientById(id);
        }

        public Patient AddPatient(Patient patient)
        {
            return _repository.AddPatient(patient);
        }

        public void UpdatePatient(int id, Patient patient)
        {
            _repository.UpdatePatient(id, patient);
        }

        public void RemovePatient(int id)
        {
            _repository.RemovePatient(id);
        }
    }
}
