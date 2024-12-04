using BulgarianViews.Data;
using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories;
using BulgarianViews.Data.Repositories.Interfaces;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BulgarianViews
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string connectionString = builder.Configuration.GetConnectionString("SqlServer");

            // Add services to the container.

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {

            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IRepository<Tag, Guid>, Repository<Tag, Guid>>();
            builder.Services.AddScoped<IRepository<Rating, Guid>, Repository<Rating, Guid>>();
            builder.Services.AddScoped<IRepository<LocationPost, Guid>, Repository<LocationPost, Guid>>();
           // builder.Services.AddScoped<IRepository<Location, Guid>, Repository<Location, Guid>>();
            builder.Services.AddScoped<IRepository<Comment, Guid>, Repository<Comment, Guid>>();
            builder.Services.AddScoped<IRepository<FavoriteViews, object>, Repository<FavoriteViews, object>>();
            builder.Services.AddScoped<IRepository<ApplicationUser, Guid>, Repository<ApplicationUser, Guid>>();



            builder.Services.AddScoped<ICommentService, CommentService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
