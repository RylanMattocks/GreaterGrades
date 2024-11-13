import React, { useState, useContext, useCallback, useEffect } from 'react';
import { UserContext } from '../functions/UserContext';
import { getStorageItem } from "../functions/LocalStorage";
import UserTile from './UserTile';
import { RoleEnum } from "../enum/Role";
import { deleteStudentFromClass, deleteTeacherFromClass } from "../greatergradesapi/Classes";
import AddStudentToClassPopup from './AddStudentToClassPopup';
import AssignmentTile from './AssignmentTile';
import AddAssignmentPopup from './AddAssignmentPopup';
import AddTeacherToClassPopup from './AddTeacherToClassPopup';

const AddTile = ({ onClick, label }) => (
    <div className="add-tile" onClick={onClick}>
        <button className="add-icon">╋</button>
        <span>Add {label}</span>
    </div>
);

const CourseContent = () => {
    const { currentUser, authToken } = useContext(UserContext);
    const currentCourse = getStorageItem('currentCourse');
    const [isAssignmentPopupOpen, setIsAssignmentPopupOpen] = useState(false);
    const [isStudentPopupOpen, setIsStudentPopupOpen] = useState(false);
    const [isTeacherPopupOpen, setIsTeacherPopupOpen] = useState(false);
    const [courseData, setCourseData] = useState(null);

    // Function to fetch course data
    const fetchCourseData = useCallback(async () => {
        try {
            const response = await fetch(`http://localhost:5000/api/Classes/${currentCourse?.classId}`, {
                headers: {
                    'Authorization': `Bearer ${authToken}`,
                    'Content-Type': 'application/json'
                }
            });
            if (!response.ok) throw new Error('Failed to fetch course data');
            const data = await response.json();
            setCourseData(data);
        } catch (error) {
            console.error("Error fetching course data:", error);
        }
    }, [authToken, currentCourse?.classId]);

    // Initial fetch
    useEffect(() => {
        fetchCourseData();
    }, [fetchCourseData]);

    const handleDeleteStudent = async (studentId) => {
        try {
            // Optimistic update
            setCourseData(prev => ({
                ...prev,
                students: prev.students.filter(student => student.userId !== studentId)
            }));

            const response = await deleteStudentFromClass(courseData.classId, studentId, authToken);
            if (response !== 'Deleted') {
                // Revert if failed
                fetchCourseData();
            }
        } catch (error) {
            console.error("Error removing student from class:", error);
            fetchCourseData();
        }
    };

    const handleDeleteTeacher = async (teacherId) => {
        try {
            // Optimistic update
            setCourseData(prev => ({
                ...prev,
                teachers: prev.teachers.filter(teacher => teacher.userId !== teacherId)
            }));

            const response = await deleteTeacherFromClass(courseData.classId, teacherId, authToken);
            if (response !== 'Deleted') {
                // Revert if failed
                fetchCourseData();
            }
        } catch (error) {
            console.error("Error removing teacher from class:", error);
            fetchCourseData();
        }
    };

    const handleDeleteAssignment = async (assignmentId) => {
        try {
            // Optimistic update
            setCourseData(prev => ({
                ...prev,
                assignments: prev.assignments.filter(assignment => assignment.assignmentId !== assignmentId)
            }));

            const response = await fetch(`http://localhost:5000/api/Classes/${courseData.classId}/assignments/${assignmentId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${authToken}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                // Revert if failed
                fetchCourseData();
            }
        } catch (error) {
            console.error("Error deleting assignment:", error);
            fetchCourseData();
        }
    };

    const handlePopupClose = () => {
        setIsStudentPopupOpen(false);
        setIsTeacherPopupOpen(false);
        setIsAssignmentPopupOpen(false);
        fetchCourseData(); // Refresh data when popup closes
    };

    const isTeacherOrAdmin = currentUser?.role >= 1;

    if (!courseData) {
        return <div>Loading course data...</div>;
    }

    return (
        <div className="course-content">
            <h3 className="course-title">{courseData.subject}</h3>
            <div className="course-body">
                {/* Students section */}
                <div className="course-list-title">
                    <p>Students: {courseData.students?.length || 0}</p>
                    <div className="course-list-line" />
                    <div className="course-list-entries">
                        {courseData.students?.map((student) => (
                            <UserTile
                                key={student.userId}
                                firstName={student.firstName}
                                lastName={student.lastName}
                                role={RoleEnum[student?.role]}
                                showDelete={isTeacherOrAdmin}
                                onDelete={() => handleDeleteStudent(student.userId)}
                            />
                        ))}
                        {isTeacherOrAdmin && (
                            <AddTile 
                                onClick={() => setIsStudentPopupOpen(true)}
                                label="Student"
                            />
                        )}
                    </div>
                </div>

                {/* Teachers section */}
                <div className="course-list-title">
                    <p>Teachers: {courseData.teachers?.length || 0}</p>
                    <div className="course-list-line" />
                    <div className="course-list-entries">
                        {courseData.teachers?.map((teacher) => (
                            <UserTile
                                key={teacher.userId}
                                firstName={teacher.firstName}
                                lastName={teacher.lastName}
                                role={RoleEnum[teacher?.role]}
                                showDelete={currentUser.role > 1}
                                onDelete={() => handleDeleteTeacher(teacher.userId)}
                            />
                        ))}
                        {currentUser.role > 1 && (
                            <AddTile 
                                onClick={() => setIsTeacherPopupOpen(true)}
                                label="Teacher"
                            />
                        )}
                    </div>
                </div>

                {/* Assignments section */}
                <div className="course-list-title">
                    <p>Assignments: {courseData.assignments?.length || 0}</p>
                    <div className="course-list-line" />
                    <div className='course-list-entries'>
                        {courseData.assignments?.map((assignment) => (
                            <AssignmentTile 
                                key={assignment.assignmentId}
                                assignment={assignment}
                                onDelete={handleDeleteAssignment}
                                onUpdate={fetchCourseData}
                                isTeacherOrAdmin={isTeacherOrAdmin}
                            />
                        ))}
                        {isTeacherOrAdmin && (
                            <AddTile 
                                onClick={() => setIsAssignmentPopupOpen(true)}
                                label="Assignment"
                            />
                        )}
                    </div>
                </div>
            </div>

            {/* Popups */}
            {isStudentPopupOpen && (
                <AddStudentToClassPopup 
                    onClose={handlePopupClose}
                    courseId={courseData.classId} 
                />
            )}
            {isAssignmentPopupOpen && (
                <AddAssignmentPopup
                    onClose={handlePopupClose}
                    classId={courseData.classId}
                />
            )}
            {isTeacherPopupOpen && (
                <AddTeacherToClassPopup 
                    onClose={handlePopupClose}
                    courseId={courseData.classId} 
                />
            )}
        </div>
    );
}

export default CourseContent;
