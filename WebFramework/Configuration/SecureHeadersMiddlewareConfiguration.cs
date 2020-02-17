using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Extensions;

namespace WebFramework.Configuration
{
    public static class SecureHeadersMiddlewareConfiguration 
    {
        public static OwaspHeaders.Core.Models.SecureHeadersMiddlewareConfiguration CustomConfiguration()
        {
            return SecureHeadersMiddlewareBuilder
                .CreateBuilder()
                .UseHsts(1200)
                .UseXSSProtection(XssMode.oneReport, "http://95.216.12.8:91")
                .UseContentDefaultSecurityPolicy()
                .UsePermittedCrossDomainPolicies(XPermittedCrossDomainOptionValue.masterOnly)
                .UseReferrerPolicy(ReferrerPolicyOptions.sameOrigin)
                .RemovePoweredByHeader()
                .UseXFrameOptions()
                .UseContentSecurityPolicy()
                .UseReferrerPolicy()
                .Build();
        }
    }
}