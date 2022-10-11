using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(o =>
        {
            // This forces challenge results to be handled by Google OpenID Handler, so there's no
            // need to add an AccountController that emits challenges for Login.
            o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
            // This forces forbid results to be handled by Google OpenID Handler, which checks if
            // extra scopes are required and does automatic incremental auth.
            o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
            // Default scheme that will handle everything else.
            // Once a user is authenticated, the OAuth2 token info is stored in cookies.
            o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie()
        .AddGoogleOpenIdConnect(options =>
        {
            options.ClientId = "1003902974858-5524oc4vferstdlr3hh3p4fv3sof9g1a.apps.googleusercontent.com";
            options.ClientSecret = "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";
        });

var app = builder.Build();
app.UseCookiePolicy(
             new CookiePolicyOptions
             {
                 HttpOnly = HttpOnlyPolicy.Always,
                 Secure = CookieSecurePolicy.SameAsRequest,
                 MinimumSameSitePolicy = SameSiteMode.Lax
             });
app.UseCookiePolicy();
app.UseSession();
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
