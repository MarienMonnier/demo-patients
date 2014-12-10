using System;
using DemoPatients.Data;
using DemoPatients.Models;
using DemoPatients.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DemoPatients.Tests
{
    [TestClass]
    public class ServicesTests
    {
        [TestMethod]
        public void PatientService_Can_Add_CallPatientRepository()
        {
            var repo = new Mock<IPatientRepository>();

            var service = new PatientService(repo.Object);

            service.AddPatient(new Patient());

            repo.Verify(r => r.AddPatient(It.IsAny<Patient>()));
        }
    }
}
