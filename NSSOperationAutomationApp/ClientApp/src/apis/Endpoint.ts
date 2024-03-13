const base_URL = window.location.origin+'/api/';  


export const routers = {

    authenticationMetadata:'consentUrl',

    callActionMaster: 'masters/callAction/get',
    documentTypeMaster: 'masters/documentType/get',
    partConsumptionTypeMaster: 'masters/partConsumptionType/get',
    callStatusMaster: 'masters/callStatus/get',

    installedUsersDatas:'getAppInstalledUsers?filter=',
    getuserprofile:'getUserProfile?Id=',

    getAdminTicketList: 'ticket/getTicketsOverview',

    assignengineer: 'ticket/assignEngineer',
    adminupdateTicket: 'ticket/updateByAdmin',

    engineeractionticket: 'ticket/engineerAction ',

    importcsvfile:'ticket/createFromCSV'
    
    
}

export const getUrl = (key: any) => {
    return base_URL + key;
}
