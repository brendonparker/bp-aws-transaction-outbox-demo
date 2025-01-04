using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Util;

public static class TokenGenerator
{
    public static string GenerateDbConnectAdminAuthToken(string? your_cluster_endpoint, Amazon.RegionEndpoint region,
        string? action)
    {
        AWSCredentials awsCredentials = FallbackCredentialsFactory.GetCredentials();

        string accessKey = awsCredentials.GetCredentials().AccessKey;
        string secretKey = awsCredentials.GetCredentials().SecretKey;
        string token = awsCredentials.GetCredentials().Token;

        const string DsqlServiceName = "dsql";
        const string HTTPGet = "GET";
        const string HTTPS = "https";
        const string URISchemeDelimiter = "://";
        const string ActionKey = "Action";

        action = action?.Trim();
        if (string.IsNullOrEmpty(action))
            throw new ArgumentException("Action must not be null or empty.");
        string ActionValue = action;
        const string XAmzSecurityToken = "X-Amz-Security-Token";

        ImmutableCredentials immutableCredentials = new ImmutableCredentials(accessKey, secretKey, token) ??
                                                    throw new ArgumentNullException("immutableCredentials");
        ArgumentNullException.ThrowIfNull(region);

        your_cluster_endpoint = your_cluster_endpoint?.Trim();
        if (string.IsNullOrEmpty(your_cluster_endpoint))
            throw new ArgumentException("Cluster endpoint must not be null or empty.");

        GenerateDsqlAuthTokenRequest authTokenRequest = new GenerateDsqlAuthTokenRequest();
        IRequest request = new DefaultRequest(authTokenRequest, DsqlServiceName)
        {
            UseQueryString = true,
            HttpMethod = HTTPGet
        };
        request.Parameters.Add(ActionKey, ActionValue);
        request.Endpoint = new UriBuilder(HTTPS, your_cluster_endpoint).Uri;

        if (immutableCredentials.UseToken)
        {
            request.Parameters[XAmzSecurityToken] = immutableCredentials.Token;
        }

        var signingResult = AWS4PreSignedUrlSigner.SignRequest(request, null, new RequestMetrics(),
            immutableCredentials.AccessKey,
            immutableCredentials.SecretKey, DsqlServiceName, region.SystemName);

        var authorization = "&" + signingResult.ForQueryParameters;
        var url = AmazonServiceClient.ComposeUrl(request);

        // remove the https:// and append the authorization
        return url.AbsoluteUri[(HTTPS.Length + URISchemeDelimiter.Length)..] + authorization;
    }

    private class GenerateDsqlAuthTokenRequest : AmazonWebServiceRequest
    {
        public GenerateDsqlAuthTokenRequest()
        {
            ((IAmazonWebServiceRequest)this).SignatureVersion = SignatureVersion.SigV4;
        }
    }
}