using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking
{
    public static class BookingHelper
    {
        public static string OverlappingBookingsExist(Booking booking, IBookingRepository repository)
        {
            if (booking.Status == "Cancelled")
                return string.Empty;

            var bookings = repository.GetActiveBookings(booking.Id);
            var overlappingBooking =
            bookings.FirstOrDefault(
            b =>
            booking.ArrivalDate >= b.ArrivalDate && booking.ArrivalDate < b.DepartureDate
            || booking.DepartureDate > b.ArrivalDate && booking.DepartureDate <= b.DepartureDate
            || booking.ArrivalDate <= b.ArrivalDate && booking.DepartureDate >= b.DepartureDate);

            return overlappingBooking == null ? string.Empty
            : overlappingBooking.Reference;
        }
    }
}
