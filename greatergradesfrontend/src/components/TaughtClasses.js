import { useContext, useState } from "react";
import { UserContext } from '../functions/UserContext';
import Tiles from "./Tiles";
import AddClassPopup from "./AddClassPopup";

const TaughtClasses = ({ setSelectedItem }) => {
    const { currentUser } = useContext(UserContext);
    const [isPopupOpen, setPopupOpen] = useState(false);

    const handleAddClassClick = () => {
        setPopupOpen(true);
    };

    return (
        <div className="dashboard-content">
            <div className="dashboard-header">
                <h3>Taught Classes</h3>
                {currentUser?.role > 0 && (
                    <button className="add-class-button" onClick={handleAddClassClick}>
                        Add New Class
                    </button>
                )}
            </div>
            <div className="dashboard-tiles">
                <Tiles courseIds={currentUser?.taughtClassIds} setSelectedItem={setSelectedItem} />
            </div>
            {isPopupOpen && (
                <AddClassPopup onClose={() => setPopupOpen(false)} institutionId={currentUser.institutionId} />
            )}
        </div>
    );
}

export default TaughtClasses;
