using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Booking.UnitTests
{
    [TestFixture]
    public class BookingTests
    {
        private Booking _existBooking;
        private Mock<IBookingRepository> _repo;

        [SetUp]
        public void SetUp()
        {
            _existBooking = new Booking
            {
                Id = 1,
                ArrivalDate = new DateTime(2018, 1, 1),
                DepartureDate = new DateTime(2018, 2, 1),
                Reference = "overbooking",
                Status = "OK"
            };

            _repo = new Mock<IBookingRepository>();
            _repo.Setup(p => p.GetActiveBookings(1)).Returns(new List<Booking> { _existBooking }.AsQueryable());
            _repo.Setup(p => p.GetActiveBookings(2)).Returns(new List<Booking> { _existBooking }.AsQueryable());
        }

        // Booking before existing one
        [Test]
        public void StartAndFinishBeforeBooking_ReturnEmptyString()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = Before(_existBooking.ArrivalDate, days: 2),
                DepartureDate = Before(_existBooking.ArrivalDate)
            },
            _repo.Object);
            Assert.That(result, Is.Empty);
        }

        // Booking starts before and end during existing one
        [Test]
        public void StartBeforeAndEndDuringBooking_ReturnReferenceString()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = Before(_existBooking.ArrivalDate, days: 2),
                DepartureDate = After(_existBooking.ArrivalDate, days: 2)
            },
            _repo.Object);
            Assert.AreEqual(result, _existBooking.Reference);
        }

        //Booking starts and ends during existing one
        [Test]
        public void StartAndEndDuringBooking_ReturnReferenceString()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = _existBooking.ArrivalDate,
                DepartureDate = _existBooking.DepartureDate

            },
            _repo.Object);
            Assert.AreEqual(result, _existBooking.Reference);
        }

        //Booking starts during the existing one and end after
        [Test]
        public void StartDuringBookingEndAfer_ReturnReferenceString()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = _existBooking.ArrivalDate,
                DepartureDate = After(_existBooking.DepartureDate, 2)

            },
            _repo.Object);
            Assert.AreEqual("overbooking", result);
        }

        //Booking starts and end after booking
        [Test]
        public void StartAndEndAferBooking_ReturnEmptyString()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = After(_existBooking.DepartureDate),
                DepartureDate = After(_existBooking.DepartureDate, 8)

            },
            _repo.Object);
            Assert.That(result, Is.Empty);
        }

        //Booking starts before and end after booking
        [Test]
        public void StartBeforeAndEndAferBooking_ReturnReferenceString()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = Before(_existBooking.ArrivalDate),
                DepartureDate = After(_existBooking.DepartureDate)

            },
            _repo.Object);
            Assert.AreEqual(result, _existBooking.Reference);
        }

      
        private DateTime Before(DateTime arrivalDate, int days = 1)
        {
            return arrivalDate.AddDays(-days);
        }

        private DateTime After(DateTime dateTime, int days = 1)
        {
            return dateTime.AddDays(days);
        }

    }



}
