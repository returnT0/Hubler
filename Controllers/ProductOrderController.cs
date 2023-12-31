﻿using System.Security.Claims;
using Hubler.DAL.Interfaces;
using Hubler.DAL.Models;
using Hubler.Models;
using Hubler.ProductManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hubler.Controllers;

[Route("api/product_order")]
[ApiController]
public class ProductOrderController : ControllerBase
{
    private readonly ISupermarketDAL _supermarketDAL;
    private readonly IProductOrderDAL _productOrderDAL;
    private readonly IProductDAL _productDAL;
    private readonly IEmployeeDAL _employeeDAL;
    private readonly ILkProductDAL _lkProductDAL;
    private readonly IPerishableDAL _perishableDAL;
    private readonly INonPerishableDAL _nonPerishableDAL;
    private readonly IWarehouseDAL _warehouseDAL;
    
    public ProductOrderController(ISupermarketDAL supermarketDAL, 
        IProductOrderDAL productOrderDAL, 
        IProductDAL productDAL, 
        IEmployeeDAL employeeDAL, 
        ILkProductDAL lkProductDAL,
        IPerishableDAL perishableDAL,
        INonPerishableDAL nonPerishableDAL,
        IWarehouseDAL warehouseDAL)
    {
        _supermarketDAL = supermarketDAL;
        _productOrderDAL = productOrderDAL;
        _productDAL = productDAL;
        _employeeDAL = employeeDAL;
        _lkProductDAL = lkProductDAL;
        _perishableDAL = perishableDAL;
        _nonPerishableDAL = nonPerishableDAL;
        _warehouseDAL = warehouseDAL;
    }

    [HttpGet("list"), Authorize]
    public ActionResult<List<ProductOrderModel>> GetAll()
    {
        var productOrders = _productOrderDAL.GetAll();
        var productOrderModels = new List<ProductOrderModel>();

        var id = int.Parse(this.User.Claims.First(i => i.Type.Equals(ClaimTypes.NameIdentifier)).Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role == "admin")
        {
            foreach (var productOrder in productOrders)
            {
                var product = _lkProductDAL.GetById(productOrder.ProductId);
                var supermarket = _supermarketDAL.GetById(productOrder.SupermarketId);
                if (product != null && supermarket != null)
                {
                    productOrderModels.Add(new ProductOrderModel
                    {
                        Id = productOrder.Id,
                        SupermarketName = supermarket.Title,
                        ProductName = product.Title,
                        Quantity = productOrder.OrderedQuantity,
                        OrderDate = productOrder.OrderDate
                    });
                }
            }
        }
        else if (role == "manager")
        {
            var manager = _employeeDAL.GetById(id);
            var managerSupermarket = _supermarketDAL.GetById(manager.SupermarketId);
            foreach (var productOrder in productOrders)
            {
                var product = _lkProductDAL.GetById(productOrder.ProductId);
                var supermarket = _supermarketDAL.GetById(productOrder.SupermarketId);
                if (product != null && supermarket != null && supermarket.Id == managerSupermarket.Id)
                {
                    productOrderModels.Add(new ProductOrderModel
                    {
                        Id = productOrder.Id,
                        SupermarketName = supermarket.Title,
                        ProductName = product.Title,
                        Quantity = productOrder.OrderedQuantity,
                        OrderDate = productOrder.OrderDate
                    });
                }
            }
        }
        else
        {
            BadRequest("You don't have permission to view this page");
        }

        return Ok(productOrderModels);
    }
    
    [HttpPost("insert/{type}"), Authorize]
    public void Post([FromBody] ProductOrderModel model, string type)
    {
        var supermarket = _supermarketDAL.GetSupermarketByTitle(model.SupermarketName);
        if(type == "perishable")
        {
            
            var product = _lkProductDAL.GetByTitle(model.ProductName);
            
            var newProductOrder = new ProductOrder
            {
                SupermarketId = supermarket.Id,
                ProductId = product.Lk_Product_Id,
                OrderedQuantity = model.Quantity,
                OrderDate = DateTime.UtcNow
            };
            
            _productOrderDAL.Insert(newProductOrder);
            
            var newProduct = new ProductNew
            {
                ProductType = "P",
                LkProduct_Id = product.Lk_Product_Id
            };
            
            var productId = _productDAL.Insert(newProduct);
            
            var newPerishableProduct = new Perishable
            {
                ProductId = productId,
                ExpiryDate = model.ExpireDate ?? DateTime.MinValue,
                StorageType = model.StorageType
            };
            
            _perishableDAL.Insert(newPerishableProduct);
            
            var newWarehouse = new Warehouse
            {
                SupermarketId = supermarket.Id,
                ProductId = productId,
                Quantity = model.Quantity
            };
            
            _warehouseDAL.Insert(newWarehouse);
        }
        else if(type == "nonperishable")
        {
            var product = _lkProductDAL.GetByTitle(model.ProductName);
            
            var newProductOrder = new ProductOrder
            {
                SupermarketId = supermarket.Id,
                ProductId = product.Lk_Product_Id,
                OrderedQuantity = model.Quantity,
                OrderDate = DateTime.UtcNow
            };
            
            _productOrderDAL.Insert(newProductOrder);
            
            var newProduct = new ProductNew
            {
                ProductType = "N",
                LkProduct_Id = product.Lk_Product_Id
            };
            
            var productId = _productDAL.Insert(newProduct);
            
            var newNonPerishableProduct = new NonPerishable
            {
                ProductId = productId,
                ShelfLife = model.ShelfLife ?? 0
            };
            
            _nonPerishableDAL.Insert(newNonPerishableProduct);
            
            var newWarehouse = new Warehouse
            {
                SupermarketId = supermarket.Id,
                ProductId = productId,
                Quantity = model.Quantity
            };
            
            _warehouseDAL.Insert(newWarehouse);
        }
        else
        {
            BadRequest("Invalid product type");
        }
    }
    
    [HttpDelete("delete/{id}")]
    public void Delete(int id)
    {
        _productOrderDAL.Delete(id);
    }
    
    [HttpGet("products")]
    public ActionResult getProducts()
    {
        var products = _lkProductDAL.GetAll();
        if(products == null)
        {
            return NotFound();
        }
        return Ok(products);
    }
    
    [HttpGet("titles"), Authorize]
    public IActionResult GetSupermarketTitles()
    {
        var id = int.Parse(this.User.Claims.First(i => i.Type.Equals(ClaimTypes.NameIdentifier)).Value);
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
        
        var supermarketTitles = _supermarketDAL.GetAllTitles();


        switch (role)
        {
            case "admin":
                return Ok(supermarketTitles);
            case "manager":
            {
                var manager = _employeeDAL.GetById(id);
                var supermarket = _supermarketDAL.GetById(manager.SupermarketId);
                supermarketTitles = supermarketTitles.Where(i => i == supermarket.Title);
                return Ok(supermarketTitles);
            }
            default:
                return BadRequest("You don't have permission to view this page.");
        }
    }
}