﻿namespace Api.Dtos
{
    public class NftDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public IFormFile? Image { get; set; }
    }
}
