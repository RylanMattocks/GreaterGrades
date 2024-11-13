import { useContext, useState } from "react"
import { UserContext } from "../functions/UserContext";
import { updateInstitution } from "../greatergradesapi/Institutions";


const UpdateInstitutionPopup = ({ onClose, institutionId }) => {
    const [error, setError] = useState('');
    const [institutionName, setInstitutionName] = useState('');
    const { authToken } = useContext(UserContext);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!institutionName){
            setError('Name is required.');
            return;
        }

        await updateInstitution(institutionId, institutionName, authToken);
        onClose();
    }

    return (
        <div className="institution-popup-overlay">
            <div className="institution-popup">
                <h2>Update Institution</h2>
                <form onSubmit={handleSubmit}>
                    <div>
                        <label htmlFor="institutionName">Institution Name:</label>
                        <input
                            type="text"
                            id="institutionName"
                            value={institutionName}
                            onChange={(e) => setInstitutionName(e.target.value)}
                            required
                        />
                    </div>
                    {error && <p className="institution-error">{error}</p>}
                    <button type="submit">Submit</button>
                    <button type="button" onClick={onClose}>Cancel</button>
                </form>
            </div>
        </div>
    )
}

export default UpdateInstitutionPopup;