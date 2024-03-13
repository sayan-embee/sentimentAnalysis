// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import axios, { AxiosResponse, AxiosRequestConfig } from "axios";
import * as microsoftTeams from "@microsoft/teams-js";
import { formatMonthDayYear } from "@fluentui/react-northstar";
// import i18n from '../i18n';

export class AxiosJWTDecorator {
    public async get<T = any, R = AxiosResponse<T>>(
        url: string,
        handleError: boolean = true,
        needAuthorizationHeader: boolean = true,
        config?: AxiosRequestConfig,
    ): Promise<R> {
        try {
            if (needAuthorizationHeader) {
                config = await this.setupAuthorizationHeader(config);
            }

            return await axios.get(url, config);
        } catch (error) {
            if (handleError) {
                this.handleError(error);
                throw error;
            }
            else {
                throw error;
            }
        }
    }

    public async delete<T = any, R = AxiosResponse<T>>(
        url: string,
        handleError: boolean = true,
        config?: AxiosRequestConfig
    ): Promise<R> {
        try {
            config = await this.setupAuthorizationHeader(config);
            return await axios.delete(url, config);
        } catch (error) {
            if (handleError) {
                this.handleError(error);
                throw error;
            }
            else {
                throw error;
            }
        }
    }

    public async post<T = any, R = AxiosResponse<T>>(
        url: string,
        data?: any,
        handleError: boolean = true,
        needAuthorizationHeader: boolean = true,
        config?: AxiosRequestConfig
    ): Promise<R> {
        try {
            if (needAuthorizationHeader) {
                config = await this.setupAuthorizationHeader(config);
            }
            return await axios.post(url, data, config);
        } catch (error) {
            if (handleError) {
                this.handleError(error);
                throw error;
            }
            else {
                throw error;
            }
        }
    }

    public async put<T = any, R = AxiosResponse<T>>(
        url: string,
        data?: any,
        handleError: boolean = true,
        config?: AxiosRequestConfig
    ): Promise<R> {
        try {
            config = await this.setupAuthorizationHeader(config);
            return await axios.put(url, data, config);
        } catch (error) {
            if (handleError) {
                this.handleError(error);
                throw error;
            }
            else {
                throw error;
            }
        }
    }

    public async patch<T = any, R = AxiosResponse<T>>(
        url: string,
        data?: any,
        handleError: boolean = true,
        config?: AxiosRequestConfig
    ): Promise<R> {
        try {
            config = await this.setupAuthorizationHeader(config);
            return await axios.patch(url, data, config);
        } catch (error) {
            if (handleError) {
                this.handleError(error);
                throw error;
            }
            else {
                throw error;
            }
        }
    }

    private handleError(error: any): void {
        if (error.hasOwnProperty("response")) {
            const errorStatus = error.response.status;
            console.log(error.response.status);
            if (errorStatus === 403) {
                window.location.href = `/errorpage/?id=403&locale=en-US`;
            } else if (errorStatus === 401) {
                window.location.href = `/errorpage/?id=401&locale=en-US`;
            } else {
                window.location.href = `/errorpage?locale=en-US`;
            }
        } else {
            window.location.href = `/errorpage?locale=en-US`;
        }
    }

    private async setupAuthorizationHeader(
        config?: AxiosRequestConfig
    ): Promise<AxiosRequestConfig> {
        microsoftTeams.initialize();

        return new Promise<AxiosRequestConfig>((resolve, reject) => {
            const authTokenRequest = {
                successCallback: (token: string) => {
                    if (!config) {
                        config = axios.defaults;
                    }
                    config.headers["Authorization"] = `Bearer ${token}`;
                    config.headers["Accept-Language"] = 'en-US';
                    resolve(config);
                },
                failureCallback: (error: string) => {
                    // When the getAuthToken function returns a "resourceRequiresConsent" error, 
                    // it means Azure AD needs the user's consent before issuing a token to the app. 
                    // The following code redirects the user to the "Sign in" page where the user can grant the consent. 
                    // Right now, the app redirects to the consent page for any error.
                    console.error("Error from getAuthToken: ", error);
                    window.location.href = `/signin?locale=en-US`;
                },
                resources: []
            };
            microsoftTeams.authentication.getAuthToken(authTokenRequest);
        });
    }

    public async getCallWithAccesstoken<T = any, R = AxiosResponse<T>>(
        url: string,
        data?: any,
        handleError: boolean = true,
        config?: AxiosRequestConfig
    ): Promise<R> {
        try {
            config = await this.setupAuthorizationHeader(config);
            return await axios.get(url, { headers: { "Authorization": config.headers.Authorization } });
        } catch (error) {
            if (handleError) {
                this.handleError(error);
                throw error;
            }
            else {
                throw error;
            }
        }
    }

    public async getAccesstokenFromIdtoken<T = any, R = AxiosResponse<T>>(
        url: string,
        handleError: boolean = true,
        config?: AxiosRequestConfig
    ) {
        try {
            config = await this.setupAuthorizationHeader(config);
            var form = new FormData();
            form.append("client_id", "36a6b756-047e-46e7-ad06-6b81dc7aa8be");
            form.append("scope", `https://graph.microsoft.com/.default`);
            form.append("code", config.headers.Authorization);
            form.append("redirect_uri", `https://05b9-202-142-101-60.ngrok.io/signin-simple-end`);
            form.append("grant_type", "client_credentials");
            form.append("client_secret", "oMA7Q~sJHmC-.jMQEYn6J~~DOpeXWfbPfrw5U");
           
            return await axios.post(url, form);
        } catch (error) {
            if (handleError) {
                this.handleError(error);
                throw error;
            }
            else {
                throw error;
            }
        }
    }


}

const axiosJWTDecoratorInstance = new AxiosJWTDecorator();
export default axiosJWTDecoratorInstance;

