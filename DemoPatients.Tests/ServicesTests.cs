using System;
using System.Data;
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
        private Mock<IPatientRepository> _repo;
        private PatientService _service;

        [TestInitialize]
        public void Init()
        {
            _repo = new Mock<IPatientRepository>();
            _service = new PatientService(_repo.Object);

        }

        [TestMethod]
        public void PatientService_Can_Add_CallPatientRepository()
        {
            _service.AddPatient(new Patient());
            _repo.Verify(r => r.AddPatient(It.IsAny<Patient>()));
        }

        [TestMethod, ExpectedException(typeof(DataException))]
        public void PatientService_Can_CatchDBException()
        {
            _repo.Setup(r => r.GetPatients()).Throws(new DataException());
            var t =_service.GetPatients();
        }
    }
}
