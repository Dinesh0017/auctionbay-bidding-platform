﻿using Api.Entities;

namespace Api.Services.NftService
{
    public interface INftRepository
    {
        ICollection<Nft> GetNfts();

        Nft GetNftById(int id);

        Task<Nft> GetBidsForNft(int nftId);

        bool NftExist(int id);

        bool CreateNft(Nft nft);

        bool UpdateNft(Nft nft);

        bool DeleteNft(Nft nft);

    }
}
