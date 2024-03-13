import axios from "axios";
import {getUrl, routers} from './Endpoint'

///////////////////////  SSO ///////////////////

export const getAuthenticationConsentMetadata = async (windowLocationOriginDomain: string, login_hint: string): Promise<any> => {
    //console.log('In api',  getUrl(routers.authenticationMetadata)+`?windowLocationOriginDomain=${windowLocationOriginDomain}&loginhint=${login_hint}`)
    return await axios.get(getUrl(routers.authenticationMetadata)+`?windowLocationOriginDomain=${windowLocationOriginDomain}&loginhint=${login_hint}`, undefined);
}

///////////////////////  Masters ///////////////////

export const getCallStatusMasterAPI = async () => {
    console.log('In api',  getUrl(routers.callStatusMaster))
    return await axios.get(getUrl(routers.callStatusMaster));
}

export const getDocumentTypeMasterAPI = async () => {
    console.log('In api',  getUrl(routers.documentTypeMaster))
    return await axios.get(getUrl(routers.documentTypeMaster));
}   

export const getPartConsumptionTypeMasterAPI = async () => {
    console.log('In api',  getUrl(routers.partConsumptionTypeMaster))
    return await axios.get(getUrl(routers.partConsumptionTypeMaster));
}

export const getCallActionMasterAPI = async () => {
    console.log('In api',  getUrl(routers.callActionMaster))
    return await axios.get(getUrl(routers.callActionMaster));
}

///////////////////////  Ticket ///////////////////

export const getAdminTicketList = async (data ?: any) => {
    console.log('In api',  getUrl(routers.getAdminTicketList),data)
    return await axios.post(getUrl(routers.getAdminTicketList),data);
}

////////////////////////// User /////////////////////////

export const getInstalledUsersAPI = async (data?:any) => {
    console.log('In api',  getUrl(routers.installedUsersDatas)+data)
    return await axios.get(getUrl(routers.installedUsersDatas)+data);
}

export const getuserprofileAPI = async (data?:any) => {
    console.log('In api',  getUrl(routers.getuserprofile)+data)
    return await axios.get(getUrl(routers.getuserprofile)+data);
}

/////////////////////////// Admin ///////////////////////////

export const assignEngineerAPI = async (data?:any) => {
    console.log('In api',  getUrl(routers.assignengineer),data)
    return await axios.post(getUrl(routers.assignengineer),data);
}

export const adminUpdateTicketAPI = async (data?:any) => {
    console.log('In api',  getUrl(routers.adminupdateTicket),data)
    return await axios.post(getUrl(routers.adminupdateTicket),data);
}

/////////////////////////// Engineer ///////////////////////////

export const engineerActionTicketAPI = async (data?:any) => {
    console.log('In api',  getUrl(routers.engineeractionticket),data)
    return await axios.post(getUrl(routers.engineeractionticket),data);
}


/////////////////////////// Import CSV ///////////////////////////

export const importCSVAPI = async (data?:any) => {
    console.log('In api',  getUrl(routers.importcsvfile),data)
    return await axios.post(getUrl(routers.importcsvfile),data);
}