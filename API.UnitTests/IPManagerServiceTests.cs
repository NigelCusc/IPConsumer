using API.CustomExceptions;
using API.DataMappers;
using API.Entities;
using API.Repositories;
using API.Services;
using Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.UnitTests
{
    [TestClass]
    public class IPManagerServiceTests
    {
        private readonly IPManagerService _sut;
        // Mocking the repository using Moq
        private readonly Mock<IIPManagerRepository> _ipManagerRepoMock = new Mock<IIPManagerRepository>();

        public IPManagerServiceTests()
        {
            _sut = new IPManagerService(_ipManagerRepoMock.Object);
        }

        [TestMethod]
        public async Task GetByIPAsync_ShouldReturnIPDetails_WhenIPDetailsExists()
        {
            // Arrange
            string ip = "10.10.10.10";
            string city = "Naxxar";
            string continent = "Europe";
            string country = "Malta";
            double longitude = 1.123;
            double latitude = 1.123;
            int id = 1;
            List<IPDetailsEntity> iPDetailsEntities = new();
            iPDetailsEntities.Add(new IPDetailsEntity()
            {
                IP = ip, 
                City = city,
                Continent = continent,
                Country = country,
                Latitude = latitude,
                Longitude = longitude,
                Id = id
            });
            _ipManagerRepoMock.Setup(x => x.FindByIPAsync(ip)).ReturnsAsync(iPDetailsEntities);

            // Act
            var details = await _sut.GetByIPAsync(ip);

            // Assert
            Assert.AreEqual(details.IP, ip);
            Assert.AreEqual(details.City, city);
            Assert.AreEqual(details.Continent, continent);
            Assert.AreEqual(details.Country, country);
            Assert.AreEqual(details.Longitude, longitude);
            Assert.AreEqual(details.Latitude, latitude);
        }

        [TestMethod]
        public async Task GetByIPAsync_ShouldReturnNull_WhenIPDetailsDoesNotExist()
        {
            // Arrange
            string ip = "10.10.10.10";
            List<IPDetailsEntity> iPDetailsEntities = new();
            _ipManagerRepoMock.Setup(x => x.FindByIPAsync(ip)).ReturnsAsync(iPDetailsEntities);

            // Act
            var details = await _sut.GetByIPAsync(ip);

            // Assert
            Assert.AreEqual(details, null);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateIPAddressException), "Duplicate detected for IP: 10.10.10.10")]
        public async Task GetByIPAsync_ShouldReturnDuplicateIPAddressException_WhenDoubleIPDetailsExists()
        {
            // Arrange
            string ip = "10.10.10.10";
            string city = "Naxxar";
            string continent = "Europe";
            string country = "Malta";
            double longitude = 1.123;
            double latitude = 1.123;
            int id = 1;
            List<IPDetailsEntity> iPDetailsEntities = new();
            var detailsEntity = new IPDetailsEntity()
            {
                IP = ip,
                City = city,
                Continent = continent,
                Country = country,
                Latitude = latitude,
                Longitude = longitude,
                Id = id
            };
            iPDetailsEntities.Add(detailsEntity);
            iPDetailsEntities.Add(detailsEntity);
            _ipManagerRepoMock.Setup(x => x.FindByIPAsync(ip)).ReturnsAsync(iPDetailsEntities);

            // Act
            var details = await _sut.GetByIPAsync(ip);

            // Assert
            Assert.AreEqual(details.IP, ip);
        }

        [TestMethod]
        public async Task ListAsync_ShouldReturn_WhenExists()
        {
            // Arrange
            string city = "Naxxar";
            string continent = "Europe";
            string country = "Malta";
            double longitude = 1.123;
            double latitude = 1.123;
            List<IPDetailsEntity> iPDetailsEntities = new List<IPDetailsEntity>() {
                new IPDetailsEntity()
                {
                    Id = 1,
                    IP = "10.10.10.10",
                    City = city,
                    Continent = continent,
                    Country = country,
                    Latitude = latitude,
                    Longitude = longitude,
                }, 
                new IPDetailsEntity()
                {
                    Id = 2,
                    IP = "20.20.20.20",
                    City = city,
                    Continent = continent,
                    Country = country,
                    Latitude = latitude,
                    Longitude = longitude,
                }
            };

            // Expected
            List<IPDetails> expectedIPDetails = new List<IPDetails>();
            foreach (IPDetailsEntity entity in iPDetailsEntities)
                expectedIPDetails.Add(IPDetailsMapper.ToModel(entity));
            _ipManagerRepoMock.Setup(x => x.ListAsync()).ReturnsAsync(iPDetailsEntities);

            // Act
            var detailsList = await _sut.ListAsync();

            // Assert
            IPDetails b;
            Assert.AreEqual(expectedIPDetails.Count, detailsList.Count);
            foreach (IPDetails a in expectedIPDetails)
            {
                b = detailsList.Where(x => x.IP == a.IP).First();
                Assert.AreEqual(a.IP, b.IP);
                Assert.AreEqual(a.City, b.City);
                Assert.AreEqual(a.Continent, b.Continent);
                Assert.AreEqual(a.Country, b.Country);
                Assert.AreEqual(a.Longitude, b.Longitude);
                Assert.AreEqual(a.Latitude, b.Latitude);
            }
        }

        [TestMethod]
        public async Task SaveAsync_ShouldAdd_WhenNotExists()
        {
            // Arrange
            string ip = "30.30.30.30";
            string city = "Naxxar";
            string continent = "Europe";
            string country = "Malta";
            double longitude = 1.123;
            double latitude = 1.123;
            IPDetails inputIPDetails = new IPDetails()
            {
                IP = ip,
                City = city,
                Continent = continent,
                Country = country,
                Latitude = latitude,
                Longitude = longitude,
            };
            IPDetailsEntity inputIPDetailsEntity = IPDetailsMapper.ToEntity(inputIPDetails);

            // Expected
            List<IPDetailsEntity> expectedIPDetails = new();

            _ipManagerRepoMock.Setup(x => x.FindByIPAsync(ip)).ReturnsAsync(expectedIPDetails);
            _ipManagerRepoMock.Setup(x => x.AddAsync(inputIPDetailsEntity)).ReturnsAsync(inputIPDetailsEntity);

            // Act
            var details = await _sut.SaveAsync(inputIPDetails);

            // Assert
            Assert.AreEqual(inputIPDetails, details);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "IP already exists within the database")]
        public async Task SaveAsync_ShouldReturnException_WhenExists()
        {
            // Arrange
            string ip = "30.30.30.30";
            string city = "Naxxar";
            string continent = "Europe";
            string country = "Malta";
            double longitude = 1.123;
            double latitude = 1.123;
            IPDetails inputIPDetails = new IPDetails()
            {
                IP = ip,
                City = city,
                Continent = continent,
                Country = country,
                Latitude = latitude,
                Longitude = longitude,
            };
            IPDetailsEntity inputIPDetailsEntity = IPDetailsMapper.ToEntity(inputIPDetails);

            // Expected
            List<IPDetailsEntity> expectedIPDetails = new List<IPDetailsEntity>()
            {
                inputIPDetailsEntity
            };

            _ipManagerRepoMock.Setup(x => x.FindByIPAsync(ip)).ReturnsAsync(expectedIPDetails);
            _ipManagerRepoMock.Setup(x => x.AddAsync(inputIPDetailsEntity)).ReturnsAsync(inputIPDetailsEntity);

            // Act
            var details = await _sut.SaveAsync(inputIPDetails);

            // Assert
            Assert.AreEqual(inputIPDetails, details);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdate_WhenExists()
        {
            // Arrange
            string ip = "10.10.10.10";
            string city = "Naxxar";
            string continent = "Europe";
            string country = "Malta";
            double longitude = 1.123;
            double latitude = 1.123;
            List<IPDetailsEntity> iPDetailsEntities = new List<IPDetailsEntity>() {
                new IPDetailsEntity()
                {
                    Id = 1,
                    IP = ip,
                    City = city,
                    Continent = continent,
                    Country = country,
                    Latitude = latitude,
                    Longitude = longitude,
                },
                new IPDetailsEntity()
                {
                    Id = 2,
                    IP = "20.20.20.20",
                    City = city,
                    Continent = continent,
                    Country = country,
                    Latitude = latitude,
                    Longitude = longitude,
                }
            };
            IPDetailsEntity inputIPDetailsEntity = new IPDetailsEntity()
            {
                Id = 1,
                IP = ip,
                City = "Mosta",
                Continent = continent,
                Country = country,
                Latitude = latitude,
                Longitude = longitude,
            };
            IPDetails inputIPDetails = IPDetailsMapper.ToModel(inputIPDetailsEntity);


            // Expected
            List<IPDetailsEntity> expectedIPDetails = new List<IPDetailsEntity>()
            {
                inputIPDetailsEntity
            };

            _ipManagerRepoMock.Setup(x => x.FindByIPAsync(ip)).ReturnsAsync(expectedIPDetails);
            _ipManagerRepoMock.Setup(x => x.UpdateAsync(inputIPDetailsEntity)).Returns(inputIPDetailsEntity);

            // Act
            var details = await _sut.UpdateAsync(inputIPDetails);

            // Assert
            Assert.AreEqual(inputIPDetails, details);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "IP details are missing for IP: 10.10.10.10")]
        public async Task UpdateAsync_ShouldReturnException_WhenNotExists()
        {
            // Arrange
            string ip = "10.10.10.10";
            string city = "Naxxar";
            string continent = "Europe";
            string country = "Malta";
            double longitude = 1.123;
            double latitude = 1.123;
            List<IPDetailsEntity> iPDetailsEntities = new List<IPDetailsEntity>() {
                new IPDetailsEntity()
                {
                    Id = 1,
                    IP = "30.30.30.30", // Changed IP
                    City = city,
                    Continent = continent,
                    Country = country,
                    Latitude = latitude,
                    Longitude = longitude,
                },
                new IPDetailsEntity()
                {
                    Id = 2,
                    IP = "20.20.20.20",
                    City = city,
                    Continent = continent,
                    Country = country,
                    Latitude = latitude,
                    Longitude = longitude,
                }
            };
            IPDetailsEntity inputIPDetailsEntity = new IPDetailsEntity()
            {
                Id = 1,
                IP = ip,
                City = "Mosta",
                Continent = continent,
                Country = country,
                Latitude = latitude,
                Longitude = longitude,
            };
            IPDetails inputIPDetails = IPDetailsMapper.ToModel(inputIPDetailsEntity);


            // Expected
            List<IPDetailsEntity> expectedIPDetails = new List<IPDetailsEntity>()
            {
            };

            _ipManagerRepoMock.Setup(x => x.FindByIPAsync(ip)).ReturnsAsync(expectedIPDetails);
            _ipManagerRepoMock.Setup(x => x.UpdateAsync(inputIPDetailsEntity)).Returns(inputIPDetailsEntity);

            // Act
            var details = await _sut.UpdateAsync(inputIPDetails);

            // Assert
            Assert.AreEqual(inputIPDetails, details);
        }
    }
}
