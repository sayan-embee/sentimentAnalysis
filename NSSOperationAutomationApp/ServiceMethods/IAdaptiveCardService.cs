using Microsoft.Bot.Schema;
using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.ServiceMethods
{
    public interface IAdaptiveCardService
    {
        Attachment GetCard_AssignTicket_PersonalScope(TicketAssignmentCardModel data);
        Attachment GetCard_TicketActionByEng_PersonalScope(TicketActionCardModel data);
        Attachment GetCard_TicketActionByAdmin_PersonalScope(TicketActionCardModel data);
        Attachment GetCard_Welcome_PersonalScope(WelcomeCardModel data);
    }
}