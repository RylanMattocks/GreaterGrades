import { useContext, useState, useEffect } from "react";
import { useGetGrades } from "../greatergradesapi/Grades";
import { UserContext } from "../functions/UserContext";
import GradePopup from "./GradePopup";
import { deleteAssignment, useGetAllAssignments } from "../greatergradesapi/Assignment";
import UpdateAssignmentPopup from "./UpdateAssignmentPopup";

const AssignmentTile = ({ assignment, onDelete, onUpdate, isTeacherOrAdmin }) => {
    const { currentUser, authToken } = useContext(UserContext);
    const [gradePopupOpen, setGradePopupOpen] = useState(false);
    const [popupOpen, setPopupOpen] = useState(false);
    const [refresh, setRefresh] = useState(false);
    const grades = useGetGrades(refresh);
    const assignments = useGetAllAssignments(refresh);

    const grade = grades?.filter(grade => grade.assignmentId === assignment.assignmentId);
    const newAssignment = assignments?.find(assign => assign.assignmentId === assignment?.assignmentId);

    const handleUpdateClick = () => {
        setPopupOpen(true);
    };

    const handleClose = () => {
        setPopupOpen(false);
        setRefresh(prev => !prev);
        if (onUpdate) {
            onUpdate();
        }
    };

    const handleDeleteClick = async (e) => {
        e.stopPropagation();
        try {
            const result = await deleteAssignment(assignment.assignmentId, authToken);
            if (result === 'Deleted') {
                if (onDelete) {
                    await onDelete(assignment.assignmentId);
                }
                if (onUpdate) {
                    onUpdate();
                }
            }
        } catch (error) {
            console.error("Error deleting assignment:", error);
        }
    };
    
    const handleTileClick = (e) => {
        e.stopPropagation();
        setGradePopupOpen(true);
    };

    const handleGradePopupClose = () => {
        setGradePopupOpen(false);
        if (onUpdate) {
            onUpdate();
        }
    };

    return (
        <div className="user-tile">
            <h4 className="user-name" onClick={handleTileClick}>{assignment.name}</h4>
            <p className="user-role">{assignment.grade}/{assignment.maxScore}</p>
            {isTeacherOrAdmin && (
                <div>
                    <button className="delete-icon" onClick={handleDeleteClick}>🗑️</button>
                    <button className="delete-icon" onClick={handleUpdateClick}>✏️</button>
                    {popupOpen && (
                        <UpdateAssignmentPopup
                            onClose={handleClose}
                            id={assignment.assignmentId}
                            classId={assignment.classId}
                        />
                    )}
                    {gradePopupOpen && (
                        <GradePopup assignmentId={assignment.assignmentId} onClose={handleGradePopupClose} />
                    )}
                </div>
            )}
        </div>
    );
};

export default AssignmentTile;
