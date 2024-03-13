import React, { Component } from 'react';
import { Button, Text, Form, FormDropdown, FormInput, FormDatepicker } from "@fluentui/react-northstar";

import Spinner from 'react-bootstrap/Spinner';

import * as microsoftTeams from "@microsoft/teams-js";

import moment from 'moment';
import { CSVLink } from "react-csv";

import Pagination from "react-js-pagination";
import 'bootstrap-css-only/css/bootstrap.min.css';
import 'mdbreact/dist/css/mdb.css';

import { getCallStatusMasterAPI, getAdminTicketList, importCSVAPI } from './../../../apis/APIList'

import "./../../../App.scss";
import "./dashboard.scss";


const base_URL = window.location.origin
const filtermenuicon = base_URL + "/images/filtermenu.png";



interface MyState {
    data?: any;
    loading?: any;
    casenumber?: string;
    callstatuslist?: any;
    casestatusId?: string;
    casestatus?: string;
    fromDate?: string;
    toDate?: string;
    partNumber?: string;
    serialNumber?: string;
    customerName?: string;
    assignedTo?: string;
    downloadData?: any;
    activePage?: any;
    itemsPerPage?: any;
    isPaneOpen?: boolean;
    loggedinUserEmail?: string;
}

interface ITaskInfo {
    title?: string;
    height?: number;
    width?: number;
    url?: string;
    card?: string;
    fallbackUrl?: string;
    completionBotId?: string;
}


interface IDashboardProps {
}

class AdminTicketList extends React.Component<IDashboardProps, MyState>{
    constructor(props: IDashboardProps) {
        super(props);
        this.state = {
            data: [],
            fromDate: '',
            toDate: '',
            casenumber: '',
            casestatusId: '',
            partNumber: '',
            serialNumber: '',
            customerName: '',
            assignedTo: '',
            loading: true,
            downloadData: [],
            itemsPerPage: 10,
            activePage: 1,
            isPaneOpen: false,
            callstatuslist: [],
            casestatus: '',
            loggedinUserEmail: ''
        }
    }

    componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            console.log("context", context)
            this.setState({
                loggedinUserEmail: context.userPrincipalName && context.userPrincipalName,
            })
        });
        this.getCallStatusMaster();
        this.search();
    }

    getCallStatusMaster() {
        getCallStatusMasterAPI().then(response => {
            console.log('In getCallStatusMasterAPI', response);
            if (response.data) {
                let result = response.data.map((item: any) => {
                    return { id: item.callStatusId, header: item.callStatus }
                })
                this.setState({ callstatuslist: result })
            }

        })
    }

    search = () => {
        const data = {
            "caseNumber": this.state.casenumber ? this.state.casenumber : null,
            "serialNumber": this.state.serialNumber ? this.state.serialNumber : null,
            "partNumber": this.state.partNumber ? this.state.partNumber : null,
            "fromDate": this.state.fromDate ? this.state.fromDate : null,
            "toDate": this.state.toDate ? this.state.toDate : null,
            "assignedTo": this.state.assignedTo ? this.state.assignedTo : null,
            "assignedToEmail": null,
            "isAssigned": null,
            "callStatusId": this.state.casestatusId ? this.state.casestatusId : null,
            "callActionId": null,
            "IsTimelineView": null
        }
        this.getAllTickets(data);

    }

    getAllTickets = (data: any) => {
        getAdminTicketList(data).then(response => {
            console.log('In getAdminTicketList', response);
            if (response.data) {
                const downloadDataList = response.data.map((e: any) => {
                    let b = {
                        "Call Date": moment(e.createdOn).format('DD/MM/YYYY HH:MM'),
                        "Case Number": e.caseNumber,
                        "Ticket ID": e.ticketId,
                        "Case Subject": e.caseSubject,
                        "Work Order Number": e.workOrderNumber,
                        "Service Account": e.serviceAccount,
                        "Contact Name": e.contactName,
                        "Contact Number": e.contactPhone,
                        "Contact Email": e.contactEmail,
                        "Street Address": e.serviceDeliveryStreetAddress,
                        "Delivery City": e.serviceDeliveryCity,
                        "Delivery Country": e.serviceDeliveryCountry,
                        "Postal Code": e.postalCode,
                        "Serial Number": e.serialNumber,
                        "Product Name": e.productName,
                        "Product Number": e.productNumber,
                        "OTC Code": e.otcCode,
                        "Part Number": e.partNumber,
                        "Part Description": e.partDescription,
                        "Email Subject": e.emailSubject
                    }
                    return b
                })
                this.setState({
                    data: response.data,
                    loading: false,
                    downloadData: downloadDataList
                })
            }

        })
    }

    selectCaseNo = (e: any) => {
        this.setState({
            casenumber: e.target.value
        })
    }

    selectCustomerName = (e: any) => {
        this.setState({
            customerName: e.target.value
        })
    }

    selectCallStatus = (e: any) => {
        this.setState({
            casestatusId: e.id,
            casestatus: e.header
        })
    }

    selectPartNumber = (e: any) => {
        this.setState({
            partNumber: e.target.value
        })
    }

    selectSeralNumber = (e: any) => {
        this.setState({
            serialNumber: e.target.value
        })
    }

    selectAssignedTo = (e: any) => {
        this.setState({
            assignedTo: e.target.value
        })
    }

    fromDateFunc = (date: any) => {
        const dateObj = new Date(date);
        this.setState({
            fromDate: dateObj.getFullYear() + "-" + ("0" + (dateObj.getMonth() + 1)).slice(-2) + "-" + ("0" + dateObj.getDate()).slice(-2) + "T00:00:00"
        });
    };

    toDateFunc = (date: any) => {
        const dateObj = new Date(date);
        this.setState({
            toDate: dateObj.getFullYear() + "-" + ("0" + (dateObj.getMonth() + 1)).slice(-2) + "-" + ("0" + dateObj.getDate()).slice(-2) + "T23:59:59",
        });
    };

    reset() {
        this.setState({
            casenumber: '',
            casestatusId: '',
            fromDate: '',
            toDate: '',
            partNumber: '',
            serialNumber: '',
            customerName: '',
            assignedTo: '',
            casestatus: '',
            loading: true
        }, () => {
            this.search()
        })
    }

    ///// page change/////
    handlePageChange(pageNumber: any) {
        this.setState({ activePage: pageNumber });
    }

    updateTicket = (item: any) => {
        console.log('In updateTicket', item);
        let taskInfo: ITaskInfo = {
            url: `${base_URL}/detailticketadmin?id=${item.caseNumber}`,
            title: "Details View",
            height: 750,
            width: 950,
            fallbackUrl: `${base_URL}/detailticketadmin?id=${item.caseNumber}`
        }
        let submitHandler = (err: any, result: any) => {
            this.setState({
                loading: true
            }, () => {
                this.search()
            })
        };
        microsoftTeams.tasks.startTask(taskInfo, submitHandler);
    }


    render() {
        return (
            <div>
                <div className="headerdiv">
                    <Button content="Import" className="btn-exp btn-full" primary disabled={this.state.loggedinUserEmail ? false : true} />
                    <Button title="Export" primary className="exportBtn champListButton btn-exp btn-full" disabled={(this.state.downloadData && this.state.downloadData.length > 0) ? false : true}>
                        <CSVLink data={this.state.downloadData} filename={"reports-file" + moment(new Date()).format('DDMMYYYY') + ".csv"}><Button content="Export" className="btn-exp btn-full" primary disabled={(this.state.downloadData && this.state.downloadData.length > 0) ? false : true} /></CSVLink>
                    </Button>
                    <div className='m-5 pointer d-flex align-items-center' onClick={() => this.setState({ isPaneOpen: !this.state.isPaneOpen })}>
                        <img src={filtermenuicon} alt='filter' style={{ height: "22px", width: "22px", marginRight: "6px" }} /><Text content="Filter" size="medium" />
                    </div>

                </div>
                <aside className={this.state.isPaneOpen ? 'to-right' : ''}>
                    <div className="admin-report">
                        {this.state.loading ? <div className='d-flex justify-content-center'>
                            <Spinner animation="grow" variant="info" />
                            <Spinner animation="grow" variant="info" />
                            <Spinner animation="grow" variant="info" />
                        </div> : <div>
                            {(this.state.data.length > 0) ? <div className="tableBody"><table className="ticketTable">
                                <thead>
                                    <tr>
                                        <th>Call Date</th>
                                        <th>Case Number</th>
                                        <th>Case Subject</th>
                                        <th>Work Order Number</th>
                                        <th>Product Number</th>
                                        {/* <th>Issue Type</th> */}
                                        <th>Serial Number</th>
                                        <th>Customer Name</th>
                                        <th>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {this.state.data.slice((this.state.activePage - 1) * this.state.itemsPerPage, (this.state.activePage - 1) * this.state.itemsPerPage + this.state.itemsPerPage).map((item: any) => {
                                        return (
                                            <tr className='ViswasTableRow'>
                                                <td className="reportTable"><Text className='mb-1 p-0 textEllipsis taskDashboardTableWidth' color="grey" content={moment(item.createdOn).format('DD/MM/YYYY HH:MM')} /></td>
                                                <td><div onClick={() => this.updateTicket(item)}><Text className='mb-1 p-0 textEllipsis taskDashboardTableWidth callDate' content={item.caseNumber} /></div></td>
                                                <td><Text className='mb-1 p-0 textEllipsis taskDashboardTableWidth' color="grey" content={item.caseSubject} /></td>
                                                <td><Text className='mb-1 p-0 textEllipsis taskDashboardTableWidth' color="grey" content={item.workOrderNumber} /></td>
                                                <td><Text className='mb-1 p-0 textEllipsis taskDashboardTableWidth' color="grey" content={item.productNumber} /></td>
                                                <td><Text className='mb-1 p-0 textEllipsis taskDashboardTableWidth' color="grey" content={item.serialNumber} /></td>
                                                <td><Text className='mb-1 p-0 textEllipsis taskDashboardTableWidth' color="grey" content={item.serviceAccount} /></td>
                                                <td><div className={`statusDiv ${!item.assignmentId ? 'notassignedStatus' : item.callStatus === 'Open' ? 'openStatus' : item.callStatus === 'Closed' ? 'closedStatus' : 'observationStatus'}`}><Text className='ml-2 mr-2 mt-1 mb-1 p-0 textEllipsis taskDashboardTableWidth' content={!item.assignmentId ? "Not Assigned" : item.callStatus} /></div></td>
                                            </tr>
                                        );
                                    })}
                                </tbody>
                            </table>
                                <div className="pagination-style">
                                    <Pagination
                                        activePage={this.state.activePage}
                                        itemsCountPerPage={this.state.itemsPerPage}
                                        totalItemsCount={this.state.data.length}
                                        pageRangeDisplayed={6}
                                        onChange={this.handlePageChange.bind(this)}
                                        itemClass="page-item"
                                        linkClass="page-link"
                                        prevPageText="< Previous"
                                        nextPageText="Next >"
                                        firstPageText=""
                                        lastPageText=""
                                        linkClassFirst="displayNone"
                                        linkClassLast="displayNone"
                                    />
                                </div>
                            </div>
                                : <div className="noDataText"><Text content="No data available" /></div>}
                        </div>}
                    </div>
                </aside>
                {this.state.isPaneOpen ? <nav className={this.state.isPaneOpen ? 'opneSidebar' : ''}>
                    <div className='navHeader'>
                        <Text content="Filter" />
                    </div>
                    <div className="filterbody">
                        <Form>
                            <FormInput
                                label="Case number"
                                autoComplete="off"
                                fluid
                                onChange={(e) => { this.selectCaseNo(e) }}
                            />
                            <FormInput
                                label="Customer name"
                                autoComplete="off"
                                fluid
                                onChange={(e) => { this.selectCustomerName(e) }}
                            />
                            <FormDatepicker
                                onDateChange={(e, data: any) => this.fromDateFunc(data.value)}
                                inputOnly
                                className="w-100 datepicker"
                                label="From date"

                            />
                            <FormDatepicker
                                onDateChange={(e, data: any) => this.toDateFunc(data.value)}
                                inputOnly
                                className="w-100 datepicker"
                                label="To date"

                            />
                            <FormDropdown fluid
                                label="Status"
                                items={this.state.callstatuslist}
                                value={this.state.casestatus}
                                onChange={(event, { value }) => this.selectCallStatus(value)}
                            //placeholder=""
                            />
                            <FormInput
                                label="Part number"
                                autoComplete="off"
                                fluid
                                onChange={(e) => { this.selectPartNumber(e) }}
                            />
                            <FormInput
                                label="Serial number"
                                autoComplete="off"
                                fluid
                                value={this.state.casenumber}
                                onChange={(e) => { this.selectSeralNumber(e) }}
                            />
                            <FormInput
                                label="Assigned to enginner"
                                autoComplete="off"
                                fluid
                                value={this.state.casenumber}
                                onChange={(e) => { this.selectAssignedTo(e) }}
                            />



                        </Form>
                    </div>
                    <div className='searchFooter'>
                        <Button content="Apply" className="btn-exp btn-full" primary onClick={() => this.search()} />
                        <Button content="Reset" className="btn-exp btn-full" onClick={() => this.reset()} />
                    </div>
                </nav> : ''}
            </div>
        );
    }
}

export default AdminTicketList;