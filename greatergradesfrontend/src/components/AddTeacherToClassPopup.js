import React, { useState, useContext } from 'react';
import { UserContext } from '../functions/UserContext';
import { addTeacherToClass } from '../greatergradesapi/Classes';
import { useGetAllUsers } from '../greatergradesapi/Users';

const AddTeacherToClassPopup = ({ onClose, courseId }) => {
    const { authToken } = useContext(UserContext);
    const [selectedTeacher, setSelectedTeacher] = useState('');
    const [error, setError] = useState('');
    const allUsers = useGetAllUsers();
    
    // Filter to only show teachers (role 1)
    const availableTeachers = allUsers.filter(user => user.role === 1);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!selectedTeacher) {
            setError('Please select a teacher');
            return;
        }

        try {
            await addTeacherToClass(courseId, selectedTeacher, authToken);
            onClose(true);
        } catch (error) {
            setError('Failed to add teacher to class');
        }
    };

    return (
        <div className="popup-overlay">
            <div className="popup">
                <h2>Add Teacher to Class</h2>
                <form onSubmit={handleSubmit}>
                    <div>
                        <label htmlFor="teacher">Select Teacher:</label>
                        <select
                            id="teacher"
                            value={selectedTeacher}
                            onChange={(e) => setSelectedTeacher(e.target.value)}
                            required
                        >
                            <option value="">Select a teacher...</option>
                            {availableTeachers.map(teacher => (
                                <option key={teacher.userId} value={teacher.userId}>
                                    {teacher.firstName} {teacher.lastName} ({teacher.username})
                                </option>
                            ))}
                        </select>
                    </div>
                    {error && <p className="error">{error}</p>}
                    <div className="popup-buttons">
                        <button type="submit">Add Teacher</button>
                        <button type="button" onClick={() => onClose(false)}>Cancel</button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default AddTeacherToClassPopup;
