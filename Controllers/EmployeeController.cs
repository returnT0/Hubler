﻿using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hubler.DAL.Interfaces;
using Hubler.DAL.Models;
using Hubler.Models;

namespace Hubler.Controllers;


[Route("api/employee")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeDAL _employeeDAL;
    private readonly ISupermarketDAL _supermarketDAL;
    private readonly ILkRoleDAL _lkRoleDAL;

    public EmployeeController(IEmployeeDAL employeeDAL, ISupermarketDAL supermarketDAL, ILkRoleDAL lkRoleDAL)
    {
        _employeeDAL = employeeDAL;
        _supermarketDAL = supermarketDAL;
        _lkRoleDAL = lkRoleDAL;
    }

    // GET: api/employee/list
    [HttpGet("list"), Authorize]
    public IActionResult GetAll()
    {
        var userId = this.User.Claims.First(i => i.Type.Equals(ClaimTypes.NameIdentifier)).Value;
        int idRegistered = int.Parse(userId);
        var roleRegistered = this.User.Claims.First(i => i.Type.Equals(ClaimTypes.Role)).Value;
        
        IEnumerable<Employee> employees;

        if (roleRegistered == "admin")
        {
            employees = _employeeDAL.GetAll();
        }
        else if (roleRegistered == "manager")
        {
            var manager = _employeeDAL.GetById(idRegistered);
            employees = _employeeDAL.GetAll()
                .Where(e => e.SupermarketId == manager.SupermarketId);
        }
        else
        {
            return BadRequest("You do not have permission to view this resource.");
        }

        var employeeModels = new List<EmployeeModel>();

        foreach (var employee in employees)
        {
            var supermarket = _supermarketDAL.GetById(employee.SupermarketId);
            var role = _lkRoleDAL.GetById(employee.RoleId);
            var admin = _employeeDAL.GetById(employee.Admin_Id);
            
            if (supermarket != null && role != null)
            {
                employeeModels.Add(new EmployeeModel
                {
                    Id = employee.Id,
                    Email = employee.Email,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    CreatedDate = employee.CreatedDate,
                    // Supermarket fields
                    SupermarketName = supermarket.Title,
                    // Role fields
                    RoleName = role.RoleName,
                    AdminId = employee.Admin_Id,
                    AdminName = admin.FirstName + " " + admin.LastName
                });
            }
        }
        
        if (!employeeModels.Any())
        {
            return NotFound("No supermarkets found.");
        }
        return Ok(employeeModels);
    }

    // POST: api/employee/insert
    [HttpPost("insert")]
    public IActionResult Insert([FromBody] EmployeeModel employeeModel)
    {
        var supermarket = _supermarketDAL.GetSupermarketByTitle(employeeModel.SupermarketName);
        var role = _lkRoleDAL.GetByRoleName(employeeModel.RoleName);

        if (supermarket == null || role == null)
        {
            return BadRequest("Invalid supermarket or role.");
        }

        var employee = new Employee
        {
            Email = employeeModel.Email,
            PassHash = BCrypt.Net.BCrypt.HashPassword(employeeModel.Password),
            FirstName = employeeModel.FirstName,
            LastName = employeeModel.LastName,
            CreatedDate = DateTime.UtcNow,
            SupermarketId = supermarket.Id,
            RoleId = role.Id
            
        };
        
        if(employeeModel.AdminId != 0)
        {
            employee.Admin_Id = employeeModel.AdminId;
        }

        var result = _employeeDAL.Insert(employee);
        return Ok(new { message = "Employee inserted successfully." });
    }
    
    // GET: api/employee/details/{email}
    [HttpGet("details/{email}")]
    public IActionResult Details(string email)
    {
        var employee = _employeeDAL.GetByEmail(email);
        var supermarket = _supermarketDAL.GetById(employee.SupermarketId);
        var role = _lkRoleDAL.GetById(employee.RoleId);
        
        if (employee == null)
        {
            return NotFound("Employee not found.");
        }

        var employeeModel = new EmployeeModel
        {
            Id = employee.Id,
            Email = employee.Email,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            CreatedDate = employee.CreatedDate,
            // Supermarket fields
            SupermarketName = supermarket.Title,
            // Role fields
            RoleName = role.RoleName,
            // Admin fields
            AdminId = employee.Admin_Id
        };

        return Ok(employeeModel);
    }
    
    [HttpGet]
    public IActionResult GetById(int id) 
    {
        var employee = _employeeDAL.GetById(id);
        var supermarket = _supermarketDAL.GetById(employee.SupermarketId);
        var role = _lkRoleDAL.GetById(employee.RoleId);
        
        if (employee == null)
        {
            return NotFound("Employee not found.");
        }

        var employeeModel = new EmployeeModel
        {
            Id = employee.Id,
            Email = employee.Email,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            CreatedDate = employee.CreatedDate,
            // Supermarket fields
            SupermarketName = supermarket.Title,
            // Role fields
            RoleName = role.RoleName,
            // Admin fields
            AdminId = employee.Admin_Id
        };

        return Ok(employeeModel);
    }

    // PUT: api/employee/edit
    [HttpPost("edit")]
    public IActionResult Edit([FromBody] EmployeeModel employeeModel)
    {
        var employee = _employeeDAL.GetById(employeeModel.Id);
        var supermarket = _supermarketDAL.GetSupermarketByTitle(employeeModel.SupermarketName);
        var role = _lkRoleDAL.GetByRoleName(employeeModel.RoleName);
        
        if (employee == null)
        {
            return NotFound("Employee not found.");
        }

        employee.FirstName = employeeModel.FirstName;
        employee.LastName = employeeModel.LastName;
        employee.Email = employeeModel.Email;
        employee.SupermarketId = supermarket.Id;
        employee.RoleId = role.Id;
        employee.Admin_Id = employeeModel.AdminId;
       

        if (employeeModel.Password != null)
        {
            employee.PassHash = BCrypt.Net.BCrypt.HashPassword(employeeModel.Password);
        }
        
        _employeeDAL.Update(employee);
        return Ok(new { message = "Employee updated successfully." });
    }

    // DELETE: api/employee
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        _employeeDAL.Delete(id);
    }
    
    // GET: api/employee/managers
    [HttpGet("managers")]
    public IActionResult GetManagers()
    {
        var employees = _employeeDAL.GetAll();
        var employeeModels = new List<EmployeeModel>();

        foreach (var employee in employees)
        {
            var supermarket = _supermarketDAL.GetById(employee.SupermarketId);
            var role = _lkRoleDAL.GetById(employee.RoleId);

            if (supermarket != null && role != null && role.RoleName.Equals("manager"))
            {
                employeeModels.Add(new EmployeeModel
                {
                    
                    Id = employee.Id,
                    Email = employee.Email,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    CreatedDate = employee.CreatedDate,
                    // Supermarket fields
                    SupermarketName = supermarket.Title,
                    // Role fields
                    RoleName = role.RoleName,
                    // Admin fields
                    AdminId = employee.Admin_Id
                });
            }
        }
        
        if (!employeeModels.Any())
        {
            return NotFound("No managers found.");
        }
        return Ok(employeeModels);
    }
    
    [HttpGet("titles"), Authorize]
    public IActionResult GetSupermarketTitles()
    {
        string roleRegistered = this.User.Claims.First(i => i.Type.Equals(ClaimTypes.Role)).Value;
        if(roleRegistered == "admin")
        {
            var result = _supermarketDAL.GetAllTitles();
            return Ok(result);
        }
        else if(roleRegistered == "manager")
        {
            var userId = this.User.Claims.First(i => i.Type.Equals(ClaimTypes.NameIdentifier)).Value;
            int idRegistered = int.Parse(userId);
            var manager = _employeeDAL.GetById(idRegistered);
            var supermarket = _supermarketDAL.GetById(manager.SupermarketId);
            var result = new List<string>();
            result.Add(supermarket.Title);
            return Ok(result);
        }
        else
        {
            return BadRequest("You do not have permission to view this resource.");
        }
    }
    
    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
            var result = _lkRoleDAL.GetAll();
            IEnumerable<string> roles = result.Select(r => r.RoleName);
            return Ok(roles);
    }
}