using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NotAlone.Models;
using NotAlone.Services.Abstract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotAlone.Services.Implementation
{
    public class RegistrationService : IRegistrationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public RegistrationService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }


        public async Task<string?> signUpUser(UserModel userModel)
        {
            var userExist = await _userManager.FindByEmailAsync(userModel.email!);
            if (userExist != null) return null;

            ApplicationUser user = new ApplicationUser()
            {
                UserName = userModel.email,
                Email = userModel.email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(user, userModel.password!);
            if (!result.Succeeded) return null;
            else
            {
                if(! await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _userManager.AddToRoleAsync(user, "Admin");

                }
                else
                {
                    if(!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "User");

                    }
                }
            }

            var token = await signInUser(userModel);

            return token;
        }

        public async Task<string?> signInUser(UserModel userModel)
        {

            var user = await _userManager.FindByEmailAsync(userModel.email!);
            if (user != null && await _userManager.CheckPasswordAsync(user,userModel.password!))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim> { 
                new Claim(ClaimTypes.Email,userModel.email!),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };
                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));
                var preToken = new JwtSecurityToken(
                    
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(1),
                    claims:authClaims,
                    signingCredentials:new SigningCredentials(authSignInKey,SecurityAlgorithms.HmacSha256)

                    );
                var token =new JwtSecurityTokenHandler().WriteToken(preToken);
                return token;
            }


            return null;
        }

       
    }
}
