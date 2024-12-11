using BulgarianViews.Data;
using BulgarianViews.Data.Models;
using BulgarianViews.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BulgarianViews.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ApplicationDbContext _dbContext;  

        public UserController(UserManager<ApplicationUser> userManager,
                              RoleManager<IdentityRole<Guid>> roleManager,
                              ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;  
        }

        // GET: Admin/User/Index
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList() 
                });
            }

            return View(model);
        }

        // GET: Admin/User/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList(),
                AllRoles = _roleManager.Roles.Select(r => r.Name).ToList() 
            };

            return View(model);
        }

        // POST: Admin/User/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;

            var userRoles = await _userManager.GetRolesAsync(user);

            
            var rolesToRemove = userRoles.Except(model.Roles).ToList();
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            
            var rolesToAdd = model.Roles.Except(userRoles).ToList();
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/User/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    
                    var relatedFavorites = _dbContext.FavoriteViews.Where(f => f.UserId == id).ToList();
                    if (relatedFavorites.Any())
                    {
                        _dbContext.FavoriteViews.RemoveRange(relatedFavorites);
                        await _dbContext.SaveChangesAsync();  
                    }

                    // Премахване на свързани коментари
                    var relatedComments = _dbContext.Comments.Where(c => c.UserId == id).ToList();
                    if (relatedComments.Any())
                    {
                        _dbContext.Comments.RemoveRange(relatedComments);
                        await _dbContext.SaveChangesAsync();  // Записваме промените
                    }

                    // Премахване на свързани рейтинги
                    var relatedRatings = _dbContext.Ratings.Where(r => r.UserId == id).ToList();
                    if (relatedRatings.Any())
                    {
                        _dbContext.Ratings.RemoveRange(relatedRatings);
                        await _dbContext.SaveChangesAsync();  // Записваме промените
                    }

                    // След като премахнем всички свързани записи, изтриваме потребителя
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        // Отменяме транзакцията, ако има грешка
                        await transaction.RollbackAsync();
                        return RedirectToAction(nameof(Index));
                    }

                    // Завършваме транзакцията
                    await transaction.CommitAsync();

                    TempData["Success"] = "User deleted successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Ако нещо се обърка, отменяме транзакцията
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, $"Error deleting user: {ex.Message}");
                    return RedirectToAction(nameof(Index));
                }
            }
        }

    }
}
