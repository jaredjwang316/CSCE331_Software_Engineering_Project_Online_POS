/*
    File: UnitOfWork.cs
    Author: Griffin Beaudreau
    Date: November 5, 2023
*/
using Npgsql;
using WebApp.Models.UnitOfWork;

namespace WebApp.Data;

/// <summary>
/// Represents the Unit of Work pattern for managing database operations.
/// </summary>
public class UnitOfWork {
    private readonly ConnectionHandler connectionHandler;
    private readonly CommandHandler commandHandler;
    private readonly DaoTypeContainer daoTypeContainer;
    private readonly EntityServices entityServices;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="databaseName">The name of the database to connect to.</param>
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

    /// <summary>
    /// Adds an entity to the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="t">The entity to be added.</param>
    public void Add<T>(T t) where T : class {
        entityServices.Add(t);
    }

    /// <summary>
    /// Updates an entity in the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="t">The original entity.</param>
    /// <param name="newT">The new entity data.</param>
    public void Update<T>(T t, T newT) where T : class {
        entityServices.Update(t, newT);
    }

    /// <summary>
    /// Deletes an entity from the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="t">The entity to be deleted.</param>
    public void Delete<T>(T t) where T : class {
        entityServices.Delete(t);
    }

    /// <summary>
    /// Retrieves an entity from the database by its ID.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The retrieved entity.</returns>
    public T Get<T>(int id) where T : class {
        return entityServices.Get<T>(id);
    }

    /// <summary>
    /// Retrieves all entities of a specified type from the database.
    /// </summary>
    /// <typeparam name="T">The type of the entities.</typeparam>
    /// <returns>A collection of entities.</returns>
    public IEnumerable<T> GetAll<T>() where T : class {
        return entityServices.GetAll<T>();
    }

    // Use connection handler to close the connection to the database
    /// <summary>
    /// Closes the connection to the database.
    /// </summary>
    public void CloseConnection() {
        connectionHandler.CloseConnection();
    }

    //====================================================================================================
    // Custom Queries for the database
    //====================================================================================================

    /// <summary>
    /// Retrieves a collection of products based on the specified series.
    /// </summary>
    /// <param name="series">The series to filter products by.</param>
    /// <returns>A collection of products belonging to the specified series.</returns>
    public IEnumerable<Product> GetProductsBySeries(string series) {
        return daoTypeContainer.GetProductDao().GetProductsBySeries(series);
    }

    /// <summary>
    /// Retrieves a collection of unique series based on optional filters.
    /// </summary>
    /// <param name="includeDrinks">Include drinks in the result.</param>
    /// <param name="includeHidden">Include hidden products in the result.</param>
    /// <param name="includeIsOption">Include option products in the result.</param>
    /// <returns>A collection of unique series based on the specified filters.</returns>
    public IEnumerable<string> GetUniqueSeries(
        bool includeDrinks = true,
        bool includeHidden = false,
        bool includeIsOption = false
    ) {
        return daoTypeContainer.GetProductDao().GetUniqueSeries(includeDrinks, includeHidden, includeIsOption);
    }

    /// <summary>
    /// Retrieves a collection of best-selling products up to the specified limit.
    /// </summary>
    /// <param name="limit">The maximum number of best-selling products to retrieve.</param>
    /// <returns>A collection of best-selling products.</returns>
    public IEnumerable<Product> GetBestSellingProducts(int limit) {
        return daoTypeContainer.GetProductDao().GetBestSellingProducts(limit);
    }

    public Product GetRecentProduct() {
        return daoTypeContainer.GetProductDao().GetRecentProduct();
    }

    /// <summary>
    /// Retrieves series information based on the specified series name.
    /// </summary>
    /// <param name="name">The name of the series to retrieve information for.</param>
    /// <returns>Series information for the specified series name.</returns>
    public SeriesInfo GetSeriesInfo(string name) {
        return daoTypeContainer.GetSeriesInfoDao().Get(name);
    }

