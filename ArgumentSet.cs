using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace FastAirFilter
{
    [JsonObject(MemberSerialization.OptIn)]
    [RestartRequired]
    public class ArgumentSet
    {
        private float __airFilterSpeed = 4.5f;

        [Option("空气过滤倍速", "默认修改5倍", "空气过滤倍速")]
        [Limit(1.0, 20.0)]
        [JsonProperty]
        public float AirFilterSpeed 
        { 
            get => __airFilterSpeed;
            set => __airFilterSpeed = Math.Max(1.0f, Math.Min(20.0f, value));
        }

        // 空气过滤的范围半径 3-6
        private int __airFilterRadius = 3;
        [Option("空气过滤范围半径", "默认修改3格", "空气过滤范围半径")]
        [Limit(3, 6)]
        [JsonProperty]
        public int AirFilterRadius
        {
            get => __airFilterRadius;
            set => __airFilterRadius = (int)Math.Max((int)3, Math.Min((int)6, value));
        }

        // 是否用电
        private bool __airFilterUsePower = true;
        [Option("空气过滤是否用电", "默认用电", "空气过滤是否用电")]
        [JsonProperty]
        public bool AirFilterUsePower
        {
            get => __airFilterUsePower;
            set => __airFilterUsePower = value;
        }
    }
}
