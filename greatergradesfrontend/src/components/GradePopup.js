// GradePopup.js
import { useContext, useState, useEffect } from "react";
import { UserContext } from "../functions/UserContext";
import { getGrades, updateGrade } from "../greatergradesapi/Grades";
import { getUserById } from '../greatergradesapi/Auth';

const GradePopup = ({ assignmentId, onClose }) => {
    const { authToken } = useContext(UserContext);
    const [grades, setGrades] = useState([]);
    const [userDetails, setUserDetails] = useState({});
    const [error, setError] = useState(null);

    useEffect(() => {
        async function fetchGradesAndUsers() {
            try {
                const fetchedGrades = await getGrades(authToken);
                const relevantGrades = fetchedGrades.filter(grade => grade.assignmentId === assignmentId);
                setGrades(relevantGrades);

                // Fetch user data for each grade
                const userDetailsPromises = relevantGrades.map(async (grade) => {
                    const user = await getUserById(grade.userId, authToken);
                    return { userId: grade.userId, user };
                });

                const userData = await Promise.all(userDetailsPromises);
                const userDetailsMap = userData.reduce((acc, { userId, user }) => {
                    acc[userId] = user;
                    return acc;
                }, {});
                setUserDetails(userDetailsMap);
            } catch (err) {
                console.error("Error fetching grades or user data:", err);
                setError("Failed to load grades or user data.");
            }
        }
        fetchGradesAndUsers();
    }, [assignmentId, authToken]);

    const handleGradeChange = (userId, score) => {
        setGrades(prevGrades =>
            prevGrades.map(grade => 
                grade.userId === userId ? { ...grade, score } : grade
            )
        );
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            for (let grade of grades) {
                await updateGrade(grade.gradeId, grade.score, 2, authToken);
            }
            onClose();
        } catch (err) {
            console.error("Error updating grades:", err);
            setError("Failed to update grades.");
        }
    };

    return (
        <div className="popup-overlay">
            <div className="popup">
                <h2>Edit Grades</h2>
                {error && <p className="error">{error}</p>}
                <form onSubmit={handleSubmit}>
                    {grades.map(grade => {
                        const user = userDetails[grade.userId];
                        return (
                            <div key={grade.userId}>
                                <label>{user?.firstName} {user?.lastName}:</label>
                                <input
                                    type="number"
                                    value={grade.score}
                                    onChange={(e) => handleGradeChange(grade.userId, Number(e.target.value))}
                                    min="0"
                                    max={grade.maxScore}
                                    required
                                />
                            </div>
                        );
                    })}
                    <div className="popup-buttons">
                        <button type="submit">Save Grades</button>
                        <button type="button" onClick={() => onClose()}>Cancel</button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default GradePopup;
