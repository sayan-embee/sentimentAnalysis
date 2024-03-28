import { Link } from 'react-router-dom';
import Button from 'react-bootstrap/Button';

const base_URL = window.location.origin;
const embeelogo = base_URL + "/images/Embee-Logo.png";

const Header = () => {
  return (
    <div className="container-fluid">
<div className="headerdiv mb-3 px-4" style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
      <img src={embeelogo} alt="embee logo" style={{ height: "32px", margin: "6px" }}/>
      {/* <div>
        <Link to="/homepage">
          <Button variant="primary" size="sm">
            Home
          </Button>
        </Link>{' '}
        <Link to="/dashboard">
          <Button variant="primary" size="sm">
            View Data
          </Button>
        </Link>{' '}
      </div> */}
      <div>
        <ul className='navigation'>
            <li>
                <a href="#" className='active'>Home</a>
            </li>
            <li>
                <a href="#">View Data</a>
            </li>
        </ul>
        
      </div>
    </div>
    </div>
    
  );
}

export default Header;
