import { useState, useContext, useEffect } from "react";
import { UserContext } from "../functions/UserContext";
import { useGetAllClasses } from "../greatergradesapi/Classes";
import Tiles from "./Tiles";
import AddClassPopup from "./AddClassPopup";

const ClassesContent = ({setSelectedItem}) => {
    const { currentUser } = useContext(UserContext);
    const [isPopupOpen, setPopupOpen] = useState(false);
    const [refreshTrigger, setRefreshTrigger] = useState(0);
    
    // Use refreshTrigger to force updates
    const allClasses = useGetAllClasses(refreshTrigger);
    const classIds = allClasses.flatMap(course => course.classId);


    const handleAddClassClick = () => {
        setPopupOpen(true);
    };

    const handlePopupClose = () => {
        setPopupOpen(false);
        setRefreshTrigger((prev) => prev + 1);
    };

    return (
        <div className="dashboard-content">
            <div className="dashboard-header">
                <h3>Classes</h3>
                <button className="add-class-button" onClick={handleAddClassClick}>
                    Add New Class
                </button>
            </div>
            <div className="dashboard-tiles">
                <Tiles courseIds={classIds} setSelectedItem={setSelectedItem}/>
            </div>
            {isPopupOpen && (
                <AddClassPopup 
                    onClose={handlePopupClose} 
                    institutionId={currentUser?.institutionId} 
                />
            )}
        </div>
    )
}

export default ClassesContent;