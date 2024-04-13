namespace ODataDemo.Middlewares;

public class OdataContextAdjusterMiddleware
{
    public const string ExpandKeyword = "$expand";
    public const string OriginalAuthorsFunction = "Authors()";
    public const string ReplacementAuthorsFunction = "Authors(*)";
    public const string OriginalBooksFunction = "Books()";
    public const string ReplacementBooksFunction = "Books(*)";
    public const string OriginalPriceOffersFunction = "PriceOffers()";
    public const string ReplacementPriceOffersFunction = "PriceOffers(*)";

    private readonly RequestDelegate _next;

    public OdataContextAdjusterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.QueryString.HasValue ||
            !context.Request.QueryString.Value.Contains(ExpandKeyword))
        {
            await _next(context);

            return;
        }

        var responseBody = context.Response.Body;
        using var newResponseBody = new MemoryStream();
        context.Response.Body = newResponseBody;

        await _next(context);

        context.Response.Body = new MemoryStream();
        newResponseBody.Seek(0, SeekOrigin.Begin);
        context.Response.Body = responseBody;
        var json = await new StreamReader(newResponseBody).ReadToEndAsync();
        json = json.Replace(OriginalAuthorsFunction, ReplacementAuthorsFunction);
        json = json.Replace(OriginalBooksFunction, ReplacementBooksFunction);
        json = json.Replace(OriginalPriceOffersFunction, ReplacementPriceOffersFunction);
        await context.Response.WriteAsync(json);
    }
}
