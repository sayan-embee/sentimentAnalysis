import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { Text, Button } from "@fluentui/react-northstar";
import * as microsoftTeams from "@microsoft/teams-js";
import "./signInPage.scss";


const SignInPage: React.FunctionComponent<RouteComponentProps> = props => {

    function onSignIn() {
        microsoftTeams.authentication.authenticate({
            url: window.location.origin + "/signin-simple-start",
            successCallback: () => {
                console.log("Login succeeded!");
                window.location.href = "/adminpage";
            },
            failureCallback: (reason) => {
                console.log("Login failed: " + reason);
                window.location.href = "/errorpage";
            }
        });
    }

    return (
        <div className="sign-in-content-container">
            <Text
                content="Please sign in to continue."
                size="medium"
            />
            <div className="space"></div>
            <Button content="Sign in" primary className="sign-in-button" onClick={onSignIn} />
        </div>
    );
};

export default SignInPage;
