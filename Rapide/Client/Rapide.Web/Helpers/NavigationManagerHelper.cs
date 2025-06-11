using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Rapide.Web.Helpers
{
    public static class NavigationManagerHelper
    {
        private static bool TryGetQueryString<T>(this NavigationManager navManager, string key, out T value)
        {
            var uri = navManager.ToAbsoluteUri(navManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString))
            {
                if (typeof(T) == typeof(int) && int.TryParse(valueFromQueryString, out var valueAsInt))
                {
                    value = (T)(object)valueAsInt;
                    return true;
                }

                if (typeof(T) == typeof(string))
                {
                    value = (T)(object)valueFromQueryString.ToString();
                    return true;
                }

                if (typeof(T) == typeof(decimal) && decimal.TryParse(valueFromQueryString, out var valueAsDecimal))
                {
                    value = (T)(object)valueAsDecimal;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public static void NavigateToCustom(this NavigationManager navManager, string redirectUrl, bool forceRedirect = false)
        {
            navManager.TryGetQueryString<string>("returnUrl", out string newRedirectUrl);

            if (string.IsNullOrEmpty(newRedirectUrl))
            {
                navManager.NavigateTo(redirectUrl, forceRedirect);
            }
            else
            {
                navManager.NavigateTo(newRedirectUrl, forceRedirect);
            }
        }
    }
}
