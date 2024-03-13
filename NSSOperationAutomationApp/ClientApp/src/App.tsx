
import * as React from "react";
import { Provider } from "@fluentui/react-northstar";
import { useEffect } from "react";
import { useTeams } from "msteams-react-base-component";
import * as microsoftTeams from "@microsoft/teams-js";
import { BrowserRouter, Route, Redirect, Switch } from "react-router-dom";
import { Routes } from './router'


function App(props: any) {
    const [{ inTeams, theme }] = useTeams();



    useEffect(() => {
        if (inTeams === true) {
            microsoftTeams.appInitialization.notifySuccess();
        }
    }, [inTeams]);

 


    return (
        <Provider theme={theme}>
            <BrowserRouter>
                <div>
                    <Switch>
                        {Routes.map(route => route.redirectTo ? <Redirect key={route.path} to={route.redirectTo} /> : <Route key={route.path} exact={route.exact} path={route.path} component={route.component} />)}
                    </Switch>
                </div>
            </BrowserRouter>
        </Provider>
    );
};

export default App;