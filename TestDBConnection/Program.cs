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

        string sql
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
    // non serve usare finally per chiudere la risorsa
}
