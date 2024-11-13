import forestImage from "../images/forest.jfif";
import { deleteInstitution } from "../greatergradesapi/Institutions";
import { useContext, useState } from "react";
import { UserContext } from "../functions/UserContext";
import UpdateInstitutionPopup from "./UpdateInstitutionPopup";

const InstitutionTile = ({ institution, toggleTrigger }) => {
    const { authToken } = useContext(UserContext);
    const [popupInstitutionId, setPopupInstitutionId] = useState(null);

    const handleRemoveInstitutionClick = async (id) => {
        try {
            const result = await deleteInstitution(id, authToken);
            if (result === 'Deleted') {
                toggleTrigger();
            }
        } catch (error) {
            console.error("Error deleting institution:", error);
        }
    };

    const handleUpdateInstitutionClick = (id) => {
        setPopupInstitutionId(prevId => prevId === id ? null : id);
    };

    const handlePopupClose = () => {
        setPopupInstitutionId(null);
        toggleTrigger();
    };

    return (
        <div className="dashboard-tile-link">
            <div className="dashboard-tile">
                <h3 className="tile-title">{institution.name}</h3>
                <img src={forestImage} alt="Institution placeholder" className="tile-image" />
                <div className="tile-footer">
                    <button 
                        className="delete-icon"
                        onClick={(e) => {
                            e.stopPropagation();
                            handleRemoveInstitutionClick(institution.institutionId);
                        }}
                    >
                        🗑️
                    </button>
                    <button 
                        className="delete-icon"
                        onClick={() => handleUpdateInstitutionClick(institution.institutionId)}
                    >
                        ✏️
                    </button>
                </div>
                {popupInstitutionId === institution.institutionId && (
                    <UpdateInstitutionPopup 
                        onClose={handlePopupClose} 
                        institutionId={institution.institutionId} 
                    />
                )}
            </div>
        </div>
    );
};

export default InstitutionTile;