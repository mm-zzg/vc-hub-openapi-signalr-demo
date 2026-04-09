using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiDemo.Common
{
    public class AccessTokenHandler(
        TokenService tokenService,
        IMemoryCache memoryCache) : DelegatingHandler
    {

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //缓存access token并在10分钟后过期 
            var accessToken = await memoryCache.GetOrCreateAsync<string>("AccessToken", async e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await tokenService.GetAccessToken();
            });
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
