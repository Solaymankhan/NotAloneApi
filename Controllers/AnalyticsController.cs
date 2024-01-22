using NotAlone.Data;
using NotAlone.Models;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin" )]
    public class AnalyticsController : ControllerBase
    {
        private ApplicationDbContext _context;

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPut("updateAnalytics")]
        public async Task<ActionResult> updateAnalytics([FromForm] AnalyticsModel analytics_info)
        {
            try
            {
                var analytics = await _context.analytics.Where(val=>val.user_id== analytics_info.user_id).FirstOrDefaultAsync();
                if (analytics != null)
                {

                    analytics.is_active = false;
                    analytics.analytics_user_ids = analytics_info.analytics_user_ids;
                    analytics.analytics_group_ids = analytics_info.analytics_group_ids;
                    await _context.SaveChangesAsync();
                    return Ok("Analytics is Updated");
                }
                else
                {
                    return NotFound("Analytics doesn't Exist");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
          
        }

        [HttpGet("getAnalytics/{userId}")]
        public async Task<ActionResult> getAnalytics(int userId)
        {

            try
            {
                var searched_analytics = await _context.analytics
                               .Where(u => u.user_id == userId).FirstOrDefaultAsync();
                _context.analytics.Where(val => val.user_id == userId)
                    .ExecuteUpdate(user => user.SetProperty(user => user.is_active, true));

                return Ok(searched_analytics);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
