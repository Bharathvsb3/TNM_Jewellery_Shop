using JewelleryShopAPI.DTOs;
using JewelleryShopAPI.Models;
using JewelleryShopAPI.Repositories;

namespace JewelleryShopAPI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDTO>> GetOrdersByCustomerAsync(int customerId);
        Task<OrderDTO?> CreateOrderAsync(CreateOrderRequest request);
        Task<OrderDTO?> UpdateOrderAsync(int id, UpdateOrderRequest request);
        Task<ExchangeOrderResponseDTO?> ExchangeOrderAsync(int originalOrderId, ExchangeOrderRequest request);
        
        // Customer Portal Methods
        Task<OrderDTO?> CreateCustomerOrderAsync(int userId, CustomerOrderRequest request);
        Task<IEnumerable<OrderDTO>> GetCustomerOrdersAsync(int userId);
        Task<OrderTrackingDTO?> GetOrderTrackingAsync(int orderId, int userId);
        Task<bool> ApproveOrderAsync(int orderId, int approvedById, ApproveOrderRequest request);
        
        // Reporting Methods
        Task<IEnumerable<CustomerOutstandingReportDTO>> GetCustomerOutstandingReportAsync(CustomerOutstandingRequestDTO request);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGoldRateRepository _goldRateRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderStatusHistoryRepository _statusHistoryRepository;

        public OrderService(
            IOrderRepository orderRepository, 
            IGoldRateRepository goldRateRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IOrderStatusHistoryRepository statusHistoryRepository)
        {
            _orderRepository = orderRepository;
            _goldRateRepository = goldRateRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _statusHistoryRepository = statusHistoryRepository;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(o => MapToDTO(o));
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order != null ? MapToDTO(order) : null;
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByCustomerAsync(int customerId)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
            return orders.Select(o => MapToDTO(o));
        }

        public async Task<OrderDTO?> CreateOrderAsync(CreateOrderRequest request)
        {
            // NOTE: Gold/Silver rates are NOT applied at order creation
            // Rates will be calculated at delivery time based on current market rates
            
            // Allow 0 advance amount for orders created from enquiries
            // Advance can be updated later before delivery
            
            var order = new Order
            {
                CustomerId = request.CustomerId,
                EnquiryId = request.EnquiryId,
                JewelryType = request.JewelryType,
                Category = request.Category,
                MetalType = request.MetalType,
                Purity = request.Purity,
                GrossWeight = request.GrossWeight,
                Wastage = request.Wastage,
                MakingCharges = request.MakingCharges,
                Gst = request.Gst,
                StoneDetails = request.StoneDetails,
                EstimatedWeight = request.EstimatedWeight,
                GoldRate = 0, // Will be set at delivery time
                TotalPrice = 0, // Will be calculated at delivery time
                AdvanceAmount = request.AdvanceAmount,
                BalanceAmount = 0 - request.AdvanceAmount, // Negative balance until final calculation
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                DesignImage = request.DesignImage,
                DesignImage2 = request.DesignImage2,
                DesignImage3 = request.DesignImage3
            };

            await _orderRepository.CreateAsync(order);

            // Reload with customer
            var created = await _orderRepository.GetByIdAsync(order.Id);
            return MapToDTO(created!);
        }

        public async Task<OrderDTO?> UpdateOrderAsync(int id, UpdateOrderRequest request)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return null;

            // NOTE: Rates are NOT automatically recalculated on update
            // They will be applied at delivery time
            // Only update if status is being changed to 'Delivered' or if explicitly needed

            order.JewelryType = request.JewelryType;
            order.Category = request.Category;
            order.MetalType = request.MetalType;
            order.Purity = request.Purity;
            order.GrossWeight = request.GrossWeight;
            order.Wastage = request.Wastage;
            order.MakingCharges = request.MakingCharges;
            order.Gst = request.Gst;
            order.StoneDetails = request.StoneDetails;
            order.EstimatedWeight = request.EstimatedWeight;
            order.AdvanceAmount = request.AdvanceAmount;
            order.Status = request.Status;
            
            // Update design images (allow null/empty to remove images)
            // Only skip update if the request field is not present at all
            order.DesignImage = request.DesignImage;
            order.DesignImage2 = request.DesignImage2;
            order.DesignImage3 = request.DesignImage3;
            
            // Update delivery fields if provided
            if (request.ActualWeight.HasValue)
                order.ActualWeight = request.ActualWeight.Value;
            
            if (request.WastageGrams.HasValue)
                order.WastageGrams = request.WastageGrams.Value;
            
            if (request.WastagePercent.HasValue)
                order.WastagePercent = request.WastagePercent.Value;
            
            if (request.AdjustmentAmount.HasValue)
                order.AdjustmentAmount = request.AdjustmentAmount.Value;
            
            if (!string.IsNullOrEmpty(request.DeliveryNotes))
                order.DeliveryNotes = request.DeliveryNotes;
            
            if (request.DeliveryDate.HasValue)
                order.DeliveryDate = request.DeliveryDate.Value;
            
            if (request.GoldRate.HasValue)
                order.GoldRate = request.GoldRate.Value;
            
            if (request.TotalPrice.HasValue)
                order.TotalPrice = request.TotalPrice.Value;
            
            if (request.BalanceAmount.HasValue)
                order.BalanceAmount = request.BalanceAmount.Value;

            // If order is being delivered, calculate final price with current rates
            if (request.Status == "Delivered" && order.GoldRate == 0)
            {
                var goldRate = await _goldRateRepository.GetLatestAsync();
                if (goldRate != null)
                {
                    decimal rateToUse;
                    if (order.MetalType == "Gold")
                    {
                        rateToUse = order.Purity == "916" ? goldRate.Gold916Rate : goldRate.GoldNormalRate;
                    }
                    else if (order.MetalType == "Silver")
                    {
                        rateToUse = goldRate.SilverRate;
                    }
                    else
                    {
                        rateToUse = 0; // Platinum or other - manual rate entry needed
                    }

                    // Calculate total: (weight * rate) + making charges
                    decimal metalCost = order.EstimatedWeight * rateToUse;
                    decimal totalBeforeGst = metalCost + order.MakingCharges;
                    
                    // Apply GST if provided
                    decimal gstAmount = 0;
                    if (order.Gst.HasValue && order.Gst.Value > 0)
                    {
                        gstAmount = (totalBeforeGst * order.Gst.Value) / 100;
                    }
                    
                    decimal totalPrice = totalBeforeGst + gstAmount;

                    order.GoldRate = rateToUse;
                    order.TotalPrice = totalPrice;
                    order.BalanceAmount = totalPrice - order.AdvanceAmount;
                }
            }

            await _orderRepository.UpdateAsync(order);
            return MapToDTO(order);
        }

        public async Task<OrderDTO?> CreateCustomerOrderAsync(int userId, CustomerOrderRequest request)
        {
            // Verify user is a customer
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Role != "CUSTOMER")
                return null;

            // Find the customer record using the user's mobile
            var customer = await _customerRepository.GetByMobileAsync(user.Mobile);
            if (customer == null)
                return null; // Customer record must exist

            // Get product if ProductId is provided
            Product? product = null;
            if (request.ProductId.HasValue)
            {
                product = await _productRepository.GetByIdAsync(request.ProductId.Value);
                if (product == null || !product.IsActive)
                    return null;
            }
            else
            {
                return null; // Product is required
            }

            // Get latest gold rate
            var goldRate = await _goldRateRepository.GetLatestAsync();
            if (goldRate == null)
                return null; // Gold rate must be entered

            // Use custom weight or product weight
            decimal estimatedWeight = request.CustomWeight ?? product.Weight;

            // Determine rate based on product metal type
            decimal rateToUse = product.MetalType == "Gold" ? goldRate.Gold916Rate : goldRate.SilverRate;

            // Calculate total price (simplified calculation - can be enhanced)
            decimal totalPrice = estimatedWeight * rateToUse;

            var order = new Order
            {
                CustomerId = customer.Id, // Use actual customer ID, not user ID
                ProductId = product.Id,
                JewelryType = product.Name,
                Category = product.Category,
                MetalType = product.MetalType,
                Purity = "916",
                EstimatedWeight = estimatedWeight,
                GoldRate = rateToUse,
                TotalPrice = totalPrice,
                AdvanceAmount = 0,
                BalanceAmount = totalPrice,
                CustomerRemarks = request.CustomerRemarks,
                ApprovalStatus = "Pending",
                Status = "Ordered",
                OrderDate = DateTime.UtcNow
            };

            await _orderRepository.CreateAsync(order);

            // Reload with customer
            var created = await _orderRepository.GetByIdAsync(order.Id);
            return MapToDTO(created!);
        }

        public async Task<IEnumerable<OrderDTO>> GetCustomerOrdersAsync(int userId)
        {
            // Find the customer record using the user's mobile
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Enumerable.Empty<OrderDTO>();

            var customer = await _customerRepository.GetByMobileAsync(user.Mobile);
            if (customer == null)
                return Enumerable.Empty<OrderDTO>();

            var orders = await _orderRepository.GetByCustomerIdAsync(customer.Id);
            return orders.Select(o => MapToDTO(o));
        }

        public async Task<OrderTrackingDTO?> GetOrderTrackingAsync(int orderId, int userId)
        {
            // Find the customer record using the user's mobile
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            var customer = await _customerRepository.GetByMobileAsync(user.Mobile);
            if (customer == null)
                return null;

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.CustomerId != customer.Id)
                return null;

            // Build tracking steps
            var steps = new List<TrackingStep>
            {
                new TrackingStep
                {
                    StepName = "Order Requested",
                    Status = "Completed",
                    CompletedAt = order.OrderDate
                }
            };

            // Approval step
            if (order.ApprovalStatus == "Approved")
            {
                steps.Add(new TrackingStep
                {
                    StepName = "Approved",
                    Status = "Completed",
                    CompletedAt = order.ApprovalDate
                });
            }
            else if (order.ApprovalStatus == "Rejected")
            {
                steps.Add(new TrackingStep
                {
                    StepName = "Rejected",
                    Status = "Completed",
                    CompletedAt = order.ApprovalDate
                });
                
                return new OrderTrackingDTO
                {
                    Id = order.Id,
                    JewelryType = order.JewelryType,
                    Category = order.Category,
                    MetalType = order.MetalType,
                    TotalPrice = order.TotalPrice,
                    ApprovalStatus = order.ApprovalStatus,
                    ApprovalDate = order.ApprovalDate,
                    Status = order.Status,
                    OrderDate = order.OrderDate,
                    CompletionDate = order.CompletionDate,
                    TrackingSteps = steps.ToArray()
                };
            }
            else
            {
                steps.Add(new TrackingStep
                {
                    StepName = "Waiting for Approval",
                    Status = "Current",
                    CompletedAt = null
                });
            }

            // Only show workflow steps if approved
            if (order.ApprovalStatus == "Approved")
            {
                var statusSteps = new[]
                {
                    ("Assigned", "Assigned"),
                    ("In Progress", "InProgress"),
                    ("Completed", "Completed"),
                    ("Ready for Pickup", "Ready"),
                    ("Delivered", "Delivered")
                };

                foreach (var (stepName, statusValue) in statusSteps)
                {
                    var stepStatus = order.Status == statusValue ? "Current" :
                                   IsStatusBefore(order.Status, statusValue) ? "Completed" : "Pending";
                    
                    steps.Add(new TrackingStep
                    {
                        StepName = stepName,
                        Status = stepStatus,
                        CompletedAt = stepStatus == "Completed" ? order.UpdatedAt : null
                    });
                }
            }

            return new OrderTrackingDTO
            {
                Id = order.Id,
                JewelryType = order.JewelryType,
                Category = order.Category,
                MetalType = order.MetalType,
                TotalPrice = order.TotalPrice,
                ApprovalStatus = order.ApprovalStatus,
                ApprovalDate = order.ApprovalDate,
                Status = order.Status,
                OrderDate = order.OrderDate,
                CompletionDate = order.CompletionDate,
                TrackingSteps = steps.ToArray()
            };
        }

        public async Task<bool> ApproveOrderAsync(int orderId, int approvedById, ApproveOrderRequest request)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return false;

            order.ApprovalStatus = request.IsApproved ? "Approved" : "Rejected";
            order.ApprovedById = approvedById;
            order.ApprovalDate = DateTime.UtcNow;
            
            if (request.IsApproved)
            {
                order.Status = "Approved";
            }
            else
            {
                order.Status = "Rejected";
                order.CustomerRemarks = request.RejectionReason;
            }

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        private bool IsStatusBefore(string currentStatus, string targetStatus)
        {
            var statusOrder = new[] { "Ordered", "Approved", "Assigned", "InProgress", "Completed", "Ready", "Delivered" };
            var currentIndex = Array.IndexOf(statusOrder, currentStatus);
            var targetIndex = Array.IndexOf(statusOrder, targetStatus);
            return currentIndex > targetIndex && currentIndex != -1 && targetIndex != -1;
        }

        public async Task<IEnumerable<CustomerOutstandingReportDTO>> GetCustomerOutstandingReportAsync(CustomerOutstandingRequestDTO request)
        {
            // Get all delivered orders
            var allOrders = await _orderRepository.GetAllAsync();
            var deliveredOrders = allOrders.Where(o => o.Status == "Delivered").ToList();

            // Filter by customer range if provided
            if (request.StartCustomerId.HasValue)
            {
                deliveredOrders = deliveredOrders.Where(o => o.CustomerId >= request.StartCustomerId.Value).ToList();
            }
            if (request.EndCustomerId.HasValue)
            {
                deliveredOrders = deliveredOrders.Where(o => o.CustomerId <= request.EndCustomerId.Value).ToList();
            }

            // Group by customer
            var groupedByCustomer = deliveredOrders.GroupBy(o => o.CustomerId);

            var reportData = new List<CustomerOutstandingReportDTO>();

            foreach (var customerGroup in groupedByCustomer)
            {
                var customerOrders = customerGroup.ToList();
                var firstOrder = customerOrders.First();

                // Calculate totals
                decimal totalAmount = 0;
                decimal totalPaid = 0;
                decimal gstAmount = 0;
                decimal wastageGrams = 0;
                decimal makingCharges = 0;

                var orderDTOs = new List<OrderDTO>();

                foreach (var order in customerOrders)
                {
                    // Calculate balance after adjustment
                    decimal adjustmentAmount = order.AdjustmentAmount ?? 0;
                    decimal orderPaid = 0;
                    
                    // Calculate how much was actually paid for this order
                    if (adjustmentAmount > 0)
                    {
                        // Positive adjustment = pending payment
                        orderPaid = order.TotalPrice - adjustmentAmount;
                    }
                    else if (adjustmentAmount < 0)
                    {
                        // Negative adjustment = waiver/discount, considered fully paid
                        orderPaid = order.TotalPrice;
                    }
                    else
                    {
                        // No adjustment (old format or exact payment)
                        orderPaid = order.AdvanceAmount;
                    }

                    totalAmount += order.TotalPrice;
                    totalPaid += orderPaid;

                    if (order.Gst.HasValue && order.Gst.Value > 0)
                    {
                        // Calculate GST amount from total
                        gstAmount += (order.TotalPrice * order.Gst.Value) / (100 + order.Gst.Value);
                    }

                    wastageGrams += order.WastageGrams ?? 0;
                    makingCharges += order.MakingCharges;

                    orderDTOs.Add(MapToDTO(order));
                }

                // Calculate actual outstanding (only positive balances)
                decimal outstanding = 0;
                foreach (var order in customerOrders)
                {
                    decimal adjustmentAmount = order.AdjustmentAmount ?? 0;
                    if (adjustmentAmount > 0)
                    {
                        outstanding += adjustmentAmount;
                    }
                }

                // Only include customers with outstanding balance > 0
                if (outstanding > 0)
                {
                    reportData.Add(new CustomerOutstandingReportDTO
                    {
                        CustomerId = firstOrder.CustomerId,
                        CustomerName = firstOrder.Customer?.Name ?? "",
                        CustomerMobile = firstOrder.Customer?.Mobile ?? "",
                        TotalOrders = customerOrders.Count,
                        TotalAmount = totalAmount,
                        TotalPaid = totalPaid,
                        Outstanding = outstanding,
                        GstAmount = gstAmount,
                        WastageGrams = wastageGrams,
                        MakingCharges = makingCharges,
                        Orders = orderDTOs
                    });
                }
            }

            return reportData.OrderBy(r => r.CustomerName);
        }

        public async Task<ExchangeOrderResponseDTO?> ExchangeOrderAsync(int originalOrderId, ExchangeOrderRequest request)
        {
            var oldOrder = await _orderRepository.GetByIdAsync(originalOrderId);
            if (oldOrder == null || oldOrder.Status != "Delivered")
                return null;

            // Calculate exchange credit:
            // If ExchangeCreditOverride > 0, use that (= total paid by customer for wrong-item exchange)
            // Otherwise fall back to metal value (weight × rate)
            decimal oldItemWeight = oldOrder.ActualWeight ?? oldOrder.GrossWeight;
            decimal metalValue = oldItemWeight * request.ExchangeRateForOldItem;
            decimal exchangeCredit = request.ExchangeCreditOverride > 0
                ? request.ExchangeCreditOverride
                : metalValue;

            // Calculate new item pricing
            decimal newMetalCost = request.GrossWeight * request.ExchangeRateForNewItem;
            decimal newWastageAmt = request.Wastage * request.ExchangeRateForNewItem;
            decimal newSubtotal = newMetalCost + newWastageAmt + request.MakingCharges;
            decimal newGstAmt = request.Gst > 0 ? (newSubtotal * request.Gst / 100) : 0;
            decimal newTotalPrice = newSubtotal + newGstAmt;

            // Advance for new order = exchange credit + any extra cash paid now
            decimal totalAdvance = exchangeCredit + request.ExtraAdvancePaid;

            // If old order had unpaid balance, customer must still settle it on the new order
            decimal oldUnpaidBalance = request.OldOrderUnpaidBalance > 0 ? request.OldOrderUnpaidBalance : 0;

            // Balance due on new order = new total + old unpaid balance - total advance
            decimal balanceDue = newTotalPrice + oldUnpaidBalance - totalAdvance;

            // Build exchange reference note for old order
            string exchangeNote = $"[EXCHANGED on {DateTime.Now:dd/MM/yyyy}] → New item: {request.JewelryType} ({request.GrossWeight}g {request.MetalType} {request.Purity}). Old item credit: ₹{exchangeCredit:N2} ({oldItemWeight}g @ ₹{request.ExchangeRateForOldItem:N2}/g). {(oldUnpaidBalance > 0 ? $"Old balance carried forward: ₹{oldUnpaidBalance:N2}. " : "")}{request.Notes}".Trim();

            // Mark old order with exchange reference and link to new order
            oldOrder.DeliveryNotes = string.IsNullOrEmpty(oldOrder.DeliveryNotes)
                ? exchangeNote
                : $"{oldOrder.DeliveryNotes}\n{exchangeNote}";
            // ExchangedToOrderId will be set after new order is created below
            await _orderRepository.UpdateAsync(oldOrder);

            // Create new order as Delivered with locked calculations
            var newOrder = new Order
            {
                CustomerId = oldOrder.CustomerId,
                JewelryType = request.JewelryType,
                Category = string.IsNullOrEmpty(request.Category) ? request.JewelryType : request.Category,
                MetalType = request.MetalType,
                Purity = request.Purity,
                GrossWeight = request.GrossWeight,
                Wastage = request.Wastage,
                MakingCharges = request.MakingCharges,
                Gst = request.Gst,
                StoneDetails = $"Exchanged from Order #{originalOrderId}",
                EstimatedWeight = request.GrossWeight,
                GoldRate = request.ExchangeRateForNewItem,
                TotalPrice = newTotalPrice,
                AdvanceAmount = totalAdvance,
                BalanceAmount = balanceDue > 0 ? balanceDue : 0,
                AdjustmentAmount = balanceDue < 0 ? balanceDue : 0,  // negative = shop owes refund
                ActualWeight = request.GrossWeight,
                WastageGrams = request.Wastage,
                WastagePercent = request.GrossWeight > 0 ? (request.Wastage / request.GrossWeight * 100) : 0,
                Status = "Pending",  // New order starts as Pending; shop delivers once item is made
                OrderDate = DateTime.UtcNow,
                DeliveryNotes = $"Exchange order. Credit from Order #{originalOrderId}: ₹{exchangeCredit:N2}. Extra advance paid: ₹{request.ExtraAdvancePaid:N2}.{(oldUnpaidBalance > 0 ? $" Old unpaid balance carried: ₹{oldUnpaidBalance:N2}." : "")} {request.Notes}".Trim()
            };

            await _orderRepository.CreateAsync(newOrder);
            var createdNewOrder = await _orderRepository.GetByIdAsync(newOrder.Id);

            // Now link old order to the new exchange order and mark it as Exchanged
            oldOrder.ExchangedToOrderId = newOrder.Id;
            oldOrder.Status = "Exchanged";
            await _orderRepository.UpdateAsync(oldOrder);

            return new ExchangeOrderResponseDTO
            {
                OriginalOrderId = originalOrderId,
                NewOrderId = newOrder.Id,
                OldItemWeight = oldItemWeight,
                OldItemRate = request.ExchangeRateForOldItem,
                ExchangeCredit = exchangeCredit,
                NewItemTotal = newTotalPrice,
                ExtraAdvancePaid = request.ExtraAdvancePaid,
                BalanceDue = balanceDue,
                NewOrder = MapToDTO(createdNewOrder!)
            };
        }

        private OrderDTO MapToDTO(Order order)
        {
            return new OrderDTO
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.Name ?? "",
                CustomerMobile = order.Customer?.Mobile ?? "",
                CustomerEmail =  "",
                EnquiryId = order.EnquiryId,
                JewelryType = order.JewelryType,
                Category = order.Category,
                MetalType = order.MetalType,
                Purity = order.Purity,
                GrossWeight = order.GrossWeight,
                Wastage = order.Wastage,
                MakingCharges = order.MakingCharges,
                Gst = order.Gst,
                StoneDetails = order.StoneDetails,
                EstimatedWeight = order.EstimatedWeight,
                GoldRate = order.GoldRate,
                TotalPrice = order.TotalPrice,
                AdvanceAmount = order.AdvanceAmount,
                BalanceAmount = order.BalanceAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                CompletionDate = order.CompletionDate,
                
                // Design Images
                DesignImage = order.DesignImage,
                DesignImage2 = order.DesignImage2,
                DesignImage3 = order.DesignImage3,
                
                // Delivery fields
                ActualWeight = order.ActualWeight,
                WastageGrams = order.WastageGrams,
                WastagePercent = order.WastagePercent,
                AdjustmentAmount = order.AdjustmentAmount,
                DeliveryNotes = order.DeliveryNotes,
                DeliveryDate = order.DeliveryDate,
                ExchangedToOrderId = order.ExchangedToOrderId
            };
        }
    }
}
