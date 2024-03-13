import React, { Component } from 'react';
import { Button, Text, FormDropdown, TextArea, Flex, FlexItem, FormInput, Loader } from "@fluentui/react-northstar";
import { Container, Row, Col } from 'react-bootstrap';

import { List20Regular, Building20Regular, Calendar20Regular, DocumentPageNumber20Regular, Person20Regular, Mail20Regular, Phone20Regular, SettingsCogMultiple20Regular, Tag20Regular, TagError20Regular, Chat20Regular } from "@fluentui/react-icons";
import { Persona, PersonaPresence, PersonaSize } from '@fluentui/react';

import Spinner from 'react-bootstrap/Spinner';

import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import 'react-tabs/style/react-tabs.css';

import * as microsoftTeams from "@microsoft/teams-js";

import { debounce } from "lodash";
import moment from 'moment';

import { getCallStatusMasterAPI, getAdminTicketList, getInstalledUsersAPI, getuserprofileAPI, assignEngineerAPI, adminUpdateTicketAPI } from './../../../apis/APIList'

import "./../../../App.scss";
import "./detailsView.scss";


const base_URL = window.location.origin



interface MyState {
    data?: any;
    loading?: any;
    casenumber?: any;
    callstatuslist?: any;
    ticketId?: any;
    casestatusId?: any;
    casestatus?: any;
    assigneePrticipantName?: any;
    selectedParticipantsName?: any;
    selectedParticipantsEmail?: any;
    selectedParticipantsADID?: any;
    participantLoading?: boolean;
    participantsList?: any;
    createdBy?: any;
    createdByEmail?: any;
    createdByADID?: any;
    adminRemarks?: any;
}


interface ITicketViewProps {
    history?: any;
    location?: any
}

class DetailTicketAdmin extends React.Component<ITicketViewProps, MyState>{
    constructor(props: ITicketViewProps) {
        super(props);
        this.state = {
            data: [],
            loading: true,
            callstatuslist: [],
            ticketId: '',
            participantsList: []
        }
    }

