/*
    File: SeriesInfoDao.cs
    Author: Griffin Beaudreau
    Date: October 31, 2023
*/

using WebApp.Models.UnitOfWork;

namespace WebApp.Data;

public class SeriesInfoDao : IDao<SeriesInfo> {
    private readonly CommandHandler commandHandler;

    public SeriesInfoDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    public SeriesInfo Get(int id) {
        throw new NotSupportedException();
    }

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

    public void Add(SeriesInfo t) {
        string sattement = (
            $"INSERT INTO series_info (name, series_image_url, multi_selectable, is_product, is_customization, is_hidden) " +
            $"VALUES ({t.Name}, {t.ImgUrl}, {t.MultiSelectable}, {t.IsProduct}, {t.IsCustomization}, {t.IsHidden})"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

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

    public void Delete(SeriesInfo t) {
        string statement = $"DELETE FROM series_info WHERE name = {t.Name}";
        commandHandler.ExecuteNonQuery(statement);
    }
}