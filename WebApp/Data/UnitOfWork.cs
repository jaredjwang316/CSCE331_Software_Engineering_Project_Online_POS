/*
    File: UnitOfWork.cs
    Author: Griffin Beaudreau
    Date: November 5, 2023
*/

using Npgsql;
using WebApp.Models.UnitOfWork;

namespace WebApp.Data;

public class UnitOfWork {
    private readonly ConnectionHandler connectionHandler;
    private readonly CommandHandler commandHandler;
    private readonly DaoTypeContainer daoTypeContainer;
    private readonly EntityServices entityServices;
    
    public UnitOfWork(string databaseName = "") {
        if (string.IsNullOrEmpty(databaseName)) {
            databaseName = Config.AWS_DB_NAME;
        }
        connectionHandler = new ConnectionHandler(databaseName);
        commandHandler = new CommandHandler(connectionHandler.GetConnection()!);

        Dictionary<Type, object> daoTypeMap = new() {
            { typeof(Employee), new EmployeeDao(commandHandler) },
            { typeof(Ingredient), new IngredientDao(commandHandler) },
            { typeof(Inventory), new InventoryDao(commandHandler) },
            { typeof(Order), new OrderDao(commandHandler) },
            { typeof(Product), new ProductDao(commandHandler) },
            { typeof(ProductIngredients), new ProductIngredientsDao(commandHandler) },
            { typeof(SeriesInfo), new SeriesInfoDao(commandHandler) },
            { typeof(User), new UserDao(commandHandler) }
        };
        daoTypeContainer = new DaoTypeContainer(daoTypeMap);
        
        entityServices = new EntityServices(daoTypeContainer);
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
        return daoTypeContainer.GetProductDao().GetProductsBySeries(series);
    }

    public IEnumerable<string> GetUniqueSeries(
        bool includeDrinks = true,
        bool includeHidden = false,
        bool includeIsOption = false
    ) {
        return daoTypeContainer.GetProductDao().GetUniqueSeries(includeDrinks, includeHidden, includeIsOption);
    }

    public IEnumerable<Product> GetBestSellingProducts(int limit) {
        return daoTypeContainer.GetProductDao().GetBestSellingProducts(limit);
    }

    public SeriesInfo GetSeriesInfo(string name) {
        return daoTypeContainer.GetSeriesInfoDao().Get(name);
    }

    public User? GetUser(string email) {
        return daoTypeContainer.GetUserDao().Get(email);
    }
}

public class DaoTypeContainer {
    private readonly Dictionary<Type, object> daoTypeMap;

    public DaoTypeContainer(Dictionary<Type, object> daoTypeMap) {
        this.daoTypeMap = daoTypeMap;
    }

    public IDao<T> GetDao<T>() where T : class {
        if (daoTypeMap.TryGetValue(typeof(T), out object? dao)) {
            return dao as IDao<T> ?? throw new InvalidCastException();
        } else {
            throw new InvalidCastException();
        }
    }

    // Unique getters for each dao
    public EmployeeDao GetEmployeeDao() {
        return daoTypeMap[typeof(Employee)] as EmployeeDao ?? throw new InvalidCastException();
    }
    public IngredientDao GetIngredientDao() {
        return daoTypeMap[typeof(Ingredient)] as IngredientDao ?? throw new InvalidCastException();
    }
    public InventoryDao GetInventoryDao() {
        return daoTypeMap[typeof(Inventory)] as InventoryDao ?? throw new InvalidCastException();
    }
    public OrderDao GetOrderDao() {
        return daoTypeMap[typeof(Order)] as OrderDao ?? throw new InvalidCastException();
    }
    public ProductDao GetProductDao() {
        return daoTypeMap[typeof(Product)] as ProductDao ?? throw new InvalidCastException();
    }
    public ProductIngredientsDao GetProductIngredientsDao() {
        return daoTypeMap[typeof(ProductIngredients)] as ProductIngredientsDao ?? throw new InvalidCastException();
    }
    public SeriesInfoDao GetSeriesInfoDao() {
        return daoTypeMap[typeof(SeriesInfo)] as SeriesInfoDao ?? throw new InvalidCastException();
    }
    public UserDao GetUserDao() {
        return daoTypeMap[typeof(User)] as UserDao ?? throw new InvalidCastException();
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
    private readonly DaoTypeContainer daoTypeContainer;

    public EntityServices(DaoTypeContainer daoTypeContainer) {
        this.daoTypeContainer = daoTypeContainer;
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
        return daoTypeContainer.GetDao<T>();
    }
}
