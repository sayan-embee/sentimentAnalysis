import { Link } from "react-router-dom";
import React from "react";

import { useHistory } from "react-router-dom";
import "./App.scss";

const base_URL = window.location.origin;
const embeelogo = base_URL + "/images/Embee-Logo.png";



interface HeaderProps {
  currentPage: string;
}




const Header = ({ currentPage }: HeaderProps) => {
 
  const history = useHistory();

 
  return (
    <div className="container-fluid">
      <div className="headerdiv mb-3 px-4" style={{ display: "flex",justifyContent: "space-between",alignItems: "center" }}>
        <img src={embeelogo} alt="embee logo" style={{ height: "32px", margin: "6px" }}/>
        
        <div>
          <ul className="navigation">
            
            <li>
              <div onClick={()=>history.push("/homepage")} className={`pointer ${currentPage === "homepage" && "active"}`}>
                Home
              </div>
            </li>
            <li>
              <div onClick={()=>history.push("/dashboard")} className={`pointer ${currentPage === "dashboard" && "active"}`}>
                {/* //href="/dashboard"
                className={this.props.currentPage === "dashboard" ? "active" : ""}
              > */}
                Data List
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>
  );
}

export default Header;
