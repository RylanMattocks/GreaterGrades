
import { RoleEnum } from "../enum/Role";
import { useContext, useState } from "react";
import { UserContext } from '../functions/UserContext';
import { useGetInstitutionById } from "../greatergradesapi/Institutions";
import { FaUser, FaUserCircle, FaBuilding, FaIdBadge, FaEdit, FaSave } from 'react-icons/fa';
import '../styles/profile.css';

const ProfileContent = () => {
    const { currentUser, setCurrentUser, authToken } = useContext(UserContext);
    const { institution, loading } = useGetInstitutionById(currentUser?.institutionId);
    const [isEditing, setIsEditing] = useState(false);
    const [editedUser, setEditedUser] = useState({
        firstName: currentUser?.firstName,
        lastName: currentUser?.lastName
    });
    const [error, setError] = useState('');

    const handleEdit = () => {
        setIsEditing(true);
    };

    const handleSave = async () => {
        try {
            const response = await fetch(`http://localhost:5000/api/Auth/${currentUser.userId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${authToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    firstName: editedUser.firstName,
                    lastName: editedUser.lastName
                })
            });

            if (response.ok) {
                setCurrentUser({
                    ...currentUser,
                    firstName: editedUser.firstName,
                    lastName: editedUser.lastName
                });
                setIsEditing(false);
                setError('');
            } else {
                const errorData = await response.text();
                setError(`Failed to update profile: ${errorData}`);
            }
        } catch (error) {
            console.error('Error updating profile:', error);
            setError('Failed to update profile. Please try again.');
        }
    };

    const handleCancel = () => {
        setEditedUser({
            firstName: currentUser?.firstName,
            lastName: currentUser?.lastName
        });
        setIsEditing(false);
        setError('');
    };

    return (
        <div className="profile-card">
            <div className="profile-header">
                <h2>Profile Information</h2>
                {!isEditing ? (
                    <button className="edit-button" onClick={handleEdit}>
                        <FaEdit /> Edit Profile
                    </button>
                ) : (
                    <div className="button-group">
                        <button className="save-button" onClick={handleSave}>
                            <FaSave /> Save
                        </button>
                        <button className="cancel-button" onClick={handleCancel}>
                            Cancel
                        </button>
                    </div>
                )}
            </div>
            {error && <div className="error-message">{error}</div>}
            <div className="profile-field">
                <FaUser />
                <span className="field-label">First Name:</span>
                {isEditing ? (
                    <input
                        type="text"
                        value={editedUser.firstName}
                        onChange={(e) => setEditedUser({...editedUser, firstName: e.target.value})}
                        className="profile-input"
                    />
                ) : (
                    currentUser?.firstName
                )}
            </div>
            <div className="profile-field">
                <FaUser />
                <span className="field-label">Last Name:</span>
                {isEditing ? (
                    <input
                        type="text"
                        value={editedUser.lastName}
                        onChange={(e) => setEditedUser({...editedUser, lastName: e.target.value})}
                        className="profile-input"
                    />
                ) : (
                    currentUser?.lastName
                )}
            </div>
            <div className="profile-field">
                <FaUserCircle />
                <span className="field-label">Username:</span>
                {currentUser?.username}
            </div>
            <div className="profile-field">
                <FaIdBadge />
                <span className="field-label">Role:</span>
                {RoleEnum[currentUser?.role]}
            </div>
            <div className="profile-field">
                <FaBuilding />
                <span className="field-label">Institution:</span>
                {loading ? 'Loading...' : institution?.name}
            </div>
        </div>
    );
};

export default ProfileContent;




