using System;

namespace Plugin.Social.Facebook
{
    public class FacebookOAuthResult
    {
        /// <summary>
        /// AccessToken
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Facebook Oauth Page
        /// </summary>
        public FacebookOAuthRequest FacebookPage { get; set; }

        /// <summary>
        /// Returns Facebook Graph Api Client
        /// </summary>
        public GraphApi Api
        {
            get
            {
                if (string.IsNullOrEmpty(AccessToken))
                {
                    throw new ArgumentException("Accesstoken is null");
                }
                if (AccessToken.Contains("http"))
                {
                    throw new ArgumentException("Accesstoken is null");
                }
                return new GraphApi(AccessToken);
            }
        }
    }
}