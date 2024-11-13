import { useState } from 'react';

const SideBar = ({ items, selectedItem, onSelectItem }) => {
    const [dropdownOpen, setDropdownOpen] = useState({});

    const handleItemClick = (item) => {
        onSelectItem(item.id);
        if (item.id === 'enrolled classes') {
            setDropdownOpen(prev => ({ ...prev, [item.id]: !prev[item.id] }));
        } else if (item.courses) {
            setDropdownOpen(prev => ({ ...prev, [item.id]: false }));
        } else {
            setDropdownOpen(false);
        }
    };

    const handleCourseClick = (courseId, event) => {
        event.stopPropagation();
        setDropdownOpen(prev => ({ ...prev, [courseId]: !prev[courseId] }));
    };

    return (
        <div className="dashboard-sidebar">
            <ul>
                {items.map(item => (
                    <li
                        key={item.id}
                        className={`${item.id === selectedItem ? 'dashboard-sidebar-li-selected' : ''}`}
                        onClick={() => handleItemClick(item)}
                    >
                        {item.label}
                        {item.id === 'enrolled classes' && dropdownOpen[item.id] && (
                            <ul>
                                {item.courses.map(course => (
                                    <li key={course.id}>
                                        <div 
                                            onClick={(event) => handleCourseClick(course.id, event)} 
                                            style={{ cursor: 'pointer' }}
                                        >
                                            {course.label}
                                        </div>
                                        {dropdownOpen[course.id] && (
                                            <ul>
                                                {course.assignments.map(assignment => (
                                                    <li key={assignment.assignmentId} className="dashboard-sidebar-assignment">{assignment.name}</li>
                                                ))}
                                            </ul>
                                        )}
                                    </li>
                                ))}
                            </ul>
                        )}
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default SideBar;