    componentDidMount() {
        this.getCallStatusMaster();
        const params = new URLSearchParams(this.props.location.search);
        this.setState({
            casenumber: params.get('id'),
        }, () => {

            this.search();
        })
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            console.log("context", context)
            this.setState({
                createdByEmail: context.userPrincipalName && context.userPrincipalName,
            }, () => { this.getuserprofileAPI() })
        });

    }

    getuserprofileAPI() {
        getuserprofileAPI(this.state.createdByEmail).then((res: any) => {
            console.log("user list", res)
            if (res.data) {
                this.setState({
                    createdBy: res.data.name,
                    createdByADID: res.data.adid,
                    createdByEmail: res.data.email
                })
            }

        })
    }

    getCallStatusMaster() {
        getCallStatusMasterAPI().then(response => {
            console.log('In getCallStatusMasterAPI', response);
            if (response.data) {
                let result = response.data.filter((e: any) => e.callStatusId !== 1).map((item: any) => {
                    return { id: item.callStatusId, header: item.callStatus }
                })
                this.setState({
                    callstatuslist: result
                }, () => {
                    this.setState({
                        callstatuslist: [...this.state.callstatuslist, { id: 0, header: 'Assign Engineer' }]
                    })
                })
            }
        })
    }

    search = () => {
        const data = {
            "caseNumber": this.state.casenumber,
            "serialNumber": null,
            "partNumber": null,
            "fromDate": null,
            "toDate": null,
            "assignedTo": null,
            "assignedToEmail": null,
            "isAssigned": null,
            "callStatusId": null,
            "callActionId": null,
            "IsTimelineView": true
        }
        this.getAllTickets(data);

    }

    getAllTickets = (data: any) => {

        getAdminTicketList(data).then((response: any) => {
            console.log('In getAdminTicketList', response.data[0]);
            if (response.data) {
                this.setState({
                    data: response.data[0],
                    loading: false
                })
            }

        })
    }


    selectTicketId(e: any) {
        this.setState({
            ticketId: e.target.value
        })
    }

    selectCallStatus = (e: any) => {
        this.setState({
            casestatusId: e.id,
            casestatus: e.header
        })
    }

    ////////////////////////// select engineer //////////////////////////


    participantsSearch(event: any) {
        console.log("user 1", event.target.value)
        this.setState({
            selectedParticipantsName: event.target.value,
            participantLoading: true,
            assigneePrticipantName: event.target.value

        }, () => {
            if (event.target.value) {
                console.log("user 2", event.target.value)
                this.debouncedSearchParticipants(event.target.value);
            }
            else {
                this.setState({
                    selectedParticipantsName: null,
                    participantsList: []
                })
            }
        })
    }

    debouncedSearchParticipants = debounce(async (criteria) => {
        console.log("user 3", criteria)
        if (this.state.selectedParticipantsName) {
            console.log("user 4", this.state.selectedParticipantsName)
            getInstalledUsersAPI(criteria).then((res: any) => {
                console.log("user list", res)
                if (res.data) {
                    this.setState({
                        participantsList: res.data,
                        participantLoading: false
                    })
                }

            })
        }

    }, 600);

    selectAssigneFunction(ele: any) {
        this.setState({
            selectedParticipantsName: ele.userName,
            selectedParticipantsEmail: ele.userEmail,
            selectedParticipantsADID: ele.userId,
            participantsList: [],
            assigneePrticipantName: ''
        })

    }

    adminRemarks(event: any) {
        this.setState({
            adminRemarks: event.target.value
        })
    }

    update() {
        if (!this.state.data.assignmentId) {
            const data = {
                "transactionType": "ENG-I",
                "caseNumber": this.state.casenumber,
                "assignedTo": this.state.selectedParticipantsName,
                "assignedToEmail": this.state.selectedParticipantsEmail,
                "assignedToADID": this.state.selectedParticipantsADID,
                "assignedBy": this.state.createdBy,
                "assignedByEmail": this.state.createdByEmail,
                "assignedByADID": this.state.createdByADID,
                "ticketId": this.state.ticketId
            }
            assignEngineerAPI(data).then((res: any) => {
                console.log("user list", res)
                if (res.data) {
                    this.setState({
                        selectedParticipantsADID: null,
                        selectedParticipantsEmail: null,
                        selectedParticipantsName: null,
                        adminRemarks: null,
                        loading: true
                    })
                    this.search()
                }

            })
        }
        else {
            if (this.state.casestatusId === 0) {
                const data = {
                    "transactionType": "ENG-U",
                    "assignmentId": this.state.data.assignmentId,
                    "caseNumber": this.state.casenumber,
                    "assignedTo": this.state.selectedParticipantsName,
                    "assignedToEmail": this.state.selectedParticipantsEmail,
                    "assignedToADID": this.state.selectedParticipantsADID,
                    "assignedBy": this.state.createdBy,
                    "assignedByEmail": this.state.createdByEmail,
                    "assignedByADID": this.state.createdByADID
                }
                assignEngineerAPI(data).then((res: any) => {
                    console.log("user list", res)
                    if (res.data) {
                        this.setState({
                            selectedParticipantsADID: null,
                            selectedParticipantsEmail: null,
                            selectedParticipantsName: null,
                            adminRemarks: null,
                            loading: true
                        })
                        this.search()
                    }

                })
            }
            else {
                const data = {
                    "transactionType": "ADMIN-U",
                    "assignmentId": this.state.data.assignmentId,
                    "caseNumber": this.state.casenumber,
                    "callStatusId": this.state.casestatusId,
                    "closedBy": this.state.createdBy,
                    "closedByEmail": this.state.createdByEmail,
                    "adminClosureRemarks": this.state.adminRemarks,
                    "closedByADID": this.state.createdByADID
                }
                adminUpdateTicketAPI(data).then((res: any) => {
                    console.log("user list", res)
                    if (res.data) {
                        this.setState({
                            selectedParticipantsADID: null,
                            selectedParticipantsEmail: null,
                            selectedParticipantsName: null,
                            casestatusId: null,
                            casestatus: null,
                            adminRemarks: null,
                            loading: true
                        })
                        this.search()
                    }
                })
            }

        }
    }


    render() {
        return (

            <div className="p-3 detailedView">
                {this.state.loading ? <div className='d-flex justify-content-center'>
                    <Spinner animation="grow" variant="info" />
                    <Spinner animation="grow" variant="info" />
                    <Spinner animation="grow" variant="info" />
                </div> : <div>
                    <Tabs>
                        <div className="reportTab">
                            <TabList >
                                <Tab>Details</Tab>
                                <Tab>Activities</Tab>
                                <Tab>Action</Tab>
                            </TabList>
                        </div>
                        <TabPanel>
                            <div className='task_module_content_container'>
                                <Container fluid>
                                    <Row>
                                        <div className="col-md-8 order-2 order-md-1">
                                            <Row>
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <List20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Case subject" size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.caseSubject} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <Building20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Customer name" size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.serviceAccount} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </Row>
                                            <Row>
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <Calendar20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Call date" size="small" timestamp />
                                                            <Text className='d-block' content={moment(this.state.data.createdOn).format('DD/MM/YYYY HH:MM')} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <DocumentPageNumber20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Work order number" size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.workOrderNumber} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </Row>
                                            <Row>
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <Person20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Contact person" size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.contactName} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <Mail20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Contact email" size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.contactEmail} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </Row>
                                            <Row>
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <Phone20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Contact phone" size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.contactPhone} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <SettingsCogMultiple20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Part Description" size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.partDescription} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </Row>
                                            <Row>
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <Tag20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Part No." size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.partNumber} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <TagError20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Serial No" size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.serialNumber} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </Row>
                                            <Row>
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <List20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Product Name" size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.productName} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
                                                {/* <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <Chat20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Remarks" size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.callAction} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div> */}
                                            </Row>

                                        </div>
                                        <div className="col-md-4 order-1 order-md-2 mb-3">
                                            <div className="details_quick_view mb-2">
                                                <div>
                                                    <div>
                                                        <Text className='d-block mb-1 caseNumber' color='blue' content={this.state.data.caseNumber} size="medium" weight="regular" />
                                                        <div className={`statusDiv ${!this.state.data.assignmentId ? 'notassignedStatus' : this.state.data.callStatus === 'Open' ? 'openStatus' : this.state.data.callStatus === 'Closed' ? 'closedStatus' : 'observationStatus'}`}><Text className='ml-2 mr-2 mt-1 mb-1 p-0 textEllipsis taskDashboardTableWidth' content={!this.state.data.assignmentId ? "Not Assigned" : this.state.data.callStatus} /></div>
                                                    </div>

                                                </div>
                                            </div>
                                            {this.state.data.engineerDetails && <div>
                                                <div className="details_quick_view mb-2"><div>
                                                    <Text className='d-block mb-1' color="grey" content="Assigned to:" size="small" timestamp />
                                                    <div>
                                                        <Persona text={this.state.data.engineerDetails.assignedTo} secondaryText={this.state.data.engineerDetails.assignedToEmail} size={PersonaSize.size40} />
                                                    </div>

                                                </div>
                                                </div>
                                            </div>}
                                            {/* <div className="details_quick_view mb-2">
                                                <Text className='d-block mb-1' color="grey" content="Documents" size="small" timestamp />
                                                <a href='#' className='d-flex justify-content-start align-items-center text-decoration-none mb-1'>
                                                    <Attach20Regular color="black" className='me-1' />
                                                    <Text className='d-block' color="brand" content="Sample-example-document.pdf" size="medium" weight="regular" />
                                                </a>
                                                <a href='#' className='d-flex justify-content-start align-items-center text-decoration-none'>
                                                    <Attach20Regular color="black" className='me-1' />
                                                    <Text className='d-block' color="brand" content="Sample-document.pdf" size="medium" weight="regular" />
                                                </a>
                                            </div> */}
                                        </div>
                                    </Row>
                                </Container>
                            </div>
                        </TabPanel>
                        <TabPanel>
                            <div className='task_module_content_container'>
                                <Container fluid>

                                    <div className='activity-timeline'>
                                        {this.state.data.ticketTimeline && this.state.data.ticketTimeline.ticketTimelineList && this.state.data.ticketTimeline.ticketTimelineList.length > 0 ? <div>{this.state.data.ticketTimeline.ticketTimelineList.map((e: any) => {
                                            return <div className="activity_item">
                                                <div className="dateTime">
                                                    <Text className='d-block' weight="semibold" content={moment(e.eventTimeIST).format('DD/MM/YYYY')} size="medium" />
                                                    <Text className='d-block' color="grey" content={moment(e.eventTimeIST).format('HH:MM')} size="small" timestamp />
                                                </div>
                                                <div className="activity_point">
                                                    <div className="highlight"></div>
                                                </div>
                                                <div className="activity_content">
                                                    <Text className='d-block mb-1' content={e.eventType} size="small" weight="regular" />
                                                    {e.slNo === 1 ? <div className="details">
                                                        <Text className='d-block mb-1' content="New case created and pending for Admin to assign engineer." size="medium" weight="regular" />
                                                    </div> : e.eventType === 'Engineer Assigned' ? <div className="details">
                                                        <div>
                                                            <Persona text={e.assignedTo} secondaryText={e.assignedToEmail} size={PersonaSize.size40} />
                                                        </div>
                                                        <div className='d-flex flex-md-row flex-column mt-2'>
                                                            <Text className='d-block' content="Assigned by: " size="small" weight="light" />
                                                            <Text className='d-block' color='grey' content={e.assignedBy + ' (' + e.assignedByEmail + ')'} size="small" weight="regular" />
                                                        </div>
                                                    </div> : <div className="details">
                                                        <div>
                                                            <Persona text={e.updatedBy} secondaryText={e.updatedByEmail} size={PersonaSize.size40} />
                                                        </div>
                                                        <Text className='d-block mb-2 mt-2 caseNumber' content="Action Details" size="medium" weight="bold" />
                                                        <Row>
                                                            <div className='col-sm-6'>
                                                                <div className='d-flex mb-3 '>
                                                                    <div className='ms-2'>
                                                                        <Text className='d-block' color="grey" content="Customer Name" size="small" timestamp />
                                                                        <Text className='d-block' content={e.customerName} size="medium" weight="regular" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div className='col-sm-6'>
                                                                <div className='d-flex mb-3'>
                                                                    <div className='ms-2'>
                                                                        <Text className='d-block' color="grey" content="Issue Description" size="small" timestamp />
                                                                        <Text className='d-block' content={e.issueDescription} size="medium" weight="regular" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </Row>
                                                        <Row>
                                                            <div className='col-sm-6'>
                                                                <div className='d-flex mb-3'>
                                                                    <div className='ms-2'>
                                                                        <Text className='d-block' color="grey" content="Part Description" size="small" timestamp />
                                                                        <Text className='d-block' content={e.requiredPart} size="medium" weight="regular" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div className='col-sm-6'>
                                                                <div className='d-flex mb-3'>
                                                                    <div className='ms-2'>
                                                                        <Text className='d-block' color="grey" content="Remarks" size="small" timestamp />
                                                                        <Text className='d-block' content={e.closerRemarks} size="medium" weight="regular" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </Row>
                                                        <Row>
                                                            <div className='col-sm-6'>
                                                                <div className='d-flex mb-3'>
                                                                    <div className='ms-2'>
                                                                        <Text className='d-block' color="grey" content="SO No" size="small" timestamp />
                                                                        <Text className='d-block' content={e.soNo} size="medium" weight="regular" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div className='col-sm-6'>
                                                                <div className='d-flex mb-3'>
                                                                    <div className='ms-2'>
                                                                        <Text className='d-block' color="grey" content="Unit Sl No" size="small" timestamp />
                                                                        <Text className='d-block' content={e.unitSlNo} size="medium" weight="regular" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </Row>
                                                        <Row>
                                                            <div className='col-sm-6'>
                                                                <div className='d-flex mb-3'>
                                                                    <div className='ms-2'>
                                                                        <Text className='d-block' color="grey" content="Task Start Date with Time" size="small" timestamp />
                                                                        <Text className='d-block' content={moment(e.taskStartDateTimeIST).format('DD/MM/YYYY HH:MM')} size="medium" weight="regular" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div className='col-sm-6'>
                                                                <div className='d-flex mb-3'>
                                                                    <div className='ms-2'>
                                                                        <Text className='d-block' color="grey" content="Task End Date with Time" size="small" timestamp />
                                                                        <Text className='d-block' content={moment(e.taskEndDateTimeIST).format('DD/MM/YYYY HH:MM')} size="medium" weight="regular" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </Row>
                                                        <Row>
                                                            <div className="col-sm-6">

                                                                <Text className='d-block mb-1' color="grey" content="Documents" size="small" timestamp />
                                                                {this.state.data.ticketTimeline && this.state.data.ticketTimeline.callDocumentList && (this.state.data.ticketTimeline.callDocumentList.length > 0) ? <div>
                                                                    {this.state.data.ticketTimeline.callDocumentList.map((e: any) => {
                                                                        return <div>
                                                                            <a href={e.documentUrlPath} className='d-flex justify-content-start align-items-center text-decoration-none'>
                                                                                <Text className='d-block' color="brand" content={e.documentName} size="medium" weight="regular" />
                                                                            </a>
                                                                        </div>
                                                                    })}
                                                                </div> : <div className="noDataText"><Text content="No documents available" /></div>}

                                                            </div>
                                                        </Row>
                                                    </div>}
                                                </div>
                                            </div>
                                        })}</div> : <div className="noDataText"><Text content="No data available" /></div>}
                                        {/* <div className="activity_item">
                                            <div className="dateTime">
                                                <Text className='d-block' weight="semibold" content="23 Jan, 2024" size="medium" />
                                                <Text className='d-block' color="grey" content="09:24" size="small" timestamp />
                                            </div>
                                            <div className="activity_point">
                                                <div className="highlight"></div>
                                            </div>
                                            <div className="activity_content">
                                                <Text className='d-block mb-1' content="1st Part order" size="small" weight="regular" />
                                                
                                            </div>
                                        </div> */}
                                    </div>
                                </Container>
                            </div>
                        </TabPanel>
                        <TabPanel>
                            <div className='task_module_content_container'>
                                <Container fluid>
                                    <div className='px-md-5'>
                                        <div className='row g-md-5'>
                                            <div className="col-md-7 order-2 order-md-1">
                                                <Row>
                                                    {!this.state.data.assignmentId && <div className="col-12 mb-3">
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="Ticket ID *" size="small" timestamp />
                                                        <FormInput
                                                            type="number"
                                                            autoComplete="off"
                                                            fluid
                                                            required
                                                            onChange={(e: any) => this.selectTicketId(e)}
                                                            min="1"
                                                        />
                                                    </div>}
                                                    {this.state.data.assignmentId && <div className="col-12 mb-3">
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="Status" size="small" timestamp />
                                                        <FormDropdown fluid
                                                            items={this.state.callstatuslist}
                                                            value={this.state.casestatus}
                                                            onChange={(event, { value }) => this.selectCallStatus(value)}
                                                        />
                                                    </div>}
                                                    {(!this.state.data.assignmentId || (this.state.casestatusId === 0)) && <div className="col-12 mb-3">
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="Assign to *" size="small" timestamp />
                                                        <FormInput
                                                            fluid
                                                            autoComplete="off"
                                                            showSuccessIndicator={false}
                                                            value={this.state.selectedParticipantsName}
                                                            onChange={(e) => this.participantsSearch(e)}
                                                        />
                                                        {(this.state.assigneePrticipantName) && <div className='searchList'>{!this.state.participantLoading ? <div>
                                                            {this.state.participantsList && (this.state.participantsList.length > 0) ? this.state.participantsList.map((ele: any, i: any) =>
                                                                <div key={i} className="p-2 d-flex flex-column pointer" onClick={() => this.selectAssigneFunction(ele)}>
                                                                    <Text size="medium" content={ele.userName}></Text>
                                                                    <Text size="small" content={ele.userEmail}></Text>
                                                                </div>
                                                            ) : <div className='searchResultList'><Text className="searchResultListEmployeeName">No Data Found</Text></div>}

                                                        </div> : <div className='searchResultList'>< Loader size="smaller" /></div>}
                                                        </div>
                                                        }
                                                    </div>}
                                                    <div className='col-12 mb-3'>
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="Remarks" size="small" timestamp />
                                                        <TextArea fluid resize="vertical" placeholder="Enter remarks..." onChange={(event) => this.adminRemarks(event)} />
                                                    </div>
                                                </Row>
                                                <Flex gap="gap.small">
                                                    <FlexItem push>
                                                        <Button content="Update" primary onClick={() => this.update()} />
                                                    </FlexItem>
                                                </Flex>
                                            </div>
                                            <div className="col-md-5 order-1 order-md-2 mb-3">
                                                <div className="details_quick_view mb-2">
                                                    <div>
                                                        <div>
                                                            <Text className='d-block mb-1 caseNumber' content={this.state.casenumber} size="medium" weight="regular" />
                                                            <div className={`statusDiv ${!this.state.data.assignmentId ? 'notassignedStatus' : this.state.data.callStatus === 'Open' ? 'openStatus' : this.state.data.callStatus === 'Closed' ? 'closedStatus' : 'observationStatus'}`}><Text className='ml-2 mr-2 mt-1 mb-1 p-0 textEllipsis taskDashboardTableWidth' content={!this.state.data.assignmentId ? "Not Assigned" : this.state.data.callStatus} /></div>
                                                            {/* <div className='status open'>{this.state.data.callStatus}</div> */}
                                                        </div>

                                                    </div>
                                                </div>
                                                {this.state.selectedParticipantsEmail && <div className="details_quick_view mb-2">
                                                    <div>
                                                        <Text className='d-block mb-1' color="grey" content="Assigned to:" size="small" timestamp />
                                                        <div>
                                                            <Persona text={this.state.selectedParticipantsName} secondaryText={this.state.selectedParticipantsEmail} size={PersonaSize.size40} />
                                                        </div>

                                                    </div>
                                                </div>}
                                            </div>
                                        </div>
                                    </div>
                                </Container>
                            </div>
                        </TabPanel>
                    </Tabs>
                </div>}
            </div>

        );
    }
}

export default DetailTicketAdmin;