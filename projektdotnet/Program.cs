using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;
using projektdotnet.Repositories;
using projektdotnet.Services;
using projektdotnet.wwwroot.seeders;
using System.Configuration;
using System.Text.Json.Serialization;
using System.Web.Mvc;

namespace projektdotnet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<NewDbContext>(x=>x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthorization();
            builder.Services.AddAntiforgery();
            builder.Services.AddHttpContextAccessor();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Home/DeniedAccess";
                options.ExpireTimeSpan = TimeSpan.FromSeconds(10);
                options.LoginPath = "/Home/Login";
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Home/DeniedAccess"; // wrzuca jak nie ma roli tutaj
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.LoginPath = "/Home/Login"; // wrzuca tutaj jak nie zalogowany
                });

            builder.Services.AddTransient<TicketService>();
            builder.Services.AddTransient<EmployeeService>();
            builder.Services.AddTransient<EmailService>();
            builder.Services.AddTransient<TicketCommentService>();
            builder.Services.AddTransient<RoomService>();
            builder.Services.AddTransient<OpinionService>();
            builder.Services.AddTransient<EmailService>();
            builder.Services.AddTransient<MeetingService>();

            builder.Services.AddTransient<MeetingRepository>();
            builder.Services.AddTransient<TicketCommentRepository>();
            builder.Services.AddTransient<OpinionRepository>();
            builder.Services.AddTransient<RoomRepository>();
            builder.Services.AddTransient<EmployeeRepository>();
            builder.Services.AddTransient<TicketRepository>();
            builder.Services.AddTransient<RoleRepository>();
            builder.Services.AddTransient<Seeder>();
            builder.Services.AddCors();
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddControllers().AddJsonOptions(x =>
               x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
                seeder.seedRoles().Wait();
                seeder.seedHR().Wait();
            }
            app.UseCors(
                     options => options
                     .AllowAnyHeader()
                     .AllowAnyOrigin()
                     .AllowAnyMethod()
                     );

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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
                pattern: "{controller=Home}/{action=Login}/{id?}");


            app.Run();
        }
    }
}
