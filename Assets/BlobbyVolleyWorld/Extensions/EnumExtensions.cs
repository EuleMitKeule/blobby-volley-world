using System;

namespace BlobbyVolleyWorld.Extensions
{
    public static class EnumExtensions
    {
        public static int Count<TEnum>(this TEnum value) where TEnum : Enum =>
            Enum.GetValues(typeof(TEnum)).Length;
        
        public static TEnum Increase<TEnum>(this TEnum value) where TEnum : Enum =>
            (TEnum) Enum.ToObject(typeof(TEnum), (value.ToInt() + 1) % value.Count());
        
        public static TEnum Decrease<TEnum>(this TEnum value) where TEnum : Enum =>
            (TEnum) Enum.ToObject(typeof(TEnum), (value.ToInt() - 1 + value.Count()) % value.Count());
        
        public static int ToInt<TEnum>(this TEnum value) where TEnum : Enum =>
            Convert.ToInt32(value);
    }
}