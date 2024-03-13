import React, { Component } from 'react';
import { Button, Text, FormDropdown, TextArea, Flex, FlexItem, FormInput, Loader, Datepicker, Input } from "@fluentui/react-northstar";
import { Container, Row, Col } from 'react-bootstrap';

import { List20Regular, Building20Regular, Calendar20Regular, DocumentPageNumber20Regular, Person20Regular, Mail20Regular, Phone20Regular, SettingsCogMultiple20Regular, Tag20Regular, TagError20Regular, Chat20Regular } from "@fluentui/react-icons";
import { Persona, PersonaPresence, PersonaSize } from '@fluentui/react';

import Spinner from 'react-bootstrap/Spinner';

import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import 'react-tabs/style/react-tabs.css';

import * as microsoftTeams from "@microsoft/teams-js";

import { debounce } from "lodash";
import moment from 'moment';
import { v4 as uuid } from 'uuid';

import { getCallStatusMasterAPI, getAdminTicketList, getuserprofileAPI, getCallActionMasterAPI, getPartConsumptionTypeMasterAPI, getDocumentTypeMasterAPI, engineerActionTicketAPI } from '../../../apis/APIList'

import "./../../../App.scss";
import "./detailsViewEnduser.scss";


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
    callActionList?: any;
    partConsumptionTypeList?: any;
    documentTypeList?: any;
    selectedCallAction?: any;
    selectedPartConsumptionType?: any;
    selectedDocumentType?: any;
    selectedCallActionId?: any;
    selectedPartConsumptionTypeId?: any;
    selectedDocumentTypeId?: any;
    remarks?: any;
    startDateTime?: any;
    endDateTime?: any;
    passid?: any;
    soNo?: any;
    requiredPart?: any;
    fileList?: any;
    documentFileList?: any;
}


interface ITicketViewProps {
    history?: any;
    location?: any
}

class DetailTicketEnduser extends React.Component<ITicketViewProps, MyState>{
    constructor(props: ITicketViewProps) {
        super(props);
        this.state = {
            data: [],
            loading: true,
            callstatuslist: [],
            ticketId: '',
            participantsList: [],
            fileList: [],
            documentFileList: [],
        }
    }

    componentDidMount() {
        this.getCallStatusMaster();
        this.getCallActionMaster();
        this.getPartConsumptionTypeMaster();
        this.getDocumentTypeMaster();
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

    getCallActionMaster() {
        getCallActionMasterAPI().then(response => {
            console.log('In getCallActionMasterAPI', response);
            if (response.data) {
                let result = response.data.map((item: any) => {
                    return { id: item.callActionId, header: item.callAction }
                })
                this.setState({
                    callActionList: result
                })
            }
        })
    }

    getPartConsumptionTypeMaster() {
        getPartConsumptionTypeMasterAPI().then(response => {
            console.log('In getPartConsumptionTypeMasterAPI', response);
            if (response.data) {
                let result = response.data.map((item: any) => {
                    return { id: item.partConsumptionTypeId, header: item.partConsumptionType }
                })
                this.setState({
                    partConsumptionTypeList: result
                })
            }
        })
    }

    getDocumentTypeMaster() {
        getDocumentTypeMasterAPI().then(response => {
            console.log('In getDocumentTypeMasterAPI', response);
            if (response.data) {
                let result = response.data.map((item: any) => {
                    return { id: item.documentTypeId, header: item.documentType }
                })
                this.setState({
                    documentTypeList: result
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

    startDateFunc = (date: any) => {
        const dateObj = new Date(date);
        this.setState({
            startDateTime: dateObj.getFullYear() + "-" + ("0" + (dateObj.getMonth() + 1)).slice(-2) + "-" + ("0" + dateObj.getDate()).slice(-2) + "T00:00:00"
        });
    };

    endDateFunc = (date: any) => {
        const dateObj = new Date(date);
        this.setState({
            endDateTime: dateObj.getFullYear() + "-" + ("0" + (dateObj.getMonth() + 1)).slice(-2) + "-" + ("0" + dateObj.getDate()).slice(-2) + "T23:59:59",
        });
    };
    selectPassId(e: any) {
        this.setState({
            passid: e.target.value
        })
    }

    selectSoNo(e: any) {
        this.setState({
            soNo: e.target.value
        })
    }

    selectRequiredPart(e: any) {
        this.setState({
            requiredPart: e.target.value
        })
    }

    selectCallStatus = (e: any) => {
        this.setState({
            casestatusId: e.id,
            casestatus: e.header
        })
    }

    selectCallAction = (e: any) => {
        this.setState({
            selectedCallAction: e.header,
            selectedCallActionId: e.id
        })
    }

    selectPartConsumptionType = (e: any) => {
        this.setState({
            selectedPartConsumptionType: e.header,
            selectedPartConsumptionTypeId: e.id
        })
    }

    userRemarks(event: any) {
        this.setState({
            remarks: event.target.value
        })
    }

    ////////////////////////////////// File Upload ////////////////////////////////////////
    fileUpload() {
        (document.getElementById('upload') as HTMLInputElement).click()
    };

    documentUploadFnc = (e: any, item: any) => {
        //console.log("file", e)
        const newFileId = uuid();
        const file = e.target.files[0]
        if (file.size < 1048576) {
            this.setState({
                fileList: [...this.state.fileList, { id: newFileId, file: e.target.files[0] }],
                documentFileList: [...this.state.documentFileList, { "callDetailId": 0,"documentId": 0,"documentTypeId": item==='CSO Copy'?1:2,"internalName": newFileId,"isActive": true }]
            });
        }

    }

    update() {
        const data = {
            "transactionType": "CALL-I",
            "assignmentId": this.state.data.assignmentId,
            "updatedBy": this.state.createdBy,
            "updatedByEmail": this.state.createdByEmail,
            "updatedByADID": this.state.createdByADID,
            "callActionId": this.state.selectedCallActionId,
            "callStatusId": this.state.casestatusId,
            "customerName": this.state.data.serviceAccount,
            "caseNumber": this.state.casenumber,
            "unitSlNo": this.state.data.serialNumber,
            "passId": this.state.passid,
            "taskStartDateTimeIST": this.state.startDateTime,
            "taskEndDateTimeIST": this.state.endDateTime,
            "closerRemarks": this.state.remarks,
            "partConsumptionTypeId": this.state.selectedPartConsumptionTypeId,
            "soNo": this.state.soNo,
            "requiredPart": null,
            "requiredSparePartNo": null,
            "requiredPartName": null,
            "faultyPartCTNo": null,
            "failureId": null,
            "issueDescription": this.state.data.caseSubject,
            "troubleshootingStep": null,
            "firstPartConsumptionTypeId": null,
            "firstPartSONo": null,
            "firstRequiredPartName": null,
            "receivedPartConsumptionTypeId": null,
            "receivedPartName": null,
            "callDocumentList": this.state.documentFileList
        }
        this.submit(data)
    }

    submit(data: any) {
        var ticketUpdateFormData = new FormData()
        ticketUpdateFormData.append("eventData", JSON.stringify(data));
        console.log('ticketUpdateFormData', ticketUpdateFormData);
        console.log('ticketUpdateFormData 2', data);

        if (this.state.fileList.length > 0) {
            Promise.all(this.state.fileList.map((item: any) => {
                ticketUpdateFormData.append(item.id, item.file);
                return ticketUpdateFormData
            })).then(() => {

                engineerActionTicketAPI(ticketUpdateFormData).then((response: any) => {
                    console.log('In adminUpdateTicketAPI', response);
                    microsoftTeams.tasks.submitTask()
                    if (response.data) {
                        this.setState({
                            loading: false
                        })
                    }
        
                })
            });
        }

        // engineerActionTicketAPI(ticketUpdateFormData).then((response: any) => {
        //     console.log('In adminUpdateTicketAPI', response);
        //     microsoftTeams.tasks.submitTask()
        //     if (response.data) {
        //         this.setState({
        //             loading: false
        //         })
        //     }

        // })
    }

    buttonDisable(value: any) {
        if(value === 1){
            return (this.state.passid && (this.state.fileList.length > 0) && this.state.startDateTime && this.state.endDateTime) ? false : true
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
                                {this.state.data.callStatus !== 'Closed' && <Tab>Action</Tab>}
                                <Tab>Details</Tab>
                                <Tab>Activities</Tab>
                            </TabList>
                        </div>
                        {this.state.data.callStatus !== 'Closed' && <TabPanel>
                            <div className=''>
                                <Container fluid>
                                    <div className=''>
                                        <Row>
                                            <div className="col-12">
                                                <div className="user_action_header_info">
                                                    <Row>
                                                        <div className="col-md-4 mb-2 mb-md-0">
                                                            <div>
                                                                <Text className='d-block mb-1 caseNumber' content={this.state.casenumber} size="medium" weight="regular" />
                                                                <div className={`statusDiv ${!this.state.data.assignmentId ? 'notassignedStatus' : this.state.data.callStatus === 'Open' ? 'openStatus' : this.state.data.callStatus === 'Closed' ? 'closedStatus' : 'observationStatus'}`}><Text className='ml-2 mr-2 mt-1 mb-1 p-0 textEllipsis taskDashboardTableWidth' content={!this.state.data.assignmentId ? "Not Assigned" : this.state.data.callStatus} /></div>
                                                            </div>
                                                        </div>
                                                        <div className='col-sm-4 mb-2 mb-md-0'>
                                                            <div className='d-flex align-items-center'>
                                                                <div>
                                                                    <Building20Regular />
                                                                </div>
                                                                <div className='ms-2'>
                                                                    <Text className='d-block' color="grey" content="Customer name" size="small" timestamp />
                                                                    <Text className='d-block' content={this.state.data.serviceAccount} size="medium" weight="regular" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div className='col-sm-4'>
                                                            <div className='d-flex align-items-center'>
                                                                <div>
                                                                    <DocumentPageNumber20Regular />
                                                                </div>
                                                                <div className='ms-2'>
                                                                    <Text className='d-block' color="grey" content="Unit sl. no." size="small" timestamp />
                                                                    <Text className='d-block' content={this.state.data.serialNumber} size="medium" weight="regular" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </Row>
                                                </div>
                                            </div>
                                        </Row>
                                        <Row>
                                            <div className="col-md-4 mb-3 mt-3">
                                                <Text className='mb-1 p-0 d-block' color="grey" content="Action type" size="small" timestamp />
                                                <FormDropdown
                                                    fluid
                                                    items={this.state.callActionList}
                                                    placeholder="Select"
                                                    checkable
                                                    onChange={(event, { value }) => this.selectCallAction(value)}
                                                />
                                            </div>

                                        </Row>
                                        {(this.state.selectedCallActionId === 1) ? <div className=''>
                                            <div className="">
                                                <Row>
                                                    <div className="col-md-4 mb-3">
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="Pass ID" size="small" timestamp />
                                                        <FormInput fluid className='flex-fill' placeholder="Enter Pass Id" onChange={(e) => { this.selectPassId(e) }} type="number" min="1" />
                                                    </div>
                                                    <div className="col-md-4 mb-3">
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="Task start date with time" size="small" timestamp />
                                                        <Datepicker
                                                            onDateChange={(e, data: any) => this.startDateFunc(data.value)}
                                                            inputOnly
                                                            maxDate={new Date()}
                                                            inputPlaceholder="Start date"
                                                        />
                                                    </div>
                                                    <div className="col-md-4 mb-3">
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="Task end date with time" size="small" timestamp />
                                                        <Datepicker
                                                            onDateChange={(e, data: any) => this.endDateFunc(data.value)}
                                                            inputOnly
                                                            maxDate={new Date()}
                                                            inputPlaceholder="End date"
                                                        />
                                                    </div>
                                                </Row>
                                                <Row>
                                                    <div className="col-md-4 mb-3">
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="Part consumnion type" size="small" timestamp />
                                                        <FormDropdown
                                                            fluid
                                                            items={this.state.partConsumptionTypeList}
                                                            placeholder="Select"
                                                            checkable
                                                            onChange={(event, { value }) => this.selectPartConsumptionType(value)}
                                                        />
                                                    </div>
                                                    {this.state.selectedPartConsumptionTypeId !== 1 && <div className="col-md-4 mb-3">
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="SO no" size="small" timestamp />
                                                        <FormInput fluid className='flex-fill' placeholder="Enter SO no" onChange={(e) => { this.selectSoNo(e) }} type="number" min="1" />
                                                    </div>}
                                                    {this.state.selectedPartConsumptionTypeId !== 1 && <div className="col-md-4 mb-3">
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="Required part" size="small" timestamp />
                                                        <FormInput fluid className='flex-fill' placeholder="Enter Required part" onChange={(e) => { this.selectRequiredPart(e) }} />
                                                    </div>}
                                                </Row>
                                                <Row>
                                                    <div className='col-md-4 mb-3'>
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="Clouser Remarks" size="small" timestamp />
                                                        <TextArea fluid resize="vertical" placeholder="Enter remarks..." onChange={(event) => this.userRemarks(event)} />
                                                    </div>
                                                    <div className="col-md-4 mb-3">
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="Status" size="small" timestamp />
                                                        <FormDropdown fluid
                                                            items={this.state.callstatuslist}
                                                            value={this.state.casestatus}
                                                            onChange={(event, { value }) => this.selectCallStatus(value)}
                                                        />
                                                    </div>
                                                </Row>
                                                <Row>
                                                    <div className='col-md-4 mb-3'>
                                                        <Text className='mb-1 p-0 d-block' color="grey" content="CSO copy" size="small" timestamp />
                                                        <div>
                                                            <Input type="file" id="upload" style={{ display: 'none' }} onChange={(value: any) => this.documentUploadFnc(value, "CSO Copy")} ></Input>
                                                            <div onClick={() => this.fileUpload()} className='issueFileUpload input-file-cus pointer'>Upload file</div>
                                                        </div>
                                                    </div>
                                                </Row>
                                                <Flex gap="gap.small">
                                                    <FlexItem push>
                                                        <Button content="Update" primary onClick={() => this.update()} disabled={this.buttonDisable(this.state.selectedCallActionId)} />
                                                    </FlexItem>
                                                </Flex>
                                            </div>

                                        </div>:
                                        <div className=''>
                                        <div className="">
                                            <Row>
                                                <div className="col-md-4 mb-3">
                                                    <Text className='mb-1 p-0 d-block' color="grey" content="Spare Part number of required part" size="small" timestamp />
                                                    <FormInput fluid className='flex-fill' placeholder="Enter part no" onChange={(e) => { this.selectPassId(e) }} type="number" min="1" />
                                                </div>
                                                <div className="col-md-4 mb-3">
                                                    <Text className='mb-1 p-0 d-block' color="grey" content="Part Name of required part" size="small" timestamp />
                                                    <FormInput fluid className='flex-fill' placeholder="Enter part name" onChange={(e) => { this.selectPassId(e) }} type="number" min="1" />
                                                </div>
                                                <div className="col-md-4 mb-3">
                                                    <Text className='mb-1 p-0 d-block' color="grey" content="Faulty part CT no" size="small" timestamp />
                                                    <FormInput fluid className='flex-fill' placeholder="Enter CT no" onChange={(e) => { this.selectPassId(e) }} type="number" min="1" />
                                                </div>
                                                
                                            </Row>
                                            <Row>
                                                <div className="col-md-4 mb-3">
                                                    <Text className='mb-1 p-0 d-block' color="grey" content="Failure id" size="small" timestamp />
                                                    <FormInput fluid className='flex-fill' placeholder="Failure id" onChange={(e) => { this.selectPassId(e) }} type="number" min="1" />
                                                </div>
                                                <div className="col-md-4 mb-3">
                                                    <Text className='mb-1 p-0 d-block' color="grey" content="Task start date with time" size="small" timestamp />
                                                    <Datepicker
                                                        onDateChange={(e, data: any) => this.startDateFunc(data.value)}
                                                        inputOnly
                                                        maxDate={new Date()}
                                                        inputPlaceholder="Start date"
                                                    />
                                                </div>
                                                <div className="col-md-4 mb-3">
                                                    <Text className='mb-1 p-0 d-block' color="grey" content="Task end date with time" size="small" timestamp />
                                                    <Datepicker
                                                        onDateChange={(e, data: any) => this.endDateFunc(data.value)}
                                                        inputOnly
                                                        maxDate={new Date()}
                                                        inputPlaceholder="End date"
                                                    />
                                                </div>
                                               
                                            </Row>
                                            <Row>
                                                <div className='col-md-4 mb-3'>
                                                    <Text className='mb-1 p-0 d-block' color="grey" content="Issue Description" size="small" timestamp />
                                                    <TextArea fluid resize="vertical" placeholder="Enter remarks..." onChange={(event) => this.userRemarks(event)} />
                                                </div>
                                                <div className="col-md-4 mb-3">
                                                    <Text className='mb-1 p-0 d-block' color="grey" content="Status" size="small" timestamp />
                                                    <FormDropdown fluid
                                                        items={this.state.callstatuslist}
                                                        value={this.state.casestatus}
                                                        onChange={(event, { value }) => this.selectCallStatus(value)}
                                                    />
                                                </div>
                                            </Row>
                                            <Row>
                                                <div className='col-md-4 mb-3'>
                                                    <Text className='mb-1 p-0 d-block' color="grey" content="CSO copy" size="small" timestamp />
                                                    <div>
                                                        <Input type="file" id="upload" style={{ display: 'none' }} onChange={(value: any) => this.documentUploadFnc(value, "CSO Copy")} ></Input>
                                                        <div onClick={() => this.fileUpload()} className='issueFileUpload input-file-cus pointer'>Upload file</div>
                                                    </div>
                                                </div>
                                                <div className='col-md-4 mb-3'>
                                                    <Text className='mb-1 p-0 d-block' color="grey" content="Part Snap" size="small" timestamp />
                                                    <div>
                                                        <Input type="file" id="upload" style={{ display: 'none' }} onChange={(value: any) => this.documentUploadFnc(value, "Part Snap")} ></Input>
                                                        <div onClick={() => this.fileUpload()} className='issueFileUpload input-file-cus pointer'>Upload file</div>
                                                    </div>
                                                </div>
                                            </Row>
                                            <Flex gap="gap.small">
                                                <FlexItem push>
                                                    <Button content="Update" primary onClick={() => this.update()} disabled={this.buttonDisable(this.state.selectedCallActionId)} />
                                                </FlexItem>
                                            </Flex>
                                        </div>

                                    </div>}
                                    </div>
                                </Container>
                            </div>
                        </TabPanel>}
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
                                                            <Text className='d-block' color="grey" content="Serial No." size="small" timestamp />
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
                                                <div className='col-sm-6'>
                                                    <div className='d-flex mb-3'>
                                                        <div>
                                                            <Chat20Regular />
                                                        </div>
                                                        <div className='ms-2'>
                                                            <Text className='d-block' color="grey" content="Remarks" size="small" timestamp />
                                                            <Text className='d-block' content={this.state.data.callAction} size="medium" weight="regular" />
                                                        </div>
                                                    </div>
                                                </div>
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
                                                        <Text className='d-block mb-2 caseNumber' content={e.callAction} size="small" weight="bold" />
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
                                                                        <Text className='d-block' color="grey" content="Required Part" size="small" timestamp />
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

                    </Tabs>
                </div>}
            </div>

        );
    }
}

export default DetailTicketEnduser;