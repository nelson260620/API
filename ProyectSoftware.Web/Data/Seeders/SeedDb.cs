using ProyectSoftware.Web.Services;
using System.Reflection.PortableExecutable;

namespace ProyectSoftware.Web.Data.Seeders
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUsersService _usersService;

        public SeedDb(DataContext context, IUsersService usersService)
        {
            _context = context;
            _usersService = usersService;
        }

        public async Task SeedAsync()
        {
            
            await new GenderTypeSeeder(_context).SeedAsync();
            await new PermissionSedder(_context).SeedAsync();
            await new UserRoleSeeder(_usersService, _context).SeedAsync();
        }
    }
    
    
}
