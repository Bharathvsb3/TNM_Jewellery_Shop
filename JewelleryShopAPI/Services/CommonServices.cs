using JewelleryShopAPI.DTOs;
using JewelleryShopAPI.Models;
using JewelleryShopAPI.Repositories;

namespace JewelleryShopAPI.Services
{
    public interface IEnquiryService
    {
        Task<IEnumerable<EnquiryDTO>> GetAllEnquiriesAsync();
        Task<IEnumerable<EnquiryDTO>> GetEnquiriesByStatusAsync(string status);
        Task<EnquiryDTO?> GetEnquiryByIdAsync(int id);
        Task<EnquiryDTO> CreateEnquiryAsync(CreateEnquiryRequest request);
        Task<EnquiryDTO?> UpdateEnquiryAsync(int id, UpdateEnquiryRequest request);
    }

    public class EnquiryService : IEnquiryService
    {
        private readonly IEnquiryRepository _enquiryRepository;

        public EnquiryService(IEnquiryRepository enquiryRepository)
        {
            _enquiryRepository = enquiryRepository;
        }

        public async Task<IEnumerable<EnquiryDTO>> GetAllEnquiriesAsync()
        {
            var enquiries = await _enquiryRepository.GetAllAsync();
            return enquiries.Select(e => MapToDTO(e));
        }

        public async Task<IEnumerable<EnquiryDTO>> GetEnquiriesByStatusAsync(string status)
        {
            var enquiries = await _enquiryRepository.GetByStatusAsync(status);
            return enquiries.Select(e => MapToDTO(e));
        }

        public async Task<EnquiryDTO?> GetEnquiryByIdAsync(int id)
        {
            var enquiry = await _enquiryRepository.GetByIdAsync(id);
            return enquiry != null ? MapToDTO(enquiry) : null;
        }

        public async Task<EnquiryDTO> CreateEnquiryAsync(CreateEnquiryRequest request)
        {
            var enquiry = new Enquiry
            {
                CustomerId = request.CustomerId,
                Subject = request.Subject,
                Description = request.Description,
                Notes = request.Notes,
                ExpectedDate = request.ExpectedDate,
                FollowupDate = request.FollowupDate,
                MetalType = request.MetalType,
                JewelryType = request.JewelryType,
                Purity = request.Purity,
                EstimatedWeight = request.EstimatedWeight,
                MakingType = request.MakingType,
                Status = "New",
                DesignImage = request.DesignImage,
                DesignImage2 = request.DesignImage2,
                DesignImage3 = request.DesignImage3
            };

            await _enquiryRepository.CreateAsync(enquiry);

            // Load customer for DTO
            var created = await _enquiryRepository.GetByIdAsync(enquiry.Id);
            return MapToDTO(created!);
        }

        public async Task<EnquiryDTO?> UpdateEnquiryAsync(int id, UpdateEnquiryRequest request)
        {
            var enquiry = await _enquiryRepository.GetByIdAsync(id);
            if (enquiry == null)
                return null;

            enquiry.Subject = request.Subject;
            enquiry.Description = request.Description;
            enquiry.Notes = request.Notes;
            enquiry.ExpectedDate = request.ExpectedDate;
            enquiry.FollowupDate = request.FollowupDate;
            enquiry.MetalType = request.MetalType;
            enquiry.JewelryType = request.JewelryType;
            enquiry.Purity = request.Purity;
            enquiry.EstimatedWeight = request.EstimatedWeight;
            enquiry.MakingType = request.MakingType;
            enquiry.Status = request.Status;
            enquiry.DesignImage = request.DesignImage;
            enquiry.DesignImage2 = request.DesignImage2;
            enquiry.DesignImage3 = request.DesignImage3;
            enquiry.ConvertedToOrderId = request.ConvertedToOrderId;

            await _enquiryRepository.UpdateAsync(enquiry);
            return MapToDTO(enquiry);
        }

        private EnquiryDTO MapToDTO(Enquiry enquiry)
        {
            return new EnquiryDTO
            {
                Id = enquiry.Id,
                CustomerId = enquiry.CustomerId,
                CustomerName = enquiry.Customer?.Name ?? "",
                Subject = enquiry.Subject,
                Description = enquiry.Description,
                Notes = enquiry.Notes,
                ExpectedDate = enquiry.ExpectedDate,
                FollowupDate = enquiry.FollowupDate,
                MetalType = enquiry.MetalType,
                JewelryType = enquiry.JewelryType,
                Purity = enquiry.Purity,
                EstimatedWeight = enquiry.EstimatedWeight,
                MakingType = enquiry.MakingType,
                Status = enquiry.Status,
                DesignImage = enquiry.DesignImage,
                DesignImage2 = enquiry.DesignImage2,
                DesignImage3 = enquiry.DesignImage3,
                ConvertedToOrderId = enquiry.ConvertedToOrderId,
                CreatedAt = enquiry.CreatedAt
            };
        }
    }

