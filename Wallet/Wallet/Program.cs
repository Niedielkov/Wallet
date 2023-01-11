using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Wallet.Data;
using static Wallet.Task;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

var app = builder.Build();

//Register Syncfusion license
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt/QHRqVVhkXVpFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF5jSH5Rd0ZjWHxbdn1SRA==;Mgo+DSMBPh8sVXJ0S0J+XE9Ad1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31Td0VhWHxecXdQQWFeUg==;ORg4AjUWIQA/Gnt2VVhkQlFacl5JXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxQdkRjW35dcHVWQ2deVkY=;ODU4MTAwQDMyMzAyZTM0MmUzMEFWa0dtZFBhSnQ3QUE0SXg4bm5wc3FPYkluOVBZNytqS2ZteEN6dTQ2Z0k9;ODU4MTAxQDMyMzAyZTM0MmUzMG94SE94VnNkdGpPVWpjS2tsWHVFNkNjNXhhMjlvWWJBOGRybWxzYjJCa0E9;NRAiBiAaIQQuGjN/V0Z+WE9EaFtDVmJLYVB3WmpQdldgdVRMZVVbQX9PIiBoS35RdUViWX1fc3RURWVUUEN1;ODU4MTAzQDMyMzAyZTM0MmUzMFBZZTR6NkJyNE5KeTN3THhjK1VWVHZMVXpzYzVnc2s1ZjdBQzRYMWtCVXM9;ODU4MTA0QDMyMzAyZTM0MmUzMEx4ZG9nQWdGc2hlQTZYSktiNXlJT1Bua3UzUUpkWnU1U0lYWWIvb0VjQk09;Mgo+DSMBMAY9C3t2VVhkQlFacl5JXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxQdkRjW35dcHVWQ2hVU0c=;ODU4MTA2QDMyMzAyZTM0MmUzMGhHWUF2cFZjWUk0Y1BLU09UZnFhSFBBVG5ETFRZcHNsVWw0V0IvSnlmeTg9;ODU4MTA3QDMyMzAyZTM0MmUzMG9FSTdERkxGcXNHWnFkdGZVVDhjc3M4ZC8wVWR5dGVBaEVLNlZsN2ROeXc9;ODU4MTA4QDMyMzAyZTM0MmUzMFBZZTR6NkJyNE5KeTN3THhjK1VWVHZMVXpzYzVnc2s1ZjdBQzRYMWtCVXM9");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//FluentScheduler initializing job and schedule to run every day
JobManager.Initialize(new ScheduleRegistry());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
