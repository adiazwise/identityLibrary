using IdentityLibrary.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace IdentityLibrary
{
   
    internal class UserRolesRepository
    {
        private readonly DatabaseContext _databaseContext;

        public UserRolesRepository(DatabaseContext database)
        {
            _databaseContext = database;
        }

        /// <summary>
        /// Returns a list of user's roles
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public IList<string> FindByUserId(string userId)
        {
            var roles = _databaseContext.Users.
                Where(u => u.Id == userId).SelectMany(r => r.Roles);
            return roles.Select(r => r.Name).ToList();
        }
    }
}