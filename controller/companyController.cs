using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using officeline.Data;   // 'officeline'
using officeline.Models; // 'officeline'
using officeline.DTO;    // 'officeline'
using officeline.Services;
using officeline.ErrorExceptions;
using Microsoft.AspNetCore.Authorization; // 'officeline'
namespace officeline.Controllers // 'officeline'
{



    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyServices _companyServices;

        public CompanyController(ICompanyServices companyServices)
        {
         _companyServices=companyServices;   
        }


        [Authorize(Roles = "superadmin")]
        [HttpPost("create")]
        public async Task<ActionResult<CompanyModel>> CreateCompany([FromBody] CreateCompanyDTO createCompanyDTO)
        {
            if (!ModelState.IsValid)
            {
                BadRequest(new { success = false,message="Invalid input data" ,errors = ModelState });
            }
            try{
            var result=await _companyServices.CreateCompanyAsync(createCompanyDTO);
            return Ok(new {success=true,message="Company created successfully",data= result});
            }
            catch (ApiException ex)
            {
                return BadRequest(new {success=false, errors = new Dictionary<string, string> { { ex.FieldName, ex.Message } }});
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Failed to create company", ex.Message }); 
            }
        }



        [Authorize()]
        [HttpGet("all")]
        public async Task<ActionResult<List<CompaniesListDTO>>> GetAllCompanies()
        {
            try
            {
                var companies = await _companyServices.GetAllCompaniesAsync();
                return Ok(new { success = true, data = companies });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Failed to retrieve companies", ex.Message });
            }
        }





        [Authorize()]
        [HttpPatch("update/{companyId}")]
        public async Task<ActionResult<UpdateCompanyDTO>> UpdateCompany([FromRoute] int companyId,  [FromBody] UpdateCompanyDTO updateCompanyDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors=ModelState.Where(e=>e.Value.Errors.Count>0).ToDictionary(kvp=> kvp.Key,
                kvp=> kvp.Value.Errors.Select(er=>er.ErrorMessage).FirstOrDefault());
                BadRequest(new {success=false, message="Failed to update company",error=errors});
            }

            try
            {
                var result=await _companyServices.UpdateCompanyAsync(companyId,updateCompanyDTO);
                return Ok(new {success=true,message="Company Updated successfully", data=result});
            }catch(ApiException ex)
            {
                return BadRequest(new{success=false, errors=new Dictionary<string,string> {{ex.FieldName,ex.Message}}});
            }catch(Exception ex)
            {
                return BadRequest(new {success=false, message="An unExpected Errors",detail=ex.Message});
            }



        }


    
        [Authorize()]
        [HttpDelete("delete/{companyId}")]
        public async Task<ActionResult<bool>> DeleteCompany([FromRoute] int companyId)
        {

            try{

            var deleted=await _companyServices.deleteCompany(companyId);
            return Ok(new {success=true,message="Company Deleted Successfully."});
            }catch(ApiException ex)
            {
                return NotFound(new {success=false,
                errors=new Dictionary<string, string> {{ex.FieldName,ex.Message}}});
            }catch(Exception ex)
            {
                return StatusCode(500, new {message="Internal Server Error"});
            }
            
        }
    
    
    }


}