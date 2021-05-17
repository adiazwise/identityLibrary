using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityLibrary
{
    public class UserLoginRepository<T> where T : IdentityUser
    {
        private readonly DatabaseContext _databaseContext;
        
        public UserLoginRepository(DatabaseContext database)
        {
            _databaseContext = database;
        }

        public bool RemoveUserLogin(UserLogins userLogin)
        {
            var result = false;

            var info = _databaseContext.UserLogins.FirstOrDefault(x => x.LoginProvider == userLogin.LoginProvider && x.ProviderKey == userLogin.ProviderKey);
            if (info != null)
            {
                _databaseContext.UserLogins.Remove(info);
               
            }
            result = _databaseContext.SaveChanges() > 0;
            return result;
        }
        public bool AddUserLogin(UserLogins userLogin)
        {
            var result = false;
           
            Users user = _databaseContext.Users.Find(userLogin.UserId);

            _databaseContext.UserLogins.Add(new UserLogins { 
            LoginProvider = userLogin.LoginProvider, 
            UserId = user.Id,
            Users = user,
            ProviderKey = userLogin.ProviderKey
            });
            result = _databaseContext.SaveChanges() > 0;
            return result;
        }
        public T FindByProvider(string provider,string providerKey)
        {
            T result = (T)Activator.CreateInstance(typeof(T));

           var loginInfo = _databaseContext.UserLogins.FirstOrDefault(l => l.LoginProvider == provider && l.ProviderKey == providerKey);

            if (loginInfo != null)
            {
                var user = loginInfo.Users;
                result.Id = user.Id;
                result.UserName = user.UserName;
                result.PasswordHash = user.PasswordHash;
                result.SecurityStamp = user.SecurityStamp;
                result.Email = result.Email;
                result.EmailConfirmed = user.EmailConfirmed;
                result.PhoneNumber = user.PhoneNumber;
                result.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                result.LockoutEnabled = user.LockoutEnabled;
                result.LockoutEndDateUtc = user.LockoutEndDateUtc;
                result.AccessFailedCount = user.AccessFailedCount;
                return result;
            }else
            {
                return null;
            }
        }

        public bool AddClaim(string id,UserClaims claims)
        {
            Users user = _databaseContext.Users.Find(id);

            _databaseContext.UserClaims.Add(new UserClaims {
            UserId = user.Id,
            ClaimType = claims.ClaimType,
            ClaimValue = claims.ClaimValue,
            Users = user
            });
            return _databaseContext.SaveChanges() > 0;
        }
        public bool RemoveClaim(string id,UserClaims claims)
        {
            var userClaim = _databaseContext.UserClaims.FirstOrDefault(c =>
                            c.UserId == id && c.ClaimValue == claims.ClaimValue);
            _databaseContext.UserClaims.Remove(userClaim);
            return _databaseContext.SaveChanges() > 0;
        }
        public List<UserClaims> Find(string id)
        {
            List<Claim> list = new List<Claim>();

            Users user = _databaseContext.Users.Find(id);

            return _databaseContext.UserClaims.ToList();
          
        }

    }
}
