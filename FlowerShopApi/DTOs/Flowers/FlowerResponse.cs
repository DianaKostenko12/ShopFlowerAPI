﻿namespace FlowerShopApi.DTOs.Flowers
{
    public class FlowerResponse
    {
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public int FlowerCount { get; set; }
        public float FlowerCost { get; set; }
        public string ImgUrl { get; set; }
    }
}
