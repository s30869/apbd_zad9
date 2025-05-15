using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Zadanie9.Model;
using Zadanie9.Model.Requests;

namespace Zadanie9.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;

    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int> AddProductToWarehouse(AddProductRequest request)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
         
            var checkProductCmd = new SqlCommand("SELECT 1 FROM Product WHERE IdProduct = @IdProduct", connection, transaction);
            checkProductCmd.Parameters.AddWithValue("@IdProduct", request.ProductId);
            var productExists = await checkProductCmd.ExecuteScalarAsync();
            if (productExists == null)
                throw new Exception("Product does not exist");

      
            var checkWarehouseCmd = new SqlCommand("SELECT 1 FROM Warehouse WHERE IdWarehouse = @IdWarehouse", connection, transaction);
            checkWarehouseCmd.Parameters.AddWithValue("@IdWarehouse", request.WarehouseId);
            var warehouseExists = await checkWarehouseCmd.ExecuteScalarAsync();
            if (warehouseExists == null)
                throw new Exception("Warehouse does not exist");

   
            var orderCmd = new SqlCommand(@"
                SELECT TOP 1 * FROM [Order]
                WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt", connection, transaction);
            orderCmd.Parameters.AddWithValue("@IdProduct", request.ProductId);
            orderCmd.Parameters.AddWithValue("@Amount", request.Amount);
            orderCmd.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);

            Order? order = null;
            using (var reader = await orderCmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    order = new Order
                    {
                        Id = (int)reader["IdOrder"],
                        IdProduct = (int)reader["IdProduct"],
                        Amount = (int)reader["Amount"],
                        CreatedAt = (DateTime)reader["CreatedAt"],
                        FulfilledAt = reader["FulfilledAt"] as DateTime?
                    };
                }
            }

            if (order == null)
                throw new Exception("Matching order not found");

       
            var fulfilledCheckCmd = new SqlCommand("SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder", connection, transaction);
            fulfilledCheckCmd.Parameters.AddWithValue("@IdOrder", order.Id);
            var fulfilled = await fulfilledCheckCmd.ExecuteScalarAsync();
            if (fulfilled != null)
                throw new Exception("Order already fulfilled");

     
            var updateOrderCmd = new SqlCommand("UPDATE [Order] SET FulfilledAt = @Now WHERE IdOrder = @IdOrder", connection, transaction);
            updateOrderCmd.Parameters.AddWithValue("@Now", DateTime.Now);
            updateOrderCmd.Parameters.AddWithValue("@IdOrder", order.Id);
            await updateOrderCmd.ExecuteNonQueryAsync();

            var priceCmd = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @IdProduct", connection, transaction);
            priceCmd.Parameters.AddWithValue("@IdProduct", request.ProductId);
            var price = (decimal)(await priceCmd.ExecuteScalarAsync())!;

  
            var insertCmd = new SqlCommand(@"
                INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt);
                SELECT SCOPE_IDENTITY();", connection, transaction);
            insertCmd.Parameters.AddWithValue("@IdWarehouse", request.WarehouseId);
            insertCmd.Parameters.AddWithValue("@IdProduct", request.ProductId);
            insertCmd.Parameters.AddWithValue("@IdOrder", order.Id);
            insertCmd.Parameters.AddWithValue("@Amount", request.Amount);
            insertCmd.Parameters.AddWithValue("@Price", price * request.Amount);
            insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

            var newId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync());

            await transaction.CommitAsync();
            return newId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int> AddProductToWarehouseUsingProcedure(AddProductRequest request)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var cmd = new SqlCommand("AddProductToWarehouse", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@IdProduct", request.ProductId);
        cmd.Parameters.AddWithValue("@IdWarehouse", request.WarehouseId);
        cmd.Parameters.AddWithValue("@Amount", request.Amount);
        cmd.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
}