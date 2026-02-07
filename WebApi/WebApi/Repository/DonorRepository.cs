using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Interface;
using WebApi.Models;
using static WebApi.DTOs.DonorDTO;

namespace WebApi.Repository
{
    public class DonorRepository : IDonorRepository
    {
        private readonly WebApiContext _context;

        public DonorRepository(WebApiContext context)
        {
            _context = context;
        }
        public async Task<Donor>CreateDonor(Donor donor)
        {
            _context.Donors.Add(donor);
            await _context.SaveChangesAsync();
            return donor;
        }
        public async Task<bool> Delete(int Id)
        {
            var donor = await _context.Donors.FindAsync(Id);
            if (donor == null)
            {
                return false;
            }
            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Donor?> FindById(int Id)
        {
            return await _context.Donors.FindAsync(Id);

        }
        public async Task<Donor?> Update(Donor donor)
        {
            var donorObj = await FindById(donor.Id);
            if (donorObj == null)
                return null;
            _context.Entry(donorObj).CurrentValues.SetValues(donor);
            await _context.SaveChangesAsync();
            return donorObj;

        }
        public async Task<ICollection<Donor?>> GetAll()
        {
            return await _context.Donors.ToListAsync();
        }
        public async Task<IEnumerable<Donor?>> GetByName(string firstName, string lastName)
        {
            return await _context.Donors
                .Where(d => d.FirstName == firstName && d.LastName == lastName)
                .ToListAsync();
        }
        public async Task<IEnumerable<Donor?>> GetByEmail(string email)
        {
            return await _context.Donors
                .Where(d => d.Email == email)
                .ToListAsync();
        }
        public async Task<IEnumerable<Donor?>> GetByGift(string gift)
        {
            return await _context.Donors
                .Include(d => d.Gifts)
                .Where(d => d.Gifts.Any(g => g.Name == gift))
                .ToListAsync();

        }
    }
}
