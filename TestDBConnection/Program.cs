using System.Data.SqlClient;

const string connectionString = 
    "Data Source=localhost;Initial Catalog=Northwind;Integrated Security=True";

string userName, password;

Console.Write("Inserisci il tuo username: ");
userName = Console.ReadLine();

Console.Write("Inserisci la tua password: ");
password = Console.ReadLine();


// istanzio la risorsa nello using
using (SqlConnection connessioneSql = new SqlConnection(connectionString))
{
    // da qui in poi posso usare la risorsa 
    try
    {
        connessioneSql.Open();
       // Console.WriteLine("Connessione effettuata!");

        string sqlQuery = $"SELECT FirstName, LastName FROM Employees " +
                            $"WHERE UserName='{userName}' AND password='{password}'";       //DA EVITARE COME LA PESTE!
        // ad esempio, provare ad inserire come password: ' or 1=1 --
        using (SqlCommand cmd=new SqlCommand(sqlQuery, connessioneSql))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            if (reader.Read())  //ho trovato una riga     
            {
                Console.WriteLine($"Benvenuto {reader["FirstName"]} {reader["LastName"]}");
                Console.WriteLine("Esegui operazioni riservate!!!");
            }

            else
                Console.WriteLine("Username o password errati!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
    // non serve usare finally per chiudere la risorsa
}
