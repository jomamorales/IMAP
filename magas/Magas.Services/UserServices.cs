using System;
using System.Collections.Generic;
using System.Text;
using Magas.Core;
using Magas.Data;
using Magas.Core;
using Magas.Data;

namespace Magas.Services
{
    public class UserServices
    {
        private IDataContext dataContext;

        public UserServices()
        {
            this.dataContext = new DataAccessProviderFactory().GetDataContext();
        }


        public User GetUserByUsername(string username)
        {
            IList<User> users = this.dataContext.GetByField<User>("Username", username);
            if (users.Count == 0)
            {
                return null;
            }

            if (users.Count == 1)
            {
                return users[0];
            }
            throw new InvalidOperationException("Expected GetUserByUsername return zero or one User");
        }
    }
}
