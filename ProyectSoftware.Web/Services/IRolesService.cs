using ProyectSoftware.Web.Data.Entities;
using ProyectSoftware.Web.Core;
using ProyectSoftware.Web.Data;
using ProyectSoftware.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using ProyectSoftware.Web.DTOs;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Storage;

namespace ProyectSoftware.Web.Services
{
    public interface IRolesService
    {
        public Task<Response<ProyectSoftwareRole>> CreateAsync(ProyectSoftwareRoleDTO dto);
        public Task<Response<object>> DeleteAsync(int id);
        public Task<Response<ProyectSoftwareRole>> EditAsync(ProyectSoftwareRoleDTO dto);

        //public Task<Response<PaginationResponse<ProyectSoftwareRole>>> GetListAsync(PaginationRequest request);

        public Task<Response<ProyectSoftwareRoleDTO>> GetOneAsync(int id);
        public Task<Response<List<ProyectSoftwareRole>>> GetListAsync();

        public Task<Response<IEnumerable<Permission>>> GetPermissionsAsync();

        public Task<Response<IEnumerable<PermissionForDTO>>> GetPermissionsByRoleAsync(int id);
    }
    public class RolesService : IRolesService
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;

        public RolesService(DataContext context, IConverterHelper converterHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
        }

    

        public async Task<Response<List<ProyectSoftwareRole>>> GetListAsync()
        {
            try
            {
                // Obtiene la lista de autores de la base de datos de manera asíncrona.
                List<ProyectSoftwareRole> list = await _context.ProyectSoftwareRoles.ToListAsync();

                // Crea una respuesta exitosa con la lista obtenida.
                return ResponseHelper<List<ProyectSoftwareRole>>.MakeResponseSuccess(list, "ProyectSoftwareRole Create created");
            }
            catch (Exception ex)
            {
                // En caso de excepción, crea una respuesta de error con el mensaje de la excepción.
                return ResponseHelper<List<ProyectSoftwareRole>>.MakeResponseSuccess(ex.Message);
            }
        }
        public async Task<Response<ProyectSoftwareRole>> CreateAsync(ProyectSoftwareRoleDTO dto)
        {
            using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Creación de Rol
                    ProyectSoftwareRole model = _converterHelper.ToRole(dto);
                    EntityEntry<ProyectSoftwareRole> modelStored = await _context.ProyectSoftwareRoles.AddAsync(model);

                    await _context.SaveChangesAsync();

                    // Inserción de permisos
                    int roleId = modelStored.Entity.Id;

                    List<int> permissionIds = new List<int>();

                    if (!string.IsNullOrWhiteSpace(dto.PermissionIds))
                    {
                        permissionIds = JsonConvert.DeserializeObject<List<int>>(dto.PermissionIds);
                    }

                    foreach (int permissionId in permissionIds)
                    {
                        RolePermission rolePermission = new RolePermission
                        {
                            RoleId = roleId,
                            PermissionId = permissionId
                        };

                        _context.RolePermissions.Add(rolePermission);
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return ResponseHelper<ProyectSoftwareRole>.MakeResponseSuccess("Rol creado con éxito");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ResponseHelper<ProyectSoftwareRole>.MakeResponseFail(ex);
                }
            }
        }

        public async Task<Response<IEnumerable<Permission>>> GetPermissionsAsync()
        {
            try
            {
                IEnumerable<Permission> permissions = await _context.Permissions.ToListAsync();

                return ResponseHelper<IEnumerable<Permission>>.MakeResponseSuccess(permissions);
            }
            catch (Exception ex)
            {
                return ResponseHelper<IEnumerable<Permission>>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<ProyectSoftwareRoleDTO>> GetOneAsync(int id)
        {
            try
            {
                ProyectSoftwareRole? proyectSoftwareRole = await _context.ProyectSoftwareRoles.FirstOrDefaultAsync(r => r.Id == id);

                if (proyectSoftwareRole is null)
                {
                    return ResponseHelper<ProyectSoftwareRoleDTO>.MakeResponseFail($"El Rol con id '{id}' no existe.");
                }

                return ResponseHelper<ProyectSoftwareRoleDTO>.MakeResponseSuccess(await _converterHelper.ToRoleDTOAsync(proyectSoftwareRole));
            }
            catch (Exception ex)
            {
                return ResponseHelper<ProyectSoftwareRoleDTO>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<ProyectSoftwareRole>> EditAsync(ProyectSoftwareRoleDTO dto)
        {
            try
            {
                if (dto.Name == Constants.SUPER_ADMIN_ROLE_NAME)
                {
                    return ResponseHelper<ProyectSoftwareRole>.MakeResponseFail($"El Rol '{Constants.SUPER_ADMIN_ROLE_NAME}' no puede ser editado");
                }

                List<int> permissionIds = new List<int>();

                if (!string.IsNullOrEmpty(dto.PermissionIds))
                {
                    permissionIds = JsonConvert.DeserializeObject<List<int>>(dto.PermissionIds);
                }

                // Eliminación de antiguos permisos
                List<RolePermission> oldRolePermissions = await _context.RolePermissions.Where(rs => rs.RoleId == dto.Id).ToListAsync();
                _context.RolePermissions.RemoveRange(oldRolePermissions);

                // Inserción de nuevos permisos
                foreach (int permissionId in permissionIds)
                {
                    RolePermission rolePermission = new RolePermission
                    {
                        RoleId = dto.Id,
                        PermissionId = permissionId
                    };

                    _context.RolePermissions.Add(rolePermission);
                }

                // Actualización de rol
                ProyectSoftwareRole model = _converterHelper.ToRole(dto);
                _context.ProyectSoftwareRoles.Update(model);

                await _context.SaveChangesAsync();

                return ResponseHelper<ProyectSoftwareRole>.MakeResponseSuccess("Rol editado con éxito");
            }
            catch (Exception ex)
            {
                return ResponseHelper<ProyectSoftwareRole>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<IEnumerable<PermissionForDTO>>> GetPermissionsByRoleAsync(int id)
        {
            try
            {
                Response<ProyectSoftwareRoleDTO> response = await GetOneAsync(id);

                if (!response.IsSuccess)
                {
                    return ResponseHelper<IEnumerable<PermissionForDTO>>.MakeResponseSuccess(response.Message);
                }

                List<PermissionForDTO> permissions = response.Result.Permissions;

                return ResponseHelper<IEnumerable<PermissionForDTO>>.MakeResponseSuccess(permissions);
            }
            catch (Exception ex)
            {
                return ResponseHelper<IEnumerable<PermissionForDTO>>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<object>> DeleteAsync(int id)
        {
            try
            {
                Response<ProyectSoftwareRole> roleResponse = await GetOneModelAsync(id);

                if (!roleResponse.IsSuccess)
                {
                    return ResponseHelper<object>.MakeResponseFail(roleResponse.Message);
                }

                if (roleResponse.Result.Name == Constants.SUPER_ADMIN_ROLE_NAME)
                {
                    return ResponseHelper<object>.MakeResponseFail($"El rol {Constants.SUPER_ADMIN_ROLE_NAME} no puede ser eliminado");
                }

                if (roleResponse.Result.Users.Count() > 0)
                {
                    return ResponseHelper<object>.MakeResponseFail($"El rol no puede ser eliminado debido a que tiene usuarios relacionados");
                }

                _context.ProyectSoftwareRoles.Remove(roleResponse.Result);
                await _context.SaveChangesAsync();

                return ResponseHelper<object>.MakeResponseSuccess("Rol eliminado con éxito");
            }
            catch (Exception ex)
            {
                return ResponseHelper<object>.MakeResponseFail(ex);
            }
        }

        private async Task<Response<ProyectSoftwareRole>> GetOneModelAsync(int id)
        {
            try
            {
                ProyectSoftwareRole? role = await _context.ProyectSoftwareRoles.Include(r => r.Users)
                                                                       .FirstOrDefaultAsync(r => r.Id == id);

                if (role is null)
                {
                    return ResponseHelper<ProyectSoftwareRole>.MakeResponseFail($"El Rol con id '{id}' no existe");
                }

                return ResponseHelper<ProyectSoftwareRole>.MakeResponseSuccess(role);

            }
            catch (Exception ex)
            {
                return ResponseHelper<ProyectSoftwareRole>.MakeResponseFail(ex);
            }
        }
    }
}
