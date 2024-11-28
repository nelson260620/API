using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectSoftware.Web.Core;
using ProyectSoftware.Web.Data;
using ProyectSoftware.Web.Data.Entities;
using ProyectSoftware.Web.DTOs;
using ProyectSoftware.Web.Helpers;
using ClaimsUser = System.Security.Claims.ClaimsPrincipal;
namespace ProyectSoftware.Web.Services
{
    public interface IUsersService
    {
        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);
        Task<Response<User>> CreateAsync(UserDTO dto);
        Task<bool> CurrentUserIsAuthorizedAsync(string permission, string module);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<IdentityResult> ResetPasswordAsync(User user, string resetToken, string newPassword);
        Task<User> GetUserAsync(string email);
        Task<Response<List<User>>> GetListAsync();
        Task<SignInResult> LoginAsync(LoginDTO model);

        Task LogoutAsync();
        Task<IdentityResult> UpdateUserAsync(User user);


    }

    public class UsersServices : IUsersService
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;
        private readonly SignInManager<User> _signInManager;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IConverterHelper _converterHelper;
        public UsersServices(UserManager<User> userManager, DataContext context, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor = null, IConverterHelper converterHelper = null)
        {

            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _converterHelper = converterHelper;
        }
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            return _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<Response<User>> CreateAsync(UserDTO dto)
        {
            try
            {
                User user = _converterHelper.ToUser(dto);
                Guid id = Guid.NewGuid();
                user.Id = id.ToString();

                IdentityResult res = await AddUserAsync(user, dto.Document);

                if (!res.Succeeded)
                {
                    return ResponseHelper<User>.MakeResponseFail("Error al crear el usuario.");
                }

                // TODO: Eliminar cuando se haga envío de Email
                string token = await GenerateEmailConfirmationTokenAsync(user);
                await ConfirmEmailAsync(user, token);

                return ResponseHelper<User>.MakeResponseSuccess(user, "Usuario creado con éxito");
            }
            catch (Exception ex)
            {
                return ResponseHelper<User>.MakeResponseFail(ex);
            }
        }
        public async Task<bool> CurrentUserIsAuthorizedAsync(string permission, string module)
        {
            ClaimsUser? claimUser = _httpContextAccessor.HttpContext?.User;
            // Valida si esta logueado
            if (claimUser is null)
            {
                return false;
            }

            string? userName = claimUser.Identity.Name;

            User? user = await GetUserAsync(userName);

            // Valida si user existe
            if (user is null)
            {
                return false;
            }

            // Valida si es admin
            if (user.ProyectSoftwareRole.Name == "Administrador")
            {
                return true;
            }

            // Si no es administrador, valida si tiene el permiso
            return await _context.Permissions.Include(p => p.RolePermissions)
                                             .AnyAsync(p => (p.Module == module && p.Name == permission)
                                                        && p.RolePermissions.Any(rp => rp.RoleId == user.ProyectSoftwareRoleId));
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }
       

        public async Task<User> GetUserAsync(string email)
        {
            User? user = await _context.Users.Include(u => u.ProyectSoftwareRole)
                                             .FirstOrDefaultAsync(u => u.Email == email);

            return user;
        }
        public async Task<Response<List<User>>> GetListAsync()
        {
            try
            {
                // Obtiene la lista de autores de la base de datos de manera asíncrona.
                List<User> list = await _context.Users.ToListAsync();

                // Crea una respuesta exitosa con la lista obtenida.
                return ResponseHelper<List<User>>.MakeResponseSuccess(list, "ProyectSoftwareRole Create created");
            }
            catch (Exception ex)
            {
                // En caso de excepción, crea una respuesta de error con el mensaje de la excepción.
                return ResponseHelper<List<User>>.MakeResponseSuccess(ex.Message);
            }
        }

        public async Task<SignInResult> LoginAsync(LoginDTO model)
        {
            return await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }
        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
        public Task<IdentityResult> ResetPasswordAsync(User user, string resetToken, string newPassword)
        {
            return _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        }
    }

}
