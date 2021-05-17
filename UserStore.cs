using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityLibrary
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity user store iterfaces
    /// </summary>
    public class UserStore<T> : IUserRoleStore<T>,
                                IUserStore<T>,
                                IUserClaimStore<T>,
                                IUserLoginStore<T>,
                                IUserPasswordStore<T>,
                                IUserEmailStore<T>,
                                IUserLockoutStore<T, string>,
                                IUserTwoFactorStore<T, string>
    where T : IdentityUser
    {
        private readonly UserRepository<T> _userTable;
        private readonly UserRolesRepository _userRolesTable;
        private readonly UserLoginRepository<T> _userLoginTable;
        public UserStore(DatabaseContext databaseContext)
        {
            _userTable = new UserRepository<T>(databaseContext);
            
            _userRolesTable = new UserRolesRepository(databaseContext);
            _userLoginTable = new UserLoginRepository<T>(databaseContext);
        }

        public Task CreateAsync(T user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.Run(() => _userTable.Insert(user));
        }

        public Task<T> FindByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            return Task.Run(() => _userTable.GeTById(userId));
        }

        public Task<bool> GetTwoFactorEnabledAsync(T user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<T> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }

            return Task.Run(() => _userTable.GeTByName(userName));
        }

        public Task<IList<string>> GetRolesAsync(T user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.Run(() => _userRolesTable.FindByUserId(user.Id));
        }

        public Task<string> GetPasswordHashAsync(T user)
        {
            return Task.Run(() => _userTable.GetPasswordHash(user.Id));
        }

        public Task SetPasswordHashAsync(T user, string passwordHash)
        {
            return Task.Run(() => user.PasswordHash = passwordHash);
        }

        public Task<T> FindByEmailAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            return Task.Run(() => _userTable.GeTByEmail(email));
        }

        public Task<string> GetEmailAsync(T user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<int> GetAccessFailedCountAsync(T user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(T user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(T user)
        {
            return
                 Task.FromResult(user.LockoutEndDateUtc.HasValue
                     ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                     : new DateTimeOffset());
        }

        public Task SetLockoutEnabledAsync(T user, bool enabled)
        {
            user.LockoutEnabled = enabled;

            return Task.Run(() => _userTable.Update(user));
        }

        public Task SetLockoutEndDateAsync(T user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(T user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(T user, string email)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(T user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.Run(() => _userTable.Update(user));
        }

        public Task AddToRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task AddLoginAsync(T user, UserLoginInfo login)
        {
            _userLoginTable.AddUserLogin(new UserLogins
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                UserId = user.Id
              
            }); ;
            return Task.Run(() =>  0);
        }

        public Task RemoveLoginAsync(T user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsync(UserLoginInfo login)
        {
            if (login==null)
            {
                throw new ArgumentNullException("login");
            }
           
            var currentUser = _userLoginTable.FindByProvider(login.LoginProvider, login.ProviderKey);
           
            return Task.Run(() => currentUser);

           
        }

        public Task<IList<Claim>> GetClaimsAsync(T user)
        {
            IList<Claim> list = new List<Claim>();
           
           var userClaims =   _userLoginTable.Find(user.Id);
            userClaims.ForEach(c => {
                Claim claim = new Claim(c.ClaimType, c.ClaimValue);
               list.Add(claim);
            });

           return Task.Run(() => list);
        }

        public Task AddClaimAsync(T user, Claim claim)
        {
          
            _userLoginTable.AddClaim(user.Id,new UserClaims { 
              UserId = user.Id,  
              ClaimValue = claim.Value,
              ClaimType = claim.Type
            });
            return Task.Run(() => 0);
        }

        public Task RemoveClaimAsync(T user, Claim claim)
        {
            _userLoginTable.RemoveClaim(user.Id, new UserClaims
            {
                UserId = user.Id,
                ClaimValue = claim.Value,
                ClaimType = claim.Type
            });
            return Task.Run(() => 0);
        }
    }

    
}