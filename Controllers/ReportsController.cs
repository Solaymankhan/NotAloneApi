using NotAlone.Data;
using NotAlone.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class ReportsController : ControllerBase
    {
        private ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("postReports")]
        public async Task<ActionResult> postReports([FromForm] ReportsModel report)
        {

            try
            {
                await _context.reports.AddAsync(report);

                await _context.analytics
                 .Where(cci => cci.user_id == report.reportedTo_user_id)
                 .ExecuteUpdateAsync(cci => cci.SetProperty(cci => cci.total_new_reports, cci => cci.total_new_reports + 1));
                
                await _context.SaveChangesAsync();

                return Ok("Succesfully reported");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        [HttpGet("getReports/{userId}")]
        public async Task<ActionResult> getReports(int userId)
        {
            try
            {
                var reporsts = _context.reports.Where(rep => rep.reportedTo_user_id == userId).ToList();
                return Ok(reporsts);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
    }
}
