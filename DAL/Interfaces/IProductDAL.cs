using Hubler.DAL.Models;

namespace Hubler.DAL.Interfaces;

public interface IProductDAL
{
    Product GetById(int id);
    int Insert(ProductNew item);
    void Update(Product item);
    void Delete(int id);
    IEnumerable<Product> GetAll();
    IEnumerable<ProductInInventory> GetProductsBySupermarket(int supermarketId);
}