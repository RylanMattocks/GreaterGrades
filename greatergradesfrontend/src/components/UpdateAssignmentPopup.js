import { useContext, useState } from "react"
import { UserContext } from "../functions/UserContext";
import { updateAssignment } from "../greatergradesapi/Assignment";


const UpdateAssignmentPopup = ({ onClose, id, classId }) => {
    const [error, setError] = useState('');
    const [assignmentName, setAssignmentName] = useState('');
    const { authToken } = useContext(UserContext);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!assignmentName){
            setError('Name is required.');
            return;
        }

        await updateAssignment(id, assignmentName, classId, authToken)
        onClose();
    }

    return (
        <div className="institution-popup-overlay">
            <div className="institution-popup">
                <h2>Update Assignment</h2>
                <form onSubmit={handleSubmit}>
                    <div>
                        <label htmlFor="assignmentName">Assignment Name:</label> 
                        <input
                            type="text"
                            id="assignmentName"
                            value={assignmentName}
                            onChange={(e) => setAssignmentName(e.target.value)}
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

export default UpdateAssignmentPopup;