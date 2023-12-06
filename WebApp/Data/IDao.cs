/*
    File: IDao.cs
    Author: Griffin Beaudreau
    Date: 10/23/2023
    Description: Interface for the DAO classes.
*/

namespace WebApp.Data;

/// <summary>
/// Interface for Data Access Object (DAO) classes responsible for CRUD operations on entities.
/// </summary>
/// <typeparam name="T">Type of entity for which the DAO is defined.</typeparam>
public interface IDao<T> {

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity with the specified identifier, or a default value if not found.</returns>
    T Get(int id);
    
    /// <summary>
    /// Retrieves all entities of the specified type.
    /// </summary>
    /// <returns>An enumerable collection of all entities of the specified type.</returns>
    IEnumerable<T> GetAll();
    
    /// <summary>
    /// Adds a new entity to the data store.
    /// </summary>
    /// <param name="t">The entity to be added.</param>
    void Add(T t);
    
    /// <summary>
    /// Updates an existing entity in the data store.
    /// </summary>
    /// <param name="t">The existing entity to be updated.</param>
    /// <param name="newT">The new values for the entity.</param>
    void Update(T t, T newT);
    
    /// <summary>
    /// Deletes an entity from the data store.
    /// </summary>
    /// <param name="t">The entity to be deleted.</param>
    void Delete(T t);
}
