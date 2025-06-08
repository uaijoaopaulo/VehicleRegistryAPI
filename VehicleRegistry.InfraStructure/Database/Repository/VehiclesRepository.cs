using Microsoft.EntityFrameworkCore;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Models;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Database;

namespace VehicleRegistry.InfraStructure.Database.Repository
{
    public class VehiclesRepository(AppDbContext context) : DbRepository<VehicleDTO>(context), IVehiclesRepository
    {
        public async Task<List<VehicleDTO>> GetVehiclesAsync(string? plate, List<int>? ids, int? page = null, int? pageSize = null)
        {
            IQueryable<VehicleDTO> query = _context.Vehicles.AsQueryable();

            if (!string.IsNullOrEmpty(plate))
            {
                query = query.Where(v => v.Plate == plate);
            }

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(v => ids.Contains(v.Id));
            }

            query = query.OrderBy(v => v.Make).ThenBy(v => v.Model);

            if (page.HasValue && pageSize.HasValue)
            {
                int skip = (page.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }
    }
}
