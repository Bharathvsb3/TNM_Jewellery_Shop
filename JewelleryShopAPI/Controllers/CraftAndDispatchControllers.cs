using JewelleryShopAPI.Data;
using JewelleryShopAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JewelleryShopAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GoldDispatchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GoldDispatchController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dispatches = await _context.GoldDispatches
                .OrderByDescending(d => d.DispatchDate)
                .ToListAsync();
            
            return Ok(dispatches);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dispatch = await _context.GoldDispatches.FindAsync(id);
            if (dispatch == null)
                return NotFound();

            return Ok(dispatch);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GoldDispatch dispatch)
        {
            dispatch.DispatchDate = DateTime.UtcNow;
            dispatch.CreatedAt = DateTime.UtcNow;
            dispatch.Status = "pending";

            _context.GoldDispatches.Add(dispatch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = dispatch.Id }, dispatch);
        }

        [HttpPut("{id}/return")]
        public async Task<IActionResult> RecordReturn(int id, [FromBody] GoldDispatchReturnRequest request)
        {
            var dispatch = await _context.GoldDispatches.FindAsync(id);
            if (dispatch == null)
                return NotFound();

            dispatch.ReturnWeight = request.ReturnWeight;
            dispatch.Status = request.Status;
            dispatch.ReturnRemarks = request.ReturnRemarks;
            dispatch.ReturnDate = DateTime.UtcNow;
            dispatch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(dispatch);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dispatch = await _context.GoldDispatches.FindAsync(id);
            if (dispatch == null)
                return NotFound();

            _context.GoldDispatches.Remove(dispatch);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CraftWorkController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CraftWorkController(AppDbContext context)
        {
            _context = context;
        }

        // Admin: Get all craft assignments with approval status
        [HttpGet("admin/pending-approvals")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            var assignments = await _context.CraftAssignments
                .Include(ca => ca.Order)
                    .ThenInclude(o => o.Customer)
                .Include(ca => ca.Worker)
                .Where(ca => ca.PendingAdminApproval)
                .OrderByDescending(ca => ca.UpdateTimestamp)
                .Select(ca => new
                {
                    ca.Id,
                    ca.OrderId,
                    CustomerName = ca.Order.Customer.Name,
                    ProductName = ca.Order.JewelryType,
                    Category = ca.Order.Category,
                    WorkerName = ca.Worker.Name,
                    ca.CraftsmanStatus,
                    ca.ReturnWeight,
                    OriginalWeight = ca.RawGoldGiven,
                    WeightDifference = ca.ReturnWeight.HasValue ? ca.RawGoldGiven - ca.ReturnWeight.Value :  0,
                    ca.CraftsmanRemarks,
                    ca.UpdateTimestamp
                })
                .ToListAsync();

            return Ok(assignments);
        }

        // Admin: Approve or request changes
        [HttpPut("admin/assignments/{id}/review")]
        public async Task<IActionResult> ReviewAssignment(int id, [FromBody] AdminReviewRequest request)
        {
            var assignment = await _context.CraftAssignments.FindAsync(id);
            if (assignment == null)
                return NotFound();

            if (request.Approved)
            {
                // Admin approves the update
                assignment.PendingAdminApproval = false;
                assignment.IsVerified = true;
                assignment.FinalWeight = assignment.ReturnWeight;
                assignment.UsedGold = assignment.RawGoldGiven - (assignment.ReturnWeight ?? 0);
                assignment.BalanceGold = assignment.ReturnWeight;
                assignment.Status = "Completed";
                assignment.CompletionDate = DateTime.UtcNow;
                assignment.AdminRemarks = request.AdminRemarks;
            }
            else
            {
                // Admin requests changes
                assignment.PendingAdminApproval = false;
                assignment.AdminRemarks = request.AdminRemarks;
                // Reset craftsman status so they can update again
                assignment.CraftsmanStatus = null;
                assignment.ReturnWeight = null;
            }

            assignment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(assignment);
        }

        // Craftsman: Get assigned orders for a craftsman
        [HttpGet("craftsman/{workerId}/orders")]
        public async Task<IActionResult> GetCraftsmanOrders(int workerId)
        {
            var assignments = await _context.CraftAssignments
                .Include(ca => ca.Order)
                    .ThenInclude(o => o.Customer)
                .Where(ca => ca.WorkerId == workerId)
                .OrderByDescending(ca => ca.AssignedDate)
                .Select(ca => new
                {
                    ca.Id,
                    ca.OrderId,
                    CustomerName = ca.Order.Customer.Name,
                    ProductName = ca.Order.JewelryType,
                    Category = ca.Order.Category,
                    MetalType = ca.Order.MetalType,
                    OriginalWeight = ca.RawGoldGiven,
                    ca.CraftsmanStatus,
                    ca.ReturnWeight,
                    ca.CraftsmanRemarks,
                    ca.AdminRemarks,
                    ca.PendingAdminApproval,
                    ca.AssignedDate,
                    ca.UpdateTimestamp
                })
                .ToListAsync();

            return Ok(assignments);
        }

        // Craftsman: Update order status
        [HttpPut("orders/{orderId}/craftsman-update")]
        public async Task<IActionResult> UpdateFromCraftsman(int orderId, [FromBody] CraftsmanUpdateRequest request)
        {
            var assignment = await _context.CraftAssignments
                .FirstOrDefaultAsync(ca => ca.OrderId == orderId);

            if (assignment == null)
                return NotFound();

            assignment.CraftsmanStatus = request.CraftsmanStatus;
            assignment.ReturnWeight = request.ReturnWeight;
            assignment.CraftsmanRemarks = request.CraftsmanRemarks;
            assignment.PendingAdminApproval = true;
            assignment.UpdateTimestamp = DateTime.UtcNow;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(assignment);
        }
    }

    // Request DTOs
    public class GoldDispatchReturnRequest
    {
        public decimal ReturnWeight { get; set; }
        public string Status { get; set; } = "returned";
        public string? ReturnRemarks { get; set; }
    }

    public class AdminReviewRequest
    {
        public bool Approved { get; set; }
        public string? AdminRemarks { get; set; }
    }

    public class CraftsmanUpdateRequest
    {
        public string CraftsmanStatus { get; set; } = string.Empty;
        public decimal ReturnWeight { get; set; }
        public string? CraftsmanRemarks { get; set; }
        public bool PendingAdminApproval { get; set; }
        public DateTime UpdateTimestamp { get; set; }
    }
}
