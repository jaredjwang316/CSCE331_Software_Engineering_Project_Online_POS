/*
    File: UnitOfWork.cs
    Author: Griffin Beaudreau
    Date: November 5, 2023
*/

using Npgsql;
using WebApp.Models;

namespace WebApp.Data;

public class UnitOfWork {
    private readonly ConnectionHandler connectionHandler;
    private readonly CommandHandler commandHandler;
    private readonly DaoContainer daoContainer;
    private readonly EntityServices entityServices;


    public UnitOfWork(string databaseName) {
        connectionHandler = new ConnectionHandler(databaseName);
        commandHandler = new CommandHandler(connectionHandler.GetConnection()!);
        daoContainer = new DaoContainer(commandHandler);
        entityServices = new EntityServices(daoContainer);
    }

    // use entity service to abstract the CRUD operations

    public void Add<T>(T t) where T : class {
        entityServices.Add(t);
    }

    public void Update<T>(T t, T newT) where T : class {
        entityServices.Update(t, newT);
    }

    public void Delete<T>(T t) where T : class {
        entityServices.Delete(t);
    }

    public T Get<T>(int id) where T : class {
        return entityServices.Get<T>(id);
    }

    public IEnumerable<T> GetAll<T>() where T : class {
        return entityServices.GetAll<T>();
    }

    // Use connection handler to close the connection to the database
    public void CloseConnection() {
        connectionHandler.CloseConnection();
    }

    //====================================================================================================
    // Custom Queries for the database
    //====================================================================================================

    public IEnumerable<Product> GetProductsBySeries(string series) {
        return daoContainer.ProductDao.GetProductsBySeries(series);
    }

    public IEnumerable<string> GetUniqueSeries(
        bool includeDrinks = true,
        bool includeHidden = false,
        bool includeIsOption = false
    ) {
        return daoContainer.ProductDao.GetUniqueSeries(includeDrinks, includeHidden, includeIsOption);
    }

    public IEnumerable<Product> GetBestSellingProducts(int limit) {
        return daoContainer.ProductDao.GetBestSellingProducts(limit);
    }

    public SeriesInfo GetSeriesInfo(string name) {
        return daoContainer.SeriesInfoDao.Get(name);
    }
}

public class DaoContainer {
    public EmployeeDao EmployeeDao { get; }
    public IngredientDao IngredientDao { get; }
    public InventoryDao InventoryDao { get; }
    public OrderDao OrderDao { get; }
    public ProductDao ProductDao { get; }
    public ProductIngredientsDao ProductIngredientsDao { get; }
    public SeriesInfoDao SeriesInfoDao { get; }

    public DaoContainer(CommandHandler commandHandler) {
        EmployeeDao = new EmployeeDao(commandHandler);
        IngredientDao = new IngredientDao(commandHandler);
        InventoryDao = new InventoryDao(commandHandler);
        OrderDao = new OrderDao(commandHandler);
        ProductDao = new ProductDao(commandHandler);
        ProductIngredientsDao = new ProductIngredientsDao(commandHandler);
        SeriesInfoDao = new SeriesInfoDao(commandHandler);
    }
}



//====================================================================================================
// ConnectionHandler
//====================================================================================================
/// <summary>
/// Class for handling connections to the database.
/// </summary>
class ConnectionHandler {
    private readonly NpgsqlConnection? connection;

    /// <summary>
    /// Constructor for ConnectionHandler.
    /// </summary>
    /// <param name="databaseName"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public ConnectionHandler(string databaseName) {
        if (string.IsNullOrEmpty(databaseName)) {
            throw new ArgumentException("Database name cannot be null or empty.");
        }

        try {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(Config.CONNECTION_STRING);
            var dataSource = dataSourceBuilder.Build();
            connection = dataSource.OpenConnection();
        } catch (Exception e) {
            throw new Exception($"Error connecting to database: {e.Message}");
        }
    }

    /// <summary>
    /// Returns the connection to the database.
    /// </summary>
    /// <returns></returns>
    public NpgsqlConnection? GetConnection() {
        return connection;
    }

    /// <summary>
    /// Closes the connection to the database.
    /// </summary>
    public void CloseConnection() {
        connection?.Close();
    }
}

//====================================================================================================
// EntityServices
//====================================================================================================
public class EntityServices {
    private readonly DaoContainer daoContainer;

    public EntityServices(DaoContainer daoContainer) {
        this.daoContainer = daoContainer;
    }

    public void Add<T>(T t) where T : class {
        IDao<T> dao = GetDao<T>();
        dao.Add(t);
    }

    public void Update<T> (T t, T newT) where T : class {
        IDao<T> dao = GetDao<T>();
        dao.Update(t, newT);
    }

    public void Delete<T>(T t) where T : class {
        IDao<T> dao = GetDao<T>();
        dao.Delete(t);
    }

    public T Get<T>(int id) where T : class {
        IDao<T> dao = GetDao<T>();
        return dao.Get(id);
    }

    public IEnumerable<T> GetAll<T>() where T : class {
        IDao<T> dao = GetDao<T>();
        return dao.GetAll();
    }

    // Helper funciton to return the correct dao for the given type
    private IDao<T> GetDao<T>() where T : class {
        if (typeof(T) == typeof(Employee)) {
            return daoContainer.EmployeeDao as IDao<T> ?? throw new InvalidCastException();
        } else if (typeof(T) == typeof(Ingredient)) {
            return daoContainer.IngredientDao as IDao<T> ?? throw new InvalidCastException();
        } else if (typeof(T) == typeof(Inventory)) {
            return daoContainer.InventoryDao as IDao<T> ?? throw new InvalidCastException();
        } else if (typeof(T) == typeof(Order)) {
            return daoContainer.OrderDao as IDao<T> ?? throw new InvalidCastException();
        } else if (typeof(T) == typeof(Product)) {
            return daoContainer.ProductDao as IDao<T> ?? throw new InvalidCastException();
        } else if (typeof(T) == typeof(ProductIngredients)) {
            return daoContainer.ProductIngredientsDao as IDao<T> ?? throw new InvalidCastException();
        } else if (typeof(T) == typeof(SeriesInfo)) {
            return daoContainer.SeriesInfoDao as IDao<T> ?? throw new InvalidCastException();
        } else {
            throw new InvalidCastException();
        }
    }
}
