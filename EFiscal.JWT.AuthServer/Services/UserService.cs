using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EFiscal.JWT.AuthServer.Common.Security;
using EFiscal.JWT.AuthServer.Data.Stores;
using EFiscal.JWT.AuthServer.Models.Security;
using EFiscal.JWT.AuthServer.Services.Communication;

namespace EFiscal.JWT.AuthServer.Services
{
    public class UserService
    {
        private readonly UserStore _userStore;
        private readonly RoleStore _roleStore;

        private readonly IPasswordHasher _passwordHasher;

        public UserService(UserStore userStore, RoleStore roleStore, IPasswordHasher passwordHasher)
        {
            _userStore = userStore;
            _passwordHasher = passwordHasher;
            _roleStore = roleStore;
        }

        public async Task<CreateUserResponse> CreateUserAsync(User user, params ERole[] userRoles)
        {
            var existingUser = _userStore.First(
                (u) => u.NormalizedEmail.Equals(user.NormalizedEmail) || u.NormalizedName.Equals(user.NormalizedName));

            if (existingUser != null)
            {
                return new CreateUserResponse(false, "Ya hay un usuario con ese correo o nombre en uso.", null);
            }

            user.Password = _passwordHasher.HashPassword(user.Password);
            await _userStore.AddUserAsync(user, userRoles);

            return new CreateUserResponse(true, null, user);
        }

        public async Task<UpdateUserResponse> ChangeUserNameAsync(string name, string newName )
        {
            var isNewNameInUse = _userStore.First( 
                (u) => u.NormalizedName.Equals(newName.ToLowerInvariant()));

            if (isNewNameInUse != null)
            {
                return new UpdateUserResponse(false, $"El nombre {newName} ya esta en uso", null);
            }

            var existingUser = _userStore.First(
                (u) => u.NormalizedName.Equals(name.ToLowerInvariant()));

            if (existingUser == null)
            {
                return new UpdateUserResponse(false, "El usuario no existe en este contexto", null);
            }

            existingUser.Name = newName;

            await _userStore.UpdateAsync(existingUser);
            return new UpdateUserResponse(true, null, existingUser);
        }

        public async Task<UpdateUserResponse> ChangeUserEmailAsync(string userName, string newEmail)
        {
            var isnewEmailInUse = _userStore.First(
                (u) => u.NormalizedEmail.Equals(newEmail.ToLowerInvariant()));

            if (isnewEmailInUse != null)
            {
                return new UpdateUserResponse(false, $"El email {newEmail} ya esta en uso", null);
            }

            var existingUser = _userStore.First(
                (u) => u.NormalizedName.Equals(userName.ToLowerInvariant()));

            if (existingUser == null)
            {
                return new UpdateUserResponse(false, "El usuario no existe en este contexto", null);
            }

            existingUser.Email = newEmail;

            await _userStore.UpdateAsync(existingUser);
            return new UpdateUserResponse(true, null, existingUser);
        }

        public async Task<UpdateUserResponse> ChangeUserPasswordAsync(string userName, string newPassword)
        {
            var existingUser = _userStore.First(
                (u) => u.NormalizedName.Equals(userName.ToLowerInvariant()));

            if (existingUser == null)
            {
                return new UpdateUserResponse(false, "El usuario no existe en este contexto", null);
            }

            existingUser.Password = _passwordHasher.HashPassword(newPassword);

            await _userStore.UpdateAsync(existingUser);
            return new UpdateUserResponse(true, null, existingUser);
        }

        public PasswordValidationResponse ValidatePassword(string userName, string password)
        {
            var existingUser = _userStore.First(
                (u) => u.NormalizedName.Equals(userName.ToLowerInvariant()));

            if (existingUser == null)
            {
                return new PasswordValidationResponse(false, "El usuario no existe en este contexto", null);
            }

            if ( !_passwordHasher.PasswordMatches(password, existingUser.Password) )
            {
                return new PasswordValidationResponse(false, "Las credenciales no son validas", null);
            }
            return new PasswordValidationResponse(true, null, existingUser);
        }

        public async Task<UpdateUserResponse> RemoveUserAsync(string name)
        {
            var existingUser = _userStore.First(
                (u) => u.NormalizedName.Equals(name.ToLowerInvariant()));

            if (existingUser == null)
            {
                return new UpdateUserResponse(false, "El usuario no existe en este contexto", null);
            }

            await _userStore.DeleteAsync(existingUser);
            return new UpdateUserResponse(true, null, existingUser);
        }

        public async Task<UpdateUserRoleResponse> AddRoleAsync(string userName, ERole userRole)
        {
            var userTo = await FindByNameAsync(userName);
            if (userTo.UserRoles.Any( (a) => a.Role.NormalizedName == userRole.ToString().ToLowerInvariant() ))
            {
                return new UpdateUserRoleResponse(false, $"El usuario ya posee el rol {userRole}", userTo);
            }

            var role = await _roleStore.FindByNameAsync(userRole.ToString().ToLowerInvariant());
            userTo.UserRoles.Add(new UserRole { RoleId = role.Id });
            await _userStore.UpdateAsync(userTo);

            return new UpdateUserRoleResponse(true, null, userTo);
        }

        public async Task<UpdateUserRoleResponse> RemoveRoleAsync(string userName, ERole userRole)
        {
            var userTo = await FindByNameAsync(userName);
            if (!userTo.UserRoles.Any((a) => a.Role.NormalizedName == userRole.ToString().ToLowerInvariant()))
            {
                return new UpdateUserRoleResponse(false, $"El usuario no posee el rol {userRole}", userTo);
            }

            var toRemove = userTo.UserRoles.Where((ur) => ur.Role.NormalizedName == userRole.ToString().ToLowerInvariant()).FirstOrDefault();
            userTo.UserRoles.Remove(toRemove);

            await _userStore.UpdateAsync(userTo);
            return new UpdateUserRoleResponse(true, null, userTo);
        }

        public virtual string GetUserName(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirstValue(ClaimTypes.Name);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userStore.FindByEmailAsync(email);
        }

        public async Task<User> FindByNameAsync(string name)
        {
            return await _userStore.FindByNameAsync(name);
        }

        public async Task<User> FindByIdAsync(string id)
        {
            return await _userStore.FindByIdAsync(id);
        }

        public async Task<bool> GetUserIsLocked(string name)
        {
            var userIs = await _userStore.FindByNameAsync(name);
            return userIs.IsLocked ? true : false;
        }

        public async Task<UpdateUserResponse> LockUser(string name)
        {
            var userIs = await _userStore.FindByNameAsync(name);
            userIs.IsLocked = true;
            await _userStore.UpdateAsync(userIs);
            return new UpdateUserResponse(true, null, userIs);
        }

        public async Task<UpdateUserResponse> UnLockUser(string name)
        {
            var userIs = await _userStore.FindByNameAsync(name);
            userIs.IsLocked = false;
            await _userStore.UpdateAsync(userIs);
            return new UpdateUserResponse(true, null, userIs);
        }

    }
}
