using JewelleryShopAPI.DTOs;
using JewelleryShopAPI.Models;
using JewelleryShopAPI.Repositories;

namespace JewelleryShopAPI.Services
{
    public interface ICraftWorkflowService
    {
        Task<IEnumerable<CraftAssignmentDTO>> GetAllAssignmentsAsync();
        Task<CraftAssignmentDTO?> GetAssignmentByIdAsync(int id);
        Task<IEnumerable<CraftAssignmentDTO>> GetWorkerAssignmentsAsync(int workerId);
        Task<CraftAssignmentDTO?> AssignCraftAsync(AssignCraftRequest request);
        Task<CraftAssignmentDTO?> StartWorkAsync(int id, StartWorkRequest request);
        Task<CraftCompletionDTO?> CompleteWorkAsync(int id, CompleteWorkRequest request);
        Task<CraftAssignmentDTO?> CompleteCraftAsync(int id, CompleteCraftRequest request);
        Task<CraftAssignmentDTO?> ConfirmCompletionAsync(int id);
        Task<CraftAssignmentDTO?> UpdateCompletionAsync(int id, CompleteCraftRequest request);
    }

    public class CraftWorkflowService : ICraftWorkflowService
    {
        private readonly ICraftAssignmentRepository _craftRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMaterialDetailRepository _materialRepository;
        private readonly ICraftCompletionRepository _completionRepository;

        public CraftWorkflowService(
            ICraftAssignmentRepository craftRepository, 
            IOrderRepository orderRepository,
            IMaterialDetailRepository materialRepository,
            ICraftCompletionRepository completionRepository)
        {
            _craftRepository = craftRepository;
            _orderRepository = orderRepository;
            _materialRepository = materialRepository;
            _completionRepository = completionRepository;
        }

