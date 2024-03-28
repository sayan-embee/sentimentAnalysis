import { Link } from "react-router-dom";
import Button from "react-bootstrap/Button";

const base_URL = window.location.origin;
const embeelogo = base_URL + "/images/Embee-Logo.png";

interface HeaderProps {
  currentPage: string;
}

const Header = ({ currentPage }: HeaderProps) => {
  return (
    <div className="container-fluid">
      <div
        className="headerdiv mb-3 px-4"
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <img
          src={embeelogo}
          alt="embee logo"
          style={{ height: "32px", margin: "6px" }}
        />
        
        <div>
          <ul className="navigation">
            

            <li>
              <a
                href="/homepage"
                className={currentPage === "homepage" ? "active" : ""}
              >
                Home
              </a>
            </li>
            <li>
              <a
                href="/dashboard"
                className={currentPage === "dashboard" ? "active" : ""}
              >
                Data List
              </a>
            </li>
          </ul>
        </div>
      </div>
    </div>
  );
};

export default Header;
