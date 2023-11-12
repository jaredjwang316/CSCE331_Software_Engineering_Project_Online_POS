/*
    File: IDao.cs
    Author: Griffin Beaudreau
    Date: 10/23/2023
    Description: Interface for the DAO classes.
*/

namespace WebApp.Data;
public interface IDao<T> {

    T Get(int id);
    IEnumerable<T> GetAll();
    void Add(T t);
    void Update(T t, T newT);
    void Delete(T t);
}
