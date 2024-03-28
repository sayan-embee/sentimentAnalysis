
import SentimentAnalysisPage from './home'
import SentimentTableList from './sentimentDataList'


export const Routes=[ 
    
    {path:'/homepage', component:SentimentAnalysisPage},
    {path:'/dashboard', component:SentimentTableList},

    {path:'/', exact:true, redirectTo:'/homepage'},
]