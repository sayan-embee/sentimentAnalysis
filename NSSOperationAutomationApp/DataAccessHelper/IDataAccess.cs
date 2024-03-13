using NSSOperationAutomationApp.Models;
using static NSSOperationAutomationApp.Models.MasterModels;

namespace NSSOperationAutomationApp.DataAccessHelper
{
    public interface IDataAccess
    {

        Task<List<CallActionModel>?> GetCallAction(int? Id);
        Task<List<DocumentTypeModel>?> GetDocumentType(int? Id);
        Task<List<PartConsumptionTypeModel>?> GetPartConsumptionType(int? Id);
        Task<List<CallStatusModel>?> GetCallStatus(int? Id);


        Task<ReturnMessageModel?> Insert(TicketDetailsModel data);
        Task<ReturnMessageModel?> InsertTicketInBulk(List<TicketCreateInBulkModel> dataList);
        Task<List<TicketDetailsModel>?> Get(FilterModel data);
        Task<ReturnMessageModel?> TicketUpdateByAdmin(TicketAssignmentModel data);
        Task<ReturnMessageModel?> AssignReassignEngineer(TicketAssignmentModel data);
        Task<ReturnMessageModel?> EngineerActionOnTicket(TicketActionModel data);
        Task<ReturnMessageModel?> EngineerDocumentUpdateOnTicket(List<CallDocumentsModel> dataList);
        Task<List<EngineerDetailsModel>?> GetAssignedTickets(TicketAssignmentModel data);
        Task<List<TicketDetailsOverviewModel>?> GetTicketsOverview(FilterModel data);
        Task<TicketTimelineViewModel?> GetTicketTimelineByCaseNo(string CaseNumber);

        Task<ReturnMessageModel?> Insert_AssignReassignTicket_CardResponse(List<TicketAssignmentCardModel> dataList);
    }
}