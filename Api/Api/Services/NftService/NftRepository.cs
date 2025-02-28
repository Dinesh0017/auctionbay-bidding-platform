﻿using Api.Data;
using Api.Dtos;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.NftService
{
    public class NftRepository : INftRepository
    {
        public readonly AppDbContext _context;

        public NftRepository(AppDbContext context) 
        {
            _context = context; 
        }

        public bool CreateNft(Nft nft)
        {
            _context.Add(nft);
            return Save();
        }

        public bool DeleteNft(Nft nft)
        {
            _context.Remove(nft);
            return Save();
        }


        public Nft GetNftById(int id)
        {
            return _context.Nfts.Where(n => n.Id == id).FirstOrDefault();
        }

        public ICollection<Nft> GetNfts()
        {
            return _context.Nfts.ToList();
        }


        public bool NftExist(int id)
        {
            return _context.Nfts.Any(n => n.Id == id);
        }

        public bool Save()
        {
            var Saved = _context.SaveChanges();
            return Saved > 0 ? true : false;
        }

        public bool UpdateNft(Nft nft)
        {
            _context.Update(nft);
            return Save();
        }


        public async Task<NftBidsDto> GetNftBidsOnlyAsync(int nftId)
        {
            return await _context.Nfts
                .Where(n => n.Id == nftId)
                .Select(n => new NftBidsDto
                {
                    NftId = n.Id,
                    NftTitle = n.Title,
                    NftDescription = n.Description,
                    Bids = n.Auctions.SelectMany(a => a.Bids).Select(b => new BidDto
                    {
                        BidId = b.Id,
                        BidPrice = b.BidPrice,
                        UserId = b.UserId,
                        UserName = b.AppUsers.FirstName
                    }).ToList()
                })
                .FirstOrDefaultAsync();

        }
        
    }
}
