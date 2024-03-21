
using Azure.Messaging.EventHubs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
            .Build();

string connectionString = configuration.GetConnectionString("sql") ?? throw new Exception("Sql Connection is required");

var batch = new Dictionary<string, EventData>();
var producerClient = DependencyFactory.CreateEventHubsProducerClient(configuration);


using SqlConnection connection = new SqlConnection(connectionString);

try
{
    connection.Open();

    string query = "SELECT * FROM Simulation WHERE status = 'Pending'";

    using SqlCommand command = new SqlCommand(query, connection);
    using SqlDataReader reader = command.ExecuteReader();

    while (reader.Read())
    {
        var se = new StartingEvent(reader["bookname"].ToString() ?? throw new Exception("BookName Could not be null, check the DataBase"), reader["booktype"].ToString(), reader["fileextension"].ToString());
        batch.Add(reader["id"].ToString(), new EventData(BinaryData.FromObjectAsJson(se)));
    }

    reader.Close();

}
catch (Exception ex)
{
    Console.Error.WriteLine($"{DateTime.Now } - Error: {ex.Message}");
}
finally
{
    if (connection.State == ConnectionState.Open)
    {
        connection.Close();
    }
}


await producerClient.SendAsync(batch.Values);

Console.WriteLine($"{DateTime.Now} -  Total of {batch.Count} Triggered on Event Hubs");

int rowsAffected = 0;

try
{
    connection.Open();
    var idsToUpdate = batch.Keys.ToList();

    string query = "UP";
    string updateQuery = "UPDATE Simulation SET status = 'Running' WHERE id IN ({0})";
    string idPlaceholders = string.Join(",", batch.Select((id, index) => $"@Id{index}"));
    updateQuery = string.Format(updateQuery, idPlaceholders);

    using SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

    
    for (int i = 0; i < idsToUpdate.Count; i++)
    {
        updateCommand.Parameters.AddWithValue($"@Id{i}", idsToUpdate[i]);
    }

    rowsAffected = updateCommand.ExecuteNonQuery();

}
catch (Exception ex)
{
    Console.WriteLine($"{DateTime.Now} -  Error: {ex.Message}");
}
finally
{
    if (connection.State == ConnectionState.Open)
    {
        connection.Close();
    }
}


Console.WriteLine($"{DateTime.Now} - Total of {rowsAffected} updated on DataBase");