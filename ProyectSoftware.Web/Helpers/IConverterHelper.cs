using Microsoft.EntityFrameworkCore;
using ProyectSoftware.Web.Data;
using ProyectSoftware.Web.Data.Entities;
using ProyectSoftware.Web.DTOs;

namespace ProyectSoftware.Web.Helpers
{
    public interface IConverterHelper
    {
        public AccountUserDTO ToAccountDTO(User user);
        public ProyectSoftwareRole ToRole(ProyectSoftwareRoleDTO dto);
        public Task<ProyectSoftwareRoleDTO> ToRoleDTOAsync(ProyectSoftwareRole role);
        public User ToUser(UserDTO dto);
    }

    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;

        public ConverterHelper(DataContext context)
        {
            _context = context;
        }
        public AccountUserDTO ToAccountDTO(User user)
        {
            return new AccountUserDTO
            {
                Id = Guid.Parse(user.Id),
                Document = user.Document,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
            };
        }

        public ProyectSoftwareRole ToRole(ProyectSoftwareRoleDTO dto)
        {
            return new ProyectSoftwareRole
            {
                Id = dto.Id,
                Name = dto.Name,
            };
        }

        public async Task<ProyectSoftwareRoleDTO> ToRoleDTOAsync(ProyectSoftwareRole role)
        {
            List<PermissionForDTO> permissions = await _context.Permissions.Select(p => new PermissionForDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Module = p.Module,
                Selected = _context.RolePermissions.Any(rp => rp.PermissionId == p.Id && rp.RoleId == role.Id)
            }).ToListAsync();

            return new ProyectSoftwareRoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = permissions,
            };

        }

        public User ToUser(UserDTO dto)
        {
            return new User
            {
                Id = dto.Id.ToString(),
                Document = dto.Document,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
                ProyectSoftwareRoleId = dto.ProyectSoftwareRoleId,
                PhoneNumber = dto.PhoneNumber,
            };
        }
    }
}
