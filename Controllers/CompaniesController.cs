using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using newTask.Models;
using newTask.Packages;
using newTask.Validators;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Numerics;

namespace newTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        IPKG_COMPANIES package;

        public CompaniesController(IPKG_COMPANIES package)
        {
            this.package = package;
        }

        [HttpPost("addCompanyAndUser")]
        public IActionResult AddCompanyAndUser([FromBody] CompanyValidator companyAndUser)
        {
            List<string> validationErrors = companyAndUser.Validate();

            if (validationErrors.Count > 0)
            {
                return BadRequest(new { Errors = validationErrors });
            }

            try
            {
                var registration = new Registration
                {
                    Company = new Company
                    {
                        CompanyId = companyAndUser.CompanyId,
                        Name = companyAndUser.Name,
                        TaxCode = companyAndUser.TaxCode,
                        Address = companyAndUser.Address
                    },
                    AdminUser = new User
                    {
                        UserId = companyAndUser.UserId,
                        CompanyId = companyAndUser.CompanyId,
                        Fname = companyAndUser.Fname,
                        Lname = companyAndUser.Lname,
                        Phone = companyAndUser.Phone,
                        Email = companyAndUser.Email,
                        Username = companyAndUser.Username,
                        Password = companyAndUser.Password,
                        IsActive = companyAndUser.IsActive,
                        RoleId = companyAndUser.RoleId,
                        Role = companyAndUser.Role
                    }
                };

                package.AddCompanyAndUser(registration);
                return Ok(registration);
            } 
             catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                return StatusCode(StatusCodes.Status500InternalServerError, "System error, try again: " + ex.Message + " Inner Exception: " + innerMessage);
            }

        }
    


        [HttpGet("getCompanies")]
        public IActionResult GetCompanies()
        {
            List<Registration> companies = package.GetAll();
            try

            {
                if (companies == null || companies.Count == 0)
                {
                    return NotFound("No companies found.");
                }
                return Ok(companies);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                return StatusCode(StatusCodes.Status500InternalServerError, "System error, try again: " + ex.Message + " Inner Exception: " + innerMessage);
            }

        }

        

        [HttpGet("getCompany/{id}")]
        public IActionResult GetCompany(int id)
        {
            try
            {
                Registration company = package.GetCompany(id);
                if (company == null)
                {
                    return NotFound($"Company with Id {id} not found.");
                }
                return Ok(company);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                return StatusCode(StatusCodes.Status500InternalServerError, "System error, try again: " + ex.Message + " Inner Exception: " + innerMessage);
            }
        }

        [HttpDelete("deleteCompany/{id}")]
        public IActionResult DeleteCompany(int id)
        {
            try
            {
                package.DeleteCompany(id);
                return Ok();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                return StatusCode(StatusCodes.Status500InternalServerError, "System error, try again: " + ex.Message + " Inner Exception: " + innerMessage);
            }
        }

     
    }
}
