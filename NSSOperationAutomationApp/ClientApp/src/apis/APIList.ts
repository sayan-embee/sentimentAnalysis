import axios from "axios";
import {getUrl, routers} from './Endpoint'


export const sentimentAnalysisAPI = async (data?:any) => {
    console.log('In api',  getUrl(routers.sentimentanalysis),data)
    return await axios.post(getUrl(routers.sentimentanalysis),data);
}

export const getSentimentAnalysisDataAPI = async () => {
    console.log('In api',  getUrl(routers.getSentimentAnalysisData))
    return await axios.get(getUrl(routers.getSentimentAnalysisData));
}

