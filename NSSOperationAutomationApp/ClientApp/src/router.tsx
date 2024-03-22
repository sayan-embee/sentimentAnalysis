
import SentimentAnalysisPage from './home'


export const Routes=[ 
    
    {path:'/homepage', component:SentimentAnalysisPage},
    

    {path:'/', exact:true, redirectTo:'/homepage'},
]