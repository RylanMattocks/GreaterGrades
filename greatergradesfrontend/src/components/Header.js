import { RoleEnum } from "../enum/Role";
import { useContext, useState } from "react";
import { UserContext } from '../functions/UserContext';

const Header = () => {
    const [menuOpen, setMenuOpen] = useState(false);
    const { currentUser, logout } = useContext(UserContext);

    const handleLogout = () => {
        logout();
    };

    return (
        <div className="dashboard-header">
            <div className="dashboard-info">
                <h3>{currentUser?.firstName} {currentUser?.lastName}</h3>
                <p className="username">{currentUser?.username}</p>
                <p className="role-badge">{RoleEnum[currentUser?.role]}</p>
            </div>
            <div className="menu-icon" onClick={() => setMenuOpen(!menuOpen)}>
                &#9776;
            </div>
            {menuOpen && (
                <div className="dropdown-menu">
                    <button onClick={handleLogout}>Logout</button>

                </div>
            )}
        </div>
    );    
    
};



export default Header;