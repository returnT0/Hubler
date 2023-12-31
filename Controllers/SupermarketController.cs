﻿using System.Security.Claims;
using Hubler.DAL.Interfaces;
using Hubler.DAL.Models;
using Hubler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hubler.Controllers;

    [Route("api/supermarket")]
    [ApiController]
    public class SupermarketController : ControllerBase
    {
        private readonly ISupermarketDAL _supermarketDAL;
        private readonly IAddressDAL _addressDAL;
        private readonly IEmployeeDAL _employeeDAL;

        public SupermarketController(ISupermarketDAL supermarketDAL, IAddressDAL addressDAL, IEmployeeDAL employeeDAL)
        {
            _supermarketDAL = supermarketDAL;
            _addressDAL = addressDAL;
            _employeeDAL = employeeDAL;
        }
        
        [HttpGet("list"), Authorize]
        public ActionResult<List<SupermarketWithAddressModel>> GetAll()
        {
            var supermarkets = _supermarketDAL.GetAll();
            var supermarketWithAddressModels = new List<SupermarketWithAddressModel>();
            
            int id = int.Parse(this.User.Claims.First(i => i.Type.Equals(ClaimTypes.NameIdentifier)).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "admin")
            {
                foreach (var supermarket in supermarkets)
                {
                    var address = _addressDAL.GetById(supermarket.AddressId);
                    if (address != null)
                    {
                        supermarketWithAddressModels.Add(new SupermarketWithAddressModel
                        {
                            SupermarketId = supermarket.Id,
                            Title = supermarket.Title,
                            Phone = supermarket.Phone,
                            // Address fields
                            Street = address.Street,
                            House = address.House,
                            City = address.City,
                            PostalCode = address.PostalCode,
                            Country = address.Country
                        });
                    }
                }
            }
            else if(role == "manager")
            {
                var employee = _employeeDAL.GetById(id);
                var managerSupermarket = _supermarketDAL.GetById(employee.SupermarketId);
                var address = _addressDAL.GetById(managerSupermarket.AddressId);
                
                supermarketWithAddressModels.Add(new SupermarketWithAddressModel
                {
                    SupermarketId = managerSupermarket.Id,
                    Title = managerSupermarket.Title,
                    Phone = managerSupermarket.Phone,
                    // Address fields
                    Street = address.Street,
                    House = address.House,
                    City = address.City,
                    PostalCode = address.PostalCode,
                    Country = address.Country
                });
            }
            else
            {
                return NotFound("You don't have permission to view this page.");
            }
            

            if (!supermarketWithAddressModels.Any())
            {
                return NotFound("No supermarkets found.");
            }
            return Ok(supermarketWithAddressModels);
        }
        
        [HttpPost("insert")]
        public void Post([FromBody] SupermarketWithAddressModel model)
        {
            var newAddress = new Address
            {
                Street = model.Street,
                House = model.House,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country
            };
            
            var addressId = _addressDAL.Insert(newAddress);
            
            var newSupermarket = new Supermarket
            {
                Title = model.Title,
                Phone = model.Phone,
                AddressId = addressId
            };
            _supermarketDAL.Insert(newSupermarket);
        }
        
    [HttpGet("get/{title}")]
    public ActionResult<SupermarketWithAddressModel> GetByTitle(string title)
    {
        var supermarket = _supermarketDAL.GetSupermarketByTitle(title);
        
        if (supermarket == null)
        {
            return NotFound();
        }

        var address = _addressDAL.GetById(supermarket.AddressId);
        if (address == null)
        {
            return NotFound("Address not found.");
        }

        var model = new SupermarketWithAddressModel
        {
            SupermarketId = supermarket.Id,
            Title = supermarket.Title,
            Phone = supermarket.Phone,
            Street = address.Street,
            House = address.House,
            City = address.City,
            PostalCode = address.PostalCode,
            Country = address.Country
        };

        return Ok(model);
    }

    [HttpPost("update")]
    public IActionResult Update([FromBody] SupermarketWithAddressModel model)
    {
        var supermarket = _supermarketDAL.GetById(model.SupermarketId);
        if (supermarket == null)
        {
            return NotFound();
        }

        var updatedAddress = new Address
        {
            Id = supermarket.AddressId,
            Street = model.Street,
            House = model.House,
            City = model.City,
            PostalCode = model.PostalCode,
            Country = model.Country
        };

        _addressDAL.Update(updatedAddress);

        supermarket.Title = model.Title;
        supermarket.Phone = model.Phone;
        _supermarketDAL.Update(supermarket);

        return NoContent();
    }
        
        [HttpDelete("delete")]
        public void Delete(string title)
        {
            _supermarketDAL.Delete(title);
        }
        
    }