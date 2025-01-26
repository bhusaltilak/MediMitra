using MediMitra.Models;
using System.Collections.Concurrent;

namespace MediMitra.Services
{
    public class BookingQueueService
    {

        private readonly ConcurrentQueue<int> _bookingQueue=new ConcurrentQueue<int>();
        public void EnqueueQueue(int bookingId)
        {
            _bookingQueue.Enqueue(bookingId);
        }

        public int GetTotalQueueSize()
        {
            return _bookingQueue.Count;
        }
        public int? GetNextBooking()
        {
            if (_bookingQueue.TryPeek(out int nextBookingId))
            {
                return nextBookingId;
            }
            return null; 
        }

        public int? GetNextBookingForDelayed()
        {
            if (_bookingQueue.TryPeek(out int nextBookingId))
            {
            
                if (_bookingQueue.TryDequeue(out int nextDequeueBookingId))
                {
                    return nextDequeueBookingId;
                }
            }
      
            return nextBookingId; 
        }

    }
}
