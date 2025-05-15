using Zadanie9.Model.Requests;

namespace Zadanie9.Services;

public interface IDbService
{
    Task<int> AddProductToWarehouse(AddProductRequest request);
    Task<int> AddProductToWarehouseUsingProcedure(AddProductRequest request);
}