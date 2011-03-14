using System;

namespace GoToBrowser.Utils
{
    /// <summary>
    /// <seealso cref="IServiceProvider"/>の拡張メソッドを定義するクラスです。
    /// </summary>
    public static class IServiceProviderExtensions
    {
        /// <summary>
        /// 指定した型のサービスを取得します。
        /// </summary>
        public static T GetService<T>(this IServiceProvider provider) where T : class
        {
            return GetService<T, T>(provider);
        }

        /// <summary>
        /// 指定した型のサービスを取得します。
        /// </summary>
        public static TTo GetService<TFrom, TTo>(this IServiceProvider provider) where TTo : class
        {
            var service = provider.GetService(typeof(TFrom)) as TTo;
            if (service == null)
            {
                throw new InvalidOperationException(string.Format("The service [{0}] is not found.", typeof(TTo).Name));
            }

            return service;
        }
    }
}