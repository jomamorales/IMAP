using System;
using System.Collections.Generic;
using System.Text;

namespace Magas.Data
{
    public interface IDataContext
        : IDisposable
    {
        /// <summary>
        /// Reports whether this <c>IObjectSpaceServices</c> contain any changes which must be synchronized with the database
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Reports whether this <c>IObjectSpaceServices</c> is working transactionally
        /// </summary>
        bool IsInTransaction { get; }

        /// <summary>
        /// Return the persistent instance of the given entity class with the given identifier, or null if there is no such persistent instance.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="id">The identifier of the object</param>
        /// <returns>The persistent instance or null</returns>
        T GetById<T>(object id) where T : class, new();

        /// <summary>
        /// Return the persistent instance of the given entity class with the given value of fieldname, or an empty list if there is no such persistent instance.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="fieldname">The fieldname to check</param>
        /// <param name="value">The value of fieldname</param>
        /// <returns>List of persistent instance match condition</returns>
        IList<T> GetByField<T>(string fieldname, object value) where T : class, new();

        /// <summary>
        /// Returns the amount of objects of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <returns>The amount of objects</returns>
        int GetCount<T>();

        /// <summary>
        /// Retrieves all the persisted instances of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve</typeparam>
        /// <returns>The list of persistent objects</returns>
        IList<T> GetAll<T>() where T : class, new();

        ///// <summary>
        ///// Returns current registry
        ///// </summary>
        ///// <returns>the registry of datacontext</returns>
        //IRegistry GetRegistry();

        /// <summary>
        /// Adds an object to the repository
        /// </summary>
        /// <param name="item">The object to add</param>
        void Add(object item);

        /// <summary>
        /// Deletes an object from the repository
        /// </summary>
        /// <param name="item">The object to delete</param>
        void Delete(object item);

        /// <summary>
        /// Saves an object to the repository
        /// </summary>
        /// <param name="item">The object to save</param>
        void Save(object item);

        /// <summary>
        /// Begins a transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is an already active transaction</exception>
        void BeginTransaction();

        /// <summary>
        /// Commits the active transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there isn't an active transaction</exception>
        void Commit();

        /// <summary>
        /// Rollbacks the active transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there isn't an active transaction</exception>
        void Rollback();

    }
}
