import { useContext, useState } from "react";
import { addAssignment } from "../greatergradesapi/Assignment";
import { UserContext } from "../functions/UserContext";

const AddAssignmentPopup = ({ onClose, classId }) => {
    const { authToken } = useContext(UserContext);
    const [error, setError] = useState('');
    const [assignmentName, setAssignmentName] = useState('');
    const [maxScore, setMaxScore] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!assignmentName || !maxScore) {
            setError('All fields are required');
            return;
        }

        await addAssignment(assignmentName, classId, maxScore, authToken)
        onClose();
    }

    return (
        <div className="popup-overlay">
            <div className="popup">
                <h2>Add New Assignment</h2>
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
                        <label htmlFor="maxScore">Max Score:</label>
                        <input
                            type="text"
                            id="maxScore"
                            value={maxScore}
                            onChange={(e) => setMaxScore(e.target.value)}
                            required
                        />
                    </div>
                    {error && <p className="error">{error}</p>}
                    <button type="submit">Submit</button>
                    <button type="button" onClick={onClose}>Cancel</button>
                </form>
            </div>
        </div>
    )
}

export default AddAssignmentPopup;