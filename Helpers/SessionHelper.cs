namespace RaceDay.Api.Helpers
{
    public static class SessionHelper
    {
        public static int? GetUserId(HttpContext context)
        {
            return context.Session.GetInt32("UserID");
        }

        public static string? GetRole(HttpContext context)
        {
            return context.Session.GetString("Role");
        }

        public static bool IsAuthenticated(HttpContext context)
        {
            return GetUserId(context) != null;
        }
    }
}
