using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Magas.Core;
using Magas.Core;
using Magas.Data;

namespace Magas.Data.Xml
{
    public class XmlDataContext 
        : IDataContext
        , IDisposable
    {
        string filesDirectory = string.Empty;

        public XmlDataContext()
        {
            this.filesDirectory = ConfigurationManager.AppSettings["DataDirectory"];
            if (string.IsNullOrEmpty(this.filesDirectory))
            {
                throw new InvalidOperationException("No data directory was specified");
            }
        }

        #region IDataContext Members

         /// <summary>
        /// Retrieves all the persisted instances of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve</typeparam>
        /// <returns>The list of persistent objects</returns>
        public IList<T> GetAll<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the persistent instance of the given entity class with the given identifier, or null if there is no such persistent instance.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="id">The identifier of the object</param>
        /// <returns>The persistent instance or null</returns>
        public T GetById<T>(object key) where T : class, new()
        {
            if (typeof(T) == typeof(User))
            {
                object result = new XmlUserMapper().GetById(key);
                return (T)result;
            }

            if (typeof(T) == typeof(MailboxAccount))
            {
                object result = new XmlMailboxAccountMapper().GetById(key);
                return (T)result;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the persistent instance of the given entity class with the given value of fieldname, or an empty list if there is no such persistent instance.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="fieldname">The fieldname to check</param>
        /// <param name="value">The value of fieldname</param>
        /// <returns>List of persistent instance match condition</returns>
        public IList<T> GetByField<T>(string fieldname, object value) where T : class, new()
        {
            if (typeof(T) == typeof(User))
            {
                return (IList<T>)new XmlUserMapper().GetByField(fieldname, value);
            }

            if (typeof(T) == typeof(MailboxAccount))
            {
                return (IList<T>)new XmlMailboxAccountMapper().GetByField(fieldname, value);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the amount of objects of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <returns>The amount of objects</returns>
        public int GetCount<T>()
        {
            throw new NotImplementedException();
        }


        ///// <summary>
        ///// Returns current registry
        ///// </summary>
        ///// <returns>the registry of datacontext</returns>
        //public IRegistry GetRegistry()
        //{
        //    return new NHibernateRegistry(session);
        //}

        /// <summary>
        /// Adds an object to the repository
        /// </summary>
        /// <param name="item">The object to add</param>
        public void Add(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            throw new NotImplementedException();

        }

        /// <summary>
        /// Deletes an object from the repository
        /// </summary>
        /// <param name="item">The object to delete</param>
        public void Delete(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            throw new NotImplementedException();

        }

        /// <summary>
        /// Saves an object to the repository
        /// </summary>
        /// <param name="item">The object to save</param>
        public void Save(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            if (item.GetType() == typeof(User))
            {
                new XmlUserMapper().SaveUser((User)item);
                return;
            }
            throw new NotImplementedException();

        }

        #endregion

        #region Transaction management

        /// <summary>
        /// Reports whether this <c>ObjectSpaceServices</c> contain any changes which must be synchronized with the database
        /// </summary>
        public bool IsDirty
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Reports whether this <c>ObjectSpaceServices</c> is working transactionally
        /// </summary>
        public bool IsInTransaction
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Begins a transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is an already active transaction</exception>
        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Commits the active transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there isn't an active transaction</exception>
        public void Commit()
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Rollbacks the active transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there isn't an active transaction</exception>        
        public void Rollback()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
