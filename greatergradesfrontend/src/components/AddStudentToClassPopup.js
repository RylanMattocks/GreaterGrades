import React, { useState, useContext } from 'react';
import { useGetAllUsers } from '../greatergradesapi/Users';
import { UserContext } from '../functions/UserContext';
import { addStudentToClass } from '../greatergradesapi/Classes';

const AddStudentToClassPopup = ({ onClose, courseId }) => {
    const { authToken, currentUser } = useContext(UserContext);
    const [selectedStudent, setSelectedStudent] = useState('');
    const [error, setError] = useState('');
    const allUsers = useGetAllUsers();
    
    // Filter to only show students (role 0)
    const availableStudents = allUsers.filter(user => user.role === 0);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!selectedStudent) {
            setError('Please select a student');
            return;
        }

        const result = await addStudentToClass(courseId, selectedStudent, authToken);
        if (result === 'Added') {
            onClose(true);
        } else {
            setError('Failed to add student to class');
        }
    };

    return (
        <div className="popup-overlay">
            <div className="popup">
                <h2>Add Student to Class</h2>
                <form onSubmit={handleSubmit}>
                    <div>
                        <label htmlFor="student">Select Student:</label>
                        <select
                            id="student"
                            value={selectedStudent}
                            onChange={(e) => setSelectedStudent(e.target.value)}
                            required
                        >
                            <option value="">Select a student...</option>
                            {availableStudents.map(student => (
                                <option key={student.userId} value={student.userId}>
                                    {student.firstName} {student.lastName} ({student.username})
                                </option>
                            ))}
                        </select>
                    </div>
                    {error && <p className="error">{error}</p>}
                    <div className="popup-buttons">
                        <button type="submit">Add Student</button>
                        <button type="button" onClick={() => onClose(false)}>Cancel</button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default AddStudentToClassPopup;
