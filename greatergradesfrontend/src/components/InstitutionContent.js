import { useState, useContext } from "react";
import { UserContext } from "../functions/UserContext";
import AddInstitutionPopup from "./AddInstitutionPopup";
import InstitutionTile from "./InstitutionTile";
import { useGetAllInstitutions } from "../greatergradesapi/Institutions";

const InstitutionContent = () => {
    const { currentUser } = useContext(UserContext);
    const [isAddPopupOpen, setAddPopupOpen] = useState(false);
    const [refreshTrigger, setRefreshTrigger] = useState(0);
    const institutions = useGetAllInstitutions(refreshTrigger);
    const toggleTrigger = () => setRefreshTrigger(prev => prev + 1);

    const handleAddInstitutionClick = () => {
        setAddPopupOpen(true);
    };

    const handlePopupClose = () => {
        setAddPopupOpen(false);
        setRefreshTrigger(prev => prev + 1);
    };

    return (
        <div className="dashboard-content">
            <div className="dashboard-header">
                <h3>Institutions</h3>
                <button className="add-class-button" onClick={handleAddInstitutionClick}>
                    Add New Institution
                </button>
            </div>
            <div className="tiles-container">
                {institutions.map((institution) => (
                    <InstitutionTile 
                        key={institution.institutionId} 
                        institution={institution} 
                        toggleTrigger={toggleTrigger} 
                    />
                ))}
            </div>
            {isAddPopupOpen && (
                <AddInstitutionPopup 
                    onClose={handlePopupClose}  
                />
            )}
        </div>
    );
};

export default InstitutionContent;