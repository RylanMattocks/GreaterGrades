import { useGetUsersClasses } from "../greatergradesapi/Classes";
import forestImage from "../images/forest.jfif";
import { setStorageItem } from '../functions/LocalStorage';
import { useContext, useState, useEffect } from "react";
import { UserContext } from "../functions/UserContext";

const Tiles = ({ courseIds, setSelectedItem }) => { 
    const classes = useGetUsersClasses(courseIds);
    const { currentUser, authToken } = useContext(UserContext);
    const [localClasses, setLocalClasses] = useState([]);

    // Update localClasses when classes changes
    useEffect(() => {
        setLocalClasses(classes);
    }, [classes]);

    const clickOnCourse = (course) => {
        setStorageItem('currentCourse', course);
        setSelectedItem('course info');
    };

    const handleDeleteCourse = async (courseId) => {
        try {
            const response = await fetch(`http://localhost:5000/api/Classes/${courseId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${authToken}`,
                    'Content-Type': 'application/json'
                }
            });
            if (response.status === 204) {
                // Update local state to remove the deleted class
                setLocalClasses(prevClasses => 
                    prevClasses.filter(course => course.classId !== courseId)
                );
            }
        } catch (error) {
            console.error("Error deleting course:", error);
        }
    };

    return (
        <div className="tiles-container">
            {localClasses.map((course, index) => (
                <div key={index} className="dashboard-tile-link">
                    <div 
                        className="dashboard-tile"
                        onClick={() => clickOnCourse(course)}
                    >
                        <h3 className="tile-title">{course.subject}</h3>
                        <img src={forestImage} alt="Course placeholder" className="tile-image" />
                        <div className="tile-footer">
                            <p>{course.students?.length || 0} Students</p>
                            <p>{course.teachers?.length || 0} Teachers</p>
                            {currentUser?.role > 1 && (
                                <button 
                                    className="delete-icon"
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        handleDeleteCourse(course.classId);
                                    }}
                                >
                                    🗑️
                                </button>
                            )}
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default Tiles;
