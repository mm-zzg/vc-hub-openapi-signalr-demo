using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenApiDemo.Common
{
    public class OpenIdConfiguration
    {
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; }

        [JsonPropertyName("jwks_uri")]
        public string JwksUri { get; set; }

        [JsonPropertyName("grant_types_supported")]
        public IEnumerable<string> GrantTypesSupported { get; set; } = new List<string>();

        [JsonPropertyName("scopes_supported")]
        public IEnumerable<string> ScopesSupported { get; set; } = new List<string>();

        [JsonPropertyName("claims_supported")]
        public IEnumerable<string> ClaimsSupported { get; set; } = new List<string>();

        [JsonPropertyName("id_token_signing_alg_values_supported")]
        public IEnumerable<string> IdTokenSigningAlgValuesSupported { get; set; } = new List<string>();

        [JsonPropertyName("subject_types_supported")]
        public IEnumerable<string> SubjectTypesSupported { get; set; } = new List<string>();

        [JsonPropertyName("token_endpoint_auth_methods_supported")]
        public IEnumerable<string> TokenEndpointAuthMethodsSupported { get; set; } = new List<string>();

        [JsonPropertyName("claims_parameter_supported")]
        public bool ClaimsParameterSupported { get; set; }

        [JsonPropertyName("request_parameter_supported")]
        public bool RequestParameterSupported { get; set; }

        [JsonPropertyName("request_uri_parameter_supported")]
        public bool RequestUriParameterSupported { get; set; }

        [JsonPropertyName("authorization_response_iss_parameter_supported")]
        public bool AuthorizationResponseIssParameterSupported { get; set; }
    }
}
