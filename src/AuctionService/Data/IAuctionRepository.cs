using AuctionService.DTOs;
using AuctionService.Entities;

namespace AuctionService;

//Mocking for Unit Testing

public interface IAuctionRepository
{
    Task<List<AuctionDto>> GetAuctionsAsync(string date);
    Task<AuctionDto> GetAuctionByIdAsync(Guid Id);
    Task<Auction> GetAuctionEntityById(Guid Id);
    void AddAuction(Auction auction);
    void RemoveAuction(Auction auction);
    Task <bool> SaveChangesAsync();
}
