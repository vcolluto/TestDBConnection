using System;
using System.Data.SqlClient;

const string connectionString = 
    "Data Source=localhost;Initial Catalog=Northwind;Integrated Security=True";

string userName, password;

Console.Write("Inserisci il tuo username: ");
userName = Console.ReadLine();

Console.Write("Inserisci la tua password: ");
password = Console.ReadLine();

if (loginSenzaInjection(userName,password))
{    
    Console.WriteLine($"Numero prodotti disponibili: {NumeroProdotti()}");
    //if (inserisciProdotto("Birra", false) == 1)
    //    Console.WriteLine("Prodotto correttamente inserito");
    //else
    //    Console.WriteLine("Prodotto non inserito!");

    //prima dell'ordine abbiamo 13 unità in stock del prodotto 3
    InserisciOrdine(3, "ANTON", 4);
}


static void loginConInjection(string userName, string password)
{
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
            using (SqlCommand cmd = new SqlCommand(sqlQuery, connessioneSql))
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
    }
}

    static bool loginSenzaInjection(string userName, string password)
    {
        bool res = false;
        // istanzio la risorsa nello using
        using (SqlConnection connessioneSql = new SqlConnection(connectionString))
        {
            // da qui in poi posso usare la risorsa 
            try
            {
                connessioneSql.Open();
                // Console.WriteLine("Connessione effettuata!");

                string sqlQuery = $"SELECT FirstName, LastName FROM Employees " +
                                    $"WHERE UserName=@UserName AND password=@password";       //non ho usato la concatenazione di stringhe
                                                                                                    
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connessioneSql))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())  //ho trovato una riga     
                        {
                            res = true;
                            Console.WriteLine($"Benvenuto {reader["FirstName"]} {reader["LastName"]}");
                            Console.WriteLine("Esegui operazioni riservate!!!");
                        }

                        else
                            Console.WriteLine("Username o password errati!");
                    }
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        return res;
    }

static int inserisciProdotto(string productName, bool discontinued)
{
    int res = 0;
    // istanzio la risorsa nello using
    using (SqlConnection connessioneSql = new SqlConnection(connectionString))
    {
        // da qui in poi posso usare la risorsa 
        try
        {
            connessioneSql.Open();
            // Console.WriteLine("Connessione effettuata!");

            string sqlQuery =
                "INSERT INTO Products(ProductName, Discontinued) " +
                "VALUES(@ProductName, @Discontinued)";

            using (SqlCommand cmd = new SqlCommand(sqlQuery, connessioneSql))
            {
                cmd.Parameters.AddWithValue("@ProductName", productName);
                cmd.Parameters.AddWithValue("@Discontinued", discontinued);
                res = cmd.ExecuteNonQuery();

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
    return res;
}


static int ModificaTabellaProdotto()
{
    int res = 0;
    // istanzio la risorsa nello using
    using (SqlConnection connessioneSql = new SqlConnection(connectionString))
    {
        // da qui in poi posso usare la risorsa 
        try
        {
            connessioneSql.Open();
            // Console.WriteLine("Connessione effettuata!");

            string sqlQuery =
                "ALTER TABLE Products ADD Valutazione int";

            using (SqlCommand cmd = new SqlCommand(sqlQuery, connessioneSql))
            {
                
                res = cmd.ExecuteNonQuery();

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
    return res;
}



static int NumeroProdotti()
{
    int res = 0;
    // istanzio la risorsa nello using
    using (SqlConnection connessioneSql = new SqlConnection(connectionString))
    {
        // da qui in poi posso usare la risorsa 
        try
        {
            connessioneSql.Open();
            // Console.WriteLine("Connessione effettuata!");

            string sqlQuery =
                "SELECT COUNT(ProductID) FROM Products";

            using (SqlCommand cmd = new SqlCommand(sqlQuery, connessioneSql))
            {

                res =(int) cmd.ExecuteScalar();

                //OPPURE: 

                /*
                using (SqlDataReader reader = cmd.ExecuteReader()) 
                {
                    if (reader.Read())
                        res=reader.GetInt32(0); 
                }
                */
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
    return res;
}


static int InserisciOrdine(int ProductId, string CustomerId, int Quantity)
{
    int res = 0;
    // istanzio la risorsa nello using
    using (SqlConnection connessioneSql = new SqlConnection(connectionString))
    {
        // da qui in poi posso usare la risorsa 
        try
        {
            connessioneSql.Open();
            
            using (SqlCommand cmd = new SqlCommand())
            {
                using (SqlTransaction transaction = connessioneSql.BeginTransaction("Inserimento ordine"))
                {
                    cmd.Connection = connessioneSql;
                    cmd.Transaction = transaction;
                    try
                    {
                        //INSERISCO UNA RIGA DI ORDINE
                        cmd.CommandText =
                            "INSERT INTO Orders(CustomerId, OrderDate) " +
                            "VALUES(@CustomerId, GETDATE())";       //data recuperata dal server
                        cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                        int nr=cmd.ExecuteNonQuery();


                        //Recupero l'id dell'ordine inserito (autogenerato)
                        cmd.CommandText =
                            "select SCOPE_IDENTITY()";
                        int OrderId = (int) cmd.ExecuteScalar();

                        //INSERISCO UNA RIGA DI DETTAGLIO ORDINE
                        cmd.CommandText =
                            "INSERT INTO [Order Details](OrderID, ProductID, UnitPrice, Quantity, Discount) " +
                            "VALUES(@OrderID, @ProductID, 0, @Quantity, 0)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@OrderID", OrderId);
                        cmd.Parameters.AddWithValue("@ProductID", ProductId);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.ExecuteNonQuery();

                        //AGGIORNO LA GIACENZA SULLA TABELLA PRODOTTI
                        cmd.CommandText = "UPDATE Products SET UnitsInStock=UnitsInStock-@Quantity " +
                                        "WHERE ProductID=@ProductID";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@ProductID", ProductId);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        transaction.Rollback();
                    }
                }

                
            }

        }//
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
    return res;
}
