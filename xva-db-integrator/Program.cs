
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

string connectionString = "Data Source=YourServer;Initial Catalog=Library;Integrated Security=True;";

var batch = new Dictionary<string, EventData>();
var producerClient = DependencyFactory.CreateEventHubsProducerClient(configuration);


using SqlConnection connection = new SqlConnection(connectionString);

try
{
    connection.Open();

    string query = "SELECT * FROM Schedule WHERE status = 'Pending'";

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
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    if (connection.State == ConnectionState.Open)
    {
        connection.Close();
    }
}


await producerClient.SendAsync(batch.Values);

Console.WriteLine($"Total of {batch.Count} Triggered on Event Hubs");

int rowsAffected = 0;

try
{
    connection.Open();
    var idsToUpdate = batch.Keys.ToList();

    string query = "UP";
    string updateQuery = "UPDATE Schedule SET status = 'Running' WHERE id IN ({0})";
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
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    if (connection.State == ConnectionState.Open)
    {
        connection.Close();
    }
}


Console.WriteLine($"Total of {rowsAffected} updated on DataBase");