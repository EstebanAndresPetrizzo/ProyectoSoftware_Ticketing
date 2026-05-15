using TicketingAPI.Data;
using TicketingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TicketingAPI.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> GetByIdAsync(Guid id)
        {
            return await _context.Payments
                .Include(p => p.Reservation)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id) ?? throw new ArgumentException("Pago no encontrado");
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Payment>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId)
                .Include(p => p.Reservation)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByReservationIdAsync(Guid reservationId)
        {
            return await _context.Payments
                .Where(p => p.ReservationId == reservationId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
