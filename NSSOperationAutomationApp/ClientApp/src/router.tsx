
import SignInPage from './admin/pages/SignInPage/signInPage'
import SignInSimpleStart from './admin/pages/SignInPage/signInSimpleStart'
import SignInSimpleEnd from './admin/pages/SignInPage/signInSimpleEnd'

import ErrorPage from './admin/pages/ErrorPage/errorPage'

import AdminTicketList from './admin/pages/DashboardPage/Dashboard'
import EnduserTicketList from './enduser/pages/DashboardPage/EndUserDashboard'

import DetailTicketAdmin from './admin/Components/DetailsView/detailsView'

import DetailTicketEnduser from './enduser/Components/DetailsView/detailsViewEnduser'


export const Routes=[ 
    
    {path:'/signin', component:SignInPage},
    {path:'/signin-simple-start', component:SignInSimpleStart},
    {path:'/signin-simple-end', component:SignInSimpleEnd},
    {path:'/errorpage', component:ErrorPage},

    {path:'/admindashboardpage', component:AdminTicketList},
    {path:'/detailticketadmin', component:DetailTicketAdmin},
    {path:'/enduserdashboardpage', component:EnduserTicketList},
    {path:'/detailticketenduser', component:DetailTicketEnduser},

    {path:'/', exact:true, redirectTo:'/admindashboardpage'},
]