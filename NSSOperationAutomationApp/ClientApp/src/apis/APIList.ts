import axios from "axios";
import {getUrl, routers} from './Endpoint'


export const sentimentAnalysisAPI = async (data?:any) => {
    console.log('In api',  getUrl(routers.sentimentanalysis),data)
    return await axios.post(getUrl(routers.sentimentanalysis),data);
}