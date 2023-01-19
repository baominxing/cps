namespace Wimi.BtlCore.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    using Abp;
    using Abp.Localization;

    public static class EnumExtensions
    {
        /// <summary>
        /// A generic extension method that aids in reflecting
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <param name="targetEnum">
        /// The enum Value.
        /// </param>
        /// <returns>
        /// The <see cref="TAttribute"/>.
        /// </returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum targetEnum) where TAttribute : Attribute
        {
            return targetEnum.GetType().GetMember(targetEnum.ToString()).First().GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        /// 将枚举类型封装成键值对类型
        /// 如果枚举项有标记attribute，eg.[Display(Name = "新增")]，则显示Name为DisplayAttribute的Name内容
        /// 如果没有标记attribute，则显示Name为枚举项的Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetEnum"></param>
        /// <returns></returns>
        public static List<NameValue<T>> ToNameValueList<T>(this Type targetEnum)
        {
            var typeofEnum = targetEnum;

            if (!targetEnum.IsEnum)
            {
                throw new AbpException($"{LocalizationHelper.GetString(BtlCoreConsts.LocalizationSourceName, "EnumerationTypesCanUseTheMethod")}:{targetEnum}");
            }

            var returnList = new List<NameValue<T>>();

            foreach (T enumValue in Enum.GetValues(typeofEnum))
            {
                var usedStateName = Enum.GetName(typeofEnum, enumValue);

                // get name in attribute display eg. [Display(Name = "新增")]
                FieldInfo fi = typeofEnum.GetField(usedStateName);
                var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

                var disPlayName = attributes.Length > 0 ? attributes.First().Name : usedStateName;

                returnList.Add(new NameValue<T>(disPlayName, enumValue));
            }

            return returnList;
        }
    }
}