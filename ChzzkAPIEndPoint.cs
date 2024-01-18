namespace ChzzkAPI
{

    public static class ChzzkAPIEndPoint
    {
        public const string LiveDetailUrl = "https://api.chzzk.naver.com/service/v2/channels/{0}/live-detail";
        public const string ChannelInfo = "https://api.chzzk.naver.com/service/v1/channels/{0}";
        public const string LiveStatusUrl = "https://api.chzzk.naver.com/polling/v2/channels/{0}/live-status";
        public const string DonationSettingUrl = "https://api.chzzk.naver.com/service/v1/channels/{0}/donations/setting";
        public const string ChatRuleUrl = "https://api.chzzk.naver.com/service/v1/channels/{0}/chat-rules";
    }
}