    /// <summary>
    /// Retrieves a user based on the specified email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>The user associated with the specified email address, or null if not found.</returns>
    public User? GetUser(string email) {
        return daoTypeContainer.GetUserDao().Get(email);
    }

    public Ingredient GetRecentIngredient() {
        return daoTypeContainer.GetIngredientDao().GetRecentIngredient();
    }

    public Inventory GetRecentInventory() {
        return daoTypeContainer.GetInventoryDao().GetRecentInventory();
    }

    public List<Order> GetOrdersBetween(DateTime starttime, DateTime endtime) {
        return daoTypeContainer.GetOrderDao().GetOrdersBetween(starttime, endtime);
    }

    public List<Tuple<string, string,int>> GetSalesTogether(DateTime starttime, DateTime endtime) {
        return daoTypeContainer.GetOrderDao().GetSalesTogether(starttime, endtime);
    }

    public List<Tuple<int, double>> GetSalesReport(DateTime starttime, DateTime endtime){
        return daoTypeContainer.GetOrderDao().GetSalesReport(starttime, endtime);
    }

}

/// <summary>
/// Represents a container for accessing various DAOs.
/// </summary>
public class DaoTypeContainer {
    private readonly Dictionary<Type, object> daoTypeMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="DaoTypeContainer"/> class with a given DAO map.
    /// </summary>
    /// <param name="daoTypeMap">The map containing DAO instances for different types.</param>
    public DaoTypeContainer(Dictionary<Type, object> daoTypeMap) {
        this.daoTypeMap = daoTypeMap;
    }

    /// <summary>
    /// Gets the DAO instance for a specified type.
    /// </summary>
    /// <typeparam name="T">The type of the DAO.</typeparam>
    /// <returns>The DAO instance for the specified type.</returns>
    /// <exception cref="InvalidCastException">Thrown if the DAO instance is not of the expected type.</exception>
    public IDao<T> GetDao<T>() where T : class {
        if (daoTypeMap.TryGetValue(typeof(T), out object? dao)) {
            return dao as IDao<T> ?? throw new InvalidCastException();
        } else {
            throw new InvalidCastException();
        }
    }

    // Unique getters for each dao
    
    /// <summary>
    /// Gets the DAO instance for the <see cref="Employee"/> type.
    /// </summary>
    /// <returns>The DAO instance for the <see cref="Employee"/> type.</returns>
    /// <exception cref="InvalidCastException">Thrown if the DAO instance is not of the expected type.</exception>
    public EmployeeDao GetEmployeeDao() {
        return daoTypeMap[typeof(Employee)] as EmployeeDao ?? throw new InvalidCastException();
    }

    /// <summary>
    /// Gets the DAO instance for the <see cref="Ingredient"/> type.
    /// </summary>
    /// <returns>The DAO instance for the <see cref="Ingredient"/> type.</returns>
    /// <exception cref="InvalidCastException">Thrown if the DAO instance is not of the expected type.</exception>
    public IngredientDao GetIngredientDao() {
        return daoTypeMap[typeof(Ingredient)] as IngredientDao ?? throw new InvalidCastException();
    }
    
    /// <summary>
    /// Gets the DAO instance for the <see cref="Inventory"/> type.
    /// </summary>
    /// <returns>The DAO instance for the <see cref="Inventory"/> type.</returns>
    /// <exception cref="InvalidCastException">Thrown if the DAO instance is not of the expected type.</exception>
    public InventoryDao GetInventoryDao() {
        return daoTypeMap[typeof(Inventory)] as InventoryDao ?? throw new InvalidCastException();
    }
    
    /// <summary>
    /// Gets the DAO instance for the <see cref="Order"/> type.
    /// </summary>
    /// <returns>The DAO instance for the <see cref="Order"/> type.</returns>
    /// <exception cref="InvalidCastException">Thrown if the DAO instance is not of the expected type.</exception>
    public OrderDao GetOrderDao() {
        return daoTypeMap[typeof(Order)] as OrderDao ?? throw new InvalidCastException();
    }
    
