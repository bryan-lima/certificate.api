using Microsoft.Extensions.DependencyInjection;

namespace Certificate.Infra.CrossCutting.IoC
{
    public class DependencyInjectionConfiguration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            #region Singleton

            /// Add dependency injections with <see cref="ServiceLifetime.Singleton"/>.

            #endregion Singleton

            #region Scoped

            /// Add dependency injections with <see cref="ServiceLifetime.Scoped"/>.

            #endregion Scoped

            #region Transient

            /// Add dependency injections with <see cref="ServiceLifetime.Transient"/>.

            #endregion Transient
        }
    }
}
