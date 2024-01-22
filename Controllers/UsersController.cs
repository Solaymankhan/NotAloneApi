
using NotAlone.Data;
using NotAlone.Models;
using NotAlone.Services.Abstract;
using NotAlone.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        int pageSize = 20;
        private readonly IFileService _fileService;
        private readonly IRegistrationService _RegistrationService;
        private ApplicationDbContext _context;

        public UserController(IFileService fileService, ApplicationDbContext context, IRegistrationService RegistrationService)
        {
            _fileService = fileService;
            _context = context;
            _RegistrationService = RegistrationService;
        }

        [HttpPost("signUpUser")]
        public async Task<ActionResult> signUpUser([FromForm] UserModel user)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var token = await _RegistrationService.signUpUser(user);
                if (token == null) return BadRequest("Failed to Signed Up User");
                await _context.users.AddAsync(user);
                await _context.SaveChangesAsync();
                var userCounting = new UsersCountingInformationModel { user_id=user.user_id};
                var userAnalytics = new AnalyticsModel
                {
                    is_active = true,
                    user_id = user.user_id,
                };

                await _context.analytics.AddAsync(userAnalytics);
                await _context.usersCounting.AddAsync(userCounting);
                await _context.SaveChangesAsync();

                await _context.users.Where(usr => usr.user_id == user.user_id)
                    .ExecuteUpdateAsync(usr => usr.SetProperty(usr => usr.analytics_id, userAnalytics.analytics_id)
                    .SetProperty(usr => usr.usersCountingInformation_id,userCounting.usersCountingInformation_id));
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(token);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }

        }

        [HttpPost("signInUser")]
        public async Task<ActionResult> signInUser([FromForm] UserModel user)
        {
            try
            {
                var token = await _RegistrationService.signInUser(user);
                if (token == null) return BadRequest("Failed to Sign in User");
                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("getUsers/{searchKey}/{pageNumber}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<IEnumerable<UserModel>>> getUser(string? searchKey = null, int pageNumber = 1)
        {
            try
            {


                var query = _context.users
                .Include(fr => fr.usersCounting)
                .Include(fr => fr.analytics)
                .Where(u => u.first_name.ToLower().Contains(searchKey)
                       || u.last_name.ToLower().Contains(searchKey)
                        || u.email.ToLower().Contains(searchKey)
                         || u.address.ToLower().Contains(searchKey)).AsQueryable();


                var search_users = await query
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .ToListAsync();

                return Ok(search_users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpDelete("deleteUsers/{userId=int}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> deleteUser(int userId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                Task task1 = _context.users.Where(usr => usr.user_id == userId).ExecuteDeleteAsync();
                Task task2 = _context.analytics
                   .Where(val => val.user_id == userId).ExecuteDeleteAsync();
                Task task3 = _context.usersCounting
               .Where(val => val.user_id == userId).ExecuteDeleteAsync();
                Task task4 = _context.posts
               .Where(val => val.user_id == userId).ExecuteDeleteAsync();
                Task task5 = _context.notifications
                   .Where(val => val.to_user_id == userId).ExecuteDeleteAsync();

                await Task.WhenAll(task1, task2, task3, task4, task5);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("User is Sucessfully Deleted");

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }

        }
     
      
        [HttpPut("updateUsers")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> updateUser([FromForm] UserModel user_info)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {

                var user = await _context.users.FindAsync(user_info.user_id);
                if (user != null)
                {
                    string? profileImgstr = null, bgImgstr=null;
                    if (user_info.profile_picture_file != null) 
                        profileImgstr = await _fileService.uploadSingleFile(user_info.profile_picture_file, "profileimages");
                    if (profileImgstr == null && user_info.profile_picture_file != null) return BadRequest("Failed to Upload");
                    if (user_info.baground_picture_file != null)
                        bgImgstr = await _fileService.uploadSingleFile(user_info.baground_picture_file, "backgroundimages");
                    if (bgImgstr == null && user_info.baground_picture_file != null) return BadRequest("Failed to Upload");


                    if (!string.IsNullOrEmpty(user_info.first_name))
                        user.first_name = user_info.first_name;
                    if (!string.IsNullOrEmpty(user_info.last_name))
                        user.last_name = user_info.last_name;
                    if (!string.IsNullOrEmpty(profileImgstr))
                        user.profile_picture_url = profileImgstr;
                    if (!string.IsNullOrEmpty(bgImgstr))
                        user.baground_picture_url = bgImgstr;
                    if (!string.IsNullOrEmpty(user_info.address))
                        user.address = user_info.address;
                    if (!string.IsNullOrEmpty(user_info.email))
                        user.email = user_info.email;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok("User information is Sucessfully Updated");
                }
                else
                {
                    await transaction.RollbackAsync();
                    return NotFound("User doesn't Exist");
                }

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }
        }

    }
}
