import React from 'react';

const UserTile = ({ firstName, lastName, role, showDelete, onDelete }) => {
    return (
        <div className="user-tile">
            <h4 className="user-name">{`${firstName} ${lastName}`}</h4>
            <p className="user-role">{role}</p>
            {showDelete && (
                <button className="delete-icon" onClick={onDelete} aria-label="Remove student">
                    🗑️
                </button>
            )}
        </div>
    );
};

export default UserTile;
