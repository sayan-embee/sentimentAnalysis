const base_URL = window.location.origin+'/api/';  


export const routers = {

    sentimentanalysis:'openAI/audioSummary',

    
    
}

export const getUrl = (key: any) => {
    return base_URL + key;
}
