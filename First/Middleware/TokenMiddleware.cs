namespace WebApplication3.Middleware
{

    public class TokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string token;
        public TokenMiddleware(RequestDelegate next, string token)
        {
            this.next = next;
            this.token = token;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Query["token"];
            if (token != this.token)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Token is invalid");
            }
            else
            {
                await next.Invoke(context);
            }
        }
    }
}