    /// <summary>
    /// Gets the DAO instance for the <see cref="Product"/> type.
    /// </summary>
    /// <returns>The DAO instance for the <see cref="Product"/> type.</returns>
    /// <exception cref="InvalidCastException">Thrown if the DAO instance is not of the expected type.</exception>
    public ProductDao GetProductDao() {
        return daoTypeMap[typeof(Product)] as ProductDao ?? throw new InvalidCastException();
    }
    
    /// <summary>
    /// Gets the DAO instance for the <see cref="ProductIngredients"/> type.
    /// </summary>
    /// <returns>The DAO instance for the <see cref="ProductIngredients"/> type.</returns>
    /// <exception cref="InvalidCastException">Thrown if the DAO instance is not of the expected type.</exception>
    public ProductIngredientsDao GetProductIngredientsDao() {
        return daoTypeMap[typeof(ProductIngredients)] as ProductIngredientsDao ?? throw new InvalidCastException();
    }
    
    /// <summary>
    /// Gets the DAO instance for the <see cref="SeriesInfo"/> type.
    /// </summary>
    /// <returns>The DAO instance for the <see cref="SeriesInfo"/> type.</returns>
    /// <exception cref="InvalidCastException">Thrown if the DAO instance is not of the expected type.</exception>
    public SeriesInfoDao GetSeriesInfoDao() {
        return daoTypeMap[typeof(SeriesInfo)] as SeriesInfoDao ?? throw new InvalidCastException();
    }
    
    /// <summary>
    /// Gets the DAO instance for the <see cref="User"/> type.
    /// </summary>
    /// <returns>The DAO instance for the <see cref="User"/> type.</returns>
    /// <exception cref="InvalidCastException">Thrown if the DAO instance is not of the expected type.</exception>
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

/// <summary>
/// Provides services for basic CRUD (Create, Read, Update, Delete) operations on entities.
/// </summary>
public class EntityServices {
    private readonly DaoTypeContainer daoTypeContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityServices"/> class with a specified <see cref="DaoTypeContainer"/>.
    /// </summary>
    /// <param name="daoTypeContainer">The container providing access to Data Access Objects (DAOs).</param>
    public EntityServices(DaoTypeContainer daoTypeContainer) {
        this.daoTypeContainer = daoTypeContainer;
    }

    /// <summary>
    /// Adds a new entity of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="t">The entity to be added.</param>
    public void Add<T>(T t) where T : class {
        IDao<T> dao = GetDao<T>();
        dao.Add(t);
    }

    /// <summary>
    /// Updates an existing entity of type <typeparamref name="T"/> with a new entity.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="t">The existing entity to be updated.</param>
    /// <param name="newT">The new entity data.</param>
    public void Update<T> (T t, T newT) where T : class {
        IDao<T> dao = GetDao<T>();
        dao.Update(t, newT);
    }

    /// <summary>
    /// Deletes an entity of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="t">The entity to be deleted.</param>
    public void Delete<T>(T t) where T : class {
        IDao<T> dao = GetDao<T>();
        dao.Delete(t);
    }

    /// <summary>
    /// Retrieves an entity of type <typeparamref name="T"/> by its unique identifier.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity with the specified identifier, or null if not found.</returns>
    public T Get<T>(int id) where T : class {
        IDao<T> dao = GetDao<T>();
        return dao.Get(id);
    }

    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <returns>An enumerable collection of entities of the specified type.</returns>
    public IEnumerable<T> GetAll<T>() where T : class {
        IDao<T> dao = GetDao<T>();
        return dao.GetAll();
    }

    /// <summary>
    /// Retrieves the appropriate DAO (Data Access Object) for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <returns>The DAO for the specified entity type.</returns>
    private IDao<T> GetDao<T>() where T : class {
        return daoTypeContainer.GetDao<T>();
    }
}