        public async Task<IEnumerable<CraftAssignmentDTO>> GetAllAssignmentsAsync()
        {
            try
            {
                var assignments = await _craftRepository.GetAllAsync();
                return assignments.Select(a => MapToDTO(a)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAssignmentsAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<CraftAssignmentDTO?> GetAssignmentByIdAsync(int id)
        {
            var assignment = await _craftRepository.GetByIdAsync(id);
            return assignment != null ? MapToDTO(assignment) : null;
        }

        public async Task<IEnumerable<CraftAssignmentDTO>> GetWorkerAssignmentsAsync(int workerId)
        {
            var assignments = await _craftRepository.GetByWorkerIdAsync(workerId);
            return assignments.Select(a => MapToDTO(a));
        }

        public async Task<CraftAssignmentDTO?> AssignCraftAsync(AssignCraftRequest request)
        {
            try
            {
                // Validate order exists
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                    return null; // Order not found

                // Check if order already assigned
                var existing = await _craftRepository.GetByOrderIdAsync(request.OrderId);
                if (existing != null)
                {
                    // Allow reassignment only if status is Completed
                    if (existing.Status == "Assigned" || existing.Status == "InProgress")
                        return null; // Order currently being worked on
                    // If completed, we can reassign - no return here
                }

                var assignment = new CraftAssignment
                {
                    OrderId = request.OrderId,
                    WorkerId = request.WorkerId,
                    AssignedDate = request.AssignedDate,
                    RawGoldGiven = request.RawGoldGiven,
                    StoneGiven = request.StoneGiven,
                    Status = "Assigned"
                };

            await _craftRepository.CreateAsync(assignment);

            // Add stone details if provided
            if (request.StoneGiven && request.StoneDetails != null && request.StoneDetails.Any())
            {
                assignment.StoneDetails = request.StoneDetails.Select(sd => new StoneDetail
                {
                    CraftAssignmentId = assignment.Id,
                    StoneType = sd.StoneType,
                    Weight = sd.Weight
                }).ToList();

                await _craftRepository.UpdateAsync(assignment);
            }

            // Add material details if provided
            if (request.MaterialDetails != null && request.MaterialDetails.Any())
            {
                foreach (var material in request.MaterialDetails)
                {
                    var materialDetail = new MaterialDetail
                    {
                        CraftAssignmentId = assignment.Id,
                        MaterialType = material.MaterialType,
                        Description = material.Description,
                        Weight = material.Weight,
                        Unit = material.Unit,
                        EstimatedValue = material.EstimatedValue,
                        Notes = material.Notes
                    };
                    await _materialRepository.CreateAsync(materialDetail);
                }
            }

            // Update order status
             order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order != null)
            {
                order.Status = "Assigned";
                await _orderRepository.UpdateAsync(order);
            }

                // Reload with all data
                var created = await _craftRepository.GetByIdAsync(assignment.Id);
                return MapToDTO(created!);
            }
            catch (Exception ex)
            {
                // Log error and return null
                Console.WriteLine($"Error in AssignCraftAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<CraftAssignmentDTO?> StartWorkAsync(int id, StartWorkRequest request)
        {
            var assignment = await _craftRepository.GetByIdAsync(id);
            if (assignment == null)
                return null;

            // Update status to InProgress
            assignment.Status = "InProgress";
            assignment.CraftsmanRemarks = request.InitialRemarks;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _craftRepository.UpdateAsync(assignment);

            // Update order status
            var order = await _orderRepository.GetByIdAsync(assignment.OrderId);
            if (order != null)
            {
                order.Status = "InProgress";
                await _orderRepository.UpdateAsync(order);
            }

            return MapToDTO(assignment);
        }

        public async Task<CraftCompletionDTO?> CompleteWorkAsync(int id, CompleteWorkRequest request)
        {
            var assignment = await _craftRepository.GetByIdAsync(id);
            if (assignment == null)
                return null;

            // Calculate total raw materials given
            decimal totalRawGiven = assignment.RawGoldGiven;
            
            // Add weights from MaterialDetails if any
            if (assignment.MaterialDetails != null && assignment.MaterialDetails.Any())
            {
                totalRawGiven = assignment.MaterialDetails.Sum(m => m.Weight);
            }

            // Calculate system return (Total Given - Final Product)
            decimal systemReturn = totalRawGiven - request.FinalWeight;

            // Calculate difference if actual return is provided
            decimal? returnDifference = null;
            if (request.ActualReturnWeight.HasValue)
            {
                returnDifference = systemReturn - request.ActualReturnWeight.Value;
            }

            // Create CraftCompletion record
            var completion = new CraftCompletion
            {
                OrderId = assignment.OrderId,
                CraftAssignmentId = assignment.Id,
                FinalWeight = request.FinalWeight,
                TotalRawGiven = totalRawGiven,
                SystemCalculatedReturn = systemReturn,
                ActualReturnWeight = request.ActualReturnWeight,
                ReturnDifference = returnDifference,
                CompletionRemarks = request.CompletionRemarks,
                ReturnRemarks = request.ReturnRemarks,
                CompletedAt = DateTime.UtcNow,
                IsVerified = false
            };

            await _completionRepository.CreateAsync(completion);

            // Update assignment
            assignment.Status = "Completed";
            assignment.FinalWeight = request.FinalWeight;
            assignment.ReturnWeight = request.ActualReturnWeight;
            assignment.CompletionDate = DateTime.UtcNow;
            assignment.PendingAdminApproval = request.ActualReturnWeight.HasValue && returnDifference != 0;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _craftRepository.UpdateAsync(assignment);

            // Update order status
            var order = await _orderRepository.GetByIdAsync(assignment.OrderId);
            if (order != null)
            {
                order.Status = "Completed";
                order.CompletionDate = DateTime.UtcNow;
                await _orderRepository.UpdateAsync(order);
            }

            return MapToCraftCompletionDTO(completion);
        }

        public async Task<CraftAssignmentDTO?> CompleteCraftAsync(int id, CompleteCraftRequest request)
        {
            var assignment = await _craftRepository.GetByIdAsync(id);
            if (assignment == null)
                return null;

            // Calculate used gold and balance
            decimal usedGold = request.FinalWeight - (request.StoneWeight + request.OtherMaterialWeight);
            decimal balanceGold = assignment.RawGoldGiven - usedGold;

            assignment.CompletionDate = request.CompletionDate;
            assignment.FinalWeight = request.FinalWeight;
            assignment.StoneWeight = request.StoneWeight;
            assignment.OtherMaterialWeight = request.OtherMaterialWeight;
            assignment.UsedGold = usedGold;
            assignment.BalanceGold = balanceGold;
            assignment.IsVerified = request.IsVerified;
            assignment.ManualUsedGold = request.ManualUsedGold;
            assignment.ManualBalanceGold = request.ManualBalanceGold;
            assignment.MaterialReturn = request.MaterialReturn;
            assignment.StoneReturn = request.StoneReturn;
            assignment.ReturnRemarks = request.ReturnRemarks;
            assignment.AdminRemarks = request.AdminRemarks;
            assignment.Status = "Completed";

            await _craftRepository.UpdateAsync(assignment);

            // Update order status
            var order = await _orderRepository.GetByIdAsync(assignment.OrderId);
            if (order != null)
            {
                order.Status = "Completed";
                order.CompletionDate = request.CompletionDate;
                await _orderRepository.UpdateAsync(order);
            }

            return MapToDTO(assignment);
        }

        private CraftAssignmentDTO MapToDTO(CraftAssignment assignment)
        {
            try
            {
                return new CraftAssignmentDTO
                {
                    Id = assignment.Id,
                    OrderId = assignment.OrderId,
                    WorkerId = assignment.WorkerId,
                    WorkerName = assignment.Worker?.Name ?? "Unknown Worker",
                    CustomerName = assignment.Order?.Customer?.Name ?? "Unknown Customer",
                    JewelryType = assignment.Order?.JewelryType ?? "Unknown Type",
                AssignedDate = assignment.AssignedDate,
                RawGoldGiven = assignment.RawGoldGiven,
                StoneGiven = assignment.StoneGiven,
                CompletionDate = assignment.CompletionDate,
                FinalWeight = assignment.FinalWeight,
                StoneWeight = assignment.StoneWeight,
                OtherMaterialWeight = assignment.OtherMaterialWeight,
                UsedGold = assignment.UsedGold,
                BalanceGold = assignment.BalanceGold,
                IsVerified = assignment.IsVerified,
                ManualUsedGold = assignment.ManualUsedGold,
                ManualBalanceGold = assignment.ManualBalanceGold,
                Status = assignment.Status,
                CraftsmanStatus = assignment.CraftsmanStatus,
                ReturnWeight = assignment.ReturnWeight,
                CraftsmanRemarks = assignment.CraftsmanRemarks,
                PendingAdminApproval = assignment.PendingAdminApproval,
                AdminConfirmed = assignment.AdminConfirmed,
                AdminRemarks = assignment.AdminRemarks,
                MaterialReturn = assignment.MaterialReturn,
                StoneReturn = assignment.StoneReturn,
                ReturnRemarks = assignment.ReturnRemarks,
                
                // Order fields for craftsman display
                MetalType = assignment.Order?.MetalType,
                Purity = assignment.Order?.Purity,
                GrossWeight = assignment.Order?.GrossWeight,
                Category = assignment.Order?.Category,
                ExpectedCompletionDate = assignment.Order?.ExpectedCompletionDate,
                
                // Design images from Order
                DesignImage = assignment.Order?.DesignImage ?? assignment.DesignImage,
                DesignImage2 = assignment.Order?.DesignImage2,
                DesignImage3 = assignment.Order?.DesignImage3,
                
                StoneDetails = assignment.StoneDetails?.Select(sd => new StoneDetailDTO
                {
                    Id = sd.Id,
                    StoneType = sd.StoneType,
                    Weight = sd.Weight
                }).ToList() ?? new List<StoneDetailDTO>(),
                MaterialDetails = assignment.MaterialDetails?.Select(md => new MaterialDetailDTO
                {
                    Id = md.Id,
                    MaterialType = md.MaterialType,
                    Description = md.Description,
                    Weight = md.Weight,
                    Unit = md.Unit,
                    EstimatedValue = md.EstimatedValue,
                    Notes = md.Notes
                }).ToList() ?? new List<MaterialDetailDTO>()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error mapping assignment {assignment.Id}: {ex.Message}");
                throw;
            }
        }

        private CraftCompletionDTO MapToCraftCompletionDTO(CraftCompletion completion)
        {
            return new CraftCompletionDTO
            {
                Id = completion.Id,
                OrderId = completion.OrderId,
                CraftAssignmentId = completion.CraftAssignmentId,
                FinalWeight = completion.FinalWeight,
                TotalRawGiven = completion.TotalRawGiven,
                SystemCalculatedReturn = completion.SystemCalculatedReturn,
                ActualReturnWeight = completion.ActualReturnWeight,
                ReturnDifference = completion.ReturnDifference,
                CompletionRemarks = completion.CompletionRemarks,
                ReturnRemarks = completion.ReturnRemarks,
                CompletedAt = completion.CompletedAt,
                IsVerified = completion.IsVerified,
                VerifiedBy = completion.VerifiedBy,
                VerifiedAt = completion.VerifiedAt,
                VerificationRemarks = completion.VerificationRemarks
            };
        }

        public async Task<CraftAssignmentDTO?> ConfirmCompletionAsync(int id)
        {
            var assignment = await _craftRepository.GetByIdAsync(id);
            if (assignment == null || assignment.Status != "Completed")
                return null;

            assignment.AdminConfirmed = true;
            assignment.PendingAdminApproval = false;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _craftRepository.UpdateAsync(assignment);
            return MapToDTO(assignment);
        }

        public async Task<CraftAssignmentDTO?> UpdateCompletionAsync(int id, CompleteCraftRequest request)
        {
            var assignment = await _craftRepository.GetByIdAsync(id);
            if (assignment == null || assignment.Status != "Completed")
                return null;

            // Recalculate with new values
            decimal usedGold = request.FinalWeight - (request.StoneWeight + request.OtherMaterialWeight);
            decimal balanceGold = assignment.RawGoldGiven - usedGold;

            assignment.CompletionDate = request.CompletionDate;
            assignment.FinalWeight = request.FinalWeight;
            assignment.StoneWeight = request.StoneWeight;
            assignment.OtherMaterialWeight = request.OtherMaterialWeight;
            assignment.UsedGold = usedGold;
            assignment.BalanceGold = balanceGold;
            assignment.IsVerified = request.IsVerified;
            assignment.ManualUsedGold = request.ManualUsedGold;
            assignment.ManualBalanceGold = request.ManualBalanceGold;
            assignment.MaterialReturn = request.MaterialReturn;
            assignment.StoneReturn = request.StoneReturn;
            assignment.ReturnRemarks = request.ReturnRemarks;
            assignment.AdminRemarks = request.AdminRemarks;
            assignment.AdminConfirmed = true;
            assignment.PendingAdminApproval = false;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _craftRepository.UpdateAsync(assignment);

            // Update order if needed
            var order = await _orderRepository.GetByIdAsync(assignment.OrderId);
            if (order != null)
            {
                order.CompletionDate = request.CompletionDate;
                await _orderRepository.UpdateAsync(order);
            }

            return MapToDTO(assignment);
        }
    }
}
