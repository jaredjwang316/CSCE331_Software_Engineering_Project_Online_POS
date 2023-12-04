/*
    File: SeriesInfoDao.cs
    Author: Griffin Beaudreau
    Date: October 31, 2023
*/

using WebApp.Models.UnitOfWork;

namespace WebApp.Data;

/// <summary>
/// Data Access Object (DAO) for handling operations related to <see cref="SeriesInfo"/>.
/// </summary>
/// <seealso cref="IDao{SeriesInfo}"/>
public class SeriesInfoDao : IDao<SeriesInfo> {
    private readonly CommandHandler commandHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeriesInfoDao"/> class.
    /// </summary>
    /// <param name="commandHandler">The command handler used for database operations.</param>
    public SeriesInfoDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    /// <inheritdoc/>
    public SeriesInfo Get(int id) {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public SeriesInfo Get(string name) {
        name = char.ToUpper(name[0]) + name[1..].ToLower();
        var query = $"SELECT * FROM series_info WHERE name = '{name}'";
        var reader = commandHandler.ExecuteReader(query);

        if (reader == null) {
            return new SeriesInfo("", "", false, false, false, false);
        }

        reader.Read();
        SeriesInfo seriesInfo = new(
            reader.GetString(0),
            reader.GetString(1),
            reader.GetBoolean(2),
            reader.GetBoolean(3),
            reader.GetBoolean(4),
            reader.GetBoolean(5)
        );

        reader?.Close();
        return seriesInfo;
    }

    /// <inheritdoc/>
    public IEnumerable<SeriesInfo> GetAll() {
        var query = "SELECT * FROM series_info";
        var reader = commandHandler.ExecuteReader(query);

        List<SeriesInfo> seriesInfo = new();

        while (reader?.Read() == true) {
            seriesInfo.Add(new SeriesInfo(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetBoolean(2),
                reader.GetBoolean(3),
                reader.GetBoolean(4),
                reader.GetBoolean(5)
            ));
        }

        reader?.Close();
        return seriesInfo;
    }

    /// <inheritdoc/>
    public void Add(SeriesInfo t) {
        string sattement = (
            $"INSERT INTO series_info (name, series_image_url, multi_selectable, is_product, is_customization, is_hidden) " +
            $"VALUES ({t.Name}, {t.ImgUrl}, {t.MultiSelectable}, {t.IsProduct}, {t.IsCustomization}, {t.IsHidden})"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

    /// <inheritdoc/>
    public void Update(SeriesInfo t, SeriesInfo newT) {
        string statement = (
            $"UPDATE series_info SET " +
            $"name = {newT.Name}, " +
            $"series_image_url = {newT.ImgUrl}, " +
            $"multi_selectable = {newT.MultiSelectable}, " +
            $"is_product = {newT.IsProduct}, " +
            $"is_customization = {newT.IsCustomization}, " +
            $"is_hidden = {newT.IsHidden} " +
            $"WHERE name = {t.Name}"
        );
        
        commandHandler.ExecuteNonQuery(statement);
    }

    /// <inheritdoc/>
    public void Delete(SeriesInfo t) {
        string statement = $"DELETE FROM series_info WHERE name = {t.Name}";
        commandHandler.ExecuteNonQuery(statement);
    }
}