    public interface IWorkerService
    {
        Task<IEnumerable<WorkerDTO>> GetAllWorkersAsync(bool includeInactive = false);
        Task<WorkerDTO?> GetWorkerByIdAsync(int id);
        Task<WorkerDTO> CreateWorkerAsync(CreateWorkerRequest request);
        Task<WorkerDTO?> UpdateWorkerAsync(int id, UpdateWorkerRequest request);
        Task<bool> DeleteWorkerAsync(int id);
    }

    public class WorkerService : IWorkerService
    {
        private readonly IWorkerRepository _workerRepository;

        public WorkerService(IWorkerRepository workerRepository)
        {
            _workerRepository = workerRepository;
        }

        public async Task<IEnumerable<WorkerDTO>> GetAllWorkersAsync(bool includeInactive = false)
        {
            var workers = await _workerRepository.GetAllAsync(includeInactive);
            return workers.Select(w => MapToDTO(w));
        }

        public async Task<WorkerDTO?> GetWorkerByIdAsync(int id)
        {
            var worker = await _workerRepository.GetByIdAsync(id);
            return worker != null ? MapToDTO(worker) : null;
        }

        public async Task<WorkerDTO> CreateWorkerAsync(CreateWorkerRequest request)
        {
            var worker = new Worker
            {
                Name = request.Name,
                Mobile = request.Mobile,
                WorkerType = request.WorkerType,
                IsActive = request.IsActive
            };

            // Hash password for External workers if provided
            if (request.WorkerType == "External" && !string.IsNullOrEmpty(request.Password))
            {
                worker.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _workerRepository.CreateAsync(worker);
            return MapToDTO(worker);
        }

        public async Task<WorkerDTO?> UpdateWorkerAsync(int id, UpdateWorkerRequest request)
        {
            var worker = await _workerRepository.GetByIdAsync(id);
            if (worker == null)
                return null;

            worker.Name = request.Name;
            worker.Mobile = request.Mobile;
            worker.WorkerType = request.WorkerType;
            worker.IsActive = request.IsActive;

            // Hash and update password for External workers if provided
            if (request.WorkerType == "External" && !string.IsNullOrEmpty(request.Password))
            {
                worker.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }
            // If changing from External to Internal, clear the password
            else if (request.WorkerType == "Internal")
            {
                worker.PasswordHash = null;
            }

            await _workerRepository.UpdateAsync(worker);
            return MapToDTO(worker);
        }

        public async Task<bool> DeleteWorkerAsync(int id)
        {
            var worker = await _workerRepository.GetByIdAsync(id);
            if (worker == null)
                return false;

            await _workerRepository.DeleteAsync(id);
            return true;
        }

        private WorkerDTO MapToDTO(Worker worker)
        {
            return new WorkerDTO
            {
                Id = worker.Id,
                Name = worker.Name,
                Mobile = worker.Mobile,
                WorkerType = worker.WorkerType,
                IsActive = worker.IsActive,
                CreatedAt = worker.CreatedAt
            };
        }
    }

    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync(bool includeInactive = false);
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<ProductDTO> CreateProductAsync(CreateProductRequest request);
        Task<ProductDTO?> UpdateProductAsync(int id, ProductDTO request);
        Task DeleteProductAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(bool includeInactive = false)
        {
            var products = await _productRepository.GetAllAsync(includeInactive);
            return products.Select(p => MapToDTO(p));
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? MapToDTO(product) : null;
        }

        public async Task<ProductDTO> CreateProductAsync(CreateProductRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                JewelryType = request.JewelryType,
                Category = request.Category,
                MetalType = request.MetalType,
                Purity = request.Purity,
                Weight = request.Weight,
                Wastage = request.Wastage,
                MakingCharges = request.MakingCharges,
                Gst = request.Gst,
                Quantity = request.Quantity,
                StoneDetails = request.StoneDetails,
                Description = request.Description,
                Image1 = request.Image1,
                Image2 = request.Image2,
                Image3 = request.Image3,
                IsActive = true
            };

            await _productRepository.CreateAsync(product);
            return MapToDTO(product);
        }

        public async Task<ProductDTO?> UpdateProductAsync(int id, ProductDTO request)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            product.Name = request.Name;
            product.JewelryType = request.JewelryType;
            product.Category = request.Category;
            product.MetalType = request.MetalType;
            product.Purity = request.Purity;
            product.Weight = request.Weight;
            product.Wastage = request.Wastage;
            product.MakingCharges = request.MakingCharges;
            product.Gst = request.Gst;
            product.Quantity = request.Quantity;
            product.StoneDetails = request.StoneDetails;
            product.Description = request.Description;
            product.Image1 = request.Image1;
            product.Image2 = request.Image2;
            product.Image3 = request.Image3;
            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return MapToDTO(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }

        private ProductDTO MapToDTO(Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                JewelryType = product.JewelryType,
                Category = product.Category,
                MetalType = product.MetalType,
                Purity = product.Purity,
                Weight = product.Weight,
                Wastage = product.Wastage,
                MakingCharges = product.MakingCharges,
                Gst = product.Gst,
                Quantity = product.Quantity,
                StoneDetails = product.StoneDetails,
                Description = product.Description,
                Image1 = product.Image1,
                Image2 = product.Image2,
                Image3 = product.Image3,
                IsActive = product.IsActive
            };
        }
    }
}
