using System.Linq;
using DemoPatients.Models;

namespace DemoPatients.Data
{
    public interface IPatientRepository
    {
        IQueryable<Patient> GetPatients();
        Patient GetPatientById(int id);
        Patient AddPatient(Patient patient);
        void RemovePatient(int id);
        void UpdatePatient(int id, Patient patient);
    }
}