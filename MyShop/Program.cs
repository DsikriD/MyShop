
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyShop_DataAccess;
using MyShop_DataAccess.Initializer;
using MyShop_DataAccess.Repository;
using MyShop_DataAccess.Repository.IRepository;
using MyShop_Utility;
using MyShop_Utility.BrainTree;
using System.Configuration;
using System.Security.Claims;

namespace MyShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContexts>(options => options.UseSqlServer(
               builder.Configuration.GetConnectionString("DefaultConnetction")));//��� ����������� 

            builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContexts>();//��� �����������

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromSeconds(10);//����� ������
                Options.Cookie.HttpOnly = true;
                Options.Cookie.IsEssential = true;
            });

            //builder.Configuration.GetSection("BrainTree").Get<BrainTreeSettings>();
            builder.Services.Configure<BrainTreeSettings>(builder.Configuration.GetSection("BrainTree"));
            builder.Services.AddSingleton<IBrainTreeGate, BrainTreeGate>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
            builder.Services.AddScoped<IInquiryDetailRepository, InquiryDetailRepository>();
            builder.Services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

           builder.Services.AddScoped<IDbInitializer, DbInitializer>();

           builder.Services.AddAuthentication()
                   .AddVkontakte(options =>
                   {
                       
                       options.ClientId = "51784449";
                       options.ClientSecret = "XwNG4Ag7GY6RbLhPj0Po";
                       options.Scope.Add("email");
                       options.SaveTokens = true;
                   });


            //builder.Services.AddAuthentication().AddOAuth("VK", "Vkontakte", config =>
            //{
            //    config.ClientId = "51784449";
            //    config.ClientSecret = "XwNG4Ag7GY6RbLhPj0Po";
            //    config.ClaimsIssuer = "VK";
            //    config.CallbackPath= new PathString("/signin-vkontakte-token");
            //    config.AuthorizationEndpoint = "https://oauth.vk.com/authorize";
            //    config.TokenEndpoint = "https://oauth.vk.com/access_token";

            //    config.SaveTokens = true;

            //});

            builder.Services.AddControllersWithViews();
           
            var app = builder.Build();

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


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider.GetRequiredService<IDbInitializer>;
                services.Invoke().Initialize();
            }

            app.UseSession();

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}