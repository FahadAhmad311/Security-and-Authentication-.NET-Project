using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var headers = context.Response.Headers;

        // Prevent Clickjacking Attacks
        headers["X-Frame-Options"] = "DENY";

        // Enable XSS Protection (Legacy)
        headers["X-XSS-Protection"] = "1; mode=block";

        // Prevent MIME-Sniffing Attacks
        headers["X-Content-Type-Options"] = "nosniff";

        // Content Security Policy (CSP) - Restrict sources
        headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self'";

        // Referrer Policy - Protect Referrer Information
        headers