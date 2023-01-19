namespace Wimi.BtlCore.ThirdpartyApis
{
    public static class ThirdpartyApiExtensions
    {
        public static T GetPropertyValue<T>(this object obj, string name)
        {
            var value = GetValue(obj, name);
            if (value == null)
            {
                return default(T);
            }

            return (T)value;
        }

        private static object GetValue(this object obj, string name)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null)
                {
                    return null;
                }

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null)
                {
                    return null;
                }

                obj = info.GetValue(obj, null);
            }

            return obj;
        }
    }
}