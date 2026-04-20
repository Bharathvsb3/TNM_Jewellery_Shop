using JewelleryShopAPI.DTOs;
using JewelleryShopAPI.Models;
using JewelleryShopAPI.Repositories;

namespace JewelleryShopAPI.Services
{
    public interface IGoldRateService
    {
        Task<GoldRateDTO?> GetLatestRateAsync();
        Task<IEnumerable<GoldRateDTO>> GetTodayRatesAsync();
        Task<IEnumerable<GoldRateDTO>> GetAllRatesAsync();
        Task<GoldRateDTO?> GetRateByIdAsync(int id);
        Task<GoldRateDTO?> CreateRateAsync(CreateGoldRateRequest request);
        Task<GoldRateDTO?> UpdateRateAsync(int id, UpdateGoldRateRequest request);
    }

    public class GoldRateService : IGoldRateService
    {
        private readonly IGoldRateRepository _goldRateRepository;

        public GoldRateService(IGoldRateRepository goldRateRepository)
        {
            _goldRateRepository = goldRateRepository;
        }

        public async Task<GoldRateDTO?> GetLatestRateAsync()
        {
            var rate = await _goldRateRepository.GetLatestAsync();
            return rate != null ? MapToDTO(rate) : null;
        }

        public async Task<IEnumerable<GoldRateDTO>> GetTodayRatesAsync()
        {
            // Use India Standard Time (UTC+5:30)
            var istNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            var rates = await _goldRateRepository.GetByDateAsync(istNow);
            return rates.Select(r => MapToDTO(r));
        }

        public async Task<IEnumerable<GoldRateDTO>> GetAllRatesAsync()
        {
            var rates = await _goldRateRepository.GetAllAsync();
            return rates.Select(r => MapToDTO(r)).OrderByDescending(r => r.EntryDate);
        }

        public async Task<GoldRateDTO?> GetRateByIdAsync(int id)
        {
            var rate = await _goldRateRepository.GetByIdAsync(id);
            return rate != null ? MapToDTO(rate) : null;
        }

        public async Task<GoldRateDTO?> CreateRateAsync(CreateGoldRateRequest request)
        {
            // Validate time slot selection
            if (request.TimeSlot != "Morning" && request.TimeSlot != "Evening")
            {
                return null; // Invalid time slot
            }

            // Use India Standard Time (UTC+5:30) for date operations
            var istNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            
            // Check if already 2 entries today (max allowed)
            var todayCount = await _goldRateRepository.GetTodayEntryCountAsync();
            if (todayCount >= 2)
                return null; // Maximum 2 entries per day allowed

            // Check if this time slot already has entry today
            var todayRates = await _goldRateRepository.GetByDateAsync(istNow);
            if (todayRates.Any(r => r.TimeSlot == request.TimeSlot))
                return null; // This time slot already has an entry today

            var goldRate = new GoldRate
            {
                Gold24KRate = request.Gold24KRate,
                Gold916Rate = request.Gold916Rate,
                GoldNormalRate = request.GoldNormalRate,
                SilverRate = request.SilverRate,
                TimeSlot = request.TimeSlot,
                EntryDate = istNow
            };

            await _goldRateRepository.CreateAsync(goldRate);
            return MapToDTO(goldRate);
        }

        public async Task<GoldRateDTO?> UpdateRateAsync(int id, UpdateGoldRateRequest request)
        {
            var existingRate = await _goldRateRepository.GetByIdAsync(id);
            if (existingRate == null)
                return null;

            // Validate time slot
            if (request.TimeSlot != "Morning" && request.TimeSlot != "Evening")
                return null;

            // Update the rate
            existingRate.Gold24KRate = request.Gold24KRate;
            existingRate.Gold916Rate = request.Gold916Rate;
            existingRate.GoldNormalRate = request.GoldNormalRate;
            existingRate.SilverRate = request.SilverRate;
            existingRate.TimeSlot = request.TimeSlot;

            await _goldRateRepository.UpdateAsync(existingRate);
            return MapToDTO(existingRate);
        }

        private GoldRateDTO MapToDTO(GoldRate goldRate)
        {
            return new GoldRateDTO
            {
                Id = goldRate.Id,
                Gold24KRate = goldRate.Gold24KRate,
                Gold916Rate = goldRate.Gold916Rate,
                GoldNormalRate = goldRate.GoldNormalRate,
                SilverRate = goldRate.SilverRate,
                TimeSlot = goldRate.TimeSlot,
                EntryDate = goldRate.EntryDate,
                CreatedAt = goldRate.CreatedAt
            };
        }
    }
}
