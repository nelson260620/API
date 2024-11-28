using Microsoft.EntityFrameworkCore;
using ProyectSoftware.Web.Data.Entities;
using static System.Collections.Specialized.BitVector32;

namespace ProyectSoftware.Web.Data.Seeders
{
    public class GenderTypeSeeder
    {
        private readonly DataContext _context;

        public GenderTypeSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            List<GenderType> gendertype = new List<GenderType>
                {
                    new GenderType { GenderName = "Samba" },
                    new GenderType { GenderName = "LaPinga" },
                };

            foreach (GenderType genderType in gendertype)
            {
                bool exists = await _context.GenderTypes.AnyAsync(s => s.GenderName == genderType.GenderName);

                if (!exists)
                {
                    await _context.GenderTypes.AddAsync(genderType);  // This should be genderType, not gendertype.
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

