import { useEffect, useState, useContext } from "react";
import Header from "../components/Header";
import SideBar from "../components/SideBar";
import DashboardContent from "../components/DashboardContent";
import ProfileContent from "../components/ProfileContent";
import EnrolledClasses from "../components/EnrolledClasses";
import TaughtClasses from "../components/TaughtClasses";
import CourseContent from "../components/CourseContent";
import { UserContext } from '../functions/UserContext';
import { useGetUsersClasses } from "../greatergradesapi/Classes";
import ClassesContent from "../components/ClassesContent";
import InstitutionContent from "../components/InstitutionContent";

const Dashboard = () => {
    const { currentUser } = useContext(UserContext);
    const [selectedItem, setSelectedItem] = useState('dashboard');
    const [sidebarItems, setSidebarItems] = useState([]);
    const courses = useGetUsersClasses(currentUser?.classIds);

    useEffect(() => {
        const newSidebarItems = [];

        if (currentUser?.role === 0) {
            if (currentUser?.taughtClassIds > 0){
                newSidebarItems.push(
                    { id: 'dashboard', label: 'Dashboard' },
                    { id: 'profile', label: 'Profile' },
                    {
                        id: 'enrolled classes',
                        label: 'Enrolled Classes',
                        courses: courses.map(course => ({
                            id: course.id,
                            label: course.subject,
                            assignments: course.assignments || []
                        }))
                    },
                    { id: 'taught classes', label: 'Taught Classes' }
                );
            } else {
                newSidebarItems.push(
                    { id: 'dashboard', label: 'Dashboard' },
                    { id: 'profile', label: 'Profile' },
                    {
                        id: 'enrolled classes',
                        label: 'Enrolled Classes',
                        courses: courses.map(course => ({
                            id: course.id,
                            label: course.subject,
                            assignments: course.assignments || []
                        }))
                    }
                );
            }
        }

        if (currentUser?.role === 1) {
            if (currentUser?.classIds > 0){
                newSidebarItems.push(
                    { id: 'dashboard', label: 'Dashboard' },
                    { id: 'profile', label: 'Profile' },
                    {
                        id: 'enrolled classes',
                        label: 'Enrolled Classes',
                        courses: courses.map(course => ({
                            id: course.id,
                            label: course.subject,
                            assignments: course.assignments || []
                        }))
                    },
                    { id: 'taught classes', label: 'Taught Classes' }
                );
            } else {
                newSidebarItems.push(
                    { id: 'dashboard', label: 'Dashboard' },
                    { id: 'profile', label: 'Profile' },
                    { id: 'taught classes', label: 'Taught Classes' }
                );
            }
        }

        if (currentUser?.role === 2) {
            newSidebarItems.push(
                { id: 'dashboard', label: 'Dashboard' },
                { id: 'profile', label: 'Profile' },
                { id: 'classes', label: 'Classes' }
            );
        }

        if (currentUser?.role === 3) {
            newSidebarItems.push(
                { id: 'dashboard', label: 'Dashboard'},
                { id: 'profile', label: 'Profile' },
                { id: 'classes', label: 'Classes' },
                { id: 'institutions', label: 'Institutions' }
            );
        }

        setSidebarItems(newSidebarItems);
    }, [currentUser, courses]);

    const renderContent = () => {

        switch (selectedItem) {
            case 'dashboard':
                return <DashboardContent setSelectedItem={setSelectedItem}/>;
            case 'profile':
                return <ProfileContent />;
            case 'enrolled classes':
                return <EnrolledClasses courses={courses} setSelectedItem={setSelectedItem}/>;
            case 'taught classes':
                return <TaughtClasses setSelectedItem={setSelectedItem}/>;
            case 'classes':
                return <ClassesContent setSelectedItem={setSelectedItem}/>;
            case 'institutions':
                return <InstitutionContent />
            case 'course info':
                return <CourseContent />;
            default:
                return <div>doesnt match....</div>;
        }
    };

    return (
        <main className='student-dashboard'>
            <SideBar items={sidebarItems} selectedItem={selectedItem} onSelectItem={setSelectedItem} />
            <div className='student-body'>
                <Header />
                {renderContent()}
            </div>
        </main>
    );
};

export default Dashboard;
