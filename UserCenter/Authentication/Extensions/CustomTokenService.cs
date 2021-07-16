using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserCenter.Common;

namespace Authentication.Extensions
{
    public class CustomTokenService : DefaultTokenService, ITokenService
    {
        private IHttpContextAccessor _contextAccessor;
        private IRefreshTokenStore _refreshTokenStore;
        public CustomTokenService(
            IClaimsService claimsProvider,
            IReferenceTokenStore referenceTokenStore,
            ITokenCreationService creationService,
            IHttpContextAccessor contextAccessor,
            ISystemClock clock,
            IKeyMaterialService keyMaterialService,
            ILogger<DefaultTokenService> logger,
            IdentityServerOptions options,
            IRefreshTokenStore refreshTokenStore) : base(claimsProvider, referenceTokenStore, creationService, contextAccessor, clock, keyMaterialService, options, logger)
        {
            this._contextAccessor = contextAccessor;
            _refreshTokenStore = refreshTokenStore;
        }

        /// <summary>
        /// Creates an access token.
        /// </summary>
        /// <param name="request">The token creation request.</param>
        /// <returns>
        /// An access token
        /// </returns>
        public new async Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
        {
            var jti = _contextAccessor.HttpContext.Items[JwtClaimTypes.JwtId];

            Logger.LogTrace("Creating access token");

            var claims = new List<Claim>();
            claims.AddRange(await ClaimsProvider.GetAccessTokenClaimsAsync(
                request.Subject,
                request.Resources,
                request.ValidatedRequest));

            var refreshClaim = claims.FirstOrDefault(p => p.Type == ExClaimTypesConst.IsRefresh);
            if (request.ValidatedRequest.Client.IncludeJwtId && string.IsNullOrWhiteSpace(refreshClaim?.Value))
            {
                claims.Add(new Claim(JwtClaimTypes.JwtId, CryptoRandom.CreateUniqueId(16)));
            }

            if (!string.IsNullOrWhiteSpace(refreshClaim?.Value))
            {
                claims.Add(new Claim(JwtClaimTypes.JwtId, refreshClaim?.Value));
                claims.Remove(refreshClaim);
                var userId = claims.FirstOrDefault(p => p.Type == ClaimTypes.Name);
                var sub = claims.FirstOrDefault(p => p.Type == JwtClaimTypes.Subject);
                if (!string.IsNullOrWhiteSpace(sub?.Value))
                {
                    claims.Remove(sub);
                    claims.Add(new Claim(JwtClaimTypes.Subject, userId?.Value));
                }

                //update database

                var isUpdateDb = claims.FirstOrDefault(p => p.Type == ExClaimTypesConst.IsUpdateDb);
                if (!string.IsNullOrWhiteSpace(isUpdateDb?.Value))
                {
                    claims.Remove(isUpdateDb);
                    var newReq = (ValidatedTokenRequest)request.ValidatedRequest;
                    await UpdateDb(newReq, claims, userId?.Value);
                }
            }

            var issuer = Context.HttpContext.GetIdentityServerIssuerUri();
            var token = new Token(OidcConstants.TokenTypes.AccessToken)
            {
                CreationTime = Clock.UtcNow.UtcDateTime,
                Audiences = { string.Format("{0}resources", issuer.EnsureTrailingSlash()) },
                Issuer = issuer,
                Lifetime = request.ValidatedRequest.AccessTokenLifetime,
                Claims = claims,
                ClientId = request.ValidatedRequest.Client.ClientId,
                AccessTokenType = request.ValidatedRequest.AccessTokenType
            };

            foreach (var api in request.Resources.ApiResources)
            {
                if (api.Name.IsPresent())
                {
                    token.Audiences.Add(api.Name);
                }
            }


            if (jti != null)
            {
                var oldJtiClaim = token.Claims.FirstOrDefault(i => i.Type == JwtClaimTypes.JwtId);
                token.Claims.Remove(oldJtiClaim);
                token.Claims.Add(new Claim(JwtClaimTypes.JwtId, jti.ToString()));
            }
            return token;
        }

        private async Task UpdateDb(ValidatedTokenRequest req, List<Claim> claims, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(req?.RefreshTokenHandle) || req?.RefreshToken?.AccessToken == null)
            {
                return;
            }
            req.RefreshToken.AccessToken.Claims = claims;
            await _refreshTokenStore.UpdateRefreshTokenAsync(req.RefreshTokenHandle, req.RefreshToken);
        }
    }
}
