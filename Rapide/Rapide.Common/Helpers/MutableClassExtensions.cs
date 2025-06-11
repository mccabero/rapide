using System.Linq.Expressions;
using System.Reflection;

namespace Rapide.Common.Helpers
{
    public static class MutableClassExtensions
    {
        public static T MemberwiseApply<T, TM>(this T value, Func<TM, TM> fn) where T : class
        {
            var t = value.GetType();
            var newValue = (T)typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(value, new object[0]);
            var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(c => c.PropertyType == typeof(TM));
            foreach (var propertyInfo in properties)
            {
                if (!(propertyInfo.CanRead && propertyInfo.CanWrite)) 
                    continue;

                // Add fields to skip convert to uppercase
                if (propertyInfo.Name.Equals("InspectionDetails", StringComparison.InvariantCultureIgnoreCase))
                    continue;
                if (propertyInfo.Name.Equals("Email", StringComparison.InvariantCultureIgnoreCase))
                    continue;
                if (propertyInfo.Name.Equals("PasswordHash", StringComparison.InvariantCultureIgnoreCase))
                    continue;
                if (propertyInfo.Name.Equals("ConfirmPasswordHash", StringComparison.InvariantCultureIgnoreCase))
                    continue;
                if (propertyInfo.Name.Equals("Salt", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var propValue = (TM)propertyInfo.GetValue(newValue);
                propValue = fn(propValue);
                propertyInfo.SetValue(newValue, propValue);
            }

            return newValue;
        }
    }
}