using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.HelperMethods
{
    public interface INotificationHelper
    {
        Task<List<TicketAssignmentCardModel>?> ProcessNotification_AssignReassignTicket(List<TicketAssignmentCardModel> dataList);
        Task<List<TicketAssignmentCardModel>?> ProcessNotification_TicketActionByEngineer(List<TicketActionCardModel> dataList);
        Task<List<TicketAssignmentCardModel>?> ProcessNotification_TicketActionByAdmin(List<TicketActionCardModel> dataList);
        Task<List<TicketAssignmentCardModel>?> ProcessNotification_AssignReassignTicketInBulk(List<TicketAssignmentCardModel> dataList);
    }
}