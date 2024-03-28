
import Spinner from "react-bootstrap/Spinner";
import { getSentimentAnalysisDataAPI } from "./apis/APIList";
import "bootstrap-css-only/css/bootstrap.min.css";
import { Modal, Button } from "react-bootstrap";
import "mdbreact/dist/css/mdb.css";
import React from "react";
import Header from './header';
import { Text } from "@fluentui/react-northstar";
import { Eye20Regular} from '@fluentui/react-icons'

interface MyState {
  data?: any;
  loading?: any;
  transcribeText: string;
  show: boolean;
  currentItem: any;
  modalContent: any; 
}

interface IDashboardProps {}

class SentimentTableList extends React.Component<IDashboardProps, MyState> {
  constructor(props: IDashboardProps) {
    super(props);
    this.state = {
      data: [],
      loading: true,
      transcribeText: "",
      show: false,
      currentItem: null,
      modalContent: null, 
    };
  }

  componentDidMount() {
    this.getSentimentAnalysisData();
  }

  getSentimentAnalysisData = () => {
    getSentimentAnalysisDataAPI()
      .then((response) => {
        console.log("In getSentimentAnalysisDataAPI", response);
        this.setState({ data: response.data, loading: false });
      })
      .catch((error) => {
        console.error("Error fetching data:", error);
        this.setState({ loading: false });
      });
  };

  handleClose = () => {
    this.setState({ show: false });
  };

  
  handleShow = (item: any, modalContent: any) => {
    this.setState({ currentItem: item, modalContent: modalContent, show: true });
  };

  render() {
    const { currentItem, modalContent } = this.state;

    return (
      <div>
        <Header/>
        <div className="bodyContainer">
          <div className="admin-report">
            {this.state.loading ? (
              <div className="d-flex justify-content-center">
                <Spinner animation="grow" variant="info" />
                <Spinner animation="grow" variant="info" />
                <Spinner animation="grow" variant="info" />
              </div>
            ) : (
              <div className="container-fluid">
                {this.state.data.length > 0 ? (
                  <div >
                    <table className="table table-bordered dataTable">
                      <thead>
                        <tr>
                          <th className="text-center">Id</th>
                          <th className="text-center">Sentiment</th>                          
                          <th className="text-center">File Internal Name</th>                          
                          <th className="text-center" style={{width:"300px"}}>File Url</th>
                          <th className="text-center" style={{minWidth:"300px"}}>Reason</th>
                          <th className="text-center">Summary</th>
                          <th className="text-center">Transcribe</th>
                        </tr>
                      </thead>
                      <tbody>
                        {this.state.data.map((item: any, index: number) => (
                          <tr key={index}>
                            <td>{item.autoId}</td>
                            <td>
                                <Text className={`status ${item.sentiment === "Positive" ? "positive" : item.sentiment === "Negative" ? "negative" :item.sentiment === "Neutral"? "neutral":"mixed"}`} content={item.sentiment}/>
                            </td>
                            <td>{item.fileInternalName}</td>
                            <td>
                                <div onClick={() => window.open(item.fileUrl, "_blank")}><p className="url_link">{item.fileUrl}</p>
                                {/* <Text content={} /> */}
                                </div>
                                </td>
                            <td>{item.reason}</td>                           
                            
                            <td className="text-center">       
                                <button className="tbl_buton" onClick={() => this.handleShow(item, "summary")} type="button"><Eye20Regular/></button>
                            </td>
                            <td className="text-center">       
                                <button className="tbl_buton" onClick={() => this.handleShow(item, "transcribe")} type="button"><Eye20Regular/></button>
                            </td>
                            {/* <td>  
                            <Button variant="btn btn-primary btn-sm" onClick={() => this.handleShow(item, "transcribe")}>View Transcribe</Button>
                            </td> */}
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                ) : (
                  <div className="noDataText">No data available</div>
                )}
              </div>
            )}
          </div>
        </div>
        <Modal show={this.state.show} onHide={this.handleClose}>
          <Modal.Header >
          <Modal.Title>{modalContent === "summary" ? "Summary" : "Transcribe"}</Modal.Title>            
          </Modal.Header>
          <Modal.Body>
            {modalContent === "summary" && currentItem && (<p>{currentItem.summaryText}</p>)}
            {modalContent === "transcribe" && currentItem && (<p>{currentItem.transcribeText}</p>)}
          </Modal.Body>
          <Modal.Footer>
            <Button variant="btn btn-primary btn-sm" onClick={this.handleClose}>
              Close
            </Button>
          </Modal.Footer>
        </Modal>
      </div>
    );
  }
}

export default SentimentTableList;


