/*
    File: SeriesInfoDao.cs
    Author: Griffin Beaudreau
    Date: October 31, 2023
*/

using WebApp.Models;

namespace WebApp.Data;

public class SeriesInfoDao : IDao<SeriesInfo> {
    private readonly CommandHandler commandHandler;

    public SeriesInfoDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    public SeriesInfo Get(int id) {
        throw new NotSupportedException();
    }

    public IEnumerable<SeriesInfo> GetAll() {
        var query = "SELECT * FROM series_info";
        var reader = commandHandler.ExecuteReader(query);

        List<SeriesInfo> seriesInfo = new();

        while (reader?.Read() == true) {
            seriesInfo.Add(new SeriesInfo(
                reader.GetString(0),
                reader.GetString(1)
            ));
        }

        reader?.Close();
        return seriesInfo;
    }

    public void Add(SeriesInfo t) {
        string sattement = (
            $"INSERT INTO series_info (name, series_image_url) " +
            $"VALUES ({t.Name}, {t.ImgUrl})"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

    public void Update(SeriesInfo t, SeriesInfo newT) {
        string statement = (
            $"DELETE FROM series_info WHERE name = {t.Name}"
        );
        
        commandHandler.ExecuteNonQuery(statement);
    }

    public void Delete(SeriesInfo t) {
        string statement = $"DELETE FROM series_info WHERE name = {t.Name}";
        commandHandler.ExecuteNonQuery(statement);
    }
}