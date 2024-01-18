using Newtonsoft.Json.Linq;
using System.Net;

namespace ChzzkAPI
{
    public class ChzzkAPIHandler
    {
        private string NID_SES = string.Empty;
        private string NID_AUT = string.Empty;

        public string UserAgent { get; set; }

        private Dictionary<string, Dictionary<ChzzkApiType, JObject>> internalData = new Dictionary<string, Dictionary<ChzzkApiType, JObject>>();

        public ChzzkAPIHandler(string nid_ses, string nid_aut)
        {
            Initialize(nid_ses, nid_aut);
        }

        public void Initialize(string nid_ses, string nid_aut)
        {
            NID_SES = nid_ses;
            NID_AUT = nid_aut;
        }

        public bool RefreshOrFetchDataChannelInfo(string channelId)
        {
            bool value = internalData.ContainsKey(channelId) && internalData[channelId].ContainsKey(ChzzkApiType.CZK_CHANNEL_INFO);
            JObject data = JObject.Parse(SendToServer(new Uri(string.Format(ChzzkAPIEndPoint.ChannelInfo, channelId))));
            if (value)
            {
                internalData[channelId][ChzzkApiType.CZK_CHANNEL_INFO] = data;
            }
            else
            {
                if (!internalData.ContainsKey(channelId))
                {
                    internalData.Add(channelId, new Dictionary<ChzzkApiType, JObject>());
                }
                internalData[channelId][ChzzkApiType.CZK_CHANNEL_INFO] = data;
            }
            return value;
        }

        public bool RefreshOrFetchDataLiveDetail(string channelId)
        {
            bool value = internalData.ContainsKey(channelId) && internalData[channelId].ContainsKey(ChzzkApiType.CZK_LIVE_DETAIL);
            JObject data = JObject.Parse(SendToServer(new Uri(string.Format(ChzzkAPIEndPoint.LiveDetailUrl, channelId))));
            if (value)
            {
                internalData[channelId][ChzzkApiType.CZK_LIVE_DETAIL] = data;
            }
            else
            {
                if (!internalData.ContainsKey(channelId))
                {
                    internalData.Add(channelId, new Dictionary<ChzzkApiType, JObject>());
                }
                internalData[channelId][ChzzkApiType.CZK_LIVE_DETAIL] = data;
            }
            return value;
        }

        public bool CheckDataLoaded(string channelId, ChzzkApiType apiType)
        {
            return internalData.ContainsKey(channelId) && internalData[channelId].ContainsKey(apiType);
        }

        public void CheckData(ChzzkApiType apiType, string channelId)
        {
            if(!CheckDataLoaded(channelId, apiType))
            {
                switch(apiType)
                {
                    case ChzzkApiType.CZK_CHANNEL_INFO:
                        RefreshOrFetchDataChannelInfo(channelId);
                        break;
                    case ChzzkApiType.CZK_LIVE_DETAIL:
                        RefreshOrFetchDataLiveDetail(channelId);
                        break;
                }
            }
        }

        public string SendToServer(Uri Uri)
        {
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);

            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            client.BaseAddress = Uri;

            handler.CookieContainer = new CookieContainer();
            handler.UseCookies = true;
            
            handler.CookieContainer.Add(client.BaseAddress, new Cookie("NID_AUT", NID_AUT));
            handler.CookieContainer.Add(client.BaseAddress, new Cookie("NID_SES", NID_SES));

            var response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                return content;
            }
            else
            {
                throw new ChzzkException(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }

        public string GetChannelName(string channelId)
        {
            CheckData(ChzzkApiType.CZK_CHANNEL_INFO, channelId);
            return internalData[channelId][ChzzkApiType.CZK_CHANNEL_INFO]["content"]["channelName"].ToString();
        }

        public string GetChannelID(string channelId)
        {
            CheckData(ChzzkApiType.CZK_CHANNEL_INFO, channelId);
            return internalData[channelId][ChzzkApiType.CZK_CHANNEL_INFO]["content"]["channelId"].ToString();
        }

        public JObject GetLivePlaybackJson(string channelId)
        {
            CheckData(ChzzkApiType.CZK_LIVE_DETAIL, channelId);
            return JObject.Parse(internalData[channelId][ChzzkApiType.CZK_LIVE_DETAIL]["content"]["livePlaybackJson"].ToString());
        }

        public string GetLiveTitle(string channelId)
        {
            CheckData(ChzzkApiType.CZK_LIVE_DETAIL, channelId);
            return internalData[channelId][ChzzkApiType.CZK_LIVE_DETAIL]["content"]["liveTitle"].ToString();
        }

        public string GetMedia(string channelId)
        {
            return GetLivePlaybackJson(channelId)["media"].ToString();
        }

        public string GetStreamUrl(string channelId)
        {
            return GetLivePlaybackJson(channelId)["media"].First["path"].ToString();
        }

        public string GetStatus(string channelId)
        {
            CheckData(ChzzkApiType.CZK_LIVE_DETAIL, channelId);
            return internalData[channelId][ChzzkApiType.CZK_LIVE_DETAIL]["content"]["status"].ToString();
        }

        public bool IsOpenLive(string channelId)
        {
            CheckData(ChzzkApiType.CZK_CHANNEL_INFO, channelId);
            return Convert.ToBoolean(internalData[channelId][ChzzkApiType.CZK_CHANNEL_INFO]["content"]["openLive"]);
        }

        public string GetChannelDescription(string channelId)
        {
            CheckData(ChzzkApiType.CZK_CHANNEL_INFO, channelId);
            return internalData[channelId][ChzzkApiType.CZK_CHANNEL_INFO]["content"]["channelDescription"].ToString();
        }

        public string GetChannelImageUrl(string channelId)
        {
            CheckData(ChzzkApiType.CZK_CHANNEL_INFO, channelId);
            return internalData[channelId][ChzzkApiType.CZK_CHANNEL_INFO]["content"]["channelImageUrl"].ToString();
        }

        public JObject GetJObject(ChzzkApiType apiType, string channelId)
        {
            CheckData(apiType, channelId);
            return internalData[channelId][apiType];
        }
    }
}