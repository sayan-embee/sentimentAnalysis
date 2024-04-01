import React from "react";

import { sentimentAnalysisAPI } from "./apis/APIList";
import { Loader, Text, Input } from "@fluentui/react-northstar";
import Header from "./header";
import "./App.scss";
import Accordion from 'react-bootstrap/Accordion';



const base_URL = window.location.origin;
const bgimg = base_URL + "/images/banner-graphics.png";
const buttonimg = base_URL + "/images/button.png";


interface MyState {
  loading?: any;
  searchResponse?: any;
  sentiment?: any;
  outputModel?: any;
  summary?:any
}

interface ISearchContentProps { }

class SentimentAnalysisPage extends React.Component<
  ISearchContentProps,
  MyState
> {
  constructor(props: ISearchContentProps) {
    super(props);
    this.state = {
      loading: false,
      outputModel: false
    };
  }

  componentDidMount() { }

  fileUpload() {
    (document.getElementById("upload") as HTMLInputElement).click();
  }

  documentUploadFnc = (e: any) => {
    const file = e.target.files[0];
    var fileUploadFormData = new FormData();
    fileUploadFormData.append("F1", file);
    this.setState(
      {
        loading: true,
        outputModel: false
      },
      () => {
        sentimentAnalysisAPI(fileUploadFormData).then((response) => {
          console.log("res", response);
          if (response.data) {
            this.setState({
              loading: false,
              outputModel: true,
              searchResponse: response.data.outputModel && response.data.outputModel.reason,
              sentiment: response.data.outputModel && response.data.outputModel.sentiment,
              summary: response.data.outputModel && response.data.outputModel.summaryText,
            });
          }
        });
      }
    );
  };

  render() {
    return (
      <div>
        <Header currentPage="homepage" />
        <div className="bodyContainer">
          <div className="container-fluid">
            <div className="row">
              <div className="col-sm-6 leftPart">
                <div style={{ width: "85%" }}>
                  <Text
                    content="Sentiment Analysis"
                    weight="bold"
                    className="title"
                  />
                  <div style={{ marginBottom: "10px" }}>
                    <Text
                      content="from Call Recordings with"
                      weight="bold"
                      className="subtitle"
                    />
                    <Text
                      content=" AI"
                      weight="bold"
                      className="subtitle redTag"
                      color="Red"
                    />
                  </div>
                  <Text
                    className="description"
                    content="Experience seamless sentiment analysis with our web application designed for call recording uploads. Harnessing the power of open API technology, our platform swiftly analyzes the sentiment of your recordings, providing valuable insights for enhanced communication strategies and customer satisfaction."
                  />
                  <div className="mt_15">
                    <Input
                      type="file"
                      id="upload"
                      style={{ display: "none" }}
                      onChange={(value: any) => this.documentUploadFnc(value)}
                    ></Input>
                    <div className="pointer" onClick={() => this.fileUpload()}>
                      {" "}
                      <img src={buttonimg} alt="filter" />
                    </div>
                  </div>

                  {this.state.loading ? (<div className="loaderdiv">
                    <Loader size="small" label="Uploading files and sentiment analysis is being performed" />
                  </div>)
                  
                  
                  : <div>
                    {this.state.outputModel && < Accordion className={"accordian"} defaultActiveKey={['0']} alwaysOpen>
                    <Accordion.Item eventKey="0">
                      <Accordion.Header>Sentiment</Accordion.Header>
                      <Accordion.Body>
                        <Text content={this.state.sentiment} weight="bold" className={`${this.state.sentiment === 'Negative' ? 'redTag' : this.state.sentiment === 'Positive' ? 'greenTag' : 'blueTag'} `} style={{ fontSize: "24px" }} />
                      </Accordion.Body>
                    </Accordion.Item>
                    <Accordion.Item eventKey="1">
                      <Accordion.Header>Reason</Accordion.Header>
                      <Accordion.Body>
                        <Text content={this.state.searchResponse} style={{ fontSize: "16px" }} />
                      </Accordion.Body>
                    </Accordion.Item>
                    <Accordion.Item eventKey="2">
                      <Accordion.Header>Summary</Accordion.Header>
                      <Accordion.Body>
                        <Text content={this.state.summary} style={{ fontSize: "16px" }} />
                      </Accordion.Body>
                    </Accordion.Item>
                  </Accordion>
                  }
                </div> }
              </div>
            </div>
            <div className="col-sm-6">
              <div className="rightPart">
                <img src={bgimg} alt="filter" />
              </div>
            </div>
          </div>
        </div>
      </div>
      </div >
    );
  }
}

export default SentimentAnalysisPage;
