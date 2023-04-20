using System.Data.SqlClient;

const string connectionString = 
    "Data Source=localhost;Initial Catalog=Northwind;Integrated Security=True";


// istanzio la risorsa nello using
using (SqlConnection connessioneSql = new SqlConnection(connectionString))
{
    // da qui in poi posso usare la risorsa 
    try
    {
        connessioneSql.Open();
        Console.WriteLine("Connessione effettuata!");

        string sqlQuery = "SELECT * FROM Products";
        using (SqlCommand cmd=new SqlCommand(sqlQuery, connessioneSql))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read()) 
            {
                 // int idx = reader.GetOrdinal("ProductName");
                 // Console.WriteLine($"Id: {reader.GetInt32(0)}\tName: {reader.GetString(idx)}");
                 //         OPPURE (in questo caso vengono restituiti degli Object):
                Console.WriteLine($"Id: {reader["ProductID"]}\tName: {reader["ProductName"]}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
    // non serve usare finally per chiudere la risorsa